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
            string showText = "";
            QueryHelper queryHelper = new QueryHelper();
            

            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                string _courseName = "" + datarow.Cells["courseName"].Value;
                string _subjectName = "" + datarow.Cells["subjectName"].Value;
                int _schoolYear = int.Parse("" + datarow.Cells["schoolYear"].Value);
                int _semester = int.Parse("" + datarow.Cells["semester"].Value);
                int _level = int.Parse("" + datarow.Cells["level"].Value);
                int _credit = int.Parse("" + datarow.Cells["credit"].Value);
                string _type = "" + datarow.Cells["type"].Value;

                #region 判斷是否開過課--dtCourse

                string sqlSelect = string.Format(@"
                    SELECT * FROM course 
                    WHERE course_name = '{0}' 
                    AND subject = '{1}' 
                    AND school_year = {2} 
                    AND semester = {3}
                ", _courseName, _subjectName, _schoolYear, _semester);

                DataTable dtCourse = queryHelper.Select(sqlSelect);

                #endregion

                #region 第一次開課
                if (dtCourse.Rows.Count == 0)
                {
                    // Course table 新增選修科目課程資訊
                    string sqlInsert = string.Format(@"
                    INSERT INTO 
                        course
                        (course_name,subject,school_year,semester,subj_level,credit,score_type) 
                    VALUES('{0}','{1}',{2},{3},{4},{5},'{6}')",
                    _courseName, _subjectName, _schoolYear, _semester, _level, _credit, _type
                    );
                    UpdateHelper updateHelper = new UpdateHelper();
                    updateHelper.Execute(sqlInsert);

                    // 開課成功 同步課程ID -> Subject_Course.course_id
                    string sqlSelectCourseID = string.Format(@"
                        SELECT id
                        FROM course
                        WHERE course_name = '{0}'
                        AND subject = '{1}' 
                        AND school_year = {2} 
                        AND semester = {3}
                    ", _courseName, _subjectName, _schoolYear, _semester);
                    DataTable dtCourseID = queryHelper.Select(sqlSelectCourseID);

                    foreach (DataRow dr in dtCourseID.Rows)
                    {
                        string sqlUpdate = string.Format(@"
                        UPDATE $ischool.course_selection.subjectcourse
                        SET course_id = {0}
                        WHERE $ischool.course_selection.subjectcourse.course_name = '{1}'
                        AND subject_name = '{2}' 
                        AND school_year = {3} 
                        AND semester = {4}
                        ", "" + dr["id"], _courseName, _subjectName, _schoolYear, _semester);
                        updateHelper.Execute(sqlUpdate);
                    }

                    // 紀錄開課成功的 課程名稱
                    showText += _courseName + ",";
                }
                #endregion

                #region 重複開課
                if (dtCourse.Rows.Count > 0)
                {
                    UpdateHelper updateHelper = new UpdateHelper();
                    // 刪除舊的開課資訊
                    string sqlDelete = string.Format(@"
                        DELETE FROM
                            course
                        WHERE
                            subject = '{0}'
                            AND school_year = {1}
                            AND semester = {2}
                        ", _subjectName, _schoolYear, _semester);
                    updateHelper.Execute(sqlDelete);

                    // 新增新的開課資訊
                    string sqlInsert = string.Format(@"
                        INSERT INTO 
                            course
                            (course_name,subject,school_year,semester,subj_level,credit,score_type) 
                         VALUES('{0}','{1}',{2},{3},{4},{5},'{6}')",
                     _courseName, _subjectName, _schoolYear, _semester, _level, _credit, _type
                        );
                    updateHelper.Execute(sqlInsert);

                    // 同步選修科目課程ID 與 課程ID，透過課程名稱為條件
                    foreach (DataRow dr in dtCourse.Rows)
                    {
                        string sqlUpdate = string.Format(@"
                            UPDATE $ischool.course_selection.subjectcourse
                            SET course_id = {0}
                            WHERE $ischool.course_selection.subjectcourse.course_name = '{1}'
                            ", "" + dr["id"], _courseName);
                        updateHelper.Execute(sqlUpdate);
                    }

                }
                showText += _courseName + ",";
                #endregion

            }

            MessageBox.Show(showText + "課程建立成功!");
            this.Close();
        }
    }
}
