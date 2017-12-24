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

namespace SHSchool.CourseSelection.Forms
{
    public partial class BuildCourse : BaseForm
    {
        public BuildCourse(int sy,int s)
        {
            InitializeComponent();
            #region Init DGV
            {
                string sql = string.Format(@"
                SELECT * FROM 
                    $ischool.course_selection.subjectcourse 
                WHERE
                    school_year = {0}
                    AND semester = {1}"
                , sy, s);
                QueryHelper query = new QueryHelper();
                DataTable dt = query.Select(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    DataGridViewRow datarow = new DataGridViewRow();
                    datarow.CreateCells(dataGridViewX1);

                    int index = 0;
                    datarow.Cells[index++].Value = "" + dr["course_name"];
                    datarow.Cells[index++].Value = "" + dr["subject_name"];
                    datarow.Cells[index++].Value = "" + dr["school_year"];
                    datarow.Cells[index++].Value = "" + dr["semester"];
                    datarow.Cells[index++].Value = "" + dr["level"];
                    datarow.Cells[index++].Value = "" + dr["credit"];
                    datarow.Cells[index++].Value = "" + dr["score_type"];

                    dataGridViewX1.Rows.Add(datarow);
                }
            }
            #endregion
            
        }
        
        // 開課
        private void buildCourseBtn_Click(object sender, EventArgs e)
        {
            //SHSchool.Data.SHCourse sHCourse = new SHCourse();
            string showText = "";
            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                string _courseName = "" + datarow.Cells["courseName"].Value;
                string _subjectName = "" + datarow.Cells["subjectName"].Value;
                int _schoolYear = int.Parse("" + datarow.Cells["schoolYear"].Value);
                int _semester = int.Parse("" + datarow.Cells["semester"].Value);
                int _level = int.Parse("" + datarow.Cells["level"].Value);
                int _credit = int.Parse("" + datarow.Cells["credit"].Value);
                string _type = "" + datarow.Cells["type"].Value;

                // 判斷是否開過課
                string sqlSelect = string.Format(@"
                    SELECT * FROM course 
                    WHERE course_name = '{0}' 
                    AND subject = '{1}' 
                    AND school_year = {2} 
                    AND semester = {3}
                ", _courseName, _subjectName, _schoolYear, _semester);

                QueryHelper query = new QueryHelper();
                DataTable dtCourse = query.Select(sqlSelect);

                // 沒開過課，新增課程資訊
                try
                {
                    if (dtCourse.Rows.Count == 0)
                    {
                        // Insert Course_table 選修課程
                        string sqlInsert = string.Format(@"
                        INSERT INTO 
                            course
                            (course_name,subject,school_year,semester,subj_level,credit,score_type) 
                        VALUES('{0}','{1}',{2},{3},{4},{5},'{6}')",
                        _courseName, _subjectName, _schoolYear, _semester, _level, _credit, _type
                        );
                        UpdateHelper updateHelper = new UpdateHelper();
                        updateHelper.Execute(sqlInsert);


                        // Update Subject_Course_udt 課程ID
                        string sqlSelect2 = string.Format(@"
                            SELECT id
                            FROM course
                            WHERE course_name = '{0}'
                            AND subject = '{1}' 
                            AND school_year = {2} 
                            AND semester = {3}
                        ", _courseName, _subjectName, _schoolYear, _semester);
                        
                        DataTable dt2 = query.Select(sqlSelect2);
                        foreach (DataRow dr in dt2.Rows)
                        {
                            string sqlUpdate = string.Format(@"
                            UPDATE $ischool.course_selection.subjectcourse
                            SET course_id = {0}
                            WHERE $ischool.course_selection.subjectcourse.course_name = '{1}'
                            AND subject = '{2}' 
                            AND school_year = {3} 
                            AND semester = {4}
                            ", "" + dr["id"], _courseName, _subjectName, _schoolYear, _semester);
                            updateHelper.Execute(sqlUpdate);
                        }

                        // 紀錄開課成功的 課程名稱
                        showText += _courseName + ",";
                    }
                }
                catch
                {
                    // 開課失敗
                    showText = _courseName + "課程建立失敗!";
                    MessageBox.Show(showText);
                }
                
                // 開過課，修改課程資訊
                if (dtCourse.Rows.Count >0)
                {
                    //string sqlSelect2 = string.Format(@"
                    //        SELECT id
                    //        FROM course
                    //        WHERE course_name = '{0}'
                    //    ", _courseName);

                    //DataTable dt2 = query.Select(sqlSelect2);
                    foreach (DataRow dr in dtCourse.Rows)
                    {
                        string sqlUpdate = string.Format(@"
                            UPDATE $ischool.course_selection.subjectcourse
                            SET course_id = {0}
                            WHERE $ischool.course_selection.subjectcourse.course_name = '{1}'
                            ", "" + dr["id"], _courseName);
                        UpdateHelper updateHelper = new UpdateHelper();
                        updateHelper.Execute(sqlUpdate);
                    }
                }
            }
            MessageBox.Show(showText + "課程建立成功!");
            this.Close();

            
        }
    }
}
