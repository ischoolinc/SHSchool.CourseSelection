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
using FISCA.UDT;
using K12.Data;
using FISCA.Presentation;

namespace SHSchool.CourseSelection.Forms
{
    public partial class ClassSelectCourse_Setting : BaseForm
    {
        DataGridViewCheckBoxColumn dgvCkbx = new DataGridViewCheckBoxColumn();

        Dictionary<string, string> classDic = new Dictionary<string, string>();
        List<string> selectClassDic = new List<string>();

        public ClassSelectCourse_Setting(string sy,string s,string courseType,Dictionary<string,string>selectedClassDic)
        {
            InitializeComponent();

            classDic = selectedClassDic;
            #region Init Label
            {
                schoolYearLb.Text = sy;
                semesterLb.Text = s;
                courseTypeLb.Text = courseType;
                //DevComponents.DotNetBar.LabelX lb = new DevComponents.DotNetBar.LabelX();
                selectClassDic = selectedClassDic.Values.ToList();
                selectClassDic.Sort();
                classNameLb.Text = string.Join("、", selectClassDic);
                
            }
            #endregion  

            ReloadDataGridView(sy,s,courseType);
        }

        public void ReloadDataGridView(string sy,string s,string courseType)
        {
            string condition = "ref_class_id = " + string.Join(" OR ref_class_id = ", classDic.Keys.ToList());

            #region SQL
            string selectSQL = string.Format(@"
                SELECT
	                subject.uid,
                    subject.subject_name,
                    subject.level,
                    subject.credit,
	                subject_class.ref_subject_id,
                    subject_class.ref_class_id
                FROM 
	                $ischool.course_selection.subject AS subject
                    LEFT OUTER JOIN
                    (
	                    SELECT
                            ref_subject_id,
                            ref_class_id
	                    FROM
		                    $ischool.course_selection.subject_class_selection
	                    WHERE 
                            {3}
                    )subject_class ON subject_class.ref_subject_id = subject.uid
                WHERE school_year = {0} AND semester = {1} AND type = '{2}' 
                ", sy,s,courseType, condition);
            #endregion

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(selectSQL);

            //  紀錄每個科目對應的班級數
            Dictionary<string, int> subjectCountDic = new Dictionary<string, int>();
            foreach (DataRow dr in dt.Rows)
            {
                if (subjectCountDic.ContainsKey("" + dr["uid"]))
                {
                    subjectCountDic["" + dr["uid"]] += 1;
                }
                if (!subjectCountDic.ContainsKey("" + dr["uid"]) && "" + dr["ref_subject_id"] != "")
                {
                    subjectCountDic.Add("" + dr["uid"],1);
                }
                if (!subjectCountDic.ContainsKey("" + dr["uid"]) && "" + dr["ref_subject_id"] == "")
                {
                    subjectCountDic.Add("" + dr["uid"], 0);
                }
            }
            // 避免DataGridView新增相同科目
            Dictionary<string, string> subjectIDDic = new Dictionary<string, string>();

            // DGV新增checkbox
            dgvCkbx.HeaderText = "加選";
            dgvCkbx.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            dgvCkbx.Width = 45;
            dgvCkbx.ThreeState = true;
            dataGridViewX1.Columns.Insert(0,dgvCkbx);

            foreach (DataRow dr in dt.Rows)
            {                
                int index = 0;

                DataGridViewRow datarow = new DataGridViewRow();
                datarow.CreateCells(dataGridViewX1);

                if (!subjectIDDic.ContainsKey("" + dr["uid"]))
                {
                    subjectIDDic.Add("" + dr["uid"],"" + dr["subject_name"]);
                    // 科目勾選狀態
                    if (subjectCountDic["" + dr["uid"]] == classDic.Count)
                    {
                        datarow.Cells[index].Value = CheckState.Checked;
                    }
                    if (subjectCountDic["" + dr["uid"]] == 0)
                    {
                        datarow.Cells[index].Value = CheckState.Unchecked;
                    }
                    if (subjectCountDic["" + dr["uid"]] != 0 && subjectCountDic["" + dr["uid"]] != classDic.Count)
                    {
                        datarow.Cells[index].Value = CheckState.Indeterminate;
                    }
                    index++;

                    datarow.Cells[index++].Value = dr["subject_name"];
                    datarow.Cells[index++].Value = dr["level"];
                    datarow.Cells[index++].Value = dr["credit"];
                    datarow.Tag = dr["uid"];

                    dataGridViewX1.Rows.Add(datarow);
                }
            }
        }

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        BackgroundWorker BGW = new BackgroundWorker() { WorkerReportsProgress = true };

        private void saveBtn_Click(object sender, EventArgs e)
        {
            BGW.DoWork += BGW_DoWork;

            BGW.ProgressChanged += BGW_ProgressChanged;

            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;

            BGW.RunWorkerAsync();

            this.Close();

        }

        public void BGW_DoWork(object sender,DoWorkEventArgs e)
        {
            try
            {
                int progress = 10;
                AccessHelper access = new AccessHelper();
                List<UDT.SubjectClassSelection> scsNewList = access.Select<UDT.SubjectClassSelection>();

                // 讀取要修改的科目資料 CheckState != Indeterminate
                List<string> subjectIDList = new List<string>();
                foreach (DataGridViewRow dr in dataGridViewX1.Rows)
                {
                    if ("" + dr.Cells[0].Value != "Indeterminate")
                    {
                        subjectIDList.Add("" + dr.Tag);
                    }
                }
                BGW.ReportProgress(progress);
                // 刪除舊資料(班級選課管理):刪除CheckState != Indeterminate 的資料
                foreach (var _class in classDic)
                {
                    progress += 40 / classDic.Count;
                    BGW.ReportProgress(progress);
                    foreach (string subjectID in subjectIDList)
                    {
                        List<UDT.SubjectClassSelection> scsOLDList = access.Select<UDT.SubjectClassSelection>("ref_class_id = " + _class.Key + " AND ref_subject_id =" + subjectID);
                        access.DeletedValues(scsOLDList);
                    }
                }

                // 新增資料(班級選課管理):新增CheckState == Checked 的資料
                foreach (DataGridViewRow dr in dataGridViewX1.Rows)
                {
                    progress += 60 / dataGridViewX1.Rows.Count;
                    BGW.ReportProgress(progress);
                    // 勾選的科目
                    if ("" + dr.Cells[0].Value == "Checked")
                    {
                        // 班級
                        foreach (var _class in classDic)
                        {
                            UDT.SubjectClassSelection scs = new UDT.SubjectClassSelection();
                            scs.RefSubjectID = int.Parse("" + dr.Tag);
                            scs.RefClassID = int.Parse(_class.Key);

                            scsNewList.Add(scs);
                        }

                    }
                }
                // 儲存新資料
                scsNewList.SaveAll();
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        public void BGW_ProgressChanged(object sender,ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage("資料儲存", e.ProgressPercentage);
        }

        public void BGW_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                MessageBox.Show("儲存成功");
                MotherForm.SetStatusBarMessage("班級選課管理--資料儲存成功");
            }
            if (e.Result != null)
            {
                MessageBox.Show("" + e.Result);
            }
        }

        private void dataGridViewX1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex == 0)
            //{
            //    if ("" + dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "" + CheckState.Indeterminate)
            //    {
            //        dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = CheckState.Unchecked;
            //        return;
            //    }
            //    if ("" + dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "" + CheckState.Unchecked)
            //    {
            //        dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = CheckState.Checked;
            //        return;
            //    }
            //    if ("" + dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "" + CheckState.Checked)
            //    {
            //        dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = CheckState.Unchecked;
            //        return;
            //    }
            //}
            //dgvCkbx.ThreeState = false;
        }
    }
}
