using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using K12.Data;
using DevComponents.DotNetBar.Controls;
using FISCA.Data;

namespace SHSchool.CourseSelection.Forms
{
    public partial class BlackListForm : BaseForm
    {
        private bool initFinsh = false;
        public BlackListForm()
        {
            InitializeComponent();
        }

        private void BlackListForm_Load(object sender, EventArgs e)
        {
            AccessHelper access = new AccessHelper();

            #region Init SchoolYear Semester
            List<UDT.OpeningTime> listOpenTime = access.Select<UDT.OpeningTime>();
            if (listOpenTime.Count == 0)
            {
                listOpenTime.Add(new UDT.OpeningTime() { SchoolYear = int.Parse(K12.Data.School.DefaultSchoolYear), Semester = int.Parse(K12.Data.School.DefaultSemester) });
                listOpenTime.SaveAll();
            }

            cbxSchoolYear.Items.Add(listOpenTime[0].SchoolYear + 1);
            cbxSchoolYear.Items.Add(listOpenTime[0].SchoolYear);
            cbxSchoolYear.Items.Add(listOpenTime[0].SchoolYear - 1);
            cbxSchoolYear.SelectedIndex = 1;

            cbxSemester.Items.Add(1);
            cbxSemester.Items.Add(2);
            cbxSemester.SelectedIndex = listOpenTime[0].Semester - 1;
            #endregion

            initFinsh = true;

            // Init DataGridvView
            ReloadDataGridView();
        }

        private void cbxSchoolYear_TextChanged(object sender, EventArgs e)
        {
            if (initFinsh)
            {
                ReloadDataGridView();
            }
        }

        private void cbxSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (initFinsh)
            {
                ReloadDataGridView();
            }
        }

        private void ReloadDataGridView()
        {
            dataGridViewX1.Rows.Clear();

            // 1.取得學年度學期擋修名單資料
            DataTable dt = DAO.BlackListDAO.GetBlackListBySchoolYearSemester(cbxSchoolYear.Text,cbxSemester.Text);

            // 2.填入擋修名單資料
            foreach (DataRow row in dt.Rows)
            {
                DataGridViewRow dgvrow = new DataGridViewRow();
                dgvrow.CreateCells(dataGridViewX1);

                int index = 0;
                dgvrow.Cells[index++].Value = false;
                dgvrow.Cells[index++].Value = "" + row["type"];
                dgvrow.Cells[index++].Value = "" + row["count"];

                dataGridViewX1.Rows.Add(dgvrow);
            }
        }

        private void btnProduceBlacklist_Click(object sender, EventArgs e)
        {
            List<string> listTypeName = new List<string>();

            // 取得勾選的課程時段
            foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
            {
                if ("" + dgvrow.Cells[0].Value == "True")
                {
                    listTypeName.Add("'" + dgvrow.Cells[1].Value + "'");
                }
            }

            if (listTypeName.Count == 0)
            {
                MsgBox.Show("請先勾選要產生擋修名單的課程時段!");
                return;
            }
            
            string sql = string.Format(@"
WITH target_subject AS(
    SELECT
        *
    FROM
        $ischool.course_selection.subject
    WHERE
        type IN ( {0} )
) , delete_target_subject_block AS(
	DELETE 
	FROM
		$ischool.course_selection.subject_block
	WHERE
		ref_subject_id IN ( SELECT uid FROM target_subject )
) , target_student AS(
    SELECT
    	scs.ref_subject_id
        , student.*
    FROM
    	target_subject
    	LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs
    		ON target_subject.uid = scs.ref_subject_id
        LEFT OUTER JOIN student
            ON student.ref_class_id = scs.ref_class_id
) , target_student_sems_subj_score AS(
    SELECT
	sems_subj_score_ext.ref_student_id
	, sems_subj_score_ext.grade_year
	, sems_subj_score_ext.semester
	, sems_subj_score_ext.school_year
	--, unnest(xpath('/Subject/@開課分項類別', subj_score_ele))::text AS 分項類別
	, unnest(xpath('/Subject/@科目', subj_score_ele))::text AS 科目
	, unnest(xpath('/Subject/@科目級別', subj_score_ele))::text AS 科目級別
	, unnest(xpath('/Subject/@開課學分數', subj_score_ele))::text AS 學分數
	, unnest(xpath('/Subject/@是否取得學分', subj_score_ele))::text AS 取得學分
	, unnest(xpath('/Subject/@修課必選修', subj_score_ele))::text AS 必選修
	, unnest(xpath('/Subject/@修課校部訂', subj_score_ele))::text AS 校部訂
	, unnest(xpath('/Subject/@原始成績', subj_score_ele))::text AS 原始成績
	, unnest(xpath('/Subject/@補考成績', subj_score_ele))::text AS 補考成績
	, unnest(xpath('/Subject/@重修成績', subj_score_ele))::text AS 重修成績
	, unnest(xpath('/Subject/@學年調整成績', subj_score_ele))::text AS 學年調整成績
	, unnest(xpath('/Subject/@擇優採計成績', subj_score_ele))::text AS 手動調整成績
	--, unnest(xpath('/Subject/@不計學分', subj_score_ele))::text AS 不計學分
	--, unnest(xpath('/Subject/@不需評分', subj_score_ele))::text AS 不需評分
	, unnest(xpath('/Subject/@註記', subj_score_ele))::text AS 註記
FROM (
		SELECT 
			sems_subj_score.*
			, 	unnest(xpath('/SemesterSubjectScoreInfo/Subject', xmlparse(content score_info))) as subj_score_ele
		FROM 
			sems_subj_score 
		WHERE ref_student_id IN ( SELECT id FROM target_student)
	) as sems_subj_score_ext
ORDER BY grade_year desc, semester desc, school_year desc
) , calculation_pre_subject_block_mode1 AS(
    SELECT
        target_subject.uid
        , target_student.id
        , '未取得前導課程學分'::text AS reason
    FROM
        target_subject
        LEFT OUTER JOIN target_student
        	ON target_subject.uid = target_student.ref_subject_id
    	LEFT OUTER JOIN target_student_sems_subj_score
    		ON target_student.id = target_student_sems_subj_score.ref_student_id
    		AND target_student_sems_subj_score.科目 || target_student_sems_subj_score.科目級別 = target_subject.pre_subject || target_subject.pre_subject_level
    		AND target_student_sems_subj_score.取得學分 = '是'
    WHERE
        pre_subject_block_mode = '已取得學分'
        AND target_student_sems_subj_score.ref_student_id IS NULL
        AND target_subject.pre_subject IS NOT NULL
) , calculation_pre_subject_block_mode2 AS(
	SELECT
        target_subject.uid
        , target_student.id
        , '未修過前導課程'::text AS reason
    FROM
        target_subject
        LEFT OUTER JOIN target_student
        	ON target_subject.uid = target_student.ref_subject_id
    	LEFT OUTER JOIN target_student_sems_subj_score
    		ON target_student.id = target_student_sems_subj_score.ref_student_id
    		AND target_student_sems_subj_score.科目 || target_student_sems_subj_score.科目級別 = target_subject.pre_subject || target_subject.pre_subject_level
    WHERE
        pre_subject_block_mode = '已修過'
        AND target_student_sems_subj_score.ref_student_id IS NULL
        AND target_subject.pre_subject IS NOT NULL
) , calculation_rejoin_block_mode1 AS (
	SELECT
        target_subject.uid
        , target_student.id
        , '已取得科目學分'::text AS reason
    FROM
        target_subject
        LEFT OUTER JOIN target_student
        	ON target_subject.uid = target_student.ref_subject_id
    	LEFT OUTER JOIN target_student_sems_subj_score
    		ON target_student.id = target_student_sems_subj_score.ref_student_id
    		AND target_student_sems_subj_score.科目 || target_student_sems_subj_score.科目級別 = target_subject.subject_name || target_subject.level
    		AND target_student_sems_subj_score.取得學分 = '是'
    WHERE
        pre_subject_block_mode = '已取得學分'
        AND target_student_sems_subj_score.ref_student_id IS NOT NULL
) , calculation_rejoin_block_mode2 AS (
	SELECT
        target_subject.uid 
        , target_student.id 
        , '已修過科目'::text AS reason
    FROM
        target_subject
        LEFT OUTER JOIN target_student
        	ON target_subject.uid = target_student.ref_subject_id
    	LEFT OUTER JOIN target_student_sems_subj_score
    		ON target_student.id = target_student_sems_subj_score.ref_student_id
    		AND target_student_sems_subj_score.科目 || target_student_sems_subj_score.科目級別 = target_subject.subject_name || target_subject.level
    WHERE
        pre_subject_block_mode = '已修過'
        AND target_student_sems_subj_score.ref_student_id IS NOT NULL
)
INSERT INTO $ischool.course_selection.subject_block(
	ref_subject_id
	, ref_student_id
	, reason
)
SELECT * FROM calculation_pre_subject_block_mode1
	UNION ALL
SELECT * FROM calculation_pre_subject_block_mode2
	UNION ALL
SELECT * FROM calculation_rejoin_block_mode1
	UNION ALL
SELECT * FROM calculation_rejoin_block_mode2
                ", string.Join(",",listTypeName));
            UpdateHelper up = new UpdateHelper();
            try
            {
                up.Execute(sql);
                MsgBox.Show("擋修名單產生完成!");
                ReloadDataGridView();
            }
            catch(Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
