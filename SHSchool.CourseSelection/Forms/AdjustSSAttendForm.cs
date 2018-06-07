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
using FISCA.Data;
using DevComponents.DotNetBar;
using System.Drawing.Drawing2D;
using K12.Data;
using FISCA.Authentication;
using FISCA.LogAgent;
using System.IO;
using Aspose.Cells;
using System.Diagnostics;
using FISCA.Presentation;

namespace SHSchool.CourseSelection.Forms
{
    public partial class AdjustSSAttendForm : BaseForm
    {
        // 學年度學期所有科目
        private Dictionary<string, string> allSubjectDic = new Dictionary<string, string>();
        // 紀錄科目顏色
        private Dictionary<string, Color> subjectColorDic = new Dictionary<string, Color>();
        // 紀錄人數限制
        private Dictionary<string, SubjectCountLimit> _DicSubjectData = new Dictionary<string, SubjectCountLimit>();

        private List<DataRow> _DataRowList = new List<DataRow>();

        private string _actor;

        private string _client_info;

        private ContextMenu menu = new ContextMenu();

        AccessHelper access = new AccessHelper();
        QueryHelper qh = new QueryHelper();

        public AdjustSSAttendForm()
        {

            _actor = DSAServices.UserAccount;

            _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            InitializeComponent();

            #region Init SchoolYearLb、SemesterLb
            {
                List<UDT.OpeningTime> timeList = access.Select<UDT.OpeningTime>();

                if (timeList.Count == 0)
                {
                    timeList.Add(new UDT.OpeningTime() { SchoolYear = int.Parse(K12.Data.School.DefaultSchoolYear), Semester = int.Parse(K12.Data.School.DefaultSemester) });
                    timeList.SaveAll();
                }

                schoolYearCbx.Items.Add(timeList[0].SchoolYear + 1);
                schoolYearCbx.Items.Add(timeList[0].SchoolYear);
                schoolYearCbx.Items.Add(timeList[0].SchoolYear - 1);

                schoolYearCbx.SelectedIndex = 1;

                semesterCbx.Items.Add(1);
                semesterCbx.Items.Add(2);
                if ("" + timeList[0].Semester == "1")
                {
                    semesterCbx.SelectedIndex = 0;
                }

                if ("" + timeList[0].Semester == "2")
                {
                    semesterCbx.SelectedIndex = 1;
                }
            }
            #endregion

            #region Init 分發順序
            if (btnEasy.Checked)
            {
                //seedCbx.Enabled = false;
            }
            //seedCbx.Items.Add("隨機");
            //seedCbx.SelectedIndex = 0;
            #endregion

            #region Init 右鍵選單
            MenuItem item = new MenuItem("鎖定選課");
            item.Click += delegate
            {
                foreach (DataGridViewRow datarow in dataGridViewX1.SelectedRows)
                {
                    if ("" + datarow.Cells[5].Value != "") // 已選上課程學生才能鎖課
                    {
                        datarow.Cells["Lock"].Value = "是";
                        datarow.DefaultCellStyle.BackColor = Color.YellowGreen;
                        foreach (DataRow row in _DataRowList)
                        {
                            if (("" + datarow.Cells[2].Tag) == ("" + row["id"]))
                            {
                                row["lock"] = true;
                            }
                        }
                    }
                }
            };
            menu.MenuItems.Add(item);

            MenuItem item2 = new MenuItem("解除鎖定");
            item2.Click += delegate
            {
                foreach (DataGridViewRow datarow in dataGridViewX1.SelectedRows)
                {
                    datarow.Cells["Lock"].Value = "";
                    datarow.DefaultCellStyle.BackColor = Color.White;

                    foreach (DataRow row in _DataRowList)
                    {
                        if ("" + datarow.Cells[2].Tag == "" + row["id"])
                        {
                            row["lock"] = false;
                        }
                    }
                }

            };
            menu.MenuItems.Add(item2);
            #endregion

        }

        private void ReloadCourseTypeCbx()
        {
            courseTypeCbx.Items.Clear();
            string sql = string.Format(@"
                    SELECT DISTINCT 
                        type 
                    FROM 
                        $ischool.course_selection.subject
                    WHERE school_year = {0} AND semester = {1} AND type IS NOT NULL
                    ", schoolYearCbx.Text, semesterCbx.Text);

            DataTable dt = qh.Select(sql);

            foreach (DataRow dr in dt.Rows)
            {
                courseTypeCbx.Items.Add(dr["type"]);
            }
            if (courseTypeCbx.Items.Count > 0)
            {
                courseTypeCbx.SelectedIndex = 0;
            }
            if (courseTypeCbx.Items.Count == 0)
            {
                conditionCbx.Items.Clear();
                dataGridViewX1.Rows.Clear();
                flowLayoutPanel1.Controls.Clear();
            }
        }

        private void ReloadAllSubjectDic()
        {
            allSubjectDic.Clear();
            List<UDT.Subject> allSbList = access.Select<UDT.Subject>("school_year = " + schoolYearCbx.Text + " AND semester = " + semesterCbx.Text);
            foreach (UDT.Subject sb in allSbList)
            {
                allSubjectDic.Add(sb.UID, sb.SubjectName);
            }
            // 新增空白按鈕
            allSubjectDic.Add("", "空白");
        }

        private void schoolYearCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (semesterCbx.Text != "")
            {
                ReloadAllSubjectDic();
                ReloadCourseTypeCbx();
            }
        }

        private void semesterCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (schoolYearCbx.Text != "")
            {
                ReloadAllSubjectDic();
                ReloadCourseTypeCbx();
            }
        }

        private void courseTypeCbx_TextChanged(object sender, EventArgs e)
        {
            #region SQL
            string sql = string.Format(@"
SELECT 
	subject.uid
	, subject_name
	, subject.school_year
	, subject.semester
	, subject.type
	, subject.limit
	, count(ss_attend.ref_student_id)
FROM 
	$ischool.course_selection.subject AS subject
	LEFT OUTER JOIN	$ischool.course_selection.ss_attend AS ss_attend
		ON ss_attend.ref_subject_id = subject.uid
WHERE
	school_year = {0}
	AND semester = {1}
	AND type = '{2}'
GROUP BY 
	subject.uid
	, subject_name
	, subject.school_year
	, subject.semester
	, subject.type
	, subject.limit
", schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);

            #endregion

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql);

            _DicSubjectData.Clear();
            flowLayoutPanel1.Controls.Clear();

            ReloadConditionCbx(dt);

            ReloadFlowLayoutPanel(dt);

            ReloadDataGridView();

            seedCbx.Items.Clear();
            seedCbx.Items.Add("隨機");
            seedCbx.SelectedIndex = 0;
        }

        private void conditionCbx_TextChanged(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                ShowDataRow(true);
            }
        }

        public void ReloadConditionCbx(DataTable dt)
        {
            if (courseTypeCbx.Text != "")
            {
                conditionCbx.Items.Clear();
                conditionCbx.Items.Add("全部");
                conditionCbx.Items.Add("空白");
                foreach (DataRow row in dt.Rows)
                {
                    string subjectName = "" + row["subject_name"];
                    string subjectID = "" + row["uid"];
                    int subjectLimit = int.Parse("" + row["limit"]);
                    int studentCount = int.Parse("" + row["count"]);

                    conditionCbx.Items.Add(subjectName);
                    _DicSubjectData.Add(subjectID, new SubjectCountLimit());
                    _DicSubjectData[subjectID].SubjectLimit = subjectLimit;
                    _DicSubjectData[subjectID].StuCount = studentCount;
                }
                // 設定空白按鈕
                #region sql 取得未分發選修科目學生人數

                string sql = string.Format(@"
WITH target_subject AS(
	SELECT 
		uid 
	FROM
		$ischool.course_selection.subject AS subject
	WHERE
		subject.school_year = {0}
		AND subject.semester = {1}
		AND subject.type = '{2}'
) , target_class AS(
	SELECT
		ref_class_id
	FROM
		$ischool.course_selection.subject_class_selection
	WHERE
		ref_subject_id IN(
			SELECT 
				*
			FROM
				target_subject
		)
) , target_student AS(
	SELECT
		*
	FROM
		student
	WHERE
		ref_class_id IN(
			SELECT
				*
			FROM
				target_class
		)
)
SELECT 
	count(target_student.id)
FROM 
	target_student
WHERE
	target_student.id NOT IN(
		SELECT ss_attend.ref_student_id FROM $ischool.course_selection.ss_attend AS ss_attend
		WHERE ss_attend.ref_subject_id IN(
				SELECT * FROM target_subject
			)
		)"
                    , schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);

                #endregion
                //_DicSubjectData.Clear();
                QueryHelper qh = new QueryHelper();
                DataTable dataTable = qh.Select(sql);
                _DicSubjectData.Add("", new SubjectCountLimit());
                _DicSubjectData[""].SubjectLimit = 0;
                _DicSubjectData[""].StuCount = int.Parse(("" + dataTable.Rows[0][0]) == "" ? "0" : "" + dataTable.Rows[0][0]);
            }
            conditionCbx.SelectedIndex = 0;
        }

        public void ReloadFlowLayoutPanel(DataTable dt)
        {
            Color[] colors = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.PowderBlue, Color.Orange, Color.Green, Color.Purple, Color.Brown, Color.Gray };

            #region Init Button
            subjectColorDic.Clear();
            int n = 0;
            foreach (DataRow row in dt.Rows)
            {
                ButtonX button = new ButtonX();
                button.FocusCuesEnabled = false;
                button.Style = eDotNetBarStyle.Office2007;
                button.ColorTable = eButtonColor.Flat;
                //button.AutoSize = true;
                button.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(15);
                button.TextAlignment = eButtonTextAlignment.Left;
                button.Size = new Size(220, 20);

                button.Text = "( " + row["count"] + "/" + row["limit"] + " )" + row["subject_name"];
                if (button.Text.Length > 17)
                {
                    button.Text = button.Text.Substring(0, 17);
                }
                if (n >= 8)
                {
                    n = n % 8;
                }
                // 紀錄科目顏色
                subjectColorDic.Add("" + row["uid"], colors[n]);
                button.Image = GetColorBallImage(colors[n++]);
                // Subject UID
                button.Name = "" + row["subject_name"];
                button.Tag = "" + row["uid"];

                button.Margin = new System.Windows.Forms.Padding(3);
                button.Click += new EventHandler(Swap);

                this.flowLayoutPanel1.Controls.Add(button);
            }

            #region 空白按鈕

            ButtonX btn = new ButtonX();
            btn.FocusCuesEnabled = false;
            btn.Style = eDotNetBarStyle.Office2007;
            btn.ColorTable = eButtonColor.Flat;
            //button.AutoSize = true;
            btn.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(15);
            btn.TextAlignment = eButtonTextAlignment.Left;
            btn.Size = new Size(220, 20);

            btn.Text = "( " + _DicSubjectData[""].StuCount + "/0 ) 空白";
            if (btn.Text.Length > 17)
            {
                btn.Text = btn.Text.Substring(0, 17);
            }
            // 紀錄科目顏色
            subjectColorDic.Add("", Color.White);
            btn.Image = GetColorBallImage(Color.White);
            // Subject UID
            btn.Name = "空白";
            btn.Tag = "";

            btn.Margin = new System.Windows.Forms.Padding(3);
            btn.Click += new EventHandler(Swap);

            this.flowLayoutPanel1.Controls.Add(btn);
            #endregion
            #endregion
        }

        BackgroundWorker _BKWReloadDataGridView = new BackgroundWorker();

        public void ReloadDataGridView()
        {
            _DataRowList.Clear();
            dataGridViewX1.Rows.Clear();
            if (courseTypeCbx.Text != "" && !_BKWReloadDataGridView.IsBusy)
            {
                string schoolYear = schoolYearCbx.Text;
                string semester = semesterCbx.Text;
                string courseType = courseTypeCbx.Text;
                pictureBox1.Visible = true;
                // isLoading = true;
                #region SQL
                string sql = string.Format(@"

WITH target_subject AS(
	SELECT
		*
	FROM
		$ischool.course_selection.subject
	WHERE
		school_year = {0}
		AND semester = {1}
		AND type = '{2}'
),target_class AS(
	SELECT DISTINCT
		scs.ref_class_id
	FROM
		$ischool.course_selection.subject AS subject
		LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs
			ON scs.ref_subject_id = subject.uid
	WHERE
		school_year = {0}
		AND semester = {1}
		AND type = '{2}'
	ORDER BY scs.ref_class_id
), target_student AS(
	SELECT 
		student.id
		, student.ref_class_id
		, class.class_name
		, class.display_order
		, class.grade_year
		, student.seat_no
		, student.name
	FROM 
		student
			LEFT OUTER JOIN class 
				ON class.id = student.ref_class_id
	WHERE 
		student.ref_class_id IN(
			SELECT
				ref_class_id
			FROM
				target_class
		)
), student_attend AS(
	SELECT
		ref_student_id
		, ref_subject_id
		, ref_subject_course_id
        , lock
        , attend_type
		, subject.subject_name
	FROM
		$ischool.course_selection.ss_attend AS attend
		LEFT OUTER JOIN $ischool.course_selection.subject AS subject ON subject.uid = attend.ref_subject_id
	WHERE ref_subject_id IN(
			SELECT
				uid
			FROM
				target_subject
		)
) , wish_row AS(
	SELECT
		wish.ref_student_id
		, wish.ref_subject_id
		, wish.sequence
		, $ischool.course_selection.subject.subject_name
	FROM
		$ischool.course_selection.ss_wish AS wish
		LEFT OUTER JOIN $ischool.course_selection.subject ON $ischool.course_selection.subject.uid = wish.ref_subject_id
	WHERE
		ref_subject_id IN(
			SELECT 
				uid
			FROM
				target_subject
		)
) , wish AS(
	SELECT
		ref_student_id
		, ROW_NUMBER() OVER(PARTITION BY ref_student_id ORDER BY sequence) AS sequence	
		, ref_subject_id
		, subject_name
	FROM
		wish_row
) 
SELECT
	target_student.*
    , null AS 分發順位
    , null AS 分發志願
    , student_attend.lock
    , student_attend.attend_type
    , student_attend.ref_subject_id
	, student_attend.subject_name AS 選課課程
	, wish1.subject_name AS 志願1
	, wish2.subject_name AS 志願2
	, wish3.subject_name AS 志願3
	, wish4.subject_name AS 志願4
	, wish5.subject_name AS 志願5
	, wish6.subject_name AS 志願6
	, wish7.subject_name AS 志願7
	, wish8.subject_name AS 志願8
	, wish1.ref_subject_id AS 志願1ref_subject_id
	, wish2.ref_subject_id AS 志願2ref_subject_id
	, wish3.ref_subject_id AS 志願3ref_subject_id
	, wish4.ref_subject_id AS 志願4ref_subject_id
	, wish5.ref_subject_id AS 志願5ref_subject_id
	, wish6.ref_subject_id AS 志願6ref_subject_id
	, wish7.ref_subject_id AS 志願7ref_subject_id
	, wish8.ref_subject_id AS 志願8ref_subject_id
FROM
	target_student
	LEFT OUTER JOIN student_attend
		ON student_attend.ref_student_id = target_student.id
	LEFT OUTER JOIN wish as wish1
		ON wish1.ref_student_id = target_student.id
			AND wish1.sequence = 1
	LEFT OUTER JOIN wish as wish2
		ON wish2.ref_student_id = target_student.id
			AND wish2.sequence = 2
	LEFT OUTER JOIN wish as wish3
		ON wish3.ref_student_id = target_student.id
			AND wish3.sequence = 3
	LEFT OUTER JOIN wish as wish4
		ON wish4.ref_student_id = target_student.id
			AND wish4.sequence = 4
	LEFT OUTER JOIN wish as wish5
		ON wish5.ref_student_id = target_student.id
			AND wish5.sequence = 5
	LEFT OUTER JOIN wish as wish6
		ON wish6.ref_student_id = target_student.id
			AND wish6.sequence = 6
	LEFT OUTER JOIN wish as wish7
		ON wish7.ref_student_id = target_student.id
			AND wish7.sequence = 7
	LEFT OUTER JOIN wish as wish8
		ON wish8.ref_student_id = target_student.id
			AND wish8.sequence = 8
ORDER BY 
	target_student.grade_year
	, target_student.display_order
	, target_student.class_name
	, target_student.seat_no
	, target_student.id
"
                    , schoolYear, semester, courseType);
                #endregion
                QueryHelper qh = new QueryHelper();
                DataTable dt = null;
                _BKWReloadDataGridView = new BackgroundWorker();

                _BKWReloadDataGridView.DoWork += delegate
                {
                    dt = qh.Select(sql);
                };
                _BKWReloadDataGridView.RunWorkerCompleted += delegate
                {
                    if (schoolYear == schoolYearCbx.Text && semester == semesterCbx.Text && courseType == courseTypeCbx.Text)
                    {
                        // isLoading = false;
                        foreach (DataRow row in dt.Rows)
                        {
                            _DataRowList.Add(row);
                            int index = 0;
                            DataGridViewRow datarow = new DataGridViewRow();
                            datarow.CreateCells(dataGridViewX1);

                            datarow.Cells[index++].Value = "" + row["class_name"];
                            datarow.Cells[index++].Value = "" + row["seat_no"];
                            datarow.Cells[index].Tag = "" + row["id"]; // 記錄學生ID
                            datarow.Cells[index++].Value = "" + row["name"];
                            datarow.Cells[index++].Value = ("" + row["lock"]) == "true" ? "是" : "";
                            if ("" + row["lock"] == "true")
                            {
                                datarow.DefaultCellStyle.BackColor = Color.YellowGreen;
                            }
                            datarow.Cells[index++].Value = "" + row["分發順位"];

                            if ("" + row["ref_subject_id"] != string.Empty)
                            {
                                ((DataGridViewColorBallTextCell)datarow.Cells[index]).Value = "" + row["選課課程"];
                                ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = subjectColorDic["" + row["ref_subject_id"]];
                            }
                            datarow.Cells[index++].Tag = "" + row["ref_subject_id"];
                            datarow.Cells[index++].Value = "" + row["志願1"];
                            datarow.Cells[index++].Value = "" + row["志願2"];
                            datarow.Cells[index++].Value = "" + row["志願3"];
                            datarow.Cells[index++].Value = "" + row["志願4"];
                            datarow.Cells[index++].Value = "" + row["志願5"];
                            datarow.Cells[index++].Value = ""; // 分發志願
                            datarow.Cells[index++].Value = "" + row["attend_type"];

                            datarow.Tag = row;
                            dataGridViewX1.Rows.Add(datarow);
                        }
                        pictureBox1.Visible = false;
                    }
                    else
                    {
                        ReloadDataGridView();
                    }
                };
                _BKWReloadDataGridView.RunWorkerAsync();


            }
        }

        private void Swap(object sender, EventArgs e)
        {
            bool swapEnabled = true;
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                if ("" + row.Cells["Lock"].Value == "是")
                {
                    swapEnabled = false;
                }
            }
            if (swapEnabled)
            {
                ButtonX button = (ButtonX)sender;
                #region 警告人數超過
                // 剩餘名額與調整學生人數比較
                int limit = _DicSubjectData["" + button.Tag].SubjectLimit;
                int 剩餘名額 = _DicSubjectData["" + button.Tag].SubjectLimit - _DicSubjectData["" + button.Tag].StuCount;
                int 調整人數 = dataGridViewX1.SelectedRows.Count;
                if (limit != 0)
                {
                    if (剩餘名額 == 0)
                    {
                        var result = MessageBox.Show(button.Name + "已達人數限制! \n是否確定將學生加入選修科目?", "  警告", MessageBoxButtons.YesNo);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                    if (剩餘名額 < 調整人數 && 剩餘名額 != 0)
                    {
                        var result = MessageBox.Show(button.Name + "名額不足! \n是否確定將學生加入選修科目?", "  警告", MessageBoxButtons.YesNo);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                }
                #endregion
                #region 更新DataRow
                foreach (DataGridViewRow dgvrow in dataGridViewX1.SelectedRows)
                {
                    DataRow row = (DataRow)dgvrow.Tag;
                    row["ref_subject_id"] = button.Tag;

                    row["選課課程"] = allSubjectDic["" + button.Tag];
                    if ("" + button.Tag == "")
                    {
                        row["attend_type"] = "";
                    }
                    if ("" + button.Tag != "")
                    {
                        row["attend_type"] = "指定";
                    }
                }
                #endregion
                //更新顯示
                ShowDataRow();
            }
            else
            {
                MessageBox.Show("學生已鎖定，無法調整選課結果!");
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

        private void leaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            // 兜資料
            List<string> dataList = new List<string>();

            int seed = 0;
            string _seed = seedCbx.Text;
            string[] _seedArray = _seed.Split(':');
            foreach (string s in _seedArray)
            {
                if (int.TryParse(s, out seed))
                {
                    seed = int.Parse(s);
                }
            }
            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                DataRow row = (DataRow)datarow.Tag;
                string data = string.Format(@"
    SELECT
        {0}::BIGINT AS ref_student_id
        , {1}::BIGINT AS ref_subject_id
        , '{2}'::TEXT AS subject_name
        , {3}::BOOLEAN AS lock
        , '{4}'::TEXT AS attend_type
        , {5}::INTEGER AS school_year
        , {6}::INTEGER AS semester
        , '{7}'::TEXT AS type
        , '{8}'::INTEGER AS seed
                ", row["id"]
                , "" + datarow.Cells[5].Tag == "" ? "NULL" : "" + datarow.Cells[5].Tag
                , "" + datarow.Cells[5].Value == "" ? "NULL" : "" + datarow.Cells[5].Value
                , "" + datarow.Cells["lock"].Value == "是" ? "true" : "false"
                , "" + datarow.Cells["attendType"].Value == "" ? "NULL" : "" + datarow.Cells["attendType"].Value
                , schoolYearCbx.Text
                , semesterCbx.Text
                , courseTypeCbx.Text
                , "" + seed
                );
                dataList.Add(data);
            }

            string attendData = string.Join(" UNION ALL", dataList);
            #region SQL
            string sql = string.Format(@"
WITH data_row AS(
    {0}           
) ,source AS (
    SELECT
        data_row.*
        , ss_attend.uid AS ref_attend_id
        , ss_attend.ref_subject_id AS orig_subject_id
        , ss_attend.subject_name AS orig_subject_name
        , ss_attend.lock AS orig_lock
        , ss_attend.attend_type AS orig_attend_type
        , ROW_NUMBER() OVER( PARTITION BY data_row.ref_student_id ) AS index
    FROM
        data_row
        LEFT OUTER JOIN(
            SELECT
                ss_attend.uid
                , ss_attend.ref_subject_id
                , ss_attend.ref_student_id
                , ss_attend.lock
                , ss_attend.attend_type
                , subject.subject_name
                , subject.school_year
                , subject.semester
                , subject.type
            FROM
                $ischool.course_selection.ss_attend AS ss_attend
                LEFT OUTER JOIN $ischool.course_selection.subject AS subject
                    ON subject.uid = ss_attend.ref_subject_id 
        ) AS ss_attend
            ON ss_attend.ref_student_id = data_row.ref_student_id
            AND ss_attend.school_year = data_row.school_year
            AND ss_attend.semester = data_row.semester
            AND ss_attend.type = data_row.type
) ,data_source AS(
    SELECT
        source.*
        ,CASE 
            WHEN source.orig_subject_id = source.ref_subject_id AND source.orig_lock <> source.lock AND source.index = 1 THEN 'update'  
            WHEN source.orig_subject_id is null AND source.ref_subject_id is not null AND source.index = 1 THEN 'insert' 
            WHEN source.orig_subject_id <> source.ref_subject_id AND source.ref_subject_id is not null AND source.index = 1 THEN 'delete_insert'
            WHEN source.ref_attend_id is not null AND( source.orig_subject_id <> source.ref_subject_id OR source.ref_subject_id is null OR source.index > 1) THEN 'delete'
            --WHEN source.orig_subject_id = source.ref_subject_id AND ss_attend.lock = source.lock THEN 'nochange' 
            ELSE 'nochange'
            END AS status
    FROM
        source
) ,log_data AS(
    SELECT
        data_source.*
        ,CASE 
            WHEN data_source.status = 'update' THEN '學生「'|| student.name || '」課程類別「'|| data_source.type ||'」選課鎖定狀態「' || data_source.orig_lock || '」變更為「' || data_source.lock || ' 」選修方式「data_source.attend_type」 使用者「{2}」'
            WHEN data_source.status = 'insert' THEN '學生「'|| student.name || '」課程類別「'|| data_source.type ||'」 選修科目結果為「' || data_source.subject_name || '」選課鎖定狀態為「' || data_source.lock || '」分發順位代碼「' || data_source.seed || ' 」選修方式「data_source.attend_type」使用者「{2}」'
            WHEN data_source.status = 'delete_insert' THEN '學生「'|| student.name || '」課程類別「'|| data_source.type ||'」選修科目結果「' || data_source.orig_subject_name || '」變更為「' || data_source.subject_name || ' 」選課鎖定狀態「' || data_source.orig_lock || '」變更為「' || data_source.lock || ' 」分發順位代碼「' || data_source.seed || ' 」選修方式「data_source.attend_type」 使用者「{2}」'
            WHEN data_source.status = 'delete' THEN '刪除學生「'|| student.name || '」課程類別「'|| data_source.type ||'」 原選修科目結果「' || data_source.orig_subject_name || '」   使用者「{1}」'
        END AS description
    FROM
        data_source
        LEFT OUTER JOIN student
            ON student.id = data_source.ref_student_id
) ,delete_data AS(
    DELETE
    FROM
        $ischool.course_selection.ss_attend
    WHERE
        uid IN (
            SELECT 
                ref_attend_id 
            FROM 
                data_source 
            WHERE 
                status = 'delete' 
                OR status = 'delete_insert' 
        )
    RETURNING $ischool.course_selection.ss_attend.*
) ,insert_data AS(
    INSERT INTO $ischool.course_selection.ss_attend(
        ref_student_id
        , ref_subject_id
        , attend_type
        , lock
    )
    SELECT 
        ref_student_id
        , ref_subject_id
        , attend_type
        , lock
    FROM
        data_source
    WHERE
        status = 'delete_insert' 
        OR status = 'insert'
    RETURNING *
) ,update_data AS(
    UPDATE $ischool.course_selection.ss_attend
    SET
        lock = data_source.lock
    FROM
        data_source
    WHERE
        $ischool.course_selection.ss_attend.uid = data_source.ref_attend_id
        AND status = 'update'
    RETURNING $ischool.course_selection.ss_attend.*
) 
-- 新增 LOG
INSERT INTO log(
    actor
    , action_type
    , action
    , target_category
    , target_id
    , server_time
    , client_info
    , action_by
    , description
)
SELECT
    '{1}'::TEXT AS actor
    , 'Record' AS action_type
    , '選課結果及分發' AS action
    , 'student'::TEXT AS target_category
    , ref_student_id AS target_id
    , now() AS server_time
    , '{2}' AS client_info
    , '選課結果及分發'AS action_by   
    , description AS description 
FROM
    log_data
WHERE
    description is not null


            ", attendData, _actor, _client_info);
            #endregion

            #region SQL文字檔
            // SQL 文字檔
            //string path = "SQL.txt";
            //FileStream file = new FileStream(path,FileMode.Create);
            //StreamWriter sw = new StreamWriter(file);
            //sw.Write(sql);

            //sw.Flush();
            //sw.Close();
            //file.Close();

            //QueryHelper qh = new QueryHelper();
            //qh.Select(sql);
            #endregion
            _BKWReloadDataGridView = new BackgroundWorker();

            UpdateHelper up = new UpdateHelper();

            pictureBox1.Visible = true;
            schoolYearCbx.Enabled = false;
            semesterCbx.Enabled = false;
            courseTypeCbx.Enabled = false;
            conditionCbx.Enabled = false;
            dataGridViewX1.Enabled = false;
            exportBtn.Enabled = false;
            buttonX1.Enabled = false;
            seedCbx.Enabled = false;
            saveBtn.Enabled = false;
            leaveBtn.Enabled = false;


            _BKWReloadDataGridView.DoWork += delegate
            {
                up.Execute(sql);
            };

            _BKWReloadDataGridView.RunWorkerCompleted += delegate
            {
                pictureBox1.Visible = false;
                schoolYearCbx.Enabled = true;
                semesterCbx.Enabled = true;
                courseTypeCbx.Enabled = true;
                conditionCbx.Enabled = true;
                dataGridViewX1.Enabled = true;
                exportBtn.Enabled = true;
                buttonX1.Enabled = true;
                seedCbx.Enabled = true;
                saveBtn.Enabled = true;
                leaveBtn.Enabled = true;
                MessageBox.Show("儲存成功!");

                ReloadDataGridView();
            };
            _BKWReloadDataGridView.RunWorkerAsync();


        }

        private void ShowDataRow()
        {
            ShowDataRow(false);
        }
        private void ShowDataRow(bool renewRow)
        {
            dataGridViewX1.SuspendLayout();
            if (renewRow)
                dataGridViewX1.Rows.Clear();

            if (renewRow)
            {
                foreach (var row in _DataRowList)
                {
                    if (
                        (conditionCbx.Text == "全部")
                        || (("" + row["選課課程"]) == conditionCbx.Text)
                        || (("" + row["選課課程"]) == "" && conditionCbx.Text == "空白")
                    )
                    {
                        int index = 0;
                        DataGridViewRow datarow = new DataGridViewRow();
                        datarow.CreateCells(dataGridViewX1);

                        datarow.Cells[index++].Value = "" + row["class_name"];
                        datarow.Cells[index++].Value = "" + row["seat_no"];
                        datarow.Cells[index].Tag = "" + row["id"]; // 紀錄學生ID
                        datarow.Cells[index++].Value = "" + row["name"];
                        datarow.Cells[index++].Value = ("" + row["lock"]) == "true" ? "是" : "";
                        if ("" + row["lock"] == "true")
                        {
                            datarow.DefaultCellStyle.BackColor = Color.GreenYellow;
                        }
                        datarow.Cells[index++].Value = "" + row["分發順位"];
                        if ("" + row["ref_subject_id"] != string.Empty)
                        {
                            ((DataGridViewColorBallTextCell)datarow.Cells[index]).Value = "" + row["選課課程"];
                            ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = subjectColorDic["" + row["ref_subject_id"]];
                        }
                        datarow.Cells[index++].Tag = "" + row["ref_subject_id"];
                        for (int i = 1; i <= 5; i++)
                        {
                            if ("" + row["志願" + i + "ref_subject_id"] == "" + row["ref_subject_id"] && "" + row["ref_subject_id"] != "")
                            {
                                datarow.Cells[index].Style.ForeColor = Color.Red;
                            }
                            datarow.Cells[index++].Value = "" + row["志願" + i];
                        }
                        datarow.Cells[index++].Value = "" + row["分發志願"];
                        datarow.Cells[index++].Value = "" + row["attend_type"];

                        datarow.Tag = row;
                        dataGridViewX1.Rows.Add(datarow);
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
                {
                    DataRow row = (DataRow)datarow.Tag;
                    int index = 0;
                    datarow.Cells[index++].Value = "" + row["class_name"];
                    datarow.Cells[index++].Value = "" + row["seat_no"];
                    datarow.Cells[index].Tag = "" + row["id"]; // 紀錄學生ID
                    datarow.Cells[index++].Value = "" + row["name"];
                    datarow.Cells[index++].Value = ("" + row["lock"]) == "true" ? "是" : "";
                    if ("" + row["lock"] == "true")
                    {
                        datarow.DefaultCellStyle.BackColor = Color.GreenYellow;
                    }
                    else
                    {
                        datarow.DefaultCellStyle.BackColor = dataGridViewX1.DefaultCellStyle.BackColor;
                    }
                    datarow.Cells[index++].Value = "" + row["分發順位"];
                    if ("" + row["ref_subject_id"] != string.Empty)
                    {
                        ((DataGridViewColorBallTextCell)datarow.Cells[index]).Value = "" + row["選課課程"];
                        ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = subjectColorDic["" + row["ref_subject_id"]];
                    }
                    else
                    {
                        ((DataGridViewColorBallTextCell)datarow.Cells[index]).Value = "";
                        ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = dataGridViewX1.DefaultCellStyle.BackColor;
                    }
                    datarow.Cells[index++].Tag = "" + row["ref_subject_id"];
                    for (int i = 1; i <= 5; i++)
                    {
                        if ("" + row["志願" + i + "ref_subject_id"] == "" + row["ref_subject_id"] && "" + row["ref_subject_id"] != "")
                        {
                            datarow.Cells[index].Style.ForeColor = Color.Red;
                        }
                        else
                        {
                            datarow.Cells[index].Style.ForeColor = dataGridViewX1.DefaultCellStyle.ForeColor;
                        }
                        datarow.Cells[index++].Value = "" + row["志願" + i];
                    }
                    datarow.Cells[index++].Value = "" + row["分發志願"];
                    datarow.Cells[index++].Value = "" + row["attend_type"];
                }
            }
            dataGridViewX1.ResumeLayout();

            #region 更新按鈕人數統計
            ReCountSubjectStu();

            foreach (ButtonX btn in flowLayoutPanel1.Controls)
            {
                string subjectID = "" + btn.Tag;
                string subjectName = allSubjectDic[subjectID];

                btn.Text = string.Format("({0}/{1})", ("" + _DicSubjectData[subjectID].StuCount).PadLeft(3), ("" + _DicSubjectData[subjectID].SubjectLimit).PadRight(3)) + subjectName;
                if (btn.Text.Length > 17)
                {
                    btn.Text = btn.Text.Substring(0, 17);
                }
            }
            #endregion
        }

        private void ReCountSubjectStu()
        {
            foreach (SubjectCountLimit sc in _DicSubjectData.Values)
            {
                sc.StuCount = 0; // 清空人數紀錄
            }
            foreach (DataRow row in _DataRowList)
            {
                // 重新計算選修科目人數
                _DicSubjectData["" + row["ref_subject_id"]].StuCount++;
            }
        }

        private void dataGridViewX1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column == Column10)
            {
                int v1 = 0, v2 = 0;
                int.TryParse("" + e.CellValue1, out v1);
                int.TryParse("" + e.CellValue2, out v2);
                e.SortResult = v1.CompareTo(v2);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 匯出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportBtn_Click(object sender, EventArgs e)
        {
            Workbook template = new Workbook(new MemoryStream(Properties.Resources.匯出選課結果樣板));

            Workbook book = new Workbook();
            book.Copy(template);
            Worksheet sheet = book.Worksheets[0];

            int row = 1;
            Style style = sheet.Cells.GetCellStyle(0, 0);
            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                for (int col = 0; col < dataGridViewX1.Columns.Count; col++)
                {
                    sheet.Cells[row, col].PutValue(datarow.Cells[col].Value);
                    sheet.Cells[row, col].SetStyle(style);
                }
                row++;
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel (*.xlsx)|*.xlsx|所有檔案 (*.*)|*.*";
            string fileName = string.Format("{0}_{1}_選課結果", courseTypeCbx.Text, conditionCbx.Text);
            saveFile.FileName = fileName;
            try
            {
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    book.Save(saveFile.FileName);
                    Process.Start(saveFile.FileName);
                    MotherForm.SetStatusBarMessage("課堂點名明細,列印完成!!");
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("檔案未儲存");
                    return;
                }
            }
            catch
            {
                FISCA.Presentation.Controls.MsgBox.Show("檔案儲存錯誤,請檢查檔案是否開啟中!!");
                MotherForm.SetStatusBarMessage("檔案儲存錯誤,請檢查檔案是否開啟中!!");
            }
        }

        /// <summary>
        /// 志願分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (btnEasy.Checked == true) // 簡單模式
            {
                btnClear_Click(null, null);
                btnOrder_Click(null, null);
                buttonItem9_Click(null, null);

                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 簡單模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEasy_Click(object sender, EventArgs e)
        {
            btnEasy.Checked = true;
            btnPro.Checked = false;
            btnClear.Enabled = false;
            btnDistribute.Enabled = false;
            btnOrder.Enabled = false;
            buttonX1.AutoExpandOnClick = false;
            //seedCbx.Enabled = false;
        }

        /// <summary>
        /// 進階模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPro_Click(object sender, EventArgs e)
        {
            btnEasy.Checked = false;
            btnPro.Checked = true;
            btnClear.Enabled = true;
            btnDistribute.Enabled = true;
            btnOrder.Enabled = true;
            buttonX1.AutoExpandOnClick = true;
            //seedCbx.Enabled = true;
        }

        /// <summary>
        /// 清除分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in _DataRowList)
            {
                if ("" + row["lock"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
                {
                    row["ref_subject_id"] = "";
                    row["選課課程"] = "";
                    row["分發志願"] = "";
                }
            }
            ReCountSubjectStu();
            if (sender == btnClear)
                ShowDataRow();
        }

        /// <summary>
        /// 產生分發順位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOrder_Click(object sender, EventArgs e)
        {
            bool match = false;
            int seed = 0;
            if (seedCbx.Text == "隨機")
            {
                match = true;
                seed = new Random().Next(3000);
                int index = seedCbx.Items.Count;
                string text = string.Format("代碼{0}: ", index);
                seedCbx.Items.Insert(1, text + seed);
                seedCbx.SelectedIndex = 0;

                lblCurrentSeed.Text = text + seed;
            }
            else
            {
                if (int.TryParse(seedCbx.Text, out seed))
                {
                    match = true;
                    lblCurrentSeed.Text = "指定: " + seed;
                }
                else
                {
                    string _seed = seedCbx.Text;
                    string[] _seedArray = _seed.Split(':');
                    foreach (string s in _seedArray)
                    {
                        if (int.TryParse(s, out seed))
                        {
                            match = true;
                            lblCurrentSeed.Text = _seed;
                            break;
                        }
                    }
                }
            }
            if (match)
            {
                List<DataRow> processRow = new List<DataRow>();
                foreach (DataRow row in _DataRowList)
                {
                    row["分發順位"] = "";
                    if ("" + row["lock"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
                    {
                        processRow.Add(row);
                    }
                }


                Random random = new Random(seed);
                List<int> list = new List<int>(processRow.Count);
                for (int i = 0; i < processRow.Count; i++)
                {
                    list.Add(i + 1);
                }
                // 更新DataRow
                foreach (var row in _DataRowList)
                {
                    if ("" + row["lock"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
                    {
                        int orderIndex = random.Next(list.Count);
                        int order = list[orderIndex];
                        list.RemoveAt(orderIndex);
                        row["分發順位"] = order;
                    }
                }
            }
            if (sender == btnOrder)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 依序分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem9_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 5; i++)
            {
                distribute(i, i == 1);
            }

            // ShowDataRow
            if (sender == buttonItem9)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願一分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem4_Click(object sender, EventArgs e)
        {
            distribute(1, true);

            if (sender == buttonItem4)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願二分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem5_Click(object sender, EventArgs e)
        {
            distribute(2, true);

            if (sender == buttonItem5)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願三分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem6_Click(object sender, EventArgs e)
        {
            distribute(3, true);

            if (sender == buttonItem6)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願四分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem7_Click(object sender, EventArgs e)
        {
            distribute(4, true);

            if (sender == buttonItem7)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願五分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem8_Click(object sender, EventArgs e)
        {
            distribute(5, true);

            if (sender == buttonItem8)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 分發
        /// </summary>
        /// <param name="wishOrder"></param>
        private void distribute(int wishOrder, bool showErrorMsg)
        {
            List<int> list = new List<int>(_DataRowList.Count);
            // 將DataRow按分發順位排序
            Dictionary<int, DataRow> dicSortDataRow = new Dictionary<int, DataRow>();
            foreach (var row in _DataRowList)
            {
                if ("" + row["分發順位"] == "" && "" + row["lock"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
                {
                    if (showErrorMsg)
                        MessageBox.Show("請先產生分發順位!");
                    break;
                }
                if ("" + row["lock"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
                {
                    dicSortDataRow.Add(int.Parse("" + row["分發順位"]), row);
                }
            }

            for (int i = 1; i <= dicSortDataRow.Count; i++)
            {
                DataRow row = dicSortDataRow[i];
                string selectedSubjectID = "" + row["ref_subject_id"];
                string wishSubjectID = "" + row["志願" + wishOrder + "ref_subject_id"];

                if (
                    selectedSubjectID == ""
                    && wishSubjectID != ""
                    && _DicSubjectData.ContainsKey(wishSubjectID)
                    && _DicSubjectData[wishSubjectID].StuCount < _DicSubjectData[wishSubjectID].SubjectLimit
                ) // 未分發科目
                {
                    row["ref_subject_id"] = wishSubjectID;
                    row["選課課程"] = "" + row["志願" + wishOrder];
                    row["分發志願"] = wishOrder;
                    row["attend_type"] = "志願分發";
                    _DicSubjectData[wishSubjectID].StuCount++;
                }
            }
            ReCountSubjectStu();
        }

        private void dataGridViewX1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                menu.Show(dataGridViewX1, new Point(e.X, e.Y));
            }
        }

        private void labelX2_DoubleClick(object sender, EventArgs e)
        {
            btnTrial.Visible = true;
        }

        private void btnTrialClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("此功能為測試選填使用，清除志願選填後將無法回復", "刪除選填志願", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;

            var refSubjectIDList = new List<int>();
            int i;
            foreach (var k in _DicSubjectData.Keys)
            {
                if (int.TryParse(k, out i))
                    refSubjectIDList.Add(i);
            }
            var list = access.Select<SHSchool.CourseSelection.UDT.SSWish>("ref_subject_id IN (-1, " + string.Join(",", refSubjectIDList) + ")");
            foreach (var item in list)
            {
                item.Deleted = true;
            }
            list.SaveAll();

            courseTypeCbx_TextChanged(null, null);
        }

        private void btnTrialFill_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("此功能為測試選填使用，模擬學生完成志願選填的情況", "模擬選填志願", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;
            var rand = new Random(new Random().Next(3000));
            var bkw = new BackgroundWorker();
            bkw.DoWork += delegate
            {
                var refSubjectIDKey = new Dictionary<int, int>();
                int scale = 0;
                int tryParseInt;
                foreach (var k in _DicSubjectData.Keys)
                {
                    if (int.TryParse(k, out tryParseInt))
                    {
                        scale += _DicSubjectData[k].SubjectLimit;
                        refSubjectIDKey.Add(tryParseInt, scale);
                    }
                }
                var list = new List<SHSchool.CourseSelection.UDT.SSWish>();
                foreach (DataRow row in _DataRowList)
                {
                    var refStudentID = "" + row["id"];
                    bool empty = true;
                    for (int i = 1; i <= 5; i++)
                    {
                        if ("" + row["志願" + i + "ref_subject_id"] != "")
                            empty = false;
                    }
                    if (empty)
                    {
                        int seed = rand.Next(100);
                        //Trace.WriteLine(seed);
                        int wc = 0;
                        if (seed <= 45)
                            wc = 5;
                        else if (seed <= 70)
                            wc = 4;
                        else if (seed <= 88)
                            wc = 3;
                        else if (seed <= 93)
                            wc = 2;
                        else if (seed <= 97)
                            wc = 1;
                        else
                            wc = 0;
                        var subjectList = new List<int>(refSubjectIDKey.Keys);
                        for (int wishOrder = 1; wishOrder <= wc; wishOrder++)
                        {
                            var c = 0;
                            foreach (var subject in subjectList)
                            {
                                c += refSubjectIDKey[subject];
                            }
                            var targetSeed = rand.Next(c);
                            //Trace.WriteLine(targetSeed);
                            var targetID = 0;
                            foreach (var subject in subjectList)
                            {
                                c -= refSubjectIDKey[subject];
                                if (c <= targetSeed)
                                {
                                    targetID = subject;
                                    break;
                                }
                            }
                            subjectList.Remove(targetID);
                            if (targetID > 0)
                            {
                                list.Add(new UDT.SSWish()
                                {
                                    StudentID = int.Parse(refStudentID),
                                    SubjectID = targetID,
                                    Order = wishOrder
                                });
                            }
                        }
                    }
                }
                list.SaveAll();
            };

            bkw.RunWorkerCompleted += delegate
            {
                courseTypeCbx_TextChanged(null, null);
            };

            bkw.RunWorkerAsync();
        }
    }
}

/// <summary>
/// 紀錄科目修課人數與人數限制
/// </summary>
class SubjectCountLimit
{
    public int SubjectLimit { get; set; }
    public int StuCount { get; set; }
}
