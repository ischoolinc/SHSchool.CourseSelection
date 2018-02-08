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
                schoolYearCbx.Items.Add(int.Parse(School.DefaultSchoolYear) - i);
            }
            semesterCbx.Items.Add(1);
            semesterCbx.Items.Add(2);

            // Init Label
            string SQL = "SELECT count(*) FROM student WHERE status = 1";
            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(SQL);

            stuCountLb.Text = "" + dt.Rows[0]["count"] + " 位學生";
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
            bgw.DoWork += bgw_DoWork;
            bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
            bgw.ProgressChanged += bgw_ProgressChanged;
            bgw.RunWorkerAsync();
        }

        public void bgw_DoWork(object sneder,DoWorkEventArgs e)
        {
            // 取得學生ID
            string SQL = "SELECT id AS ref_student_id FROM student WHERE status = 1";
            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(SQL);
            // 取得科目ID
            AccessHelper access = new AccessHelper();
            List<UDT.Subject> subjectList = access.Select<UDT.Subject>("school_year =" + sy + " AND semester = " + s);
            //
            UpdateHelper up = new UpdateHelper();
            string deleteSQL = "DELETE FROM $ischool.course_selection.ss_wish WHERE ";
            List<string> studentIDList = new List<string>();
            bgw.ReportProgress(5);
            // Init Random
            Random random = new Random();
            foreach (DataRow dr in dt.Rows)
            {
                string studentID = "ref_student_id = " + dr["ref_student_id"];
                studentIDList.Add(studentID);

                List<int> orderList = new List<int>();
                int order = 0;
                int s = 1;
                for (int i = 0; i < 3; i++)
                {
                    UDT.SSWish wish = new UDT.SSWish();
                    do
                    {
                        order = random.Next(1, count);
                    } while (orderList.Contains(order));
                    orderList.Add(order);

                    wish.Order = s;

                    wish.SubjectID = int.Parse(subjectList[random.Next(1, order)].UID);
                    wish.StudentID = int.Parse("" + dr["ref_student_id"]);

                    wishList.Add(wish);
                    s++;
                }

                //foreach (UDT.Subject sb in subjectList)
                //{
                //    UDT.SSWish wish = new UDT.SSWish();
                //    wish.StudentID = int.Parse("" + dr["ref_student_id"]);
                //    wish.SubjectID = int.Parse("" + sb.UID);
                    
                //    do
                //    {
                //        order = random.Next(1, count + 1);

                //    } while (orderList.Contains(order));
                //    orderList.Add(order);

                //    wish.Order = order;

                //    wishList.Add(wish);
                //}
            }
            bgw.ReportProgress(30);
            deleteSQL += string.Join(" OR ", studentIDList);
            bgw.ReportProgress(40);
            up.Execute(deleteSQL);
            bgw.ReportProgress(50);
            access.SaveAll(wishList);
            bgw.ReportProgress(100);
        }
        public void bgw_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("儲存成功!");
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            string SQL = "SELECT id AS ref_student_id FROM student WHERE status = 1";
            //QueryHelper qh = 
            List<string> studentIDList = new List<string>();
            string deleteSQL = "DELETE FROM $ischool.course_selection.ss_wish WHERE ";
            deleteSQL += string.Join(" OR ", studentIDList);
        }

        public void bgw_ProgressChanged(object sender,ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage("隨機分配學生選課志願!",e.ProgressPercentage);
        }
    }
}
