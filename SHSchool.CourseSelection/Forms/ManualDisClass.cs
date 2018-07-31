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

        /// <summary>
        /// Key:(以分班/選修科目總人數)科目名稱  Value: 科目編號
        /// </summary>
        private Dictionary<string, string> dicSubjectName = new Dictionary<string, string>();
        
        /// <summary>
        /// Key: SubjectCourseID Value: 班別
        /// </summary>
        private SortedList<string, string> _CourseName = new SortedList<string, string>();

        /// <summary>
        /// SubjectCourseID 所對應到的顏色
        /// </summary>
        private SortedList<string, Color> _CourseColor = new SortedList<string, Color>();
        
        /// <summary>
        /// 紀錄選修課程分配到的學生 
        /// </summary>
        private Dictionary<string, List<string>> _CourseStudentIDdic = new Dictionary<string, List<string>>();

        /// <summary>
        /// 畫面初始化完成
        /// </summary>
        private bool _initFinsih = false;

        public ManualDisClass()
        {
            InitializeComponent();
        }

        private void ManualDisClass_Load(object sender, EventArgs e)
        {
            #region Init SchoolYear Semester
            AccessHelper access = new AccessHelper();
            List<UDT.OpeningTime> opTimeList = access.Select<UDT.OpeningTime>();
            if (opTimeList.Count == 0)
            {
                opTimeList.Add(new UDT.OpeningTime() { SchoolYear = int.Parse(K12.Data.School.DefaultSchoolYear), Semester = int.Parse(K12.Data.School.DefaultSemester) });
                opTimeList.SaveAll();
            }
            cbxSchoolYear.Items.Add(opTimeList[0].SchoolYear + 1);
            cbxSchoolYear.Items.Add(opTimeList[0].SchoolYear);
            cbxSchoolYear.Items.Add(opTimeList[0].SchoolYear - 1);
            cbxSchoolYear.SelectedIndex = 1;

            cbxSemester.Items.Add(1);
            cbxSemester.Items.Add(2);
            cbxSemester.SelectedIndex = opTimeList[0].Semester - 1;
            #endregion

            ReloadCourseTypeCbx();

            _initFinsih = true;
        }

        private void schoolYearCbx_TextChanged(object sender, EventArgs e)
        {
            if (_initFinsih)
            {
                ReloadCourseTypeCbx();
            }
        }

        private void semesterCbx_TextChanged(object sender, EventArgs e)
        {
            if (_initFinsih)
            {
                ReloadCourseTypeCbx();
            }
        }

        private void courseTypeCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadSubjectCbx();
        }

        private void subjectCbx_TextChanged(object sender, EventArgs e)
        {
            string subjectID = "";
            if (dicSubjectName.ContainsKey(cbxSubject.Text))
            {
                subjectID = dicSubjectName[cbxSubject.Text];
            }

            ReloadFlowLayoutPanel(subjectID);

            Application.DoEvents();

            ReloadDataGridView(subjectID);
        }

        /// <summary>
        /// 課程時段更新
        /// </summary>
        private void ReloadCourseTypeCbx()
        {
            cbxCourseType.Items.Clear();

            string sql = string.Format(@"
SELECT DISTINCT
    type
FROM
    $ischool.course_selection.subject
WHERE
    school_year = {0}
    AND semester = {1}
    AND type IS NOT NULL
                ",cbxSchoolYear.SelectedItem.ToString(),cbxSemester.SelectedItem.ToString());

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql);

            foreach (DataRow row in dt.Rows)
            {
                cbxCourseType.Items.Add("" + row["type"]);   
            }
            if (cbxCourseType.Items.Count > 0)
                cbxCourseType.SelectedIndex = 0;
        }

        /// <summary>
        /// 科目更新
        /// </summary>
        private void ReloadSubjectCbx()
        {
            // (以分班/選修科目總人數)科目名稱 
            cbxSubject.Items.Clear();
            dicSubjectName.Clear();

            #region SQL
            string selectSQL = string.Format(@"
SELECT 
uid
, subject_name 
, subject.level
, ss_attend.count AS 學生人數 --s_count
, attend.count AS 課程人數 --c_count
FROM 
$ischool.course_selection.subject  AS subject
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
WHERE 
school_year = {0} 
AND semester = {1} 
AND type = '{2}'
ORDER BY 
subject.type
, subject.subject_name
, subject.level
, subject.credit
--s_count
            ", cbxSchoolYear.Text, cbxSemester.Text, cbxCourseType.Text);
            #endregion

            QueryHelper queryHelper = new QueryHelper();
            DataTable subjectRecord = queryHelper.Select(selectSQL);

            foreach (DataRow row in subjectRecord.Rows)
            {
                string subjectName = string.Format("({0}/{1}){2} {3}", row["課程人數"], row["學生人數"], row["subject_name"], Tool.RomanChar("" + row["level"]));
                cbxSubject.Items.Add(subjectName);
                dicSubjectName.Add(subjectName, "" + row["uid"]);
            }

            cbxSubject.SelectedIndex = 0;
        }

        /// <summary>
        /// 課程班級按鈕更新
        /// </summary>
        private void ReloadFlowLayoutPanel(string subjectID)
        {
            _CourseColor.Clear();
            _CourseName.Clear();

            this.flowLayoutPanel1.Controls.Clear();

            #region 取得課班資料
            string sql = string.Format(@"
SELECT
    subject_course.*
FROM
    $ischool.course_selection.subject_course AS subject_course
    LEFT OUTER JOIN $ischool.course_selection.subject AS subject
        ON subject.uid = subject_course.ref_subject_id
WHERE 
    subject.school_year = {0}
    AND subject.semester = {1}
    AND subject_course.ref_subject_id = {2}
                    ", cbxSchoolYear.Text, cbxSemester.Text, subjectID);
            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql); 
            #endregion

            Color[] colors = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.PowderBlue, Color.Orange, Color.Green, Color.Purple };

            #region FlowLayoutPanel 課班Button
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                ButtonX button = new ButtonX();
                button.FocusCuesEnabled = false;
                button.Style = eDotNetBarStyle.Office2007;
                button.ColorTable = eButtonColor.Flat;
                button.AutoSize = true;
                button.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(15);
                button.TextAlignment = eButtonTextAlignment.Left;
                button.Size = new Size(110, 23);
                button.Text = "" + row["class_type"];
                button.Image = GetColorBallImage(colors[i]);
                button.Tag = "" + row["uid"]; // 課班UID
                button.Margin = new System.Windows.Forms.Padding(3);
                button.Click += new EventHandler(Swap);
                // 課班UID
                _CourseName.Add("" + row["uid"], "" + row["class_type"]);
                _CourseColor.Add("" + row["uid"], colors[i++]);
                this.flowLayoutPanel1.Controls.Add(button);
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

        }

        private BackgroundWorker BGW = new BackgroundWorker();

        private DataTable studentData = null;

        /// <summary>
        /// 課程按鈕事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Swap(object sender, EventArgs e)
        {
            ButtonX button = (ButtonX)sender;
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                ((DataGridViewColorBallTextCell)row.Cells[5]).Color = _CourseColor["" + button.Tag];
                ((DataGridViewColorBallTextCell)row.Cells[5]).Value = _CourseName["" + button.Tag];

                dataGridViewX1.Rows[row.Index].Cells[5].Tag = button.Tag;
            }

            #region 重新計算課班人數
            _CourseStudentIDdic.Clear();

            foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
            {
                if (!_CourseStudentIDdic.ContainsKey("" + dgvrow.Cells[5].Tag))
                {
                    _CourseStudentIDdic.Add("" + dgvrow.Cells[5].Tag, new List<string>());
                }
                _CourseStudentIDdic["" + dgvrow.Cells[5].Tag].Add("" + dgvrow.Tag);
            } 
            #endregion

            UpdateCourseBtn();
        }

        private void ReloadDataGridView(string subjectID)
        {
            dataGridViewX1.Rows.Clear();

            this.pictureBox1.Visible = true; // 畫面loading圖示

            string schoolYear = cbxSchoolYear.Text;
            string semester = cbxSemester.Text;
            string courseType = cbxCourseType.Text;
            string subject = cbxSubject.Text;

            BGW = new BackgroundWorker();
            BGW.DoWork += delegate
            {
                QueryHelper qh = new QueryHelper();
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
            ", subjectID);

                #endregion
                studentData = qh.Select(selectSQL);
                _CourseStudentIDdic.Clear();
            };
            BGW.RunWorkerCompleted += delegate
            {
                if (schoolYear == cbxSchoolYear.Text && semester == cbxSemester.Text && courseType == cbxCourseType.Text && subject == cbxSubject.Text)
                {
                    foreach (DataRow student in studentData.Rows)
                    {
                        DataGridViewRow datarow = new DataGridViewRow();
                        datarow.CreateCells(dataGridViewX1);

                        #region 性別資料Parse
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
                        #endregion

                        int index = 0;
                        datarow.Cells[index++].Value = student["student_number"]; // 學號
                        datarow.Cells[index++].Value = student["name"]; // 姓名
                        datarow.Cells[index++].Value = student["gender"]; // 性別
                        datarow.Cells[index++].Value = student["class_name"]; // 班級
                        datarow.Cells[index++].Value = student["seat_no"]; // 座號

                        #region 分班
                        string subjectCourseID = "" + student["ref_subject_course_id"];
                        string studentID = "" + student["ref_student_id"];

                        if (subjectCourseID != "")
                        {
                            ((DataGridViewColorBallTextCell)datarow.Cells[5]).Value = "" + student["class_type"];
                            ((DataGridViewColorBallTextCell)datarow.Cells[5]).Color = _CourseColor[subjectCourseID];
                            // 紀錄課程、學生資訊。
                            if (!_CourseStudentIDdic.ContainsKey(subjectCourseID))
                            {
                                _CourseStudentIDdic.Add(subjectCourseID, new List<string>());
                            }
                            _CourseStudentIDdic[subjectCourseID].Add(studentID);
                        }
                        else
                        {
                            ((DataGridViewColorBallTextCell)datarow.Cells[5]).Value = _CourseName[""];
                            ((DataGridViewColorBallTextCell)datarow.Cells[5]).Color = _CourseColor[""];
                            // 紀錄課程、學生資訊。
                            if (!_CourseStudentIDdic.ContainsKey(subjectCourseID))
                            {
                                _CourseStudentIDdic.Add(subjectCourseID, new List<string>());
                            }
                            _CourseStudentIDdic[subjectCourseID].Add(studentID);
                        } 
                        #endregion

                        datarow.Tag = studentID;  // 學生編號
                        datarow.Cells[5].Tag = subjectCourseID; // 課班編號

                        dataGridViewX1.Rows.Add(datarow);
                    }
                    this.pictureBox1.Visible = false;
                }
                else
                {
                    ReloadDataGridView(dicSubjectName[cbxSubject.Text]);
                }
                // 計算課班男、女人數
                UpdateCourseBtn();
            };
            //透過科目ID取得學生選課資料
            if (cbxSubject.Text != "")
            {
                BGW.RunWorkerAsync();
            }

        }

        /// <summary>
        /// 更新選修課程統計人數
        /// </summary>
        private void UpdateCourseBtn()
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
            string subjectID = dicSubjectName[cbxSubject.Text];
            List<string> dataList = new List<string>();
            foreach (DataGridViewRow dr in dataGridViewX1.Rows)
            {
                string data = string.Format(@"
                SELECT
	               {0}::BIGINT AS ref_subject_course_id
	               , {1}::BIGINT AS ref_student_id
	               , {2}::BIGINT AS ref_subject_id

                    ", ("" + dr.Cells[5].Tag) == "" ? "NULL" : ("" + dr.Cells[5].Tag), "" + dr.Tag, "" + subjectID);
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
            //ReloadDataGridView(subjectID);
            foreach (var subject in dicSubjectName)
            {
                if (subject.Value == subjectID)
                {
                    cbxSubject.Text = subject.Key;
                }
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 自動分班
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoDisClassBtn_Click(object sender, EventArgs e)
        {
            string subjectID = dicSubjectName[cbxSubject.Text];
            AccessHelper access = new AccessHelper();
            //取得選修科目班級
            List<UDT.SubjectCourse> subjectList = access.Select<UDT.SubjectCourse>(string.Format("ref_subject_id = '{0}'", subjectID));
            //// 取得選課學生
            List<UDT.SSAttend> ssAttendList = access.Select<UDT.SSAttend>(string.Format("ref_subject_id = '{0}'", subjectID));

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

            #region 新增為分班
            classDic.Add("", new Subject_Class());
            classDic[""].ClassType = "未分班";

            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if (classDic.ContainsKey("" + datarow.Cells[5].Tag))
                {
                    classDic["" + datarow.Cells[5].Tag].StudentCount += 1;
                }
            }
            #endregion

            #region 亂數分班
            Random random = new Random();

            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if ("" + datarow.Cells[5].Tag == "")
                {
                    int n;
                    do
                    {
                        n = random.Next(0, subjectCourse.Count());
                    } while (classDic[subjectCourse[n]].StudentCount >= classDic[subjectCourse[n]].Limit);

                    datarow.Cells[5].Tag = subjectCourse[n];
                    classDic[subjectCourse[n]].StudentCount += 1;

                    ((DataGridViewColorBallTextCell)datarow.Cells[5]).Value = classDic[subjectCourse[n]].ClassType;
                    ((DataGridViewColorBallTextCell)datarow.Cells[5]).Color = _CourseColor[subjectCourse[n]];
                }
            }
            #endregion

            #region 計算科目班級人數
            Dictionary<string, Count> CountDic = new Dictionary<string, Count>();
            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if (CountDic.ContainsKey("" + datarow.Cells[5].Tag))
                {
                    if ("" + datarow.Cells[2].Value == "女")
                    {
                        CountDic["" + datarow.Cells[5].Tag].girl += 1;
                    }
                    if ("" + datarow.Cells[2].Value == "男")
                    {
                        CountDic["" + datarow.Cells[5].Tag].boy += 1;
                    }
                    if ("" + datarow.Cells[2].Value != "男" && "" + datarow.Cells[2].Value != "女")
                    {
                        CountDic["" + datarow.Cells[5].Tag].understand += 1;
                    }
                    CountDic["" + datarow.Cells[5].Tag].total += 1;
                }
                if (!CountDic.ContainsKey("" + datarow.Cells[5].Tag))
                {
                    CountDic.Add("" + datarow.Cells[5].Tag, new Count());

                    if ("" + datarow.Cells[2].Value == "女")
                    {
                        CountDic["" + datarow.Cells[5].Tag].girl = 1;
                    }
                    if ("" + datarow.Cells[2].Value == "男")
                    {
                        CountDic["" + datarow.Cells[5].Tag].boy = 1;
                    }
                    if ("" + datarow.Cells[2].Value != "男" && "" + datarow.Cells[2].Value != "女")
                    {
                        CountDic["" + datarow.Cells[5].Tag].understand = 1;
                    }
                    CountDic["" + datarow.Cells[5].Tag].total = 1;
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
            #endregion
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
