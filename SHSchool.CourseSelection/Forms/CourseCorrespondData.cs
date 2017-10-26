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
using FISCA.UDT;
using FISCA.Data;

namespace SHSchool.CourseSelection.Forms
{
    public partial class CourseCorrespondData : BaseForm
    {
        private string _SelectCourseID = "";
        private string _SelectUID = "";
        private string _RefCourseID;
        private int _SchoolYear;
        private int _Semester;

        private List<CourseRecord> _CourseList;

        public CourseCorrespondData(int _schoolyear, int _semester, string _selectUID, string _refCourseID)
        {
            InitializeComponent();

            #region 取得參數值
            _SelectUID = _selectUID;
            _SchoolYear = _schoolyear;
            _Semester = _semester;
            _RefCourseID = _refCourseID;
            #endregion

            // label初始值
            labelX1.Text = "學年:  " + _schoolyear + "         學期:  " + _semester;
            // 取得學年學期課程資料
            _CourseList = Course.SelectBySchoolYearAndSemester(_schoolyear, _semester);

            #region ComboBox 初始值

            List<string> subjectList = new List<string>();
            subjectList.Add("");
            string selectCourseSubject = null;
            foreach (CourseRecord cr in _CourseList)
            {
                if (!subjectList.Contains(cr.Subject))
                    subjectList.Add(cr.Subject);
                if (cr.ID == _refCourseID)
                    selectCourseSubject = cr.Subject;
            }
            subjectcbx.Items.AddRange(subjectList.ToArray());
            if (selectCourseSubject != null && subjectList.Contains(selectCourseSubject))
            {
                subjectcbx.Text = selectCourseSubject;
            }

            #endregion

            ReloadListViewItem();
        }

        public void ReloadListViewItem()
        {
            listViewEx1.Items.Clear();
            int n = 0;
            foreach (CourseRecord cr in _CourseList)
            {
                // 初始 ListViewItem
                # region 篩選相同科目課程。或原有對應課程。

                if (subjectcbx.SelectedItem != null && subjectcbx.Text == cr.Subject || cr.ID == _RefCourseID)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = cr.Name;
                    lvi.Name = cr.ID;

                    listViewEx1.Items.Add(lvi);
                    // 如果是原有的對應課程 預設為勾選，並且調成第一順位。
                    if (cr.ID == _RefCourseID)
                    {
                        listViewEx1.Items[n].Text = listViewEx1.Items[0].Text;
                        listViewEx1.Items[n].Name = listViewEx1.Items[0].Name;

                        listViewEx1.Items[0].Text = cr.Name;
                        listViewEx1.Items[0].Name = cr.ID;
                        listViewEx1.Items[0].Checked = true;
                    }
                    n++;
                }
                #endregion

                #region 如果選擇的 subject_ComboBox.Text為空白項目
                if (subjectcbx.Text == "" && cr.ID != _RefCourseID)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = cr.Name;
                    lvi.Name = cr.ID;
                    listViewEx1.Items.Add(lvi);
                }
                #endregion

            }
        }

        private void listViewEx1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                // 取得勾選課程ID
                _SelectCourseID = e.Item.Name;
                // ListViewItem 設定為單選
                for (int n = 0; n < listViewEx1.Items.Count; n++)
                {
                    if (listViewEx1.Items[n] != null)
                    {
                        if (listViewEx1.Items[n].Name != e.Item.Name)
                        {
                            listViewEx1.Items[n].Checked = false;
                        }
                    }
                }
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // 取得 Subject.UDT 的資料
            AccessHelper accHelper = new AccessHelper();
            List<UDT.Subject> data_list = accHelper.Select<UDT.Subject>();
            
            // 將目前勾選的課程ID儲存在 Subject.RefCourseID
            foreach (UDT.Subject data in data_list)
            {
                if (data.UID == _SelectUID && _SelectCourseID != "")
                {
                    data.RefCourseID = int.Parse(_SelectCourseID);
                    data_list.SaveAll();
                }
            }
            Close();
        }

        private void CancerBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void subjectcbx_TextChanged(object sender, EventArgs e)
        {
            ReloadListViewItem();
        }
    }
}
