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
using FISCA.Data;
using SHSchool.Data;
using FISCA.UDT;

namespace SHSchool.CourseSelection.Forms
{
    public partial class BuildCourse : BaseForm
    {
        int schoolYear, semester;        

        public BuildCourse(DataGridView dgv,int sy,int s,string type)
        {
            InitializeComponent();

            schoolYear = sy;
            semester = s;

            // Init Lb
            schoolYearLb.Text = "" + sy;
            semesterLb.Text = "" + s;

            #region Init DGV
            {
                foreach (DataGridViewRow dr in dgv.Rows)
                {
                    // 設定開班數
                    int count = int.Parse("" + dr.Cells["buildCourseCount"].Value);
                    // 已開班數
                    int _count = int.Parse("" + dr.Cells["courseCount"].Value);
                    // 已開班數 = 設定開班數 : 修改
                    if (_count == count)
                    {
                        int sbID = int.Parse("" + dr.Tag);
                        QueryHelper qh = new QueryHelper();
                        string sql = string.Format(@"
                            SELECT *
                            FROM
                                $ischool.course_selection.subject_course
                            WHERE 
                                ref_subject_id = {0}
                        ",sbID);
                        DataTable subjectCourseUDT = qh.Select(sql);
                        foreach (DataRow scr in subjectCourseUDT.Rows)
                        {
                            if (count == 1)
                            {
                                scr["class_type"] = "";
                            }
                            DataGridViewRow drX1 = new DataGridViewRow();
                            drX1.CreateCells(dataGridViewX1);
                            int index = 0;
                            drX1.Cells[index++].Value = "修改";
                            drX1.Cells[index++].Value = scr["course_name"];
                            drX1.Cells[index++].Value = scr["course_type"];
                            drX1.Cells[index++].Value = scr["subject_name"];
                            drX1.Cells[index++].Value = scr["level"];
                            drX1.Cells[index++].Value = scr["class_type"];
                            drX1.Cells[index++].Value = scr["credit"];
                            drX1.Tag = scr["uid"];

                            dataGridViewX1.Rows.Add(drX1);
                        }
                    }

                    // 已開班數 > 設定開班數 : 刪除
                    if (_count > count)
                    {
                        int sbID = int.Parse("" + dr.Tag);
                        QueryHelper qh = new QueryHelper();
                        string sql = string.Format(@"
                            SELECT *
                            FROM
                                $ischool.course_selection.subject_course
                            WHERE 
                                ref_subject_id = {0}
                        ", sbID);
                        DataTable subjectCourseUDT = qh.Select(sql);
                        int n = 0;
                        foreach (DataRow scr in subjectCourseUDT.Rows)
                        {
                            DataGridViewRow drX1 = new DataGridViewRow();
                            drX1.CreateCells(dataGridViewX1);
                            n++;
                            if (count == 1)
                            {
                                scr["class_type"] = "";
                            }
                            if (n > count)
                            {
                                drX1.Cells[0].Value = "刪除";
                                //drX1.DefaultCellStyle.BackColor = Color.Red;
                                drX1.DefaultCellStyle.ForeColor = Color.Red;
                            }
                            else
                            {
                                drX1.Cells[0].Value = "修改";
                            }
                            int index = 1;
                            drX1.Cells[index++].Value = scr["course_name"];
                            drX1.Cells[index++].Value = scr["course_type"];
                            drX1.Cells[index++].Value = scr["subject_name"];
                            drX1.Cells[index++].Value = scr["level"];
                            drX1.Cells[index++].Value = scr["class_type"];
                            drX1.Cells[index++].Value = scr["credit"];
                            drX1.Tag = scr["uid"];

                            dataGridViewX1.Rows.Add(drX1);
                        }
                    }

                    // 已開班數 < 設定開班數 : 新增
                    if (_count < count)
                    {
                        int sbID = int.Parse("" + dr.Tag);
                        QueryHelper qh = new QueryHelper();
                        string sql = string.Format(@"
                            SELECT *
                            FROM
                                $ischool.course_selection.subject_course
                            WHERE 
                                ref_subject_id = {0}
                        ", sbID);
                        DataTable subjectCourseUDT = qh.Select(sql);
                        int n = 0;
                        foreach (DataRow scr in subjectCourseUDT.Rows)
                        {
                            if (count == 1)
                            {
                                scr["class_type"] = "";
                            }
                            DataGridViewRow drX1 = new DataGridViewRow();
                            drX1.CreateCells(dataGridViewX1);
                            n++;
                            int index = 0;
                            drX1.Cells[index++].Value = "修改";
                            drX1.Cells[index++].Value = scr["course_name"];
                            drX1.Cells[index++].Value = scr["course_type"];
                            drX1.Cells[index++].Value = scr["subject_name"];
                            drX1.Cells[index++].Value = scr["level"];
                            drX1.Cells[index++].Value = scr["class_type"];
                            drX1.Cells[index++].Value = scr["credit"];
                            drX1.Tag = scr["uid"];

                            dataGridViewX1.Rows.Add(drX1);
                        }
                        for (int i = 1;i <= count - n; i++)
                        {
                            InitDataGridView("新增", i + n , type, dr);
                        }
                    }
                }
            }
            #endregion

            
        }

        public void InitDataGridView(string _dataType,int i,string type,DataGridViewRow dr)
        {
            #region Switch 級別、班別
            string[] mark = new string[10];
            string level = "";
            switch (i)
            {
                case 1:
                    mark[i] = "A";
                    break;
                case 2:
                    mark[i] = "B";
                    break;
                case 3:
                    mark[i] = "C";
                    break;
                case 4:
                    mark[i] = "D";
                    break;
                case 5:
                    mark[i] = "E";
                    break;
                case 6:
                    mark[i] = "F";
                    break;
                case 7:
                    mark[i] = "G";
                    break;
                case 8:
                    mark[i] = "H";
                    break;
                case 9:
                    mark[i] = "I";
                    break;
                case 10:
                    mark[i] = "J";
                    break;
                default:
                    mark[i] = "";
                    break;
            }

            switch (int.Parse("" + dr.Cells["level"].Value))
            {
                case 1:
                    level = "I";
                    break;
                case 2:
                    level = "II";
                    break;
                case 3:
                    level = "III";
                    break;
            }
            #endregion
            
            //如果 設定開班數 count = 1，不加班別
            if (int.Parse("" + dr.Cells["buildCourseCount"].Value) == 1)
            {
                mark[i] = "";
            }
            
            DataGridViewRow drX1 = new DataGridViewRow();
            drX1.CreateCells(dataGridViewX1);
            int index = 0;
            drX1.Cells[index++].Value = _dataType;
            drX1.Cells[index++].Value = type +" "+ dr.Cells["subjectName"].Value + " "+level +" "+ mark[i];
            drX1.Cells[index++].Value = type;
            drX1.Cells[index++].Value = dr.Cells["subjectName"].Value;
            drX1.Cells[index++].Value = dr.Cells["level"].Value;
            drX1.Cells[index++].Value = mark[i];
            drX1.Cells[index++].Value = dr.Cells["credit"].Value;
            if (_dataType == "刪除")
            {
                //drX1.DefaultCellStyle.BackColor = Color.Red;
                drX1.DefaultCellStyle.ForeColor = Color.Red;
            }
            // 這邊的TAG 是 subjectID
            drX1.Tag = dr.Tag;
            dataGridViewX1.Rows.Add(drX1);
        }

        private void dataGridViewX1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                string level = "";
                switch (int.Parse("" + dataGridViewX1.Rows[e.RowIndex].Cells[4].Value))
                {
                    case 1:
                        level = "I";
                        break;
                    case 2:
                        level = "II";
                        break;
                    case 3:
                        level = "III";
                        break;
                }
                dataGridViewX1.Rows[e.RowIndex].Cells[1].Value = dataGridViewX1.Rows[e.RowIndex].Cells[2].Value + " " +
                                                                 dataGridViewX1.Rows[e.RowIndex].Cells[3].Value + " " +
                                                                 level + " " +
                                                                 dataGridViewX1.Rows[e.RowIndex].Cells[5].Value;
            }
        }

        // 欄位驗證 : 判斷課程名稱是否重複
        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Dictionary<string, string> courseNameDic = new Dictionary<string, string>();
            foreach (DataGridViewRow dr in dataGridViewX1.Rows)
            {
                if (courseNameDic.ContainsKey("" + dr.Cells[1].Value))
                {
                    dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "error";
                    return;
                }
                if (!courseNameDic.ContainsKey("" + dr.Cells[1].Value))
                {
                    courseNameDic.Add("" + dr.Cells["courseName"].Value, "" + dr.Cells["subjectName"].Value);
                    dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = string.Empty;
                }

            }
        }

        // 開課
        private void buildCourseBtn_Click(object sender, EventArgs e)
        {
            List<string> courseNameList = new List<string>();
            bool repeat = false;
            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if (courseNameList.Contains("" + datarow.Cells["courseName"].Value))
                {
                    repeat = true;
                    MessageBox.Show("課程名稱重複!!");
                    return;
                }
                if(!courseNameList.Contains("" + datarow.Cells["courseName"].Value))
                {
                    courseNameList.Add("" + datarow.Cells["courseName"].Value);
                }
            }

            AccessHelper access = new AccessHelper();
            List<UDT.SubjectCourse> subCourseList = access.Select<UDT.SubjectCourse>();

            foreach (DataGridViewRow dr in dataGridViewX1.Rows)
            {
                if ("" + dr.Cells["dataType"].Value == "新增")
                {
                    //-- Course Table 新增課程資訊
                    //-- SubjectCourse UDT 同步 courseID
                    SHCourseRecord scr = new SHCourseRecord();
                    scr.Subject = "" + dr.Cells["subjectName"].Value;
                    scr.Level = int.Parse("" + dr.Cells["level"].Value);
                    scr.Name = "" + dr.Cells["courseName"].Value;
                    scr.Credit = int.Parse("" + dr.Cells["credit"].Value);
                    scr.SchoolYear = int.Parse(schoolYearLb.Text);
                    scr.Semester = int.Parse(semesterLb.Text);
                    
                    string courseID = SHCourse.Insert(scr);
                    
                    // -- SubjectCourse UDT 新增 科目課程資訊
                    UDT.SubjectCourse sb = new UDT.SubjectCourse();
                    sb.RefSubjectID = int.Parse("" + dr.Tag);
                    sb.RefCourseID = int.Parse("" + courseID);
                    sb.SubjectName = "" + dr.Cells["subjectName"].Value;
                    sb.CourseName = "" + dr.Cells["courseName"].Value;
                    sb.Course_type = "" + dr.Cells["courseType"].Value;
                    sb.Level = int.Parse("" + dr.Cells["level"].Value);
                    sb.Credit = int.Parse("" + dr.Cells["credit"].Value);
                    sb.SchoolYear = int.Parse(schoolYearLb.Text);
                    sb.Semester = int.Parse(semesterLb.Text);
                    sb.Class_type = "" + dr.Cells["classType"].Value;
                    sb.Save();

                }
                if ("" + dr.Cells["dataType"].Value == "修改")
                {
                    string level = "";
                    switch (int.Parse("" + dr.Cells["level"].Value))
                    {
                        case 1:
                            level = "I";
                            break;
                        case 2:
                            level = "II";
                            break;
                        case 3:
                            level = "III";
                            break;
                    }
                    dr.Cells["courseName"].Value = "" + dr.Cells["courseType"].Value + " " +dr.Cells["subjectName"].Value + " " +level + " " +dr.Cells["classType"].Value;
                    // SubjectCourse UDT 修改選修科目課程名稱、選修科目班別
                    UpdateHelper uph = new UpdateHelper();
                    string updateSql = string.Format(@"
                        UPDATE $ischool.course_selection.subject_course 
                        SET course_name = '{0}',
                            class_type = '{1}'
                        WHERE uid = {2}
                    ", dr.Cells["courseName"].Value, dr.Cells["classType"].Value, int.Parse("" + dr.Tag));
                    uph.Execute(updateSql);
                    // 取得要修改的課程ID
                    string selUpDateCourseID = string.Format(@"
                        SELECT ref_course_id
                        From $ischool.course_selection.subject_course 
                        WHERE uid = {0}
                    ",int.Parse("" + dr.Tag));
                    QueryHelper qh = new QueryHelper();
                    DataTable dt = qh.Select(selUpDateCourseID);
                    // Course Table 修改課程資訊
                    //修改課程資訊
                    foreach (DataRow datarow in dt.Rows)
                    {
                        int course_id = int.Parse("" + datarow["ref_course_id"]);
                        string updateSql2 = string.Format(@"
                            UPDATE course
                            SET course_name = '{0}'
                            WHERE id = {1}
                        ", dr.Cells["courseName"].Value, course_id);
                        uph.Execute(updateSql2);
                    }
                }
                if ("" + dr.Cells["dataType"].Value == "刪除")
                {
                    UpdateHelper uph = new UpdateHelper();

                    // 取得要刪除的課程ID
                    QueryHelper qh = new QueryHelper();
                    string selDeleteCourseID = string.Format(@"
                        SELECT ref_course_id
                        From $ischool.course_selection.subject_course 
                        WHERE uid = {0}
                    ", int.Parse("" + dr.Tag));
                    DataTable dt = qh.Select(selDeleteCourseID);

                    // Course Table 刪除課程資訊
                    // 刪除課程資訊
                    foreach (DataRow datarow in dt.Rows)
                    {
                        int course_id = int.Parse("" + datarow["ref_course_id"]);
                        string deleteSql = string.Format(@"
                            DELETE FROM course
                            WHERE id = {0}
                        ",course_id);
                        uph.Execute(deleteSql);
                    }

                    // SubjectCourse UDT 刪除科目課程資訊
                    string deleteSql2 = string.Format(@"
                        DELETE FROM $ischool.course_selection.subject_course 
                        WHERE uid = {0}
                    ", int.Parse("" + dr.Tag));
                    uph.Execute(deleteSql2);

                    // SSAttend UDT 刪除SubjectCourseID
                    string updateSql = string.Format(@"
                        UPDATE $ischool.course_selection.ss_attend 
                        SET ref_subject_course_id = null 
                        WHERE ref_subject_course_id = {0}
                        ", "" + dr.Tag);
                    uph.Execute(updateSql);
                }
            }

            if (repeat == false)
            {
                MessageBox.Show("課程建立成功!");
                this.Close();
            }
            if (repeat == true)
            {
                MessageBox.Show("課程建立失敗!");
            }
            
        }
    }
}
