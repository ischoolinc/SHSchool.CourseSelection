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
        Dictionary<string, string> classDic = new Dictionary<string, string>();
        List<string> selectClassDic = new List<string>();

        private ContextMenu _menu = new ContextMenu(); 

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

            // Init datagridview 右鍵選單
            MenuItem add = new MenuItem("可選");
            add.Click += delegate 
            {
                foreach (DataGridViewRow datarow in dataGridViewX1.SelectedRows)
                {
                    datarow.Cells[0].Value = "Checked";
                }
            };
            _menu.MenuItems.Add(add);
            MenuItem remove = new MenuItem("不可選");
            remove.Click += delegate 
            {
                foreach (DataGridViewRow datarow in dataGridViewX1.SelectedRows)
                {
                    datarow.Cells[0].Value = "Unchecked";
                }
            };
            _menu.MenuItems.Add(remove);

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
                if (subjectCountDic.ContainsKey("" + dr["uid"]) && "" + dr["ref_subject_id"] != "")
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

            foreach (DataRow dr in dt.Rows)
            {
                int index = 0;

                DataGridViewRow datarow = new DataGridViewRow();
                datarow.CreateCells(dataGridViewX1);

                if (!subjectIDDic.ContainsKey("" + dr["uid"]))
                {
                    subjectIDDic.Add("" + dr["uid"], "" + dr["subject_name"]);
                    // 科目勾選狀態
                    if (subjectCountDic["" + dr["uid"]] == classDic.Count)
                    {
                        datarow.Cells[index].Value = "Checked";
                    }
                    if (subjectCountDic["" + dr["uid"]] == 0)
                    {
                        datarow.Cells[index].Value = "Unchecked";
                    }
                    if (subjectCountDic["" + dr["uid"]] != 0 && subjectCountDic["" + dr["uid"]] < classDic.Count)
                    {
                        datarow.Cells[index].Value = "Indeterminate";
                    }
                    index++;

                    datarow.Cells[index++].Value = "" + dr["subject_name"];
                    datarow.Cells[index++].Value = "" + dr["level"];
                    datarow.Cells[index++].Value = "" + dr["credit"];
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
            dataGridViewX1.Enabled = false;
            saveBtn.Enabled = false;
            leaveBtn.Enabled = false;

            BGW.DoWork += BGW_DoWork;

            BGW.ProgressChanged += BGW_ProgressChanged;

            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;

            BGW.RunWorkerAsync();

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

                string subjectIDs = string.Join(",",subjectIDList);
                foreach (var _class in classDic)
                {
                    progress += 40 / classDic.Count;
                    BGW.ReportProgress(progress);

                    List<UDT.SubjectClassSelection> scsOLDList = access.Select<UDT.SubjectClassSelection>("ref_class_id = " + _class.Key + " AND ref_subject_id IN(" + subjectIDs + ")");
                    access.DeletedValues(scsOLDList);
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

            this.Close();
        }

        private void dataGridViewX1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _menu.Show(dataGridViewX1,new Point(e.X,e.Y));
            }
        }


    }
}
