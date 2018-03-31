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
        public TurnIntoCourseStudent()
        {
            InitializeComponent();

            #region Init ComboBox
            {
                AccessHelper access = new AccessHelper();
                List<UDT.OpeningTime> openingTime = access.Select<UDT.OpeningTime>();
                int sy = openingTime[0].SchoolYear;
                int s = openingTime[0].Semester;
                // SchoolYear 預設: 開放選課學年度
                schoolYearCbx.Text = "" + sy;
                for (int i = 0; i < 3;i++)
                {
                    schoolYearCbx.Items.Add(sy - i);
                }
                // Semester 預設: 開放選課學期
                semesterCbx.Text = "" + s;
                semesterCbx.Items.Add(1);
                semesterCbx.Items.Add(2);
            }    
            #endregion
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
                        school_year = {0} AND semester = {1}"
                    ,schoolYearCbx.Text,semesterCbx.Text);
                QueryHelper qh = new QueryHelper();
                DataTable courseTypes = qh.Select(selectSQL);

                foreach (DataRow type in courseTypes.Rows)
                {
                    courseTypeCbx.Items.Add(type["type"]);
                }
                courseTypeCbx.SelectedIndex = 0;
            }
        }

        public void ReloadDataGridView()
        {
            #region SQL
            string selectSQL = string.Format(@"
SELECT 
    subject.uid,
    subject.subject_name,
    subject.level,
    subject.credit,
    subject_course.count as course_count,
	ss_attend.count as student_count,
    ss_attend.course_student_count
FROM 
    $ischool.course_selection.subject  AS subject
    LEFT OUTER JOIN
	(
		SELECT 
		    ref_subject_id,
		    count(ref_subject_id) 
	    FROM 
		    $ischool.course_selection.subject_course AS subject_course
            LEFT OUTER JOIN $ischool.course_selection.subject AS subject
                ON subject.uid = subject_course.ref_subject_id
	    WHERE subject.type = '{0}'
	    GROUP BY subject_course.ref_subject_id
	)subject_course ON subject_course.ref_subject_id = subject.uid
    LEFT OUTER JOIN
    (
	    SELECT 
            ref_subject_id,
            count(ref_subject_id),
            count(ref_subject_course_id) as course_student_count
	    FROM 
            $ischool.course_selection.ss_attend
	    GROUP BY ref_subject_id
    )ss_attend ON ss_attend.ref_subject_id = subject.uid
WHERE subject.school_year = {1} AND subject.semester = {2} AND type = '{0}'"
            , courseTypeCbx.Text, schoolYearCbx.Text, semesterCbx.Text);
            #endregion

            QueryHelper qh = new QueryHelper();
            DataTable dataTable = qh.Select(selectSQL);

            foreach (DataRow dr in dataTable.Rows)
            {
                DataGridViewRow datarow = new DataGridViewRow();
                datarow.CreateCells(dataGridViewX1);
                int index = 0;
                datarow.Cells[index].Tag = "" + dr["uid"]; // 紀錄:科目ID
                datarow.Cells[index++].Value = "" + dr["subject_name"];
                datarow.Cells[index++].Value = "" + dr["level"];
                datarow.Cells[index++].Value = "" + dr["credit"];

                // 開班數
                int courseCount = "" + dr["course_count"] == "" ? 0 : int.Parse("" + dr["course_count"]);
                // 選課人數
                int studentCount = "" + dr["student_count"] == "" ? 0 : int.Parse("" + dr["student_count"]);
                // 科目已分班人數
                int courseStudentCount = "" + dr["course_student_count"] == "" ? 0 : int.Parse("" + dr["course_student_count"]);
                // 如果沒開班
                if (courseCount == 0)
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                    datarow.Cells[index++].Value = "科目未開班!";
                    datarow.Tag = "error";
                }
                // 有開班、有選課學生、但有未分班學生
                if (courseCount > 0 && studentCount > 0 && (studentCount - courseStudentCount) > 0 )
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                    datarow.Cells[index++].Value = "已開班數:"+ courseCount + "、尚有"+ (studentCount - courseStudentCount) + "位學生未分班!";
                    datarow.Tag = "error";
                }
                // 有開班、沒有學生選課
                if (courseCount > 0  && studentCount == 0 )
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                    datarow.Cells[index++].Value = "已開班數:" + courseCount + "、 沒有學生選課!";
                    datarow.Tag = "error";
                }
                // 有開班、有選課學生、學生有分班
                if (courseCount > 0  && studentCount > 0 && studentCount == courseStudentCount)
                {
                    datarow.Cells[index++].Value = "已開班數:" + courseCount + "、 選課人數:" + studentCount + "、 已分班人數:" + courseStudentCount;
                    datarow.Tag = "correct";
                }
                
                dataGridViewX1.Rows.Add(datarow);
            }
        }

        private void schoolYearCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadCourseTypeCbx();
        }

        private void semesterCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadCourseTypeCbx();
        }

        private void courseTypeCbx_TextChanged(object sender, EventArgs e)
        {
            dataGridViewX1.Rows.Clear();
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
WHERE 
    subject.school_year = {0} 
    AND subject.semester = {1} 
    AND subject.type = '{2}'"
            , schoolYearCbx.Text, semesterCbx.Text,courseTypeCbx.Text);
            #endregion

            QueryHelper qh = new QueryHelper();
            DataTable dataTable = qh.Select(selectSQl);

            AccessHelper access = new AccessHelper();
            List<SCAttendRecord> scrNewList = new List<SCAttendRecord>();
            // 避免重複新增修課紀錄: 透過StudentID、CourseID取得修課紀錄並刪除，並且刪除課程成績!
            Dictionary<string, string> studentCourseDic = new Dictionary<string, string>();
            foreach (DataRow dr in dataTable.Rows)
            {
                studentCourseDic.Add("" + dr["ref_student_id"], "" + dr["ref_course_id"]);
            }
            List<SCAttendRecord> scrOldList = SCAttend.SelectByStudentIDAndCourseID(studentCourseDic.Keys.ToList(),studentCourseDic.Values.ToList());
            List<SCETakeRecord> sctList = SCETake.SelectByStudentAndCourse(studentCourseDic.Keys.ToList(), studentCourseDic.Values.ToList());

            if (scrOldList.Count > 0)
            {
                DialogResult result = MessageBox.Show("已轉入修課學生，確定重複轉入修課學生將會清除學生原課程成績以及原修課紀錄 ","警告",MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            SCAttend.Delete(scrOldList);
            SCETake.Delete(sctList);

            // 新增:New修課紀錄
            foreach (DataRow dr in dataTable.Rows)
            {
                SCAttendRecord scAttend = new SCAttendRecord();
                scAttend.RefCourseID = "" + dr["ref_course_id"];
                scAttend.RefStudentID = "" + dr["ref_student_id"];
                scrNewList.Add(scAttend);
            }
            SCAttend.Insert(scrNewList);
            MessageBox.Show("轉入成功!");
        }
    }
}
