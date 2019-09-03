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
                bool _disOpen = false;
                bool disOpen = bool.TryParse("" + row["disabled"], out _disOpen);
                // 如果沒開班
                if (courseCount == 0)
                {
                    if (disOpen) // 不開課
                    {
                        datarow.Cells[index++].Value = row["subject_name"] + "不開課";
                        datarow.Tag = "correct";
                    }
                    else
                    {
                        datarow.Cells[index].Style.ForeColor = Color.Red;
                        datarow.Cells[index++].Value = "科目未開班!";
                        datarow.Tag = "error";
                    }
                }
                // 有開班、有選課學生、但有未分班學生
                if (courseCount > 0 && studentCount > 0 && (studentCount - courseStudentCount) > 0)
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                    datarow.Cells[index++].Value = string.Format("已開班數:{0}、尚有{1}位學生未分班!", courseCount, studentCount - courseStudentCount);
                    datarow.Tag = "error";
                }
                // 有開班、沒有學生選課
                if (courseCount > 0 && studentCount == 0)
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                    datarow.Cells[index++].Value = string.Format("已開班數:{0}、 沒有學生選課!", courseCount);
                    datarow.Tag = "error";
                }
                // 有開班、有選課學生、學生有分班
                if (courseCount > 0 && studentCount > 0 && studentCount == courseStudentCount)
                {
                    datarow.Cells[index++].Value = string.Format("已開班數:{0}、 選課人數:{1}、 已分班人數:{2}", courseCount, studentCount, courseStudentCount);
                    datarow.Tag = "correct";
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
            #region 判斷可否轉入修課學生
            foreach (DataGridViewRow dr in dataGridViewX1.Rows)
            {
                if ("" + dr.Tag == "error")
                {
                    MessageBox.Show("無法轉入修課學生!");
                    return;
                }
            }
            #endregion

            #region SQL - 取得目標選課紀錄 進行後續轉入修課學生
            string selectSQl = string.Format(@"
SELECT 
	ss_attend.ref_student_id,
	ss_attend.ref_subject_id,
	ss_attend.ref_subject_course_id, 
	subject_course.ref_course_id,
	subject.school_year,
	subject.semester,
    subject.type
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
"
            , schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);
            #endregion
            DataTable dataTable = qh.Select(selectSQl);

            // 避免重複新增修課紀錄:
            // 1. 透過StudentID、CourseID取得修課紀錄並刪除
            // 2. 刪除課程成績!
            {
                Dictionary<string, string> studentCourseDic = new Dictionary<string, string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    studentCourseDic.Add("" + row["ref_student_id"], "" + row["ref_course_id"]);
                }
                List<SCAttendRecord> scrOldList = SCAttend.SelectByStudentIDAndCourseID(studentCourseDic.Keys.ToList(), studentCourseDic.Values.ToList());
                List<SCETakeRecord> sctList = SCETake.SelectByStudentAndCourse(studentCourseDic.Keys.ToList(), studentCourseDic.Values.ToList());

                if (scrOldList.Count > 0)
                {
                    DialogResult result = MessageBox.Show("已轉入修課學生，確定重複轉入修課學生將會清除學生原課程成績以及原修課紀錄 ", "警告", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                SCETake.Delete(sctList);
                SCAttend.Delete(scrOldList);
            }

            // 新增:New修課紀錄
            {
                List<SCAttendRecord> scrNewList = new List<SCAttendRecord>();
                foreach (DataRow row in dataTable.Rows)
                {
                    SCAttendRecord scAttend = new SCAttendRecord();
                    scAttend.RefCourseID = "" + row["ref_course_id"];
                    scAttend.RefStudentID = "" + row["ref_student_id"];
                    scrNewList.Add(scAttend);
                }
                SCAttend.Insert(scrNewList);
                MessageBox.Show("轉入成功!");
            }
        }

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
