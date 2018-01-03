﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using SHSchool.Data;
using FISCA.Data;
using FISCA.UDT;
using DevComponents.DotNetBar;
using System.Drawing.Drawing2D;
using DevComponents.DotNetBar.Rendering;
using K12.Data;


namespace SHSchool.CourseSelection.Forms
{
    public partial class ManualDisClass : BaseForm
    {
        // DIC(subjectName,subjectID)
        Dictionary<string, string> subjectIDdic = new Dictionary<string, string>();
        // DIC(studentID,subjectID)
        Dictionary<string, string> studentIDdic = new Dictionary<string, string>();
        // List
        private SortedList<string, string> _CourseName = new SortedList<string, string>();
        private SortedList<string, Color> _CourseColor = new SortedList<string, Color>();
        // 紀錄選修課程分配到的學生
        Dictionary<string, List<string>> _CourseStudentIDdic = new Dictionary<string, List<string> >();

        public ManualDisClass()
        {
            InitializeComponent();

            // SchoolYear ComboBox
            schoolYearCbx.Text = SHSchoolInfo.DefaultSchoolYear;
            for (int i = 0;i < 3; i++)
            {
                schoolYearCbx.Items.Add(int.Parse(SHSchoolInfo.DefaultSchoolYear) + i );
            }

            // Semester ComboBox
            semesterCbx.Text = SHSchoolInfo.DefaultSemester;
            semesterCbx.Items.Add(1);
            semesterCbx.Items.Add(2);

            // Subject ComboBox
            AccessHelper accessSubject = new AccessHelper();
            List<UDT.Subject> s_list = accessSubject.Select<UDT.Subject>();
            foreach (UDT.Subject sc in s_list)
            {
                if (int.Parse(schoolYearCbx.Text) == sc.SchoolYear && int.Parse(semesterCbx.Text) == sc.Semester)
                {
                    subjectCbx.Items.Add(sc.SubjectName);
                    subjectIDdic.Add(sc.SubjectName,sc.UID);
                }
            }
        }

        private void subjectCbx_TextChanged(object sender, EventArgs e)
        {
            _CourseColor.Clear();
            _CourseName.Clear();

            #region Course ButtonX
            this.flowLayoutPanel1.Controls.Clear();
            AccessHelper accessCourse = new AccessHelper();
            List<UDT.SubjectCourse> sc_list = accessCourse.Select<UDT.SubjectCourse>();
            Color[] colors = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.PowderBlue, Color.Orange, Color.Green, Color.Purple };

            #region FlowLayoutPanel 課班Button
            int i = 0;
            foreach (UDT.SubjectCourse sc in sc_list)
            {
                if (subjectCbx.Text == sc.SubjectName && int.Parse(schoolYearCbx.Text) == sc.SchoolYear && int.Parse(semesterCbx.Text) == sc.Semester)
                {
                    ButtonX button = new ButtonX();
                    button.FocusCuesEnabled = false;
                    button.Style = eDotNetBarStyle.Office2007;
                    button.ColorTable = eButtonColor.Flat;
                    button.AutoSize = true;
                    button.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(15);
                    button.TextAlignment = eButtonTextAlignment.Left;
                    button.Size = new Size(110, 23);
                    button.Text = sc.CourseName;
                    button.Image = GetColorBallImage(colors[i++]);
                    button.Tag = "" + sc.CourseID;
                    button.Margin = new System.Windows.Forms.Padding(3);
                    button.Click += new EventHandler(Swap);
                    _CourseName.Add("" + sc.CourseID, sc.CourseName);
                    _CourseColor.Add("" + sc.CourseID, colors[i++]);
                    this.flowLayoutPanel1.Controls.Add(button);
                }
            }
            #endregion

            #region 未分班按鈕

            ButtonX btn = new ButtonX();
            btn.FocusCuesEnabled = false;
            btn.Style = eDotNetBarStyle.Office2007;
            btn.ColorTable = eButtonColor.Flat;
            btn.AutoSize = true;
            btn.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(15);
            btn.TextAlignment = eButtonTextAlignment.Left;
            btn.Size = new Size(110, 23);
            btn.Text = "未分班";
            btn.Image = GetColorBallImage(Color.Gray);
            btn.Tag = "";
            btn.Margin = new System.Windows.Forms.Padding(3);
            btn.Click += new EventHandler(Swap);
            _CourseName.Add("", "未分班");
            _CourseColor.Add("", Color.Gray);
            this.flowLayoutPanel1.Controls.Add(btn);
            flowLayoutPanel1.SetFlowBreak(btn, true);
            #endregion

            #endregion

            ReloadDataGridView();
        }

        public void ReloadDataGridView()
        {
            #region DataGridView 
            dataGridViewX1.Rows.Clear();
            AccessHelper access = new AccessHelper();
            studentIDdic.Clear();
            // 取得選課學生資料
            List<UDT.SSAttend> ssa_list = access.Select<UDT.SSAttend>();
            foreach (UDT.SSAttend ssa in ssa_list)
            {
                if (ssa.SubjectID == int.Parse("" + subjectIDdic[subjectCbx.Text]))
                {
                    studentIDdic.Add("" + ssa.StudentID, "" + ssa.SubjectID);
                }
            }
            // 取得修課學生資料
            _CourseStudentIDdic.Clear();
            List<SHStudentRecord> shs_list = SHStudent.SelectByIDs(studentIDdic.Keys.ToList());
            foreach (SHStudentRecord shs in shs_list)
            {
                DataGridViewRow datarow = new DataGridViewRow();
                datarow.CreateCells(dataGridViewX1);

                int index = 0;
                datarow.Cells[index++].Value = shs.StudentNumber;
                datarow.Cells[index++].Value = shs.Name;
                datarow.Cells[index++].Value = shs.Gender;
                datarow.Cells[index++].Value = shs.Class.Name;
                datarow.Cells[index++].Value = shs.SeatNo;

                // 透過學生ID、科目ID取得選修課程ID
                string sql = string.Format(@"
                    SELECT ref_course_id
                    FROM $ischool.course_selection.ss_attend
                    WHERE 
                        ref_student_id = {0}
                        AND ref_subject_id = {1}
                ", shs.ID, studentIDdic[shs.ID]);
                QueryHelper qh = new QueryHelper();
                DataTable courseNameDt = qh.Select(sql);


                foreach (DataRow dr in courseNameDt.Rows)
                {
                    SHCourseRecord sr = SHCourse.SelectByID("" + dr["ref_course_id"]);
                    if (sr != null)
                    {
                        //datarow.Cells[index++].Value = "" + sr.Name;
                        //datarow.Cells[index++].Value = "" + sr.Name;
                        ((DataGridViewColorBallTextCell)datarow.Cells[5]).Value = _CourseName[sr.ID];
                        ((DataGridViewColorBallTextCell)datarow.Cells[6]).Value = _CourseName[sr.ID];
                        ((DataGridViewColorBallTextCell)datarow.Cells[5]).Color = _CourseColor[sr.ID];
                        ((DataGridViewColorBallTextCell)datarow.Cells[6]).Color = _CourseColor[sr.ID];
                        datarow.Cells[6].Tag = sr.ID;
                        
                        // 紀錄課程、學生資訊。
                        if (_CourseStudentIDdic.ContainsKey(sr.ID))
                        {
                            _CourseStudentIDdic[sr.ID].Add(shs.ID);
                        }
                        else
                        {
                            _CourseStudentIDdic.Add(sr.ID, new List<string>());
                            _CourseStudentIDdic[sr.ID].Add(shs.ID);
                        }
                    }
                    if (sr == null)
                    {
                        //datarow.Cells[index++].Value = _CourseName[""];
                        //datarow.Cells[index++].Value = _CourseName[""];
                        ((DataGridViewColorBallTextCell)datarow.Cells[5]).Value = _CourseName[""];
                        ((DataGridViewColorBallTextCell)datarow.Cells[6]).Value = _CourseName[""];
                        ((DataGridViewColorBallTextCell)datarow.Cells[5]).Color = _CourseColor[""];
                        ((DataGridViewColorBallTextCell)datarow.Cells[6]).Color = _CourseColor[""];
                        datarow.Cells[6].Tag = ""; //courseID = ""
                        // 紀錄課程、學生資訊。
                        if (_CourseStudentIDdic.ContainsKey(""))
                        {
                            _CourseStudentIDdic[""].Add(shs.ID);
                        }
                        else
                        {
                            _CourseStudentIDdic.Add("", new List<string>());
                            _CourseStudentIDdic[""].Add(shs.ID);
                        }
                    }
                }

                datarow.Tag = shs.ID;

                dataGridViewX1.Rows.Add(datarow);
            }
            // 計算課班男、女人數
            CountStudents();
            #endregion
        }

        private void Swap(object sender,EventArgs e)
        {
            ButtonX button = (ButtonX)sender;
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                ((DataGridViewColorBallTextCell)row.Cells[6]).Color = _CourseColor["" + button.Tag];
                ((DataGridViewColorBallTextCell)row.Cells[6]).Value = _CourseName["" + button.Tag];
                // 修改 DataGridView
                //dataGridViewX1.Rows[row.Index].Cells[6].Value = _CourseName["" + button.Tag];
                dataGridViewX1.Rows[row.Index].Cells[6].Tag = button.Tag;
            }
            _CourseStudentIDdic.Clear();
            // row.tag = studentID, row.cell[6].tag = courseID
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                
                if (_CourseStudentIDdic.ContainsKey("" + row.Cells[6].Tag))
                {
                    _CourseStudentIDdic["" + row.Cells[6].Tag].Add("" + row.Tag);
                }
                else
                {
                    _CourseStudentIDdic.Add("" + row.Cells[6].Tag, new List<string>());
                    _CourseStudentIDdic["" + row.Cells[6].Tag].Add("" + row.Tag);
                }

            }

            CountStudents();
        }

        private void CountStudents()
        { 
            foreach (Control btn in flowLayoutPanel1.Controls)
            {
                string courseID = "" + btn.Tag;
                if (!_CourseStudentIDdic.ContainsKey(courseID))
                {
                    btn.Text = _CourseName[courseID] + "(0人)";
                }
                else
                {
                    int totle = _CourseStudentIDdic[courseID].Count();
                    int b = 0, g = 0;

                    foreach (SHStudentRecord sr in SHStudent.SelectByIDs(_CourseStudentIDdic[courseID].ToArray()))
                    {
                        
                        if (sr.Gender == "男") b++;
                        if (sr.Gender == "女") g++;
                    }
                    btn.Text = _CourseName[courseID] + "(" + (b > 0 ? " " + b + "男" : "") + (g > 0 ? " " + g + "女" : "") + (totle - b - g > 0 ? " " + (totle - b - g) + "未知性別" : "") + " 共" + totle + "人" + " )";
                }
            }
        }

        public Image GetColorBallImage(Color color)
        {
            Bitmap bmp = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            int w = 14,
                    x = 1,
                    y = 1;
            Color[] myColors = { color, Color.White, color, color };
            float[] myPositions = { 0.0f, 0.05f, 0.6f, 1.0f };
            ColorBlend myBlend = new ColorBlend();
            myBlend.Colors = myColors;
            myBlend.Positions = myPositions;
            using (LinearGradientBrush brush = new LinearGradientBrush(new Point(x, y), new Point(w, w), Color.White, color))
            {
                brush.InterpolationColors = myBlend;
                brush.GammaCorrection = true;
                graphics.FillRectangle(brush, x, y, w, w);
            }
            graphics.DrawRectangle(new Pen(Color.Black), x, y, w, w);
            return bmp;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in dataGridViewX1.Rows)
            {
                if ("" + dr.Cells[6].Tag != "" && dr.Cells[6].Tag != null) // 有分班學生
                {
                    string updateSQL = string.Format(@"
                    UPDATE $ischool.course_selection.ss_attend
                    SET ref_course_id = {0}
                    WHERE ref_student_id = {1} AND ref_subject_id = {2}
                ", "" + dr.Cells[6].Tag, "" + dr.Tag, subjectIDdic[subjectCbx.Text]);

                    UpdateHelper updateHelper = new UpdateHelper();
                    updateHelper.Execute(updateSQL);
                }
                if ("" + dr.Cells[6].Tag == "") // 未分班學生
                {
                    string updateSQL = string.Format(@"
                    UPDATE $ischool.course_selection.ss_attend
                    SET ref_course_id = {0}
                    WHERE ref_student_id = {1} AND ref_subject_id = {2}
                ", "null", "" + dr.Tag, subjectIDdic[subjectCbx.Text]);

                    UpdateHelper updateHelper = new UpdateHelper();
                    updateHelper.Execute(updateSQL);
                }
            }
            MessageBox.Show("儲存成功");
            ReloadDataGridView();

        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    class AttInfo
    {
        private string _AttendID, _StudentID, _CourseID, _OldCourseID;
        public bool HasChanged { get { return _CourseID != _OldCourseID; } }
        public string AttendID { get { return _AttendID; } }
        public string CourseID { get { return _CourseID; } set { _CourseID = value; } }
        public string StudentID { get { return _StudentID; } }
        public AttInfo(string attendID, string courseID, string studentID)
        {
            _AttendID = attendID;
            _StudentID = studentID;
            _CourseID = _OldCourseID = courseID;
        }
    }
    class DataGridViewColorBallTextCell : DataGridViewTextBoxCell
    {
        private Color _Color = Color.Transparent;
        public Color Color
        {
            get { return _Color; }
            set { _Color = value; }
        }
        protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (_Color != Color.Transparent)
            {
                SmoothingMode mode = graphics.SmoothingMode;//跟妳說喔，NotNetBar很機車喔，如果你把SmoothingMode改掉沒改回去，格線會亂亂劃喔。
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, paintParts);

                int w = cellBounds.Height - 9,
                    x = cellBounds.X + 3,
                    y = cellBounds.Y + 4;
                Color[] myColors = { _Color, Color.White, _Color, _Color };
                float[] myPositions = { 0.0f, 0.05f, 0.6f, 1.0f };
                ColorBlend myBlend = new ColorBlend();
                myBlend.Colors = myColors;
                myBlend.Positions = myPositions;
                using (LinearGradientBrush brush = new LinearGradientBrush(new Point(x, y), new Point(x + w, y + w), Color.White, _Color))
                {
                    brush.InterpolationColors = myBlend;
                    brush.GammaCorrection = true;
                    graphics.FillRectangle(brush, x, y, w, w);
                }
                graphics.DrawRectangle(new Pen(Color.Black), x, y, w, w);

                cellBounds = new System.Drawing.Rectangle(cellBounds.X + cellBounds.Height - 4, cellBounds.Y, cellBounds.Width - cellBounds.Height + 4, cellBounds.Height);
                graphics.SmoothingMode = mode;
            }
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        public override Type EditType
        {
            get
            {
                return null;
            }
        }
    }
    class DataGridViewColorBallTextColumn : DataGridViewColumn
    {
        public DataGridViewColorBallTextColumn()
        {
            this.CellTemplate = new DataGridViewColorBallTextCell();
        }
    }

}
