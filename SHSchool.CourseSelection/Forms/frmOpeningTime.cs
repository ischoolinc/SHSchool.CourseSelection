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
using DevComponents.DotNetBar.Controls;

namespace SHSchool.CourseSelection.Forms
{
    //2016/8/16 穎驊改寫選課系統，為文華專案的其中一部份

    public partial class frmOpeningTime : BaseForm
    {
        private AccessHelper Access;
        private ErrorProvider errorProvider1;

        public frmOpeningTime()
        {
            InitializeComponent();

            this.Load += new EventHandler(frmOpeningTime_Load);
        }

        private void frmOpeningTime_Load(object sender, EventArgs e)
        {
            this.Access = new AccessHelper();

            this.InitSchoolYear();
            this.InitSemester();
            this.initSelectMode();
            this.InitOpeningTime();
            

            errorProvider1 = new ErrorProvider();
        }

        private void InitOpeningTime()
        {
            try
            {
                List<UDT.OpeningTime> records = Access.Select<UDT.OpeningTime>();

                if (records == null || records.Count == 0)
                    return;

                this.StartTime1.Text = records[0].P1StartTime.ToString("yyyy/M/d HH:mm:ss");
                
                this.EndTime1.Text = records[0].P1EndTime.ToString("yyyy/M/d HH:mm:ss");

                this.StartTime2.Text = records[0].P2StartTime.ToString("yyyy/M/d HH:mm:ss");

                this.EndTime2.Text = records[0].P2EndTime.ToString("yyyy/M/d HH:mm:ss");


             

                for (int i = 0; i < this.cboSchoolYear.Items.Count; i++)
                {
                    if (this.cboSchoolYear.Items[i].ToString() == records[0].SchoolYear.ToString())
                    {
                        this.cboSchoolYear.Text = records[0].SchoolYear.ToString();
                        break;
                    }
                }

                for (int i = 0; i < this.cboSemester.Items.Count; i++)
                {
                    if (this.cboSemester.Items[i].ToString() == records[0].Semester.ToString())
                    {
                        this.cboSemester.Text = records[0].Semester.ToString();
                        break;
                    }
                }


                for (int i = 0; i < this.P1ModecomboBoxEx.Items.Count; i++)
                {
                    if (this.P1ModecomboBoxEx.Items[i].ToString() == records[0].P1Mode.ToString())
                    {
                        this.P1ModecomboBoxEx.Text = records[0].P1Mode.ToString();
                        break;
                    }
                }

                for (int i = 0; i < this.P2ModecomboBoxEx.Items.Count; i++)
                {
                    if (this.P2ModecomboBoxEx.Items[i].ToString() == records[0].P2Mode.ToString())
                    {
                        this.P2ModecomboBoxEx.Text = records[0].P2Mode.ToString();
                        break;
                    }
                }

                this.MemotextBoxX.Text = ""+records[0].Memo;

            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        private void InitSchoolYear()
        {
            this.cboSchoolYear.Items.Clear();

            int school_year;

            if (!int.TryParse(K12.Data.School.DefaultSchoolYear, out school_year))
                school_year = DateTime.Today.Year - 1911;

            this.cboSchoolYear.Items.Add("");
            this.cboSchoolYear.Items.Add(school_year);
            this.cboSchoolYear.Items.Add(school_year + 1);
            this.cboSchoolYear.Items.Add(school_year + 2);
        }

        private void InitSemester()
        {
            this.cboSemester.Items.Clear();

            this.cboSemester.Items.Add("");
            this.cboSemester.Items.Add("1");
            this.cboSemester.Items.Add("2");
        }

        private void initSelectMode() 
        {
            this.P1ModecomboBoxEx.Items.Clear();
            this.P2ModecomboBoxEx.Items.Clear();

            this.P1ModecomboBoxEx.Items.Add("不開放");
            this.P1ModecomboBoxEx.Items.Add("志願序");
            this.P1ModecomboBoxEx.Items.Add("先搶先贏");

            this.P2ModecomboBoxEx.Items.Add("不開放");
            this.P2ModecomboBoxEx.Items.Add("志願序");
            this.P2ModecomboBoxEx.Items.Add("先搶先贏");
        
        }


        private bool Is_Validated()
        {
            bool result = true;


            #region 階段一輸入驗證
            if (string.IsNullOrEmpty(this.StartTime1.Text))
            {
                errorProvider1.SetError(this.StartTime1, "必填");
                result = false;
            }
            else
            {
                DateTime date;
                if (!DateTime.TryParse(this.StartTime1.Text.Trim(), out date))
                {
                    errorProvider1.SetError(this.StartTime1, "非日期格式");
                    result = false;
                }
                else
                    errorProvider1.SetError(this.StartTime1, "");
            }

            if (string.IsNullOrEmpty(this.EndTime1.Text))
            {
                errorProvider1.SetError(this.EndTime1, "必填");
                result = false;
            }
            else
            {
                DateTime date;
                if (!DateTime.TryParse(this.EndTime1.Text.Trim(), out date))
                {
                    errorProvider1.SetError(this.EndTime1, "非日期格式");
                    result = false;
                }
                else
                    errorProvider1.SetError(this.EndTime1, "");
            } 
            #endregion

            #region 階段二輸入驗證
            if (string.IsNullOrEmpty(this.StartTime2.Text))
            {
                errorProvider1.SetError(this.StartTime2, "必填");
                result = false;
            }
            else
            {
                DateTime date;
                if (!DateTime.TryParse(this.StartTime2.Text.Trim(), out date))
                {
                    errorProvider1.SetError(this.StartTime2, "非日期格式");
                    result = false;
                }
                else
                    errorProvider1.SetError(this.StartTime2, "");
            }

            if (string.IsNullOrEmpty(this.EndTime2.Text))
            {
                errorProvider1.SetError(this.EndTime2, "必填");
                result = false;
            }
            else
            {
                DateTime date;
                if (!DateTime.TryParse(this.EndTime2.Text.Trim(), out date))
                {
                    errorProvider1.SetError(this.EndTime2, "非日期格式");
                    result = false;
                }
                else
                    errorProvider1.SetError(this.EndTime2, "");
            } 
            #endregion

            #region 學年學期驗證
            if (string.IsNullOrEmpty(this.cboSchoolYear.Text))
            {
                errorProvider1.SetError(this.cboSchoolYear, "必填");
                result = false;
            }
            else
            {
                errorProvider1.SetError(this.cboSchoolYear, "");
            }

            if (string.IsNullOrEmpty(this.cboSemester.Text))
            {
                errorProvider1.SetError(this.cboSemester, "必填");
                result = false;
            }
            else
            {
                errorProvider1.SetError(this.cboSemester, "");
            } 
            #endregion


            #region 選課模式驗證
            if (string.IsNullOrEmpty(this.P1ModecomboBoxEx.Text))
            {
                errorProvider1.SetError(this.P1ModecomboBoxEx, "必填");
                result = false;
            }
            else
            {
                errorProvider1.SetError(this.P1ModecomboBoxEx, "");
            }

            if (string.IsNullOrEmpty(this.P2ModecomboBoxEx.Text))
            {
                errorProvider1.SetError(this.P2ModecomboBoxEx, "必填");
                result = false;
            }
            else
            {
                errorProvider1.SetError(this.P2ModecomboBoxEx, "");
            }
            #endregion

            return result;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!Is_Validated())
                return;

            try
            {
                UDT.OpeningTime record;
                List<UDT.OpeningTime> records = Access.Select<UDT.OpeningTime>();

                if (records == null || records.Count == 0)
                    record = new UDT.OpeningTime();
                else
                    record = records[0];

                //設定階段一時段
                DateTime P1startTime = DateTime.Parse(this.StartTime1.Text);

                record.P1StartTime = P1startTime;                

                DateTime P1endTime = DateTime.Parse(this.EndTime1.Text);
                if ((P1endTime.Hour + P1endTime.Minute + P1endTime.Second) == 0)
                    record.P1EndTime = P1endTime.AddDays(1).AddSeconds(-1);
                else
                    record.P1EndTime = P1endTime;


                //設定階段二時段
                DateTime P2startTime = DateTime.Parse(this.StartTime2.Text);

                record.P2StartTime = P2startTime;

                DateTime P2endTime = DateTime.Parse(this.EndTime2.Text);
                if ((P2endTime.Hour + P2endTime.Minute + P2endTime.Second) == 0)
                    record.P2EndTime = P2endTime.AddDays(1).AddSeconds(-1);
                else
                    record.P2EndTime = P2endTime;


                record.SchoolYear = int.Parse(this.cboSchoolYear.Text);
                record.Semester = int.Parse(this.cboSemester.Text);

                record.P1Mode = P1ModecomboBoxEx.Text;

                record.P2Mode = P2ModecomboBoxEx.Text;

                record.Memo = MemotextBoxX.Text;

                record.Save();

                MsgBox.Show("儲存成功");
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        private void StartTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelX1_Click(object sender, EventArgs e)
        {

        }

        private void labelX2_Click(object sender, EventArgs e)
        {

        }

        private void EndTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void labelX3_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxEx2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
