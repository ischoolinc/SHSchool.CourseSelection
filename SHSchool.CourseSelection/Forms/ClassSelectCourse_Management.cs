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
    public partial class ClassSelectCourse_Management : BaseForm
    {
        public ClassSelectCourse_Management()
        {
            InitializeComponent();

            #region Init ComboBox
            AccessHelper access = new AccessHelper();
            List<UDT.OpeningTime> openTimeList = access.Select<UDT.OpeningTime>();
            // SchoolYear
            schoolYearCbx.Items.Add(openTimeList[0].SchoolYear - 1);
            schoolYearCbx.Items.Add(openTimeList[0].SchoolYear );
            schoolYearCbx.Items.Add(openTimeList[0].SchoolYear +1);
            schoolYearCbx.SelectedIndex = 1;
            // Semester
            semesterCbx.Items.Add(1);
            semesterCbx.Items.Add(2);
            semesterCbx.SelectedIndex = int.Parse("" + openTimeList[0].Semester) - 1;
            #endregion

        }

        public void ReloadDataGridView()
        {
            dataGridViewX1.Rows.Clear();

            #region SQL
            string selectSQl = string.Format(@"
                SELECT DISTINCT 
                    student.ref_class_id,
                    class.class_name,
                    class.grade_year,
	                subject_class.ref_subject_id,
	                subject.subject_name
                FROM 
                    student 
                    LEFT OUTER JOIN
                    (
	                    SELECT 
                            class_name,
                            id,grade_year,
                            dept 
                        FROM 
                            class
		                WHERE  grade_year IS NOT NULL 
                    )class on class.id = student.ref_class_id
	                LEFT OUTER JOIN
	                (
		                SELECT
			                ref_class_id,
			                ref_subject_id
		                FROM
			                $ischool.course_selection.subject_class_selection
	                )subject_class ON subject_class.ref_class_id = student.ref_class_id
	                LEFT OUTER JOIN
	                (
		                SELECT  
			                subject_name,
			                uid
		                FROM
			                $ischool.course_selection.subject 
                        WHERE school_year = {0} AND semester = {1} AND type = '{2}'
	                )subject ON subject.uid = subject_class.ref_subject_id
                WHERE 
                    student.status = 1 AND 
                    student.ref_class_id IS NOT NULL AND 
                    class.grade_year IS NOT NULL 
                ORDER BY class.grade_year
                ", schoolYearCbx.Text,semesterCbx.Text,courseTypeCbx.Text);
            #endregion

            if (courseTypeCbx.Text != "")
            {
                Dictionary<string, string> classDic = new Dictionary<string, string>();

                QueryHelper qh = new QueryHelper();
                DataTable dt = qh.Select(selectSQl);

                int row = -1;
                foreach (DataRow dr in dt.Rows)
                {
                    // 班級對應多筆科目
                    if (classDic.ContainsKey("" + dr["ref_class_id"]) && "" +  dr["subject_name"] != "")
                    {
                        if (classDic["" + dr["ref_class_id"]] != "")
                        {
                            classDic["" + dr["ref_class_id"]] += "、";
                        }
                        classDic["" + dr["ref_class_id"]] += dr["subject_name"];

                        dataGridViewX1.Rows[row].Cells[2].Value = int.Parse("" + dataGridViewX1.Rows[row].Cells[2].Value) + 1;
                        dataGridViewX1.Rows[row].Cells[3].Value = classDic["" + dr["ref_class_id"]];
                    }
                    // 班級對應單筆科目
                    if (!classDic.ContainsKey("" + dr["ref_class_id"]))
                    {
                        classDic.Add("" + dr["ref_class_id"],"" + dr["subject_name"]);

                        DataGridViewRow datarow = new DataGridViewRow();
                        datarow.CreateCells(dataGridViewX1);

                        int index = 0;
                        datarow.Cells[index++].Value = "" + dr["grade_year"];
                        datarow.Cells[index++].Value = "" + dr["class_name"];
                        // 紀錄班級科目數
                        if ("" + dr["subject_name"] != "")
                        {
                            datarow.Cells[index++].Value = 1;
                        }
                        if ("" + dr["subject_name"] == "")
                        {
                            datarow.Cells[index++].Value = 0;
                        }
                        datarow.Cells[index++].Value = "" + dr["subject_name"];
                        datarow.Tag = "" + dr["ref_class_id"];

                        dataGridViewX1.Rows.Add(datarow);
                        row++;
                    }
                    
                }
            }
        }

        public void ReloadCourseType()
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
                    , schoolYearCbx.Text, semesterCbx.Text);
                QueryHelper qh = new QueryHelper();
                DataTable courseTypes = qh.Select(selectSQL);

                foreach (DataRow type in courseTypes.Rows)
                {
                    courseTypeCbx.Items.Add(type["type"]);
                }
                courseTypeCbx.SelectedIndex = 0;
            }
        }

        private void schoolYearCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadCourseType();
        }

        private void semesterCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadCourseType();
        }

        private void courseTypeCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadDataGridView();
        }

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void setSelectSubjecttBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> selectedClassDic = new Dictionary<string, string>();

            foreach (DataGridViewRow datarow in dataGridViewX1.SelectedRows)
            {
                selectedClassDic.Add("" + datarow.Tag,"" + datarow.Cells[1].Value);
            }
            ClassSelectCourse_Setting cscs = new ClassSelectCourse_Setting(schoolYearCbx.Text,semesterCbx.Text,courseTypeCbx.Text,selectedClassDic);
            cscs.ShowDialog();
        }  
    }
}
