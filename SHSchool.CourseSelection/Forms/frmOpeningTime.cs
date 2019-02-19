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
using FISCA.Data;
using System.Xml.Linq;

namespace SHSchool.CourseSelection.Forms
{
    //2016/8/16 穎驊改寫選課系統，為文華專案的其中一部份
    public partial class frmOpeningTime : BaseForm
    {
        private AccessHelper _access;
        private ErrorProvider _errorProvider;
        private bool _initFinish = false;

        public frmOpeningTime()
        {
            InitializeComponent();

            this.Load += new EventHandler(frmOpeningTime_Load);
        }

        private void frmOpeningTime_Load(object sender, EventArgs e)
        {
            this._access = new AccessHelper();

            List<UDT.OpeningTime> listOpeningTime = this._access.Select<UDT.OpeningTime>();

            #region 檢查有沒有選課時間設定資料
            if (listOpeningTime.Count == 0)
            {
                listOpeningTime.Add(new UDT.OpeningTime() { SchoolYear = int.Parse(K12.Data.School.DefaultSchoolYear), Semester = int.Parse(K12.Data.School.DefaultSemester) });
                listOpeningTime.SaveAll();
            } 
            #endregion

            #region InitSchoolYear
            //this.cboSchoolYear.Items.Clear();
            int schoolYear = listOpeningTime[0].SchoolYear; 
            cboSchoolYear.Items.Add(schoolYear);
            cboSchoolYear.Items.Add(schoolYear + 1);
            cboSchoolYear.Items.Add(schoolYear + 2);

            cboSchoolYear.SelectedIndex = 0;
            #endregion

            #region InitSemester
            //this.cboSemester.Items.Clear();
            cboSemester.Items.Add("1");
            cboSemester.Items.Add("2");
            cboSemester.SelectedIndex = listOpeningTime[0].Semester - 1; 
            #endregion

            #region InitSelectMode
            cbxMode.Items.Clear();

            //this.cbxMode.Items.Add("不開放");
            cbxMode.Items.Add("志願序");
            cbxMode.Items.Add("先搶先贏");
            cbxMode.Items.Add("");

            switch (listOpeningTime[0].Mode)
            {
                case "志願序":
                    cbxMode.SelectedIndex = 0;
                    break;
                case "先搶先贏":
                    cbxMode.SelectedIndex = 1;
                    break;
                default:
                    cbxMode.SelectedIndex = 2;
                    break;
            }

            #endregion

            #region InitOpeningTime

            this.StartTime.Text = listOpeningTime[0].StartTime.ToString("yyyy/M/d HH:mm:ss");
            this.EndTime.Text = listOpeningTime[0].EndTime.ToString("yyyy/M/d HH:mm:ss");

            #endregion

            reloadDataGridView(); // 課程時段

            tbxMemo.Text = listOpeningTime[0].Memo; // 備註

            _errorProvider = new ErrorProvider();
            _initFinish = true;
        }

        private void reloadDataGridView()
        {
            string schoolYear = cboSchoolYear.Text;
            string semester = cboSemester.Text;

            #region SQL
            string sql = string.Format(@"
WITH target_type AS(
    SELECT DISTINCT
        type
    FROM
        $ischool.course_selection.subject AS subject
    WHERE
        subject.school_year = {0}
        AND subject.semester = {1}
        AND type IS NOT NULL
) ,open_type AS(
    SELECT
	    unnest(xpath('/Type/text()', type_xml))::text as type
    FROM(
        SELECT
             unnest(xpath('/root/Type', xmlparse(content open_type))) as type_xml
        FROM
            $ischool.course_selection.opening_time
    ) AS  type_xml  
)
SELECT
    target_type.*
    , CASE 
        WHEN target_type.type = open_type.type THEN 'true'
        ELSE 'false'
        END AS is_open_type
FROM
    target_type
    LEFT OUTER JOIN open_type 
        ON open_type.type = target_type.type
                ", schoolYear, semester); 

            #endregion

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql);

            dataGridViewX1.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                DataGridViewRow dgvrow = new DataGridViewRow();
                dgvrow.CreateCells(dataGridViewX1);
                int index = 0;

                dgvrow.Cells[index++].Value = ("" + row["is_open_type"]) == "true" ? true : false;
                dgvrow.Cells[index++].Value = "" + row["type"];

                dataGridViewX1.Rows.Add(dgvrow);
            }
        }

        private bool Is_Validated()
        {
            bool result = true;

            #region 日期區間驗證
            if (string.IsNullOrEmpty(this.StartTime.Text))
            {
                _errorProvider.SetError(this.StartTime, "必填");
                result = false;
            }
            else
            {
                DateTime date;
                if (!DateTime.TryParse(this.StartTime.Text.Trim(), out date))
                {
                    _errorProvider.SetError(this.StartTime, "非日期格式");
                    result = false;
                }
                else
                    _errorProvider.SetError(this.StartTime, "");
            }

            if (string.IsNullOrEmpty(this.EndTime.Text))
            {
                _errorProvider.SetError(this.EndTime, "必填");
                result = false;
            }
            else
            {
                DateTime date;
                if (!DateTime.TryParse(this.EndTime.Text.Trim(), out date))
                {
                    _errorProvider.SetError(this.EndTime, "非日期格式");
                    result = false;
                }
                else
                    _errorProvider.SetError(this.EndTime, "");
            } 
            #endregion

            #region 學年學期驗證
            if (string.IsNullOrEmpty(this.cboSchoolYear.Text))
            {
                _errorProvider.SetError(this.cboSchoolYear, "必填");
                result = false;
            }
            else
            {
                _errorProvider.SetError(this.cboSchoolYear, "");
            }

            if (string.IsNullOrEmpty(this.cboSemester.Text))
            {
                _errorProvider.SetError(this.cboSemester, "必填");
                result = false;
            }
            else
            {
                _errorProvider.SetError(this.cboSemester, "");
            } 
            #endregion

            #region 選課模式驗證
            if (string.IsNullOrEmpty(this.cbxMode.Text))
            {
                _errorProvider.SetError(this.cbxMode, "必填");
                result = false;
            }
            else
            {
                _errorProvider.SetError(this.cbxMode, "");
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
                List<UDT.OpeningTime> records = _access.Select<UDT.OpeningTime>();

                if (records == null || records.Count == 0)
                    record = new UDT.OpeningTime();
                else
                    record = records[0];

                record.SchoolYear = int.Parse(this.cboSchoolYear.Text); // 學年度
                record.Semester = int.Parse(this.cboSemester.Text); // 學期
                record.Mode = cbxMode.SelectedItem.ToString(); // 選課模式
                record.StartTime = DateTime.Parse(this.StartTime.Text); // 開始時間      
                record.EndTime = DateTime.Parse(this.EndTime.Text); // 結束時間

                //DateTime endTime = DateTime.Parse(this.EndTime.Text);
                //if ((endTime.Hour + endTime.Minute + endTime.Second) == 0)
                //    record.EndTime = endTime.AddDays(1).AddSeconds(-1);
                //else
                //    record.EndTime = endTime;
                

                #region 開放課程時段

                List<string> listType = new List<string>();

                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    if (bool.Parse("" + dgvrow.Cells[0].Value))
                    {
                        listType.Add(string.Format("<Type>{0}</Type>", dgvrow.Cells[1].Value));
                    }
                }

                record.OpenType = string.Format("<root>{0}</root>", string.Join("", listType));

                #endregion

                record.Memo = tbxMemo.Text; // 備註

                record.Save();

                MsgBox.Show("儲存成功");
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._initFinish)
            {
                this.reloadDataGridView();
            }
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._initFinish)
            {
                this.reloadDataGridView();
            }
        }
    }
}
