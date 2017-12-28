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
using FISCA.Data;
using K12.Data;

namespace SHSchool.CourseSelection.Forms
{
    public partial class BuildCourseClass : BaseForm
    {
        public BuildCourseClass()
        {
            InitializeComponent();

            #region 學年、學期
            schoolYearCbx.Text = School.DefaultSchoolYear;
            for (int i = 0; i < 3; i++)
            {
                schoolYearCbx.Items.Add(int.Parse(School.DefaultSchoolYear) + i);
            }

            semesterCbx.Text = School.DefaultSemester;
            for (int i = 1; i < 3; i++)
            {
                semesterCbx.Items.Add(i);
            }
            #endregion

            #region 課程類別
            QueryHelper helper = new QueryHelper();
            string SQL = @"
                SELECT DISTINCT 
                    type 
                FROM $ischool.course_selection.subject ";
            foreach (DataRow row in helper.Select(SQL).Rows)
            {
                courseTypeCbx.Items.Add("" + row["type"]);
            }
            #endregion

        }
        public void ReloadDataGridView()
        {
            #region SQL
            string SQL = string.Format(@"
            SELECT 
                subject.subject_name,
                subject.level,
                subject.limit,
                subject.credit,
                subject.type,
                subject.uid,
                subject.ref_course_id,
                att_count.count AS student_count,
				course_count.count AS _course_count
            FROM
                $ischool.course_selection.subject AS subject
            LEFT OUTER JOIN
            (
                SELECT
                    ref_subject_id,
                    count(*)
                FROM 
		            $ischool.course_selection.ss_attend
                GROUP BY ref_subject_id
            ) att_count
            ON att_count.ref_subject_id = subject.uid 
			LEFT OUTER JOIN
			course ON course.id = subject.ref_course_id
			LEFT OUTER JOIN
			(
				SELECT
					subject,
					count(*)
				FROM
					course
				GROUP BY course.subject
			) course_count ON course_count.subject = subject.subject_name
            WHERE 
	            subject.school_year = {0}
	            AND subject.semester = {1}
                AND type = '{2}'",
                schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);
            #endregion

            #region DataGridView
            if (courseTypeCbx.Text != "")
            {
                QueryHelper qh = new QueryHelper();
                DataTable dataTable = qh.Select(SQL);

                dataGridViewX1.Rows.Clear();
                foreach (DataRow dr in dataTable.Rows)
                {
                    DataGridViewRow datarow = new DataGridViewRow();
                    datarow.CreateCells(dataGridViewX1);

                    int index = 0;
                    datarow.Cells[index++].Value = "" + dr["subject_name"];
                    datarow.Cells[index++].Value = "" + dr["level"];
                    datarow.Cells[index++].Value = "" + dr["credit"];
                    //datarow.Cells[index++].Value = "學業";
                    datarow.Cells[index++].Value = "" + dr["student_count"]/* + "/" + dr["limit"]*/;
                    // 如果未開班，開班數為0
                    if ("" + dr["_course_count"] == "")
                    {
                        datarow.Cells[index++].Value = "" + 0;
                        datarow.Cells[index++].Value = "" + 0;
                    }
                    else
                    {
                        datarow.Cells[index++].Value = "" + dr["_course_count"];
                        datarow.Cells[index].Value = "" + dr["_course_count"];
                    }
                    
                    dataGridViewX1.Rows[dataGridViewX1.Rows.Add(datarow)].Tag = "" + dr["uid"];
                    //dataGridViewX1.Rows.Add(datarow);
                }
            }
            #endregion
        }
        
        private void schoolYearCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadDataGridView();
        }

        private void semesterCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadDataGridView();
        }

        private void courseTypeCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadDataGridView();
        }

        private void buildCourseBtn_Click(object sender, EventArgs e)
        {
            #region "建立課程班級" 將datagridview資訊寫入 SubjectCourse_UDT
            /*
            {
                int count = 0;

                foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
                {
                    List<UDT.SubjectCourse> udt_list = new List<UDT.SubjectCourse>();

                    // 讀取dgv開課數
                    if (datarow.Cells["buildCourseCount"].Value != null)
                    {
                        count = int.Parse("" + datarow.Cells["buildCourseCount"].Value);
                    }

                    //讀取subjectCourse UDT 科目ID是否建立過課程 
                    string sql = string.Format(@"
                        SELECT*FROM
                            $ischool.course_selection.subjectcourse
                        WHERE 
                            subject_id = {0}
                    ",datarow.Tag);
                    QueryHelper qh = new QueryHelper();
                    DataTable dt = qh.Select(sql);

                    // 如果科目重複建立課程，刪除舊的資訊
                    if (dt.Rows.Count > 0)
                    {
                        string sqlDelete = string.Format(@"
                            DELETE FROM
                                $ischool.course_selection.subjectcourse
                            WHERE
                                subject_id = {0}
                        ", datarow.Tag);
                        UpdateHelper updateHelper = new UpdateHelper();
                        updateHelper.Execute(sqlDelete);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        #region 課程命名
                        string[] mark = new string[10];
                        string level = "";

                        switch (i)
                        {
                            case 0:
                                mark[i] = "A";
                                break;
                            case 1:
                                mark[i] = "B";
                                break;
                            case 2:
                                mark[i] = "C";
                                break;
                            case 3:
                                mark[i] = "D";
                                break;
                            case 4:
                                mark[i] = "E";
                                break;
                            case 5:
                                mark[i] = "F";
                                break;
                            case 6:
                                mark[i] = "G";
                                break;
                            case 7:
                                mark[i] = "H";
                                break;
                            case 8:
                                mark[i] = "I";
                                break;
                            case 9:
                                mark[i] = "J";
                                break;
                            default:
                                MessageBox.Show("已超出開班數上限");
                                break;
                        }

                        switch (int.Parse("" + datarow.Cells["level"].Value))
                        {
                            case 1:
                                level = "I";
                                break;
                            case 2:
                                level = "II";
                                break;
                            case 3:
                                level = "II";
                                break;
                        }
                        UDT.SubjectCourse subjectCourse = new UDT.SubjectCourse();
                        #endregion
                        subjectCourse.CourseName = "" + courseTypeCbx.Text + datarow.Cells["subjectName"].Value + " " + level + " " + mark[i];
                        subjectCourse.SubjectID = int.Parse("" + datarow.Tag.ToString());
                        subjectCourse.SubjectName = "" + datarow.Cells["subjectName"].Value;
                        subjectCourse.Course_type = "" + courseTypeCbx.Text;
                        subjectCourse.SchoolYear = int.Parse(schoolYearCbx.Text);
                        subjectCourse.Semester = int.Parse(semesterCbx.Text);
                        subjectCourse.Level = int.Parse("" + datarow.Cells["level"].Value);
                        subjectCourse.Credit = int.Parse("" + datarow.Cells["credit"].Value);
                        //subjectCourse.Sore_type = "" + datarow.Cells["type"].Value;
                        udt_list.Add(subjectCourse);
                    }
                    udt_list.SaveAll();
                }
            }
            */
            #endregion
            
            int sy = int.Parse("" + schoolYearCbx.Text);
            int s = int.Parse("" + semesterCbx.Text);
            string courseType = "" + courseTypeCbx.Text;

            BuildCourse buildCourse = new BuildCourse(dataGridViewX1,sy,s,courseType);
            buildCourse.ShowDialog();

            ReloadDataGridView();
        }

        // DataGridView 欄位驗證
        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int number = 0;
            if (!int.TryParse("" + dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, out number))
            {
                dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "error";

            }
            else if (int.Parse("" + dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) > 10 )
            {
                dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "error";
            }
            else
            {
                dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = string.Empty;
            }
        }

    }
}
