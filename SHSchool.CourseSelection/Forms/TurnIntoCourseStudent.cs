using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;
using FISCA.UDT;
using FISCA.Data;

namespace SHSchool.CourseSelection.Forms
{
    public partial class TurnIntoCourseStudent : BaseForm
    {
        private bool initFinish = false;
        private string actor = FISCA.Authentication.DSAServices.UserAccount.Replace("'", "''");
        private string clientInfo = FISCA.LogAgent.ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml.Replace("'", "''");

        private AccessHelper access = new AccessHelper();
        private QueryHelper qh = new QueryHelper();

        public TurnIntoCourseStudent()
        {
            InitializeComponent();
        }

        private void TurnIntoCourseStudent_Load(object sender, EventArgs e)
        {
            #region Init SchoolYearLb、SemesterLb
            {
                List<UDT.OpeningTime> timeList = access.Select<UDT.OpeningTime>();

                if (timeList.Count == 0)
                {
                    timeList.Add(new UDT.OpeningTime() { SchoolYear = int.Parse(K12.Data.School.DefaultSchoolYear), Semester = int.Parse(K12.Data.School.DefaultSemester) });
                    timeList.SaveAll();
                }

                schoolYearCbx.Items.Add(timeList[0].SchoolYear + 1);
                schoolYearCbx.Items.Add(timeList[0].SchoolYear);
                schoolYearCbx.Items.Add(timeList[0].SchoolYear - 1);

                schoolYearCbx.SelectedIndex = 1;

                semesterCbx.Items.Add(1);
                semesterCbx.Items.Add(2);

                if ("" + timeList[0].Semester == "1")
                {
                    semesterCbx.SelectedIndex = 0;
                }

                if ("" + timeList[0].Semester == "2")
                {
                    semesterCbx.SelectedIndex = 1;
                }
            }
            #endregion

            initFinish = true;
            ReloadCourseTypeCbx();
        }

        private void schoolYearCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (initFinish)
            {
                ReloadCourseTypeCbx();
            }
        }

        private void semesterCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (initFinish)
            {
                ReloadCourseTypeCbx();
            }
        }

        private void courseTypeCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadDataGridView();
        }

        public void ReloadCourseTypeCbx()
        {
            if (schoolYearCbx.Text != "" && semesterCbx.Text != "")
            {
                courseTypeCbx.Items.Clear();

                string selectSQL = string.Format(@"
SELECT DISTINCT 
    type 
FROM 
    $ischool.course_selection.subject 
WHERE 
    school_year = {0} 
    AND semester = {1}
    AND type IS NOT NULL
                ", schoolYearCbx.Text, semesterCbx.Text);
                DataTable courseTypes = qh.Select(selectSQL);

                foreach (DataRow type in courseTypes.Rows)
                {
                    courseTypeCbx.Items.Add(type["type"]);
                }
                if (courseTypeCbx.Items.Count > 0)
                {
                    courseTypeCbx.SelectedIndex = 0;
                }
                else
                {
                    dataGridViewX1.Rows.Clear();
                }
            }
        }

        public void ReloadDataGridView()
        {
            dataGridViewX1.Rows.Clear();
            #region SQL
            string selectSQL = string.Format(@"
WITH target_subject AS(
	SELECT
		*
	FROM
		$ischool.course_selection.subject
	WHERE
		school_year = {0}
		AND semester = {1}
		AND type = '{2}'
), calc_subj_course AS(
	SELECT
		subject_course.ref_subject_id
		, COUNT(subject_course.ref_subject_id) AS course_count
	FROM
		$ischool.course_selection.subject_course AS subject_course
		INNER JOIN target_subject
			ON target_subject.uid = subject_course.ref_subject_id
	GROUP BY
		subject_course.ref_subject_id
), calc_attend AS(
	SELECT
		ss_attend.ref_subject_id
		, COUNT(ref_subject_id) AS stu_count
		, COUNT(ref_subject_course_id) AS course_stu_count
	FROM
		$ischool.course_selection.ss_attend AS ss_attend
		INNER JOIN target_subject
			ON target_subject.uid = ss_attend.ref_subject_id
		LEFT OUTER JOIN student
			ON student.id = ss_attend.ref_student_id
	WHERE
		student.status IN (1, 2)
	GROUP BY
		ss_attend.ref_subject_id
)
SELECT
	target_subject.uid
	, target_subject.subject_name
	, target_subject.level
	, target_subject.credit
	, target_subject.disabled
	, calc_subj_course.course_count
	, calc_attend.stu_count
	, calc_attend.course_stu_count
FROM
	target_subject
	LEFT OUTER JOIN calc_subj_course
		ON calc_subj_course.ref_subject_id = target_subject.uid
	LEFT OUTER JOIN calc_attend
		ON calc_attend.ref_subject_id = target_subject.uid
            ", schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);
            #endregion

            DataTable dataTable = qh.Select(selectSQL);

            foreach (DataRow row in dataTable.Rows)
            {
                DataGridViewRow datarow = new DataGridViewRow();
                datarow.CreateCells(dataGridViewX1);
                int index = 0;
                datarow.Cells[index].Tag = "" + row["uid"]; // 紀錄:科目ID
                datarow.Cells[index++].Value = "" + row["subject_name"];
                datarow.Cells[index++].Value = "" + row["level"];
                datarow.Cells[index++].Value = "" + row["credit"];

                // 開班數
                int courseCount = "" + row["course_count"] == "" ? 0 : int.Parse("" + row["course_count"]);
                // 選課人數
                int studentCount = "" + row["stu_count"] == "" ? 0 : int.Parse("" + row["stu_count"]);
                // 科目已分班人數
                int courseStudentCount = "" + row["course_stu_count"] == "" ? 0 : int.Parse("" + row["course_stu_count"]);
                bool disabled = "" + row["disabled"] == "true" ? true : false;
                // 如果沒開班
                if (courseCount == 0)
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                    datarow.Cells[index++].Value = "科目未開班!";
                    datarow.Tag = "error";
                }
                // 不開課
                else if (disabled)
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                    datarow.Cells[index++].Value = "不開課";
                    datarow.Tag = "correct";
                }
                else
                {
                    // 有開班、有選課學生、但有未分班學生
                    if (studentCount > 0 && (studentCount - courseStudentCount) > 0)
                    {
                        datarow.Cells[index].Style.ForeColor = Color.Red;
                        datarow.Cells[index++].Value = string.Format("已開班數:{0}、尚有{1}位學生未分班!", courseCount, studentCount - courseStudentCount);
                        datarow.Tag = "error";
                    }
                    // 有開班、沒有學生選課
                    if (studentCount == 0)
                    {
                        datarow.Cells[index].Style.ForeColor = Color.Red;
                        datarow.Cells[index++].Value = string.Format("已開班數:{0}、 沒有學生選課!", courseCount);
                        datarow.Tag = "error";
                    }
                    // 有開班、有選課學生、學生有分班
                    if (studentCount > 0 && studentCount == courseStudentCount)
                    {
                        datarow.Cells[index++].Value = string.Format("已開班數:{0}、 選課人數:{1}、 已分班人數:{2}", courseCount, studentCount, courseStudentCount);
                        datarow.Tag = "correct";
                    }
                }
                
                dataGridViewX1.Rows.Add(datarow);
            }
        }

        /// <summary>
        /// 轉入修課學生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void turnInto_SC_Attend_Btn_Click(object sender, EventArgs e)
        {
            bool keepGoing = CheckData();

            if (keepGoing)
            {
                try
                {
                    DataTable dt = GetRepeatScAttend();
                    if (dt.Rows.Count > 0)
                    {
                        MsgBox.Show("轉入失敗: \n部分學生已有修課紀錄，請先到「課程」頁面刪除重複修課學生\n再進行「選課 - 轉入修課學生」動作。", "錯誤訊息");
                    }
                    else
                    {
                        TransferData();
                    }
                }
                catch (Exception error)
                {
                    MsgBox.Show(error.Message);
                }
            }
            else
            {
                MessageBox.Show("無法轉入修課學生!");
            }
        }

        /// <summary>
        /// 判斷可否轉入修課學生
        /// </summary>
        private bool CheckData()
        {
            bool isTransferable = true;
            if (dataGridViewX1.Rows.Count > 0)
            {
                foreach (DataGridViewRow dr in dataGridViewX1.Rows)
                {
                    if ("" + dr.Tag == "error")
                    {
                        isTransferable = false;
                    }
                }
            }
            else
            {
                isTransferable = false;
            }

            return isTransferable;
        }

        /// <summary>
        /// 取得重複修課紀錄
        /// </summary>
        /// <returns></returns>
        private DataTable GetRepeatScAttend()
        {
            #region 檢查是否重複轉入修課紀錄SQL
            string sql = string.Format(@"
WITH target_ss_attend AS (
	SELECT 
		ss_attend.ref_student_id
		, ss_attend.ref_subject_id
		, ss_attend.ref_subject_course_id
		, subject_course.ref_course_id
		, subject.school_year
		, subject.semester
	    , subject.type
	FROM 
	    $ischool.course_selection.ss_attend AS ss_attend
	    LEFT OUTER JOIN $ischool.course_selection.subject_course AS subject_course 
	        ON subject_course.uid = ss_attend.ref_subject_course_id
	    LEFT OUTER JOIN $ischool.course_selection.subject AS subject
	        ON subject.uid = ss_attend.ref_subject_id
	    LEFT OUTER JOIN student
	        ON student.id = ss_attend.ref_student_id
	WHERE 
	    subject.school_year = {0}
	    AND subject.semester = {1}
	    AND subject.disabled = false
	    AND subject.type = '{2}'
	    AND student.status IN ( 1, 2 )
	    AND subject_course.ref_course_id IS NOT NULL
)
SELECT
	sc_attend.*
FROM
	sc_attend
	INNER JOIN target_ss_attend
		ON target_ss_attend.ref_student_id = sc_attend.ref_student_id
		AND target_ss_attend.ref_course_id = sc_attend.ref_course_id
                ", schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);
            #endregion

            return qh.Select(sql);
        }

        /// <summary>
        /// 轉入修課學生
        /// </summary>
        private void TransferData()
        {
            #region 轉入修課紀錄SQL
            string sql = string.Format(@"

WITH target_ss_attend AS (
	SELECT 
		ss_attend.ref_student_id
		, ss_attend.ref_subject_id
		, ss_attend.ref_subject_course_id
		, subject_course.ref_course_id
		, subject.school_year
		, subject.semester
	    , subject.type
	FROM 
	    $ischool.course_selection.ss_attend AS ss_attend
	    LEFT OUTER JOIN $ischool.course_selection.subject_course AS subject_course 
	        ON subject_course.uid = ss_attend.ref_subject_course_id
	    LEFT OUTER JOIN $ischool.course_selection.subject AS subject
	        ON subject.uid = ss_attend.ref_subject_id
	    LEFT OUTER JOIN student
	        ON student.id = ss_attend.ref_student_id
	WHERE 
	    subject.school_year = {0}
	    AND subject.semester = {1}
	    AND subject.disabled = false
	    AND subject.type = '{2}'
	    AND student.status IN ( 1, 2 )
	    AND subject_course.ref_course_id IS NOT NULL
), insert_sc_attend AS(
	INSERT INTO sc_attend (
		ref_student_id
		, ref_course_id
	)
	SELECT
		target_ss_attend.ref_student_id
		, target_ss_attend.ref_course_id
	FROM
		target_ss_attend
	RETURNING *
), insert_log AS(
    INSERT INTO log(
        actor
        , action_type
        , action
        , target_category
        , target_id
        , server_time
        , client_info
        , action_by
        , description
    ) VALUES (
        '{3}'
        , '選課作業'
        , '轉入修課學生'
        , '修課紀錄'
        , NULL
        , NOW()
        , '{4}'
        , '「選課作業」「轉入修課學生」功能'
        , '課程類別:「{2}」，學生選課修課紀錄已轉入課程修課紀錄。'
    )
)
SELECT * FROM insert_sc_attend
                    ", schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text, actor, clientInfo);

            #endregion
            try
            {
                DataTable dt = qh.Select(sql);
                if (dt.Rows.Count > 0)
                {
                    MsgBox.Show("轉入成功。");
                }
                else
                {
                    MsgBox.Show("並未轉入任何資料!");
                }
            }
            catch (Exception error)
            {
                MsgBox.Show(error.Message);
            }
        }

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
