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
using FISCA.UDT;
using FISCA.Presentation;
using System.IO;

namespace SHSchool.CourseSelection.Test
{
    public partial class AutoSelectStuWish : BaseForm
    {
        List<UDT.SSWish> wishList = new List<UDT.SSWish>();
        string sy, s;
        int count;
        public AutoSelectStuWish()
        {
            InitializeComponent();
            

            // Init ComboBox
            for (int i = 0;i < 3; i++)
            {
                schoolYearCbx.Items.Add(int.Parse(School.DefaultSchoolYear) - i + 2);
            }
            schoolYearCbx.SelectedIndex = 0;
            semesterCbx.Items.Add(1);
            semesterCbx.Items.Add(2);
            semesterCbx.SelectedIndex = 0;
            // Init Label
            QueryHelper qh = new QueryHelper();
            string SQL = "SELECT count(*) FROM student WHERE status = 1";
            DataTable dt = qh.Select(SQL);
            stuCountLb.Text = "" + dt.Rows[0]["count"] + " 位學生";

            // Init CourseTypeCbx

            QueryHelper qh2 = new QueryHelper();
            string sql = string.Format(@"
SELECT DISTINCT 
type
FROM
$ischool.course_selection.subject
WHERE

school_year = {0}
AND semester = {1}
        ", schoolYearCbx.Text, semesterCbx.Text);

            DataTable dt2 = qh2.Select(sql);
            foreach (DataRow row in dt2.Rows)
            {
                courseTypeCbx.Items.Add("" + row["type"]);
            }

        }

        private void ReloadSubCountLb()
        {
            if (schoolYearCbx.Text != string.Empty && semesterCbx.Text != string.Empty)
            {
                string SQL = string.Format(@"
                    SELECT count(*) 
                    FROM $ischool.course_selection.subject 
                    WHERE school_year = {0} AND semester = {1}"
                    ,schoolYearCbx.Text,semesterCbx.Text);
                QueryHelper qh = new QueryHelper();
                DataTable dt = qh.Select(SQL);

                subCountLb.Text = "" + dt.Rows[0]["count"];

                count = int.Parse(subCountLb.Text);
            }
        }

        private void schoolYearCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadSubCountLb();
            sy = schoolYearCbx.Text;
        }

        private void semesterCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadSubCountLb();
            s = semesterCbx.Text;
        }

        BackgroundWorker bgw = new BackgroundWorker() { WorkerReportsProgress = true };
        private void AutoSelStuWishBtn_Click(object sender, EventArgs e)
        {
            AccessHelper access = new AccessHelper();
            // 取得學年度、學期、課程類別的所有科目
            string condition = string.Format("school_year = {0} AND semester = {1} AND  type = '{2}'",schoolYearCbx.Text,semesterCbx.Text,courseTypeCbx.Text);
            List<UDT.Subject> subjectList = access.Select<UDT.Subject>(condition);

            if (courseTypeCbx.Text == "")
            {
                MessageBox.Show("請選擇課程類別!");
                return;
            }

            // 取得課程類別可選擇的所有學生
            string sql3 = string.Format(@"
SELECT DISTINCT
	student.id
FROM 
	$ischool.course_selection.subject_class_selection AS scs
	LEFT OUTER JOIN $ischool.course_selection.subject AS subject
		ON subject.uid = scs.ref_subject_id
	LEFT OUTER JOIN student
		ON student.ref_class_id = scs.ref_class_id
WHERE
	subject.school_year = {0}
	AND subject.semester = {1}
	AND type = '{2}'
	AND student.status IN (1,2)
            ", schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql3);

            // 組成學生志願資料
            List<string> datarowList = new List<string>();
            Random random = new Random(1234);
            int studentCount = 0;
            int totalStudentCount = dt.Rows.Count;
            int wishCount = 5;
            foreach (DataRow row in dt.Rows)
            { 
                studentCount++;
                List<int> list = new List<int>(subjectList.Count);
                for (int n = 0; n < subjectList.Count; n++)
                {
                    list.Add(n);
                }
                if (studentCount == (int)(totalStudentCount * 0.5))
                {
                    wishCount = 4;
                }
                if (studentCount == (int)(totalStudentCount * 0.6))
                {
                    wishCount = 3;
                }
                if (studentCount == (int)(totalStudentCount * 0.7))
                {
                    wishCount = 2;
                }
                if (studentCount == (int)(totalStudentCount * 0.8))
                {
                    wishCount = 1;
                }
                int i = 1;
                string studentID = "" + row["id"];
                while (i <= wishCount)
                {
                    int orderIndex = random.Next(list.Count);
                    int order = list[orderIndex];
                    list.RemoveAt(orderIndex);

                    string subjectID = subjectList[order].UID;

                    string datarow = string.Format(@"
                        SELECT 
                           {0}::BIGINT AS ref_student_id
                           , {1}::BIGINT AS ref_subject_id
                           , {2}::INT AS sequence
                    ",studentID, subjectID,i);

                    datarowList.Add(datarow);

                    i++;
                }
                
            }

            string data = string.Join(" UNION ALL", datarowList);
            UpdateHelper up = new UpdateHelper();
            string sql = string.Format(@"
WITH data_row AS(
	{0}
)
INSERT INTO 
	$ischool.course_selection.ss_wish(
		ref_student_id
		, ref_subject_id
		, sequence
	)
	SELECT * FROM data_row"
                ,data);
            up.Execute(sql);

            //MessageBox.Show("執行成功:" + sql);

            //--
            string path = "SQL.txt";
            FileStream file = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(file);
            sw.Write(sql);

            sw.Flush();
            sw.Close();
            file.Close();
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"
DELETE
FROM
    $ischool.course_selection.ss_wish AS wish
WHERE
    wish.ref_subject_id IN(
        SELECT 
            uid
        FROM
            $ischool.course_selection.subject
        WHERE
            school_year = {0}
            AND semester = {1}
    )
            ",schoolYearCbx.Text,semesterCbx.Text);

            UpdateHelper up = new UpdateHelper();
            up.Execute(sql);
            MessageBox.Show("刪除成功");
        }

    }
}
