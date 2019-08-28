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

        public TurnIntoCourseStudent()
        {
            InitializeComponent();
        }

        private void TurnIntoCourseStudent_Load(object sender, EventArgs e)
        {
            AccessHelper access = new AccessHelper();
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
                QueryHelper qh = new QueryHelper();
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
SELECT 
    subject.uid
    , subject.subject_name
    , subject.level
    , subject.credit
    , subject.disabled
    , subject_course.count as course_count
	, ss_attend.count as student_count
    , ss_attend.course_student_count
FROM 
    $ischool.course_selection.subject  AS subject
    LEFT OUTER JOIN
	(
		SELECT 
		    subject_course.ref_subject_id,
		    count(subject_course.ref_subject_id) 
	    FROM 
		    $ischool.course_selection.subject_course AS subject_course
            LEFT OUTER JOIN $ischool.course_selection.subject AS subject
                ON subject.uid = subject_course.ref_subject_id
	    WHERE subject.type = '{0}'
	    GROUP BY subject_course.ref_subject_id
	) AS subject_course 
         ON subject_course.ref_subject_id = subject.uid
    LEFT OUTER JOIN
    (
	    SELECT 
            ref_subject_id,
            count(ref_subject_id),
            count(ref_subject_course_id) as course_student_count
	    FROM 
            $ischool.course_selection.ss_attend
            LEFT OUTER JOIN student
                ON student.id = $ischool.course_selection.ss_attend.ref_student_id
        WHERE
            student.status IN ( 1, 2 )
	    GROUP BY ref_subject_id
    ) AS ss_attend 
        ON ss_attend.ref_subject_id = subject.uid
WHERE 
    subject.school_year = {1} 
    AND subject.semester = {2} 
    AND type = '{0}'
            ", courseTypeCbx.Text, schoolYearCbx.Text, semesterCbx.Text);
            #endregion

            QueryHelper qh = new QueryHelper();
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
                int studentCount = "" + row["student_count"] == "" ? 0 : int.Parse("" + row["student_count"]);
                // 科目已分班人數
                int courseStudentCount = "" + row["course_student_count"] == "" ? 0 : int.Parse("" + row["course_student_count"]);
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
                    datarow.Cells[index++].Value = "已開班數:" + courseCount + "、尚有" + (studentCount - courseStudentCount) + "位學生未分班!";
                    datarow.Tag = "error";
                }
                // 有開班、沒有學生選課
                if (courseCount > 0 && studentCount == 0)
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                    datarow.Cells[index++].Value = "已開班數:" + courseCount + "、 沒有學生選課!";
                    datarow.Tag = "error";
                }
                // 有開班、有選課學生、學生有分班
                if (courseCount > 0 && studentCount > 0 && studentCount == courseStudentCount)
                {
                    datarow.Cells[index++].Value = "已開班數:" + courseCount + "、 選課人數:" + studentCount + "、 已分班人數:" + courseStudentCount;
                    datarow.Tag = "correct";
                }

                dataGridViewX1.Rows.Add(datarow);
            }
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

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 轉入修課學生
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

            #region SQL
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
    AND subject.type = '{2}'
    AND student.status IN ( 1, 2 )
"
            , schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);
            #endregion

            QueryHelper qh = new QueryHelper();
            DataTable dataTable = qh.Select(selectSQl);

            AccessHelper access = new AccessHelper();
            List<SCAttendRecord> scrNewList = new List<SCAttendRecord>();
            // 避免重複新增修課紀錄: 透過StudentID、CourseID取得修課紀錄並刪除，並且刪除課程成績!
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

            // 新增:New修課紀錄
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
}
