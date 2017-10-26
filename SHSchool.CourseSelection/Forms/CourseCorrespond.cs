using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;
using K12.Data;
using K12.Presentation;
using FISCA.UDT;
using SHSchool.Data;
using SHSchool.CourseSelection.UDT;

namespace SHSchool.CourseSelection.Forms
{
    public partial class CourseCorrespond : BaseForm
    {
        private QueryHelper _QueryHelper = new QueryHelper();
        private AccessHelper _AccessHelper = new AccessHelper();

        // dictionary<uid,ref_course_id>
        Dictionary<string, string> _DicSubjectCourse = new Dictionary<string, string>();

        public CourseCorrespond()
        {
            InitializeComponent();

            #region ComboBox 初始
            schoolyearcbx.Text = School.DefaultSchoolYear;
            semestercbx.Text = School.DefaultSemester;

            semestercbx.Items.Add("1");
            semestercbx.Items.Add("2");

            for (int i = 0; i < 6; i++)
            {
                schoolyearcbx.Items.Add(int.Parse(School.DefaultSchoolYear) - i);
            }
            #endregion

            #region DataGridView 初始
            int schoolyear = int.Parse(schoolyearcbx.Text);
            int semester = int.Parse(semestercbx.Text);

            ReloadDataGridView(schoolyear, semester);
            #endregion

            #region ComboBox Changed
            schoolyearcbx.TextChanged += delegate
            {
                ReloadDataGridView(int.Parse(schoolyearcbx.Text), int.Parse(semestercbx.Text));
            };
            semestercbx.TextChanged += delegate
            {
                ReloadDataGridView(int.Parse(schoolyearcbx.Text), int.Parse(semestercbx.Text));
            };
            #endregion
        }

        /// <summary>
        /// 重新載入DataGridView
        /// </summary>
        /// <param name="學年"></param>
        /// <param name="學期"></param>
        public void ReloadDataGridView(int sy, int s)
        {
            #region SQL
            string strSQL = string.Format(@"
SELECT 
	subject.*,
	att_count.count AS att_count,
	course.course_name AS ref_course_name,
    sc_att_count.count AS sc_att_count
FROM 
	$ischool.course_selection.subject AS subject
	LEFT OUTER JOIN (
		SELECT 
			ref_subject_id, 
			count(*) 
		FROM 
			$ischool.course_selection.ss_attend 
		GROUP BY ref_subject_id 
	) att_count 
		on att_count.ref_subject_id = subject.uid 
	LEFT OUTER JOIN course
		on course.id = subject.ref_course_id
    LEFT OUTER JOIN (
		SELECT 
			ref_course_id, 
			count(*) 
		FROM 
			sc_attend 
		GROUP BY ref_course_id 
    ) sc_att_count on sc_att_count.ref_course_id = course.id
WHERE 
	subject.school_year = {0} 
	AND subject.semester = {1}
", sy, s);
            #endregion
            DataTable dt = _QueryHelper.Select(strSQL);

            // 重新載入時先清空資料
            dataGridViewX1.Rows.Clear();
            _DicSubjectCourse.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                DataGridViewRow datarow = new DataGridViewRow();

                datarow.CreateCells(dataGridViewX1);
                int index = 0;
                datarow.Cells[index++].Value = "" + dr["type"];
                datarow.Cells[index++].Value = "" + dr["subject_name"];
                datarow.Cells[index++].Value = "" + dr["level"];
                datarow.Cells[index++].Value = "" + dr["credit"];
                datarow.Cells[index++].Value = "" + dr["limit"];
                datarow.Cells[index++].Value = "" + dr["att_count"];
                datarow.Cells[index++].Value = "" + dr["sc_att_count"];
                datarow.Cells[index++].Value = "" + dr["ref_course_name"];
                datarow.Cells[index++].Value = "指定";
                datarow.Cells[index++].Value = "同步";

                // 讀取 Subject uid , Course id
                _DicSubjectCourse.Add("" + dr["uid"], "" + dr["ref_course_id"]);

                // 讀取UID
                dataGridViewX1.Rows[dataGridViewX1.Rows.Add(datarow)].Tag = "" + dr["uid"];
            }
        }

        private void dataGridViewX1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string refCourseID = "";
            List<string> _listCourseID = new List<string>();
            List<UDT.SSAttend> _SSAccessAttend = _AccessHelper.Select<UDT.SSAttend>();
            
            // [指定]
            if (e.ColumnIndex == 8 && e.RowIndex >= 0)
            {
                #region 處理指定對應課程
                string selectUID = "" + dataGridViewX1.Rows[e.RowIndex].Tag;
                int tryParseSchoolYear;
                int tryParseSemester;
                if (int.TryParse(schoolyearcbx.Text, out tryParseSchoolYear))
                    ReloadDataGridView(tryParseSchoolYear, int.Parse(semestercbx.Text));
                if (int.TryParse(semestercbx.Text, out tryParseSemester))
                    ReloadDataGridView(int.Parse(schoolyearcbx.Text), tryParseSemester);

                foreach (var id in _DicSubjectCourse)
                {
                    if (id.Key.ToString() == selectUID)
                    {
                        refCourseID = "" + id.Value;
                    }
                }
                // 顯示課程資料Winform
                CourseCorrespondData courseCorrespondData = new CourseCorrespondData(tryParseSchoolYear, tryParseSemester, selectUID, refCourseID);
                courseCorrespondData.ShowDialog();
                // 課程資料Winform設定完之後，重新reload畫面
                ReloadDataGridView(tryParseSchoolYear, tryParseSemester);
                
                #endregion
            }
            
            // [同步]
            if (e.ColumnIndex == 9 && e.RowIndex >= 0)
            {
                dataGridViewX1.Enabled = false;

                #region 同步課程學生
                {
                    string selectUID = "" + dataGridViewX1.Rows[e.RowIndex].Tag;
                    string subjectName = "" + dataGridViewX1.Rows[e.RowIndex].Cells[1].Value;
                    string courseID = _DicSubjectCourse[selectUID];
                    int tryParseSchoolYear;
                    int tryParseSemester;
                    if (int.TryParse(schoolyearcbx.Text, out tryParseSchoolYear))
                        ReloadDataGridView(tryParseSchoolYear, int.Parse(semestercbx.Text));
                    if (int.TryParse(semestercbx.Text, out tryParseSemester))
                        ReloadDataGridView(int.Parse(schoolyearcbx.Text), tryParseSemester);

                    if (courseID != "" )
                    {
                        BackgroundWorker bkw = new BackgroundWorker();
                        bkw.DoWork += delegate
                        {
                            for (int i = 0; i < 100; i++)
                            {
                                System.Threading.Thread.Sleep(100);
                                bkw.ReportProgress(i + 1);
                            }
                            #region 同步選可學生與課程學生
                            _listCourseID.Add(courseID);
                            List<SCAttendRecord> _listSCAttend = SCAttend.SelectByCourseIDs(_listCourseID);
                            // 清除學生修課資料、評量成績
                            foreach (SCAttendRecord scattend in _listSCAttend)
                            {
                                List<SCETakeRecord>_listSCETake = SCETake.SelectByStudentAndCourse(scattend.RefStudentID, courseID);
                                foreach (SCETakeRecord scetake in _listSCETake)
                                {
                                    SCETake.Delete(scetake);
                                }
                                SCAttend.Delete(scattend);
                            }

                            SCAttendRecord sca = new SCAttendRecord();
                            foreach (var ssAtend in _SSAccessAttend)
                            {
                                if (ssAtend.SubjectID == int.Parse(selectUID))
                                {
                                    sca.RefCourseID = courseID;
                                    sca.RefStudentID = "" + ssAtend.StudentID;

                                    SCAttend.Insert(sca);
                                    SCAttend.Update(_listSCAttend);
                                }
                            }
                            #endregion
                        };
                        bkw.RunWorkerCompleted += delegate
                        {
                            dataGridViewX1.Enabled = true;
                            ReloadDataGridView(tryParseSchoolYear, tryParseSemester);
                            FISCA.Presentation.MotherForm.SetStatusBarMessage($"同步 {subjectName} 選課學生完成");
                        };
                        bkw.ProgressChanged += delegate (object bkws, ProgressChangedEventArgs bkwe)
                        {
                            FISCA.Presentation.MotherForm.SetStatusBarMessage($"同步 {subjectName} 選課學生", bkwe.ProgressPercentage);
                        };
                        bkw.WorkerReportsProgress = true;
                        bkw.RunWorkerAsync();
                    }
                    else
                    {
                        dataGridViewX1.Enabled = true;
                        MessageBox.Show("請選擇對應課程。");
                    } 
                }
                #endregion
            }
        }
    }
}
