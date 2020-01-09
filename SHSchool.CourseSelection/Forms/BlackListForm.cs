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
        private AccessHelper access = new AccessHelper();
        private QueryHelper qh = new QueryHelper();
        private UpdateHelper up = new UpdateHelper();

        public BlackListForm()
        {
            InitializeComponent();
        }

        private void BlackListForm_Load(object sender, EventArgs e)
        {
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
            DataTable dt = GetBlackListBySchoolYearSemester(cbxSchoolYear.Text,cbxSemester.Text);

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

        /// <summary>
        /// 透過學年度學期取得擋修名單
        /// </summary>
        private DataTable GetBlackListBySchoolYearSemester(string schoolYear, string semester)
        {
            string sql = string.Format(@"
WITH target_subject_type AS(
    SELECT DISTINCT
        type
    FROM
        $ischool.course_selection.subject
    WHERE
        school_year = {0}
        AND semester = {1}
        AND type IS NOT NULL
) 
SELECT
    target_subject_type.type
    , count(subject_block.*)
FROM
    target_subject_type
    LEFT OUTER JOIN $ischool.course_selection.subject AS subject
        ON subject.type = target_subject_type.type 
    LEFT OUTER JOIN $ischool.course_selection.subject_block AS subject_block
        ON subject.uid = subject_block.ref_subject_id
 GROUP BY
    target_subject_type.type
                ", schoolYear, semester);

            DataTable dt = qh.Select(sql);
            return dt;
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
        , CONCAT(subject_name, level) AS key1
        , CONCAT(pre_subject, pre_subject_level) AS key2
    FROM
        $ischool.course_selection.subject
    WHERE
        type IN ({0})
        AND school_year = {1}
        AND semester = {2}
), delete_target_subject_block AS(
    DELETE 
    FROM
        $ischool.course_selection.subject_block
    WHERE
        ref_subject_id IN ( SELECT uid FROM target_subject )
), target_student AS(
    SELECT
        scs.ref_subject_id
        , student.*
    FROM
        target_subject
        LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs
            ON target_subject.uid = scs.ref_subject_id
        LEFT OUTER JOIN student
            ON student.ref_class_id = scs.ref_class_id
    WHERE
        student.status IN (1,2)
), target_student_sems_subj_score AS(
    SELECT
        sems_subj_score_ext.ref_student_id
        , sems_subj_score_ext.grade_year
        , sems_subj_score_ext.semester
        , sems_subj_score_ext.school_year
        , unnest(xpath('/Subject/@科目', subj_score_ele))::text AS 科目
        , unnest(xpath('/Subject/@科目級別', subj_score_ele))::text AS 科目級別
        , unnest(xpath('/Subject/@開課學分數', subj_score_ele))::text AS 學分數
        , unnest(xpath('/Subject/@是否取得學分', subj_score_ele))::text AS 取得學分
    FROM (
            SELECT 
                sems_subj_score.*
                ,   unnest(xpath('/SemesterSubjectScoreInfo/Subject', xmlparse(content score_info))) as subj_score_ele
            FROM 
                sems_subj_score 
            WHERE ref_student_id IN (SELECT id FROM target_student)
        ) as sems_subj_score_ext
    ORDER BY grade_year desc, semester desc, school_year desc
), target_student_score_rec AS(
    SELECT
        *
        , CONCAT(科目, 科目級別) AS key
    FROM
        target_student_sems_subj_score
), target_student_sc_attend AS(
    SELECT
        sc_attend.*
        , CONCAT(course.subject, course.subj_level) AS key
    FROM
        sc_attend
        LEFT OUTER JOIN course
            ON course.id = sc_attend.ref_course_id
    WHERE
        sc_attend.ref_student_id IN(SELECT id FROM target_student)
), calculation_pre_subject_block_mode1 AS(
    SELECT
        target_subject.uid
        , target_student.id
        , '未取得前導課程學分'::text AS reason
    FROM
        target_subject
        INNER JOIN target_student
            ON target_subject.uid = target_student.ref_subject_id
        LEFT OUTER JOIN target_student_score_rec
            ON target_student_score_rec.ref_student_id = target_student.id
            AND target_student_score_rec.key = target_subject.key2
            AND target_student_score_rec.取得學分 = '是'
    WHERE
        target_subject.pre_subject_block_mode = '已取得學分'
        AND target_student_score_rec.ref_student_id IS NULL
        AND target_subject.pre_subject IS NOT NULL
), calculation_pre_subject_block_mode2 AS(
    SELECT
        target_subject.uid
        , target_student.id
        , '未修過前導課程'::text AS reason
    FROM
        target_subject
        INNER JOIN target_student
            ON target_subject.uid = target_student.ref_subject_id
        LEFT OUTER JOIN target_student_sc_attend 
            ON target_student_sc_attend.ref_student_id = target_student.id
            AND target_student_sc_attend.key = target_subject.key2
    WHERE
        target_subject.pre_subject_block_mode = '已修過'
        AND target_student_sc_attend.id IS NULL
        AND target_subject.pre_subject IS NOT NULL
), calculation_rejoin_block_mode1 AS (
    SELECT
        target_subject.uid
        , target_student.id
        , '已取得相同科目學分'::text AS reason
    FROM
        target_subject
        INNER JOIN target_student
            ON target_subject.uid = target_student.ref_subject_id
        LEFT OUTER JOIN target_student_score_rec
            ON target_student_score_rec.ref_student_id = target_student.id
            AND target_student_score_rec.key = target_subject.key1
            AND target_student_score_rec.取得學分 = '是'
    WHERE
        target_subject.rejoin_block_mode = '已取得學分'
        AND target_student_score_rec.ref_student_id IS NOT NULL
), calculation_rejoin_block_mode2 AS (
    SELECT
        target_subject.uid 
        , target_student.id 
        , '已修過相同科目'::text AS reason
    FROM
        target_subject
        INNER JOIN target_student
            ON target_subject.uid = target_student.ref_subject_id
        LEFT OUTER JOIN target_student_sc_attend
            ON target_student_sc_attend.ref_student_id = target_student.id
            AND target_student_sc_attend.key = target_subject.key1
    WHERE
        target_subject.rejoin_block_mode = '已修過'
        AND target_student_sc_attend.ref_student_id IS NOT NULL
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
                ", string.Join(",",listTypeName),cbxSchoolYear.SelectedItem.ToString(),cbxSemester.SelectedItem.ToString());

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
