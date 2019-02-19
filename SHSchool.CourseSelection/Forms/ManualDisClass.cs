using System;
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
        Dictionary<string, string> subjectNamedic = new Dictionary<string, string>();
        // DIC(studentID,subjectID)
        Dictionary<string, string> studentIDdic = new Dictionary<string, string>();
        // List
        private SortedList<string, string> _CourseName = new SortedList<string, string>();
        private SortedList<string, Color> _CourseColor = new SortedList<string, Color>();
        // 紀錄選修課程分配到的學生
        Dictionary<string, List<string>> _CourseStudentIDdic = new Dictionary<string, List<string>>();

        private string _selectedSubjectID = "";

        public ManualDisClass()
        {
            InitializeComponent();

            #region Init SchoolYear Semester
            AccessHelper access = new AccessHelper();
            List<UDT.OpeningTime> opTimeList = access.Select<UDT.OpeningTime>();
            if (opTimeList.Count == 0)
            {
                opTimeList.Add(new UDT.OpeningTime() { SchoolYear = int.Parse(K12.Data.School.DefaultSchoolYear), Semester = int.Parse(K12.Data.School.DefaultSemester) });
                opTimeList.SaveAll();
            }
            schoolYearCbx.Items.Add(opTimeList[0].SchoolYear + 1);
            schoolYearCbx.Items.Add(opTimeList[0].SchoolYear);
            schoolYearCbx.Items.Add(opTimeList[0].SchoolYear - 1);
            schoolYearCbx.SelectedIndex = 1;

            semesterCbx.Items.Add(1);
            semesterCbx.Items.Add(2);
            semesterCbx.SelectedIndex = opTimeList[0].Semester - 1;
            #endregion
        }

        private void schoolYearCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadCourseTypeCbx();
        }

        private void semesterCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadCourseTypeCbx();
        }

        private void courseTypeCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadSubjectCbx();
        }

        private void subjectCbx_TextChanged(object sender, EventArgs e)
        {

        }

        public void ReloadCourseTypeCbx()
        {
            //  Init CourseType ComboBox
            if (schoolYearCbx.Text != "" && semesterCbx.Text != "")
            {
                courseTypeCbx.Items.Clear();
                AccessHelper accessSubject = new AccessHelper();
                List<UDT.Subject> s_list = accessSubject.Select<UDT.Subject>("school_year =" + schoolYearCbx.Text + " AND semester = " + semesterCbx.Text);
                foreach (UDT.Subject sc in s_list)
                {
                    if (!courseTypeCbx.Items.Contains(sc.Type))
                    {
                        courseTypeCbx.Items.Add(sc.Type);
                    }
                }
                if (courseTypeCbx.Items.Count > 0)
                    courseTypeCbx.SelectedIndex = 0;
            }
            //ReloadSubjectCbx();
        }

        public void ReloadSubjectCbx()
        {
            // Subject ComboBox
            // 選課人數(以分班/選修科目總人數) + 科目名稱 
            this.flowLayoutPanel1.Controls.Clear();
            subjectCbx.Items.Clear();
            if (semesterCbx.Text != "" && courseTypeCbx.Text != "")
            {
                #region SQL
                string selectSQL = string.Format(@"
                SELECT 
	                uid,
	                subject_name ,
	                ss_attend.count AS s_count,
	                attend.count AS c_count
                FROM $ischool.course_selection.subject  AS subject
                LEFT OUTER JOIN
                (
	                SELECT
		                ref_subject_id,
		                count(*)
	                FROM
		                $ischool.course_selection.ss_attend AS ss_attend
	                GROUP BY ss_attend.ref_subject_id
                )ss_attend ON ss_attend.ref_subject_id = subject.uid 
                LEFT OUTER JOIN
                (
	                SELECT
		                ref_subject_id,
		                count(ref_subject_course_id)
	                FROM
		                $ischool.course_selection.ss_attend AS ss_attend
	                WHERE ref_subject_course_id IS NOT NULL
	                GROUP BY ss_attend.ref_subject_id
                )attend ON attend.ref_subject_id = subject.uid
                WHERE school_year = {0} AND semester = {1} AND type = '{2}'
                ORDER BY s_count
                ", schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);
                #endregion
                QueryHelper queryHelper = new QueryHelper();
                DataTable subjectRecord = queryHelper.Select(selectSQL);
                subjectCbx.Items.Clear();
                subjectNamedic.Clear();
                subjectRecord.DefaultView.Sort = "s_count DESC ";
                foreach (DataRow subject in subjectRecord.Rows)
                {
                    subjectCbx.Items.Add("(" + subject["c_count"] + "/" + subject["s_count"] + ")" + subject["subject_name"]);
                    subjectNamedic.Add("(" + subject["c_count"] + "/" + subject["s_count"] + ")" + subject["subject_name"], "" + subject["uid"]);
                }
                subjectCbx.SelectedIndex = 0;
            }
            //ReloadDataGridView();
        }

        BackgroundWorker BGW = new BackgroundWorker();

        DataTable studentData = null;

        public void ReloadDataGridView()
        {
            dataGridViewX1.Rows.Clear();
            studentIDdic.Clear();

            //key
            string schoolYear = schoolYearCbx.Text;
            string semester = semesterCbx.Text;
            string courseType = courseTypeCbx.Text;
            string subject = subjectCbx.Text;

            pictureBox1.Visible = true;
            BGW = new BackgroundWorker();

            BGW.DoWork += delegate
            {
                QueryHelper queryhelper = new QueryHelper();
                #region SQL 
                string selectSQL = string.Format(@"
SELECT 
    ss_attend.*
    , student.student_number
    , student.name
    , student.gender
    , student.seat_no
    , class.class_name
    , subject_course.class_type
FROM 
    $ischool.course_selection.ss_attend AS ss_attend
LEFT OUTER JOIN student
	ON student.id = ss_attend.ref_student_id
LEFT OUTER JOIN class
	ON class.id = student.ref_class_id
LEFT OUTER JOIN $ischool.course_selection.subject_course AS subject_course
    ON subject_course.uid = ss_attend.ref_subject_course_id
WHERE ss_attend.ref_subject_id = {0}
            ", "" + _selectedSubjectID);

                #endregion
                studentData = queryhelper.Select(selectSQL);
                _CourseStudentIDdic.Clear();
            };
            BGW.RunWorkerCompleted += delegate
            {
                if (schoolYear == schoolYearCbx.Text && semester == semesterCbx.Text && courseType == courseTypeCbx.Text && subject == subjectCbx.Text)
                {
                    foreach (DataRow student in studentData.Rows)
                    {
                        DataGridViewRow datarow = new DataGridViewRow();
                        datarow.CreateCells(dataGridViewX1);
                        int g;
                        if (int.TryParse("" + student["gender"], out g))
                        {
                            if ("" + student["gender"] == "1")
                            {
                                student["gender"] = "男";
                            }
                            if ("" + student["gender"] == "0")
                            {
                                student["gender"] = "女";
                            }
                        }

                        int index = 0;
                        datarow.Cells[index++].Value = student["student_number"];
                        datarow.Cells[index++].Value = student["name"];
                        datarow.Cells[index++].Value = student["gender"];
                        datarow.Cells[index++].Value = student["class_name"];
                        datarow.Cells[index++].Value = student["seat_no"];
                        datarow.Cells[index++].Value = student["name"];
                        if ("" + student["ref_subject_course_id"] != "")
                        {
                            ((DataGridViewColorBallTextCell)datarow.Cells[6]).Value = "" + student["class_type"];
                            ((DataGridViewColorBallTextCell)datarow.Cells[6]).Color = _CourseColor["" + student["ref_subject_course_id"]];
                            // 紀錄課程、學生資訊。
                            if (_CourseStudentIDdic.ContainsKey("" + student["ref_subject_course_id"]))
                            {
                                _CourseStudentIDdic["" + student["ref_subject_course_id"]].Add("" + student["ref_student_id"]);
                            }
                            else
                            {
                                _CourseStudentIDdic.Add("" + student["ref_subject_course_id"], new List<string>());
                                _CourseStudentIDdic["" + student["ref_subject_course_id"]].Add("" + student["ref_student_id"]);
                            }
                        }
                        if ("" + student["ref_subject_course_id"] == "")
                        {
                            ((DataGridViewColorBallTextCell)datarow.Cells[6]).Value = _CourseName[""];
                            ((DataGridViewColorBallTextCell)datarow.Cells[6]).Color = _CourseColor[""];
                            // 紀錄課程、學生資訊。
                            if (_CourseStudentIDdic.ContainsKey("" + student["ref_subject_course_id"]))
                            {
                                _CourseStudentIDdic["" + student["ref_subject_course_id"]].Add("" + student["ref_student_id"]);
                            }
                            else
                            {
                                _CourseStudentIDdic.Add("" + student["ref_subject_course_id"], new List<string>());
                                _CourseStudentIDdic["" + student["ref_subject_course_id"]].Add("" + student["ref_student_id"]);
                            }
                        }
                        datarow.Tag = "" + student["ref_student_id"];
                        datarow.Cells[6].Tag = "" + student["ref_subject_course_id"];
                        datarow.Cells[5].Tag = "" + student["ref_subject_course_id"];

                        dataGridViewX1.Rows.Add(datarow);
                    }

                    pictureBox1.Visible = false;
                }
                else
                {
                    ReloadDataGridView();
                }
            };
            //透過科目ID取得學生選課資料
            if (subjectCbx.Text != "")
            {
                BGW.RunWorkerAsync();
            }

            // 計算課班男、女人數
            CountStudents();
        }

        private void Swap(object sender, EventArgs e)
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
            List<string> dataList = new List<string>();
            foreach (DataGridViewRow dr in dataGridViewX1.Rows)
            {
                string data = string.Format(@"
                SELECT
	               {0}::BIGINT AS ref_subject_course_id
	               , {1}::BIGINT AS ref_student_id
	               , {2}::BIGINT AS ref_subject_id

                    ", ("" + dr.Cells[6].Tag) == "" ? "NULL" : ("" + dr.Cells[6].Tag), "" + dr.Tag, "" + _selectedSubjectID);
                dataList.Add(data);
            }
            string dataRow = string.Join(" UNION ALL", dataList);

            string sql = string.Format(@"
WITH data_row AS(
	{0}
)
UPDATE 
	$ischool.course_selection.ss_attend 
SET
	ref_subject_course_id = data_row.ref_subject_course_id
FROM
	data_row
WHERE
	$ischool.course_selection.ss_attend.ref_student_id = data_row.ref_student_id
	AND $ischool.course_selection.ss_attend.ref_subject_id = data_row.ref_subject_id
            ", dataRow);

            UpdateHelper up = new UpdateHelper();
            up.Execute(sql);


            MessageBox.Show("儲存成功");
            ReloadSubjectCbx();
            ReloadDataGridView();
            foreach (var subject in subjectNamedic)
            {
                if (subject.Value == "" + _selectedSubjectID)
                {
                    subjectCbx.Text = subject.Key;
                }
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 自動分班
        private void autoDisClassBtn_Click(object sender, EventArgs e)
        {
            AccessHelper access = new AccessHelper();
            //取得選修科目班級
            List<UDT.SubjectCourse> subjectList = access.Select<UDT.SubjectCourse>("ref_subject_id = " + "'" + _selectedSubjectID + "'");
            //// 取得選課學生
            List<UDT.SSAttend> ssAttendList = access.Select<UDT.SSAttend>("ref_subject_id = " + "'" + _selectedSubjectID + "'");

            List<Subject_Class> sbcList = new List<Subject_Class>();
            Dictionary<string, Subject_Class> classDic = new Dictionary<string, Subject_Class>();
            string[] subjectCourse = new string[subjectList.Count];

            int studentCount = ssAttendList.Count(); // 選課學生人數
            int classCount = subjectList.Count(); // 選修科目開班數
            if (classCount == 0)
            {
                MessageBox.Show("選修科目尚未開班 !");
                return;
            }
            int limit = studentCount / classCount; // 每班人數限制

            int index = 0;
            foreach (UDT.SubjectCourse sb in subjectList)
            {
                subjectCourse[index] = sb.UID;
                index++;

                Subject_Class sbc = new Subject_Class();
                sbc.RefSubjectCourseID = sb.UID;
                sbc.RefSubjectID = "" + sb.RefSubjectID;
                sbc.ClassType = sb.ClassType;
                sbc.StudentCount = 0;
                sbc.Limit = limit;

                sbcList.Add(sbc);
            }
            // 如果學生數無法均分-修改人數限制
            if (studentCount % classCount != 0)
            {
                for (int i = 0; i < studentCount % classCount; i++)
                {
                    sbcList[i].Limit += 1;
                }
            }

            foreach (Subject_Class sbc in sbcList)
            {
                classDic.Add(sbc.RefSubjectCourseID, sbc);
            }
            // 新增未分班
            classDic.Add("", new Subject_Class());
            classDic[""].ClassType = "未分班";

            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if (classDic.ContainsKey("" + datarow.Cells[6].Tag))
                {
                    classDic["" + datarow.Cells[6].Tag].StudentCount += 1;
                }
            }

            // 亂數分班
            Random random = new Random();

            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if ("" + datarow.Cells[6].Tag == "")
                {
                    int n;
                    do
                    {
                        n = random.Next(0, subjectCourse.Count());
                    } while (classDic[subjectCourse[n]].StudentCount >= classDic[subjectCourse[n]].Limit);

                    datarow.Cells[6].Tag = subjectCourse[n];
                    classDic[subjectCourse[n]].StudentCount += 1;

                    ((DataGridViewColorBallTextCell)datarow.Cells[6]).Value = classDic[subjectCourse[n]].ClassType;
                    ((DataGridViewColorBallTextCell)datarow.Cells[6]).Color = _CourseColor[subjectCourse[n]];
                }
            }

            // 計算科目班級人數
            Dictionary<string, Count> CountDic = new Dictionary<string, Count>();
            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if (CountDic.ContainsKey("" + datarow.Cells[6].Tag))
                {
                    if ("" + datarow.Cells[2].Value == "女")
                    {
                        CountDic["" + datarow.Cells[6].Tag].girl += 1;
                    }
                    if ("" + datarow.Cells[2].Value == "男")
                    {
                        CountDic["" + datarow.Cells[6].Tag].boy += 1;
                    }
                    if ("" + datarow.Cells[2].Value != "男" && "" + datarow.Cells[2].Value != "女")
                    {
                        CountDic["" + datarow.Cells[6].Tag].understand += 1;
                    }
                    CountDic["" + datarow.Cells[6].Tag].total += 1;
                }
                if (!CountDic.ContainsKey("" + datarow.Cells[6].Tag))
                {
                    CountDic.Add("" + datarow.Cells[6].Tag, new Count());

                    if ("" + datarow.Cells[2].Value == "女")
                    {
                        CountDic["" + datarow.Cells[6].Tag].girl = 1;
                    }
                    if ("" + datarow.Cells[2].Value == "男")
                    {
                        CountDic["" + datarow.Cells[6].Tag].boy = 1;
                    }
                    if ("" + datarow.Cells[2].Value != "男" && "" + datarow.Cells[2].Value != "女")
                    {
                        CountDic["" + datarow.Cells[6].Tag].understand = 1;
                    }
                    CountDic["" + datarow.Cells[6].Tag].total = 1;
                }
            }
            {
                CountDic.Add("", new Count());
                CountDic[""].boy = 0;
                CountDic[""].girl = 0;
                CountDic[""].total = 0;
            }

            foreach (Control c in flowLayoutPanel1.Controls)
            {
                c.Text = classDic["" + c.Tag].ClassType + "(" + (CountDic["" + c.Tag].boy > 0 ? " " + CountDic["" + c.Tag].boy + "男" : "") + (CountDic["" + c.Tag].girl > 0 ? " " + CountDic["" + c.Tag].girl + "女" : "") + (CountDic["" + c.Tag].understand > 0 ? " " + CountDic["" + c.Tag].understand + "未知性別" : "") + " 共" + CountDic["" + c.Tag].total + "人" + " )";
            }
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

    class Subject_Class
    {
        public string RefSubjectID { get; set; }
        public string RefSubjectCourseID { get; set; }
        public string ClassType { get; set; }
        public int? StudentCount { get; set; }
        public int Limit { get; set; }
    }

    class Count
    {
        public int boy { get; set; }
        public int girl { get; set; }
        public int understand { get; set; }
        public int total { get; set; }
    }
}
