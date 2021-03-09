using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;
using FISCA.Data;
using FISCA.Presentation.Controls;

namespace SHSchool.CourseSelection.Forms
{
    public partial class frmSubjectCreator : Form
    {
        private UDT.Subject mRecord;
        private ErrorProvider errorProvider1;
        private string school_year;
        private string semester;
        private QueryHelper qh = new QueryHelper();
        private Dictionary<string, DataRow> dicSubjectByKey = new Dictionary<string, DataRow>();

        public frmSubjectCreator(string title, string school_year, string semester)
        {
            InitializeComponent();

            Event.DeliverActiveRecord.Received += new EventHandler<Event.DeliverActiveRecordEventArgs>(Subject_Received);

            this.Text = title;

            this.school_year = school_year;
            this.semester = semester;

            this.Load += new EventHandler(frmSubjectCreator_Load);

            if (title == "檢視科目資料")
            {
                this.SubjectName.Enabled = false;
                this.Institute.Enabled = false;
                this.Level.Enabled = false;
                this.Credit.Enabled = false;
                this.Type.Enabled = false;
                this.Limit.Enabled = false;
                this.Goal.Enabled = false;
                this.Content.Enabled = false;
                this.Memo.Enabled = false;

                this.Save.Enabled = false;
                this.Cancel.Enabled = false;
                //---
                this.tbxPreSubject.Enabled = false;
                this.tbxPreSubjectLevel.Enabled = false;
                this.tbxCrossType1.Enabled = false;
                this.tbxCrossType2.Enabled = false;
                this.cbxPreSubjectBlockMode.Enabled = false;
                this.cbxRejoinMode.Enabled = false;
                this.ckbxDisable.Enabled = false;
            }
        }

        public frmSubjectCreator(string title) : this(title, string.Empty, string.Empty) { }

        private void frmSubjectCreator_Load(object sender, EventArgs e)
        {
            if (this.mRecord == null)
            {
                this.mRecord = new UDT.Subject();
                this.mRecord.SchoolYear = int.Parse(this.school_year);
                this.mRecord.Semester = int.Parse(this.semester);
            }

            GetSubjectList();
        }

        /// <summary>
        /// 取得學年度學期課程清單 用來判斷科目級別是否重複
        /// </summary>
        private void GetSubjectList()
        {
            string sql = string.Format(@"
SELECT
    *
FROM
    $ischool.course_selection.subject
WHERE
    school_year = {0}
    AND semester = {1}
            ", this.school_year, this.semester);

            DataTable dt = qh.Select(sql);

            foreach (DataRow row in dt.Rows)
            {
                string key = $"{row["subject_name"]}_{row["level"]}_{row["Type"]}";

                if (!dicSubjectByKey.ContainsKey(key))
                {
                    dicSubjectByKey.Add(key, row);
                }
            }
        }

        private void Subject_Received(object sender, Event.DeliverActiveRecordEventArgs e)
        {
            IEnumerable<ActiveRecord> records = e.ActiveRecords;
            if (records == null || records.Count() == 0)
            {
                mRecord = new UDT.Subject();
                return;
            }

            mRecord = records.ElementAt(0) as UDT.Subject;

            this.SubjectName.Text = mRecord.SubjectName.Trim();
            this.Institute.Text = mRecord.Institute.Trim();
            this.Level.Text = mRecord.Level + "";
            this.Credit.Text = mRecord.Credit + "";
            this.Type.Text = mRecord.Type.Trim();
            this.Limit.Text = mRecord.Limit.ToString();
            this.Goal.Text = mRecord.Goal.Trim().Replace("\n", "\r\n");
            this.Content.Text = mRecord.Content.Trim().Replace("\n", "\r\n");
            this.Memo.Text = mRecord.Memo.Trim().Replace("\n", "\r\n");
            //--
            this.tbxPreSubject.Text = mRecord.PreSubject.Trim();
            this.tbxPreSubjectLevel.Text = mRecord.PreSubjectLevel.ToString().Trim() == "0" ? "" : mRecord.PreSubjectLevel.ToString().Trim();
            this.tbxCrossType1.Text = mRecord.CrossType1.Trim();
            this.tbxCrossType2.Text = mRecord.CrossType2.Trim();
            this.cbxPreSubjectBlockMode.Text = mRecord.PreSubjectBlockMode.Trim();
            this.cbxRejoinMode.Text = mRecord.RejoinBlockMode.Trim();
            this.ckbxDisable.Checked = mRecord.Disabled;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Is_Valid())
                    return;

                mRecord.SubjectName = this.SubjectName.Text.Trim();
                if (!string.IsNullOrWhiteSpace(this.Level.Text))
                    mRecord.Level = int.Parse(this.Level.Text.Trim());
                else
                    mRecord.Level = null;
                if (!string.IsNullOrWhiteSpace(this.Credit.Text))
                    mRecord.Credit = int.Parse(this.Credit.Text.Trim());
                else
                    mRecord.Credit = null;
                mRecord.Institute = this.Institute.Text.Trim();
                mRecord.Type = this.Type.Text.Trim();
                mRecord.Limit = int.Parse(this.Limit.Text.Trim());
                mRecord.Goal = this.Goal.Text.Trim();
                mRecord.Content = this.Content.Text.Trim();
                mRecord.Memo = this.Memo.Text.Trim();
                //---
                mRecord.PreSubject = this.tbxPreSubject.Text.Trim();
                if (!string.IsNullOrWhiteSpace(this.tbxPreSubjectLevel.Text))
                {
                    mRecord.PreSubjectLevel = int.Parse(this.tbxPreSubjectLevel.Text.Trim());
                }
                else
                {
                    mRecord.PreSubjectLevel = null;
                }
                if (this.cbxPreSubjectBlockMode.SelectedItem == null)
                {
                    mRecord.PreSubjectBlockMode = null;
                }
                else
                {
                    mRecord.PreSubjectBlockMode = this.cbxPreSubjectBlockMode.SelectedItem.ToString().Trim();
                }
                if (this.cbxRejoinMode.SelectedItem == null)
                {
                    mRecord.RejoinBlockMode = null;
                }
                else
                {
                    mRecord.RejoinBlockMode = this.cbxRejoinMode.SelectedItem.ToString().Trim();
                }
                
                mRecord.Disabled = this.ckbxDisable.Checked;
                mRecord.CrossType1 = this.tbxCrossType1.Text.Trim();
                mRecord.CrossType2 = this.tbxCrossType2.Text.Trim();

                mRecord.Save();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
                this.DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }

        private bool Is_Valid()
        {
            errorProvider1 = new ErrorProvider();
            int no;
            bool result;
            bool is_valid = true;

            // 驗證科目名稱
            if (string.IsNullOrWhiteSpace(this.SubjectName.Text))
            {
                errorProvider1.SetError(this.SubjectName, "必填");
                is_valid = false;
            }
            else
                errorProvider1.SetError(this.SubjectName, "");

            // 驗證級別
            if (!string.IsNullOrWhiteSpace(this.Level.Text))
            {
                result = int.TryParse(this.Level.Text.Trim(), out no);

                if (!result)
                {
                    errorProvider1.SetError(this.Level, "請填阿拉伯數字");
                    is_valid = false;
                }
                else
                    errorProvider1.SetError(this.Level, "");
            }
            else
                errorProvider1.SetError(this.Level, "");

            //// 驗證學分數
            //if (!string.IsNullOrWhiteSpace(this.Credit.Text))
            //{
            //    result = int.TryParse(this.Credit.Text.Trim(), out no);

            //    if (!result)
            //    {
            //        errorProvider1.SetError(this.Credit, "請填阿拉伯數字");
            //        is_valid = false;
            //    }
            //    else
            //        errorProvider1.SetError(this.Credit, "");
            //}
            //else
            //    errorProvider1.SetError(this.Credit, "");

            // 驗證學分數
            if (string.IsNullOrWhiteSpace(this.Credit.Text))
            {
                errorProvider1.SetError(this.Credit, "必填");
                is_valid = false;
            }
            else
            {
                result = int.TryParse(this.Credit.Text.Trim(), out no);

                if (!result)
                {
                    errorProvider1.SetError(this.Credit, "請輸入正整數或0。");
                    is_valid = false;
                }
                else
                    errorProvider1.SetError(this.Credit, "");
            }
            // 驗證人數上限
            if (string.IsNullOrWhiteSpace(this.Limit.Text))
            {
                errorProvider1.SetError(this.Limit, "必填");
                is_valid = false;
            }
            else
            {
                result = int.TryParse(this.Limit.Text.Trim(), out no);

                if (!result)
                {
                    errorProvider1.SetError(this.Limit, "請輸入正整數。");
                    is_valid = false;
                }
                else
                    errorProvider1.SetError(this.Limit, "");
            }

            // 驗證前導課程級別
            if (!string.IsNullOrWhiteSpace(this.tbxPreSubjectLevel.Text))
            {
                result = int.TryParse(this.tbxPreSubjectLevel.Text.Trim(), out no);

                if (!result)
                {
                    errorProvider1.SetError(this.tbxPreSubjectLevel, "請填阿拉伯數字");
                    is_valid = false;
                }
                else
                    errorProvider1.SetError(this.tbxPreSubjectLevel, "");
            }
            else
                errorProvider1.SetError(this.tbxPreSubjectLevel, "");

            // 驗證若有前導課程科目，前導課程採計方式必填
            if (!string.IsNullOrWhiteSpace(tbxPreSubject.Text))
            {
                if (cbxPreSubjectBlockMode.SelectedItem == null)
                {
                    errorProvider1.SetError(this.cbxPreSubjectBlockMode, "請選擇前導課程採計方式。");
                    is_valid = false;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(cbxPreSubjectBlockMode.SelectedItem.ToString()))
                    {
                        errorProvider1.SetError(this.cbxPreSubjectBlockMode, "請選擇前導課程採計方式。");
                        is_valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(this.cbxPreSubjectBlockMode, "");
                    }
                }
            }


            // 驗證學年度學期科目級別 不可重複
            string key = $"{SubjectName.Text}_{Level.Text}_{Type.Text}";
            if (dicSubjectByKey.ContainsKey(key))
            {
                if (mRecord.UID == "" + dicSubjectByKey[key]["uid"])
                {
                    errorProvider1.SetError(this.SubjectName, "");
                }
                else
                {
                    errorProvider1.SetError(this.SubjectName, "科目名稱 + 級別 + 課程時段 不可重複。");
                    is_valid = false;
                }
            }
            else
            {
                errorProvider1.SetError(this.SubjectName, "");
            }

            return is_valid;
        }
    }
}