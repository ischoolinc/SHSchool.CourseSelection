﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        // 紀錄科目顏色
        private Dictionary<string, Color> subjectColorDic = new Dictionary<string, Color>();


        // 紀錄人數限制
        private Dictionary<string, SubjectCountLimit> _DicSubjectData = new Dictionary<string, SubjectCountLimit>();

        /// <summary>
        /// 學生擋修名單資料 key: 學生系統編號, key:科目系統編號
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> _DicStudentBlackList = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 不開課科目
        /// </summary>
        private Dictionary<string, UDT.Subject> _DicDisOpenSubject = new Dictionary<string, UDT.Subject>();

        /// <summary>
        /// 本學期的相關科目
        /// </summary>
        private Dictionary<string, UDT.Subject> _DicDefSubject = new Dictionary<string, UDT.Subject>();

        /// <summary>
        /// 學生志願衝堂科目
        /// </summary>
        private Dictionary<string, List<string>> _DicStudentConflictWish = new Dictionary<string, List<string>>();

        /// <summary>
        /// 學生課程時段衝堂科目
        /// </summary>
        private Dictionary<string, List<string>> _DicStudentConflictSubject = new Dictionary<string, List<string>>();

        /// <summary>
        /// 學生已選科目
        /// </summary>
        private Dictionary<string, List<string>> _DicRepeatSubjectByStudentID = new Dictionary<string, List<string>>();

        private List<DataRow> _DataRowList = new List<DataRow>();

        /// <summary>
        /// 工具
        /// </summary>
        private Tool tool;

        private string _actor;

        private string _client_info;

        private ContextMenu menu = new ContextMenu();

        private AccessHelper access = new AccessHelper();
        private QueryHelper qh = new QueryHelper();

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
                schoolYearCbx.Items.Add(timeList[0].SchoolYear + 2);  //Cyn新增
                schoolYearCbx.Items.Add(timeList[0].SchoolYear + 1);
                schoolYearCbx.Items.Add(timeList[0].SchoolYear);
                schoolYearCbx.Items.Add(timeList[0].SchoolYear - 1);
                schoolYearCbx.Items.Add(timeList[0].SchoolYear - 2); //Cyn新增
                schoolYearCbx.SelectedIndex = 2;  //Cyn 由1改為2

                semesterCbx.Items.Add(1);
                semesterCbx.Items.Add(2);

                semesterCbx.SelectedIndex = timeList[0].Semester - 1;
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
                    if ("" + datarow.Cells[6].Value != "") // 已選上課程學生才能鎖課  //Cyn 因增加學號由5改成6
                    {
                        ((DataRow)datarow.Tag)["lock"] = "true";
                    }
                }
                ShowDataRow();
            };
            menu.MenuItems.Add(item);

            MenuItem item2 = new MenuItem("解除鎖定");
            item2.Click += delegate
            {
                foreach (DataGridViewRow datarow in dataGridViewX1.SelectedRows)
                {
                    ((DataRow)datarow.Tag)["lock"] = "false";
                }
                ShowDataRow();
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
WHERE 
    school_year = {0} 
    AND semester = {1} 
    AND type IS NOT NULL
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

        private void schoolYearCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (semesterCbx.Text != "")
            {
                tool = new Tool(schoolYearCbx.Text, semesterCbx.Text);
                ReloadCourseTypeCbx();
            }
        }

        private void semesterCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (schoolYearCbx.Text != "")
            {
                tool = new Tool(schoolYearCbx.Text, semesterCbx.Text);
                ReloadCourseTypeCbx();
            }
        }

        private void courseTypeCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region SQL
            string sql = string.Format(@"
SELECT 
	subject.uid
	, subject_name
    , subject.level
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
ORDER BY
    subject.type
    , subject.subject_name
    , subject.level
    , subject.credit
", schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);

            #endregion

            DataTable dt = qh.Select(sql);

            _DicSubjectData.Clear();
            flowLayoutPanel1.Controls.Clear();

            ReloadConditionCbx(dt);

            ReloadFlowLayoutPanel(dt);

            ReloadDataGridView();

            seedCbx.Items.Clear();
            seedCbx.Items.Add("隨機");
            seedCbx.SelectedIndex = 0;

            GetBlackListData(schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);

            GetDisOpenSubject(schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);

            GetAllSubject(schoolYearCbx.Text, semesterCbx.Text);

            GetConflictWish(schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);

            GetRepeatSubject(schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);

            GetConflictSubject(schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);
        }

        /// <summary>
        /// 取得本學期科目
        /// </summary>
        private void GetAllSubject(string schoolYear, string semester)
        {
            _DicDefSubject.Clear();

            List<UDT.Subject> listSubject = access.Select<UDT.Subject>(string.Format("school_year = {0} AND semester = {1}", schoolYear, semester));

            foreach (UDT.Subject subject in listSubject)
            {
                if (subject.Disabled)
                {
                    _DicDefSubject.Add(subject.UID, subject);
                }
            }
        }

        /// <summary>
        /// 取得不開課科目資料
        /// </summary>
        /// <param name="schoolYear"></param>
        /// <param name="semester"></param>
        /// <param name="type"></param>
        private void GetDisOpenSubject(string schoolYear, string semester, string type)
        {
            _DicDisOpenSubject.Clear();

            List<UDT.Subject> listSubject = access.Select<UDT.Subject>(string.Format("school_year = {0} AND semester = {1} AND type = '{2}'", schoolYear, semester, type));

            foreach (UDT.Subject subject in listSubject)
            {
                if (subject.Disabled)
                {
                    _DicDisOpenSubject.Add(subject.UID, subject);
                }
            }
        }

        /// <summary>
        /// 取得衝堂志願資料
        /// </summary>
        /// <param name="schoolYear"></param>
        /// <param name="semester"></param>
        /// <param name="type"></param>
        private void GetConflictWish(string schoolYear, string semester, string type)
        {
            _DicStudentConflictWish.Clear();
            #region SQL
            string sql = string.Format(@"
WITH target_subject AS(
	SELECT
		subject.*
	FROM
		$ischool.course_selection.subject AS subject
	WHERE
		subject.school_year = {0}
		AND subject.semester = {1}
		AND subject.type = '{2}'
) ,target_student AS(
	SELECT DISTINCT
		student.*
	FROM
		student 
		LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs 
			ON scs.ref_class_id = student.ref_class_id
		LEFT OUTER JOIN target_subject
			ON scs.ref_subject_id = target_subject.uid
	WHERE
		target_subject.uid IS NOT NULL
) ,target_student_wish AS(
	SELECT
		target_student.id
		, wish.*
	FROM
		target_student
		LEFT OUTER JOIN (
			SELECT
				wish.*
				, target_subject.subject_name || target_subject.level AS key
			FROM
				$ischool.course_selection.ss_wish AS wish 
				LEFT OUTER JOIN target_subject 
					ON target_subject.uid = wish.ref_subject_id
			WHERE
				target_subject.uid IS NOT NULL
		) wish
			ON wish.ref_student_id = target_student.id 
) ,target_student_ss_attend AS(
	SELECT
		ss_attend.*
		, subject.subject_name || subject.level AS key
	FROM
		$ischool.course_selection.ss_attend AS ss_attend
		LEFT OUTER JOIN $ischool.course_selection.subject AS subject
			ON subject.uid = ss_attend.ref_subject_id
	WHERE
		subject.school_year = {0}
		AND subject.semester = {1}
		AND subject.uid IS NOT NULL
		AND ss_attend.ref_student_id IN ( SELECT id FROM target_student)
) ,conflict_data AS(
	SELECT
		target_student.id 
		, CASE 
			WHEN target_student_wish.key = target_student_ss_attend.key THEN '是'
			ELSE '否'
			END AS 課程衝堂
		, target_student_wish.ref_subject_id
		, target_student_wish.sequence
	FROM
		target_student
		LEFT OUTER JOIN target_student_wish
			ON target_student.id = target_student_wish.ref_student_id
		LEFT OUTER JOIN target_student_ss_attend
			ON target_student.id = target_student_ss_attend.ref_student_id
)

SELECT 
	*
FROM 
	conflict_data
WHERE
	conflict_data.課程衝堂 = '是'
            ", schoolYear, semester, type);
            #endregion

            DataTable dt = qh.Select(sql);

            foreach (DataRow row in dt.Rows)
            {
                string studentID = "" + row["id"];
                string subjectID = "" + row["ref_subject_id"];

                if (!_DicStudentConflictWish.ContainsKey(studentID))
                {
                    _DicStudentConflictWish.Add(studentID, new List<string>());
                }

                string key = tool.SubjectNameAndLevel(subjectID); // 科目名稱 + 級別

                _DicStudentConflictWish[studentID].Add(key);
            }
        }

        /// <summary>
        /// 取得課程時段衝堂科目
        /// </summary>
        private void GetConflictSubject(string schoolYear, string semester, string type)
        {
            _DicStudentConflictSubject.Clear();

            #region sql
            string sql = string.Format(@"

WITH basic_info AS( 
	SELECT 
		{0}::INT AS school_year
		, {1}::INT AS semester 
		, '{2}'::text AS type 
), target_subject AS( 
	SELECT 
		subject.* 
	FROM 
		$ischool.course_selection.subject AS subject 
		INNER JOIN basic_info 
			ON basic_info.school_year = subject.school_year 
			AND basic_info.semester = subject.semester 
			AND basic_info.type = subject.type 
), target_student AS(
	SELECT DISTINCT
		student.*
	FROM
		target_subject
		LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs
			ON scs.ref_subject_id = target_subject.uid
		LEFT OUTER JOIN student
			ON student.ref_class_id = scs.ref_class_id
			AND student.status IN (1,2)
), student_attend AS( 
	SELECT 
		ss_attend.ref_student_id
		, subject.uid AS ref_subject_id
		, subject.type 
		, subject.cross_type1 
		, subject.cross_type2 
	FROM 
		basic_info 
		LEFT OUTER JOIN $ischool.course_selection.subject AS subject 
			ON subject.school_year = basic_info.school_year 
			AND subject.semester = basic_info.semester 
		LEFT OUTER JOIN $ischool.course_selection.ss_attend AS ss_attend 
			ON ss_attend.ref_subject_id = subject.uid 
		INNER JOIN target_student
			ON target_student.id = ss_attend.ref_student_id
), conflict_data AS(
	SELECT DISTINCT
		student_attend.ref_student_id
		, target_subject.uid AS ref_subject_id
		, CASE WHEN student_attend.ref_student_id IS NOT NULL
			THEN true
			ELSE false
			END AS conflict_subject
	FROM
		target_subject
		LEFT OUTER JOIN student_attend
			ON (student_attend.type = target_subject.cross_type1 AND student_attend.type IS NOT NULL)
			OR (student_attend.type = target_subject.cross_type2 AND student_attend.type IS NOT NULL)
			OR (student_attend.cross_type1 = target_subject.cross_type1 AND student_attend.cross_type1 IS NOT NULL)
			OR (student_attend.cross_type1 = target_subject.cross_type2 AND student_attend.cross_type1 IS NOT NULL)
			OR (student_attend.cross_type2 = target_subject.cross_type1 AND student_attend.cross_type2 IS NOT NULL)
			OR (student_attend.cross_type2 = target_subject.cross_type2 AND student_attend.cross_type2 IS NOT NULL)
    WHERE
		student_attend.ref_subject_id IS NOT NULL
)
SELECT * FROM conflict_data
                
                ", schoolYear, semester, type);
            #endregion

            DataTable dt = qh.Select(sql);

            foreach (DataRow row in dt.Rows)
            {
                string studentID = "" + row["ref_student_id"];
                string subjectID = "" + row["ref_subject_id"];

                if (!_DicStudentConflictSubject.ContainsKey(studentID))
                {
                    _DicStudentConflictSubject.Add(studentID, new List<string>());
                }
                _DicStudentConflictSubject[studentID].Add(subjectID);
            }
        }

        /// <summary>
        /// 取得擋修名單資料
        /// </summary>
        private void GetBlackListData(string schoolYear, string semester, string type)
        {
            _DicStudentBlackList.Clear();

            #region SQL
            string sql = string.Format(@"
WITH target_subject AS(
	SELECT
		*
		, CASE 
			WHEN type = '{2}' THEN 'false'
			WHEN cross_type1 = '{2}' THEN 'true'
			WHEN cross_type2 = '{2}' THEN 'true'
			ELSE null 
			END as 跨課程時段
	FROM
		$ischool.course_selection.subject
	WHERE
		school_year = {0}
		AND semester = {1}
		AND (type = '{2}' OR cross_type1 = '{2}' OR cross_type2 = '{2}')
)
SELECT DISTINCT
	ref_student_id
	, ref_subject_id
	, string_agg(reason,';') AS reason
FROM
	$ischool.course_selection.subject_block 
WHERE
	ref_subject_id IN ( SELECT uid FROM target_subject )
GROUP BY
	ref_student_id
	, ref_subject_id
                ", schoolYear, semester, type);
            #endregion

            DataTable dt = qh.Select(sql);

            foreach (DataRow row in dt.Rows)
            {
                string studentID = "" + row["ref_student_id"];
                string subjectID = "" + row["ref_subject_id"];
                string reason = "" + row["reason"];
                if (!_DicStudentBlackList.ContainsKey(studentID))
                {
                    _DicStudentBlackList.Add(studentID, new Dictionary<string, string>());
                }
                if (!_DicStudentBlackList[studentID].ContainsKey(subjectID))
                {
                    _DicStudentBlackList[studentID].Add(subjectID, reason);
                }
            }
        }

        /// <summary>
        /// 取得已選科目
        /// </summary>
        private void GetRepeatSubject(string schoolYear, string semester, string type)
        {
            _DicRepeatSubjectByStudentID.Clear();

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
 ),  target_attend AS(
 	SELECT
		ss_attend.*
		, subject.subject_name
		, subject.level
	FROM
		$ischool.course_selection.ss_attend AS ss_attend
		INNER JOIN $ischool.course_selection.subject AS subject
			ON subject.uid = ss_attend.ref_subject_id
			AND subject.school_year = {0}
			AND subject.semester = {1}
            AND subject.type <> '{2}'
 ), repeat_subject AS(
 	SELECT
		target_attend.ref_student_id
		, target_subject.uid AS ref_subject_id
	FROM
		target_subject
		INNER JOIN target_attend
			ON target_attend.subject_name = target_subject.subject_name
			AND target_attend.level = target_subject.level
 )
 SELECT * FROM repeat_subject
            ", schoolYear, semester, type);

            DataTable dt = qh.Select(sql);

            foreach (DataRow row in dt.Rows)
            {
                string studentID = "" + row["ref_student_id"];
                string subjectID = "" + row["ref_subject_id"];

                if (!_DicRepeatSubjectByStudentID.ContainsKey(studentID))
                {
                    _DicRepeatSubjectByStudentID.Add(studentID, new List<string>());
                }
                _DicRepeatSubjectByStudentID[studentID].Add(subjectID);
            }
        }

        /// <summary>
        /// 驗證擋修名單
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        private string CheckBlackList(string studentID, string subjectID)
        {
            if (_DicStudentBlackList.ContainsKey(studentID))
            {
                if (_DicStudentBlackList[studentID].ContainsKey(subjectID))
                {
                    return _DicStudentBlackList[studentID][subjectID];
                }
            }
            return "";
        }

        private void conditionCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                ShowDataRow(true);
            }
            btnSave.Enabled = conditionCbx.SelectedIndex == 0;
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
                    string level = Tool.RomanChar("" + row["level"]);

                    string subjectName = string.Format("{0} {1}", row["subject_name"], level);
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
        AND student.status IN ( 1, 2 )
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
            Color[] colors = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.PowderBlue, Color.Orange, Color.Green, Color.Purple, Color.Brown };

            #region Init Button
            subjectColorDic.Clear();
            int n = 0;
            foreach (DataRow row in dt.Rows)
            {
                ButtonX button = new ButtonX();
                button.FocusCuesEnabled = false;
                button.Style = eDotNetBarStyle.Office2007;
                button.ColorTable = eButtonColor.Flat;
                button.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(15);
                button.TextAlignment = eButtonTextAlignment.Left;
                button.Size = new Size(220, 20);

                string _subjectName = "";
                if (("" + row["subject_name"]).Length > 7)
                {
                    _subjectName = ("" + row["subject_name"]).Substring(0, 7);
                }
                button.Text = string.Format("( {0}/{1} ){2} {3}", "" + row["count"], "" + row["limit"], _subjectName, Tool.RomanChar("" + row["level"]));

                //if (button.Text.Length > 17)
                //{
                //    button.Text = button.Text.Substring(0, 17);
                //}
                if (n >= 7)
                {
                    n = n % 7;
                }
                // 紀錄科目顏色
                subjectColorDic.Add("" + row["uid"], colors[n]);
                button.Image = GetColorBallImage(colors[n++]);
                // Subject UID
                button.Name = "" + row["subject_name"];
                button.Tag = row; //"" + row["uid"]; 

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

                #region SQL
                string sql = string.Format(@"
WITH target_subject AS(
	SELECT
		*
		, CASE 
			WHEN type = '{2}' THEN 'false'
			WHEN cross_type1 = '{2}' THEN 'true'
			WHEN cross_type2 = '{2}' THEN 'true'
			ELSE null 
			END as 跨課程時段
	FROM
		$ischool.course_selection.subject
	WHERE
		school_year = {0}
		AND semester = {1}
		AND (type = '{2}' OR cross_type1 = '{2}' OR cross_type2 = '{2}')
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
) ,target_subject_block AS(
	SELECT DISTINCT
		ref_student_id
		, ref_subject_id
		, string_agg(reason,';') AS reason
	FROM
		$ischool.course_selection.subject_block 
	WHERE
		ref_subject_id IN ( SELECT uid FROM target_subject )
	GROUP BY
		ref_student_id
		, ref_subject_id
) ,target_student AS(
	SELECT 
		student.id
		, student.ref_class_id
		, class.class_name
		, class.display_order
		, class.grade_year
		, student.seat_no
,student.student_number
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
        AND student.status IN ( 1, 2 )
), student_attend AS(
	SELECT
		attend.ref_student_id
		, attend.ref_subject_id
		, ref_subject_course_id
        , lock
        , attend_type
		, subject.subject_name
		, target_subject.跨課程時段
	FROM
		$ischool.course_selection.ss_attend AS attend
		LEFT OUTER JOIN $ischool.course_selection.subject AS subject
            ON subject.uid = attend.ref_subject_id
        LEFT OUTER JOIN target_subject 
        	ON target_subject.uid = subject.uid
        LEFT OUTER JOIN student 
            ON student.id = attend.ref_student_id
	WHERE attend.ref_subject_id IN(
			SELECT
				uid
			FROM
				target_subject
		)
        AND student.status IN ( 1, 2 )
) , wish_row AS(
	SELECT
		wish.ref_student_id
		, wish.ref_subject_id
		, wish.sequence
        , is_cancel
		, $ischool.course_selection.subject.subject_name
	FROM
		$ischool.course_selection.ss_wish AS wish
		LEFT OUTER JOIN $ischool.course_selection.subject 
            ON $ischool.course_selection.subject.uid = wish.ref_subject_id
        LEFT OUTER JOIN student 
            ON student.id = wish.ref_student_id
	WHERE
		wish.ref_subject_id IN(
			SELECT 
				uid
			FROM
				target_subject
            WHERE
                跨課程時段 = 'false'
		)
        AND student.status IN ( 1, 2 )
) , wish AS(
	SELECT
		ref_student_id
		, ROW_NUMBER() OVER(PARTITION BY ref_student_id ORDER BY sequence) AS sequence	
		, ref_subject_id
        , is_cancel
		, subject_name
	FROM
		wish_row
    WHERE
		is_cancel IS NOT true
) 
SELECT
	target_student.*
    , null AS 分發順位
    , null AS 分發志願
    , target_subject_block.reason 
    , student_attend.lock
    , student_attend.attend_type
    , student_attend.ref_subject_id
	, student_attend.subject_name AS 選課課程
	, student_attend.跨課程時段 AS 跨課程時段科目
	, wish1.subject_name AS 志願1
	, wish2.subject_name AS 志願2
	, wish3.subject_name AS 志願3
	, wish4.subject_name AS 志願4
	, wish5.subject_name AS 志願5
	, wish6.subject_name AS 志願6
	, wish7.subject_name AS 志願7
	, wish8.subject_name AS 志願8
	, wish9.subject_name AS 志願9
	, wish10.subject_name AS 志願10
	, wish1.ref_subject_id AS 志願1ref_subject_id
	, wish2.ref_subject_id AS 志願2ref_subject_id
	, wish3.ref_subject_id AS 志願3ref_subject_id
	, wish4.ref_subject_id AS 志願4ref_subject_id
	, wish5.ref_subject_id AS 志願5ref_subject_id
	, wish6.ref_subject_id AS 志願6ref_subject_id
	, wish7.ref_subject_id AS 志願7ref_subject_id
	, wish8.ref_subject_id AS 志願8ref_subject_id
	, wish9.ref_subject_id AS 志願9ref_subject_id
	, wish10.ref_subject_id AS 志願10ref_subject_id
    , wish1.is_cancel AS 志願1_is_cancel
	, wish2.is_cancel AS 志願2_is_cancel
	, wish3.is_cancel AS 志願3_is_cancel
	, wish4.is_cancel AS 志願4_is_cancel
	, wish5.is_cancel AS 志願5_is_cancel
	, wish6.is_cancel AS 志願6_is_cancel
	, wish7.is_cancel AS 志願7_is_cancel
	, wish8.is_cancel AS 志願8_is_cancel
	, wish9.is_cancel AS 志願9_is_cancel
	, wish10.is_cancel AS 志願10_is_cancel
FROM
	target_student
	LEFT OUTER JOIN student_attend
		ON student_attend.ref_student_id = target_student.id
	LEFT OUTER JOIN target_subject_block
		ON target_subject_block.ref_student_id = target_student.id
		AND target_subject_block.ref_subject_id = student_attend.ref_subject_id
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
	LEFT OUTER JOIN wish as wish9
		ON wish9.ref_student_id = target_student.id
			AND wish9.sequence = 9
	LEFT OUTER JOIN wish as wish10
		ON wish10.ref_student_id = target_student.id
			AND wish10.sequence = 10
ORDER BY 
	target_student.grade_year
	, target_student.display_order
	, target_student.class_name
	, target_student.seat_no
	, target_student.id
                ", schoolYear, semester, courseType);
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
                    if (schoolYear == schoolYearCbx.Text && semester == semesterCbx.Text && courseType == courseTypeCbx.Text && dt.Rows != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            _DataRowList.Add(row);
                        }
                        ShowDataRow(true);
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
            ButtonX button = (ButtonX)sender;
            // 取得科目編號
            string subjectID = ("" + button.Tag) == "" ? "" : "" + ((DataRow)button.Tag)["uid"];
            string errMsg = "";

            #region 檢查資料行是否可以指定科目
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                // 如果資料行為鎖定不能做修改
                if ("" + row.Cells["Lock"].Value == "是")
                {
                    swapEnabled = false;
                    errMsg = "學生已鎖定，無法調整選課結果。";
                }
                // 如果資料行為跨課程時段不能做修改
                DataRow tagRow = (DataRow)row.Tag;
                if ("" + tagRow["跨課程時段科目"] == "true")
                {
                    swapEnabled = false;
                    errMsg = "無法調整跨課程時段選課結果，請回到原課程時段再進行調整。";
                }
                // 檢查是否已選修科目
                string studentID = "" + tagRow["id"];
                if (_DicRepeatSubjectByStudentID.ContainsKey(studentID))
                {
                    if (_DicRepeatSubjectByStudentID[studentID].Contains(subjectID))
                    {
                        string subjectName = "";
                        string sType = "";
                        if (_DicDefSubject.ContainsKey(subjectID))
                        {
                            subjectName = _DicDefSubject[subjectID].SubjectName;
                            sType = _DicDefSubject[subjectID].Type;

                        }

                        swapEnabled = false;
                        errMsg = "已在其他課程時段選修此科目\n學生「" + "" + tagRow["name"] + "」\n科目「" + subjectName + "」\n類別「" + sType + "」。";
                    }
                }
                // 檢查科目是否衝堂
                if (_DicStudentConflictSubject.ContainsKey(studentID))
                {
                    if (_DicStudentConflictSubject[studentID].Contains(subjectID))
                    {
                        string subjectName = "";
                        string sType = "";
                        if (_DicDefSubject.ContainsKey(subjectID))
                        {
                            subjectName = _DicDefSubject[subjectID].SubjectName;
                            sType = _DicDefSubject[subjectID].Type;
                        }
                        swapEnabled = false;
                        errMsg = "課程時段衝堂，無法指定學生選修此科目\n學生「" + "" + tagRow["name"] + "」\n科目「" + subjectName + "」\n類別「" + sType + "」。";
                    }
                }

            }
            #endregion

            if (swapEnabled)
            {
                #region 警告人數超過
                // 剩餘名額與調整學生人數比較
                int limit = _DicSubjectData[subjectID].SubjectLimit;
                int 剩餘名額 = _DicSubjectData[subjectID].SubjectLimit - _DicSubjectData[subjectID].StuCount;
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
                    row["ref_subject_id"] = subjectID;

                    row["選課課程"] = tool.SubjectNameAndLevel(subjectID);

                    if (subjectID == "")
                    {
                        row["attend_type"] = "";
                    }
                    else
                    {
                        row["attend_type"] = "指定";
                    }
                    // 驗證擋修名單
                    row["reason"] = CheckBlackList("" + row["id"], subjectID);
                }
                #endregion
                //更新顯示
                ShowDataRow();
            }
            else
            {
                MessageBox.Show(errMsg);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 兜資料
            List<string> dataList = new List<string>();

            int seed = 0;
            string _seed = lblCurrentSeed.Text;
            string[] _seedArray = _seed.Split(':');
            foreach (string s in _seedArray)
            {
                if (int.TryParse(s, out seed))
                {
                    seed = int.Parse(s);
                }
            }
            string sql = "";
            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if ("" + datarow.Cells[attendType.DisplayIndex].Value != "跨課程時段")
                {
                    DataRow row = (DataRow)datarow.Tag;
                    string data = string.Format(@"
SELECT
    {0}::BIGINT AS ref_student_id
    , {1}::BIGINT AS ref_subject_id
    , {2}::TEXT AS subject_name
    , {3}::BOOLEAN AS lock
    , {4}::TEXT AS attend_type
    , {5}::INTEGER AS school_year
    , {6}::INTEGER AS semester
    , '{7}'::TEXT AS type
    , {8}::INTEGER AS seed
            ", row["id"]
                    , "" + row["ref_subject_id"] == "" ? "NULL" : "" + row["ref_subject_id"]
                    , "" + row["選課課程"] == "" ? "NULL" : "'" + ("" + row["選課課程"]).Replace("'", "''") + "'"
                    , "" + row["lock"] == "true" ? "true" : "false"
                    , "" + row["attend_type"] == "" ? "NULL" : "'" + ("" + row["attend_type"]).Replace("'", "''") + "'"
                    , schoolYearCbx.Text
                    , semesterCbx.Text
                    , courseTypeCbx.Text
                    , "" + seed
                    );
                    dataList.Add(data);
                }

                string attendData = string.Join(" UNION ALL", dataList);
                #region SQL
                sql = string.Format(@"
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
            WHEN data_source.status = 'update'::text THEN 
'學生「'|| student.name || '」
課程時段「'|| data_source.type ||'」選修科目「' || data_source.subject_name || '」
變更選課鎖定狀態為「' || data_source.lock || '」'

            WHEN data_source.status = 'insert'::text AND data_source.attend_type = '指定'::text THEN 
'學生「'|| student.name || '」
課程時段「'|| data_source.type ||'」選修科目「指定」為「' || data_source.subject_name || '」'|| (CASE WHEN data_source.lock = true THEN '
鎖定狀態為「' || data_source.lock || '」' ELSE '' END)

            WHEN data_source.status = 'insert'::text AND data_source.attend_type = '志願分發'::text THEN 
'學生「'|| student.name || '」
課程時段「'|| data_source.type ||'」選修科目「志願分發(分發順位代碼' || data_source.seed || ')」為「' || data_source.subject_name || '」'|| (CASE WHEN data_source.lock = true THEN '
鎖定狀態為「' || data_source.lock || '」' ELSE '' END)

            WHEN data_source.status = 'delete_insert'::text AND data_source.attend_type = '指定'::text THEN 
'學生「'|| student.name || '」
課程時段「'|| data_source.type ||'」
自移除原選修科目「' || data_source.orig_subject_name || '」改「指定」為「' || data_source.subject_name || '」'|| (CASE WHEN data_source.lock = true THEN '
鎖定狀態為「' || data_source.lock || '」' ELSE '' END)

            WHEN data_source.status = 'delete_insert'::text AND data_source.attend_type = '志願分發'::text THEN 
'學生「'|| student.name || '」
課程時段「'|| data_source.type ||'」
自移除原選修科目「' || data_source.orig_subject_name || '」改「志願分發(分發順位代碼' || data_source.seed || ')」為「' || data_source.subject_name || '」'|| (CASE WHEN data_source.lock = true THEN '
鎖定狀態為「' || data_source.lock || '」' ELSE '' END)

            WHEN data_source.status = 'delete'::text THEN 
'刪除學生「'|| student.name || '」課程時段「'|| data_source.type ||'」 原選修科目結果「' || data_source.orig_subject_name || '」'
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
        , last_update = now()
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
            }

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
            btnSave.Enabled = false;
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
                btnSave.Enabled = true;
                leaveBtn.Enabled = true;
                MessageBox.Show("儲存成功。");

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
            // DataGridView 畫面重載
            if (renewRow)
            {
                foreach (var row in _DataRowList)
                {
                    if (
                        (conditionCbx.Text == "全部")
                        || (tool.SubjectNameAndLevel("" + row["ref_subject_id"]) == conditionCbx.Text)
                        || (tool.SubjectNameAndLevel("" + row["ref_subject_id"]) == "" && conditionCbx.Text == "空白")
                    )
                    {
                        DataGridViewRow datarow = new DataGridViewRow();
                        datarow.CreateCells(dataGridViewX1);

                        dataGridViewX1.Rows.Add(Execute(datarow, row));
                    }
                }
            }
            // DataGridView 畫面更新
            else
            {
                foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
                {
                    DataRow row = (DataRow)datarow.Tag;

                    Execute(datarow, row);
                }
            }
            dataGridViewX1.ResumeLayout();

            #region 更新按鈕人數統計
            ReCountSubjectStu();

            foreach (ButtonX btn in flowLayoutPanel1.Controls)
            {
                string subjectID = "";
                string level = "";
                if ("" + btn.Tag == "")
                {
                    subjectID = "" + btn.Tag;
                }
                else
                {
                    DataRow row = (DataRow)btn.Tag;
                    subjectID = "" + row["uid"];
                    level = "" + row["level"];
                }

                string subjectName = tool.SubjectNameAndLevel(subjectID);//allSubjectDic[subjectID];
                if (subjectName.Length > 7)
                {
                    subjectName = subjectName.Substring(0, 7);
                }

                //btn.Text = string.Format("({0}/{1})", ("" + _DicSubjectData[subjectID].StuCount).PadLeft(3), ("" + _DicSubjectData[subjectID].SubjectLimit).PadRight(3)) + subjectName;
                btn.Text = string.Format("( {0}/{1} ){2}", "" + _DicSubjectData[subjectID].StuCount, "" + _DicSubjectData[subjectID].SubjectLimit, subjectName);
            }
            #endregion
        }

        /// <summary>
        /// 填入資料
        /// </summary>
        /// <param name="datarow"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private DataGridViewRow Execute(DataGridViewRow datarow, DataRow row)
        {
            int index = 0;

            datarow.Cells[index++].Value = "" + row["class_name"]; // 班級
            datarow.Cells[index++].Value = "" + row["seat_no"]; // 座號
            datarow.Cells[index++].Value = "" + row["student_number"]; // 學號
            datarow.Cells[index].Tag = "" + row["id"]; // 紀錄學生ID
            datarow.Cells[index++].Value = "" + row["name"]; // 姓名
            datarow.Cells[index++].Value = ("" + row["lock"]) == "true" ? "是" : ""; // 鎖定
            if ("" + row["lock"] == "true")
            {
                datarow.DefaultCellStyle.BackColor = Color.GreenYellow;
            }
            else
            {
                datarow.DefaultCellStyle.BackColor = Color.White;
            }
            datarow.Cells[index++].Value = "" + row["分發順位"]; // 分發順位

            #region 選修課程
            if ("" + row["ref_subject_id"] != string.Empty)
            {
                ((DataGridViewColorBallTextCell)datarow.Cells[index]).Value = tool.SubjectNameAndLevel("" + row["ref_subject_id"]);//"" + row["選課課程"];
                if ("" + row["跨課程時段科目"] != "true")
                {
                    ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = subjectColorDic["" + row["ref_subject_id"]];
                }
                else
                {
                    ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = Color.Gray;  // 跨課程時段顏色
                }
            }
            else
            {
                datarow.Cells[index].Value = "";
                ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = Color.Transparent;
            }
            datarow.Cells[index++].Tag = "" + row["ref_subject_id"];
            #endregion

            datarow.Cells[index++].Value = "" + row["分發志願"]; // 分發志願

            #region 修課方式

            if ("" + row["跨課程時段科目"] == "true")
            {
                datarow.DefaultCellStyle.BackColor = Color.LightGray;
                datarow.Cells[index++].Value = "跨課程時段";
            }
            else
            {
                datarow.Cells[index++].Value = "" + row["attend_type"];
            }
            #endregion

            #region 驗證訊息
            {
                List<string> msgList = new List<string>();

                // 擋修
                string blockMsg = CheckBlackList("" + row["id"], "" + row["ref_subject_id"]);
                if (!string.IsNullOrEmpty(blockMsg))
                {
                    msgList.Add(blockMsg);
                }
                // 衝堂
                if (_DicStudentConflictSubject.ContainsKey("" + row["id"]))
                {
                    if (_DicStudentConflictSubject["" + row["id"]].Contains("" + row["ref_subject_id"]))
                    {
                        msgList.Add("課程時段衝堂");
                    }
                }
                //重複加選
                if (_DicRepeatSubjectByStudentID.ContainsKey("" + row["id"]))
                {
                    if (_DicRepeatSubjectByStudentID["" + row["id"]].Contains("" + row["ref_subject_id"]))
                    {
                        msgList.Add("已重複加選此科目");
                    }
                }

                datarow.Cells[index++].Value = string.Join(" ,", msgList);

            }
            #endregion

            #region 志願
            for (int i = 1; i <= 10; i++)
            {
                if ("" + row["志願" + i + "ref_subject_id"] == "" + row["ref_subject_id"] && "" + row["ref_subject_id"] != "")
                {
                    datarow.Cells[index].Style.ForeColor = Color.Red;
                }
                else
                {
                    datarow.Cells[index].Style.ForeColor = dataGridViewX1.DefaultCellStyle.ForeColor;
                }
                datarow.Cells[index++].Value = tool.SubjectNameAndLevel("" + row[string.Format("志願{0}ref_subject_id", i)]);
            }
            #endregion

            datarow.Tag = row;

            return datarow;

            #region MyRegion
            //--
            //int index = 0;
            //datarow.Cells[index++].Value = "" + row["class_name"];
            //datarow.Cells[index++].Value = "" + row["seat_no"];
            //datarow.Cells[index].Tag = "" + row["id"]; // 紀錄學生ID
            //datarow.Cells[index++].Value = "" + row["name"];
            //datarow.Cells[index++].Value = ("" + row["lock"]) == "true" ? "是" : "";
            //if ("" + row["lock"] == "true")
            //{
            //    datarow.DefaultCellStyle.BackColor = Color.GreenYellow;
            //}
            //else if ("" + row["是否為跨課程類別科目"] == "true")
            //{
            //    datarow.DefaultCellStyle.BackColor = Color.LightGray;
            //}
            //else
            //{
            //    datarow.DefaultCellStyle.BackColor = dataGridViewX1.DefaultCellStyle.BackColor;
            //}
            //datarow.Cells[index++].Value = "" + row["分發順位"];

            //#region 選修課程欄位

            //// 所屬課程類別 
            //if ("" + row["ref_subject_id"] != "")
            //{
            //    if ("" + row["跨課程類別科目"] != "true")
            //    {
            //        ((DataGridViewColorBallTextCell)datarow.Cells[index]).Value = "" + row["選課課程"];
            //        ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = subjectColorDic["" + row["ref_subject_id"]];
            //    }
            //    // 跨課程類別
            //    else
            //    {
            //        ((DataGridViewColorBallTextCell)datarow.Cells[index]).Value = "" + row["選課課程"];
            //        ((DataGridViewColorBallTextCell)datarow.Cells[index]).Color = Color.Gray;  // 跨課程類別顏色
            //    }
            //}

            //datarow.Cells[index++].Tag = "" + row["ref_subject_id"];
            //#endregion

            //for (int i = 1; i <= 5; i++)
            //{
            //    if ("" + row["志願" + i + "ref_subject_id"] == "" + row["ref_subject_id"] && "" + row["ref_subject_id"] != "")
            //    {
            //        datarow.Cells[index].Style.ForeColor = Color.Red;
            //    }
            //    else
            //    {
            //        datarow.Cells[index].Style.ForeColor = dataGridViewX1.DefaultCellStyle.ForeColor;
            //    }
            //    datarow.Cells[index++].Value = "" + row["志願" + i];
            //}
            //datarow.Cells[index++].Value = "" + row["分發志願"];
            //datarow.Cells[index++].Value = "" + row["跨課程類別科目"] == "true" ? "跨課程類別" : "" + row["attend_type"];
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
                // 新增判斷條件避免跨課程時段科目不在dic中出現錯誤
                if (_DicSubjectData.ContainsKey("" + row["ref_subject_id"]))
                {
                    _DicSubjectData["" + row["ref_subject_id"]].StuCount++;
                }

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
                    MotherForm.SetStatusBarMessage("匯出選課結果及分發，列印完成。");
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("檔案未儲存");
                    return;
                }
            }
            catch
            {
                FISCA.Presentation.Controls.MsgBox.Show("檔案儲存錯誤，請檢查檔案是否開啟中。");
                MotherForm.SetStatusBarMessage("檔案儲存錯誤，請檢查檔案是否開啟中。");
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
                //dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
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
            //buttonItem1.Enabled = false;
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
            //buttonItem1.Enabled = true;
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
                if ("" + row["lock"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏" && "" + row["跨課程時段科目"] != "true")
                {
                    row["ref_subject_id"] = "";
                    row["選課課程"] = "";
                    row["分發志願"] = "";
                    row["attend_type"] = "";
                    row["reason"] = "";
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
                    if ("" + row["lock"] != "true" && "" + row["跨課程時段科目"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
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
                    if ("" + row["lock"] != "true" && "" + row["跨課程時段科目"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
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
            for (int i = 1; i <= 10; i++)
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
        /// 志願六分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem10_Click(object sender, EventArgs e)
        {
            distribute(6, true);

            if (sender == buttonItem10)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願七分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem11_Click(object sender, EventArgs e)
        {
            distribute(7, true);

            if (sender == buttonItem11)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願八分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem12_Click(object sender, EventArgs e)
        {
            distribute(8, true);

            if (sender == buttonItem12)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願九分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem13_Click(object sender, EventArgs e)
        {
            distribute(9, true);

            if (sender == buttonItem13)
            {
                ShowDataRow();
                dataGridViewX1.Sort(Column10, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// 志願十分發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem14_Click(object sender, EventArgs e)
        {
            distribute(10, true);

            if (sender == buttonItem14)
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
                if ("" + row["分發順位"] == "" && "" + row["lock"] != "true" && "" + row["跨課程時段科目"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
                {
                    if (showErrorMsg)
                        MessageBox.Show("請先產生分發順位!");
                    break;
                }
                if ("" + row["lock"] != "true" && "" + row["跨課程時段科目"] != "true" && "" + row["attend_type"] != "指定" && "" + row["attend_type"] != "先搶先贏")
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
                    row["選課課程"] = tool.SubjectNameAndLevel(wishSubjectID); // 透過志願(科目)ID取得 科目名稱+級別 
                    row["分發志願"] = wishOrder;
                    row["attend_type"] = "志願分發";

                    row["reason"] = CheckBlackList(wishSubjectID, "" + row["id"]); // 透過科目編號、學生邊號取得擋修名單資料

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

            courseTypeCbx_SelectedIndexChanged(null, null);
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
                    for (int i = 1; i <= 10; i++)
                    {
                        if ("" + row["志願" + i + "ref_subject_id"] != "")
                            empty = false;
                    }
                    if (empty)
                    {
                        int seed = rand.Next(100);
                        //Trace.WriteLine(seed);
                        int wc = 0;
                        if (seed <= 10)
                            wc = 10;
                        else if (seed <= 20)
                            wc = 9;
                        else if (seed <= 30)
                            wc = 8;
                        else if (seed <= 40)
                            wc = 7;
                        else if (seed <= 50)
                            wc = 6;
                        else if (seed <= 60)
                            wc = 5;
                        else if (seed <= 70)
                            wc = 4;
                        else if (seed <= 80)
                            wc = 3;
                        else if (seed <= 90)
                            wc = 2;
                        else if (seed <= 100)
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
                courseTypeCbx_SelectedIndexChanged(null, null);
            };

            bkw.RunWorkerAsync();
        }

        /// <summary>
        /// 清除不開課志願
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearDisOpenVol_Click(object sender, EventArgs e)
        {
            // 更新資料
            foreach (DataRow row in _DataRowList)
            {
                // wishOrder subjectID
                Dictionary<int, string> dicWishSubject = new Dictionary<int, string>();

                // 整理志願
                for (int i = 1; i <= 5; i++)
                {
                    dicWishSubject.Add(i, "" + row["志願" + i + "ref_subject_id"]);

                    // 清空志願
                    row["志願" + i + "ref_subject_id"] = "";
                    row["志願" + i] = "";
                }
                // 調整志願
                int wishOrder = 1;
                for (int i = 1; i <= 5; i++)
                {
                    string wishSubjectID = dicWishSubject[i];

                    if (!_DicDisOpenSubject.ContainsKey(wishSubjectID))
                    {
                        row["志願" + wishOrder + "ref_subject_id"] = wishSubjectID;
                        row["志願" + wishOrder] = tool.SubjectNameAndLevel(wishSubjectID);
                        wishOrder++;
                    }
                }
            }
            if (sender == btnClearDisOpenVol)
            {
                ShowDataRow();
            }
        }

        /// <summary>
        /// 清除衝堂志願
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearConflictVol_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in _DataRowList)
            {
                string studentID = "" + row["id"];
                if (_DicStudentConflictWish.ContainsKey(studentID))
                {
                    // wishOrder subjectID
                    Dictionary<int, string> dicWishSubject = new Dictionary<int, string>();

                    // 整理志願
                    for (int i = 1; i <= 5; i++)
                    {
                        dicWishSubject.Add(i, "" + row["志願" + i + "ref_subject_id"]);

                        // 清空志願
                        row["志願" + i + "ref_subject_id"] = "";
                        row["志願" + i] = "";
                    }
                    // 調整志願
                    int wishOrder = 1;
                    for (int i = 1; i <= 5; i++)
                    {
                        string wishSubjectID = dicWishSubject[i];
                        string key = tool.SubjectNameAndLevel(wishSubjectID); // 科目名稱 + 級別
                        // KEY 科目名稱 + 級別
                        if (!_DicStudentConflictWish[studentID].Contains(key))
                        {
                            row["志願" + wishOrder + "ref_subject_id"] = wishSubjectID;
                            row["志願" + wishOrder] = tool.SubjectNameAndLevel(wishSubjectID);
                            wishOrder++;
                        }
                    }
                }
            }
            if (sender == btnClearConflictVol)
            {
                ShowDataRow();
            }
        }

        /// <summary>
        /// 清除擋修名單志願
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearBlackList_Click(object sender, EventArgs e)
        {
            // 清除目標學生的擋修名單資料(_DicStudentBlackList)
            foreach (DataRow row in _DataRowList)
            {
                string studentID = "" + row["id"];
                if (_DicStudentBlackList.ContainsKey(studentID))
                {
                    // wishOrder subjectID
                    Dictionary<int, string> dicWishSubject = new Dictionary<int, string>();

                    // 整理志願
                    for (int i = 1; i <= 5; i++)
                    {
                        dicWishSubject.Add(i, "" + row["志願" + i + "ref_subject_id"]);

                        // 清空志願
                        row["志願" + i + "ref_subject_id"] = "";
                        row["志願" + i] = "";
                    }
                    // 調整志願
                    int wishOrder = 1;
                    for (int i = 1; i <= 5; i++)
                    {
                        string wishSubjectID = dicWishSubject[i];
                        // KEY 科目名稱 + 級別
                        if (!_DicStudentBlackList[studentID].ContainsKey(wishSubjectID))
                        {
                            row["志願" + wishOrder + "ref_subject_id"] = wishSubjectID;
                            row["志願" + wishOrder] = tool.SubjectNameAndLevel(wishSubjectID);
                            wishOrder++;
                        }
                    }
                }
            }
            if (sender == btnClearBlackList)
            {
                ShowDataRow();
            }
        }

        /// <summary>
        /// 開啟測試工具用
        /// </summary>
        private void lbSecret_Click(object sender, EventArgs e)
        {
            btnTrial.Visible = !btnTrial.Visible;
        }

        private void btnDistributeUnSelect_Click(object sender, EventArgs e)
        {
            dataGridViewX1.SuspendLayout();
            try
            {
                // 取得未選課學生可選課程清單
                #region SQL
                string sql = string.Format(@"
    WITH data_row AS( -- 條件
        SELECT
            school_year
            , semester
            , '{2}'::TEXT as type
        FROM
            $ischool.course_selection.opening_time
    ), target_subject AS(
        SELECT
            *
        FROM
            $ischool.course_selection.subject
        WHERE
            school_year = {0}
            AND semester = {1}
            AND type = '{2}'
    ), type_selectable_student AS( -- 課程時段學生
        SELECT DISTINCT
            student.id
            , student.ref_class_id
        FROM
            target_subject
            LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs
                ON scs.ref_subject_id = target_subject.uid
            LEFT OUTER JOIN student
                ON student.ref_class_id = scs.ref_class_id
    ), target_attend AS( -- 課程時段學生的修課紀錄,含跨課程時段。
        SELECT
            ss_attend.*
        FROM
            type_selectable_student
            LEFT OUTER JOIN $ischool.course_selection.ss_attend AS ss_attend
                ON ss_attend.ref_student_id = type_selectable_student.id
            LEFT OUTER JOIN $ischool.course_selection.subject AS subject
                ON subject.uid = ss_attend.ref_subject_id
            INNER JOIN data_row
                ON data_row.school_year = subject.school_year
                AND data_row.semester = subject.semester
                AND ( 
                    (data_row.type = subject.type AND subject.type <> '')
                    OR (data_row.type = subject.cross_type1 AND subject.cross_type1 <> '')
                    OR (data_row.type = subject.cross_type2 AND subject.cross_type2 <> '')
                )
    ), target_student AS( -- 目標學生,此時段未修課選課學生
        SELECT
            *
            , '{2}'::TEXT AS type
        FROM
            type_selectable_student
        WHERE
            id NOT IN(
                SELECT ref_student_id FROM target_attend
            )
    ), target_student_block_subject AS( -- 取得目標學生擋修課程
        SELECT
            target_student.id AS ref_student_id
            , block.ref_subject_id
        FROM
            target_student
            LEFT OUTER JOIN $ischool.course_selection.subject_block AS block 
                ON block.ref_student_id = target_student.id
            LEFT OUTER JOIN $ischool.course_selection.subject AS subject
                ON subject.uid = block.ref_subject_id
        WHERE
            subject.uid IN (
                SELECT uid FROM target_subject
            )
    ), target_student_attend AS( -- 取得目前學年度學期目標學生已選課程
        SELECT
            ss_attend.ref_student_id
            , ss_attend.ref_subject_id
            , subject.subject_name
            , subject.level
            , subject.type
            , subject.cross_type1
            , subject.cross_type2
        FROM
            target_student
            LEFT OUTER JOIN $ischool.course_selection.ss_attend AS ss_attend
                ON ss_attend.ref_student_id = target_student.id
            LEFT OUTER JOIN $ischool.course_selection.subject AS subject
                ON subject.uid = ss_attend.ref_subject_id
        WHERE
            subject.school_year = {0}
            AND subject.semester = {1}
    ), target_student_repeat_subject AS( -- 取得目標學生重複選課課程
        SELECT
            target_student_attend.ref_student_id
            , target_student_attend.ref_subject_id
        FROM
            target_subject
            INNER JOIN target_student_attend
                ON target_student_attend.subject_name = target_subject.subject_name
                AND target_student_attend.level = target_subject.level
    ), target_student_conflict_subject AS( -- 取得目標學生衝堂課程
        SELECT
            target_student_attend.ref_student_id
            , target_subject.uid AS ref_subject_id
        FROM
            target_subject
            INNER JOIN target_student_attend
                ON (target_student_attend.type = target_subject.type AND target_subject.type <> '')
                OR (target_student_attend.type = target_subject.cross_type1 AND target_subject.cross_type1 <> '')
                OR (target_student_attend.type = target_subject.cross_type2 AND target_subject.cross_type2 <> '')
                OR (target_student_attend.cross_type1 = target_subject.type AND target_subject.type <> '')
                OR (target_student_attend.cross_type1 = target_subject.cross_type1 AND target_subject.cross_type1 <> '')
                OR (target_student_attend.cross_type1 = target_subject.cross_type2 AND target_subject.cross_type2 <> '')
                OR (target_student_attend.cross_type2 = target_subject.type AND target_subject.type <> '')
                OR (target_student_attend.cross_type2 = target_subject.cross_type1 AND target_subject.cross_type1 <> '')
                OR (target_student_attend.cross_type2 = target_subject.cross_type2 AND target_subject.cross_type2 <> '')
    ), target_student_subject AS( -- 目標學生可分發課程
        SELECT
            target_student.id AS ref_student_id
            , target_subject.uid AS ref_subject_id
            , CASE WHEN target_subject.uid = block.ref_subject_id
                THEN true
                ELSE false
                END AS is_block
            , CASE WHEN target_subject.uid = repeat.ref_subject_id
                THEN true
                ELSE false
                END AS is_repeat
            , CASE WHEN target_subject.uid = conflict.ref_subject_id
                THEN true
                ELSE false
                END AS is_conflict
        FROM
            target_student
            LEFT OUTER JOIN target_subject
                ON target_subject.type = target_student.type
            INNER JOIN $ischool.course_selection.subject_class_selection AS scs
                ON scs.ref_class_id = target_student.ref_class_id
                AND scs.ref_subject_id = target_subject.uid
            LEFT OUTER JOIN target_student_block_subject AS block
                ON block.ref_student_id = target_student.id
                AND block.ref_subject_id = target_subject.uid
            LEFT OUTER JOIN target_student_repeat_subject AS repeat
                ON repeat.ref_student_id = target_student.id
                AND repeat.ref_subject_id = target_subject.uid
            LEFT OUTER JOIN target_student_conflict_subject AS conflict
                ON conflict.ref_student_id = target_student.id
                AND conflict.ref_subject_id = target_subject.uid
    )
    SELECT
        *
    FROM
        target_student_subject
    WHERE
        ref_student_id IS NOT NULL
        AND is_block = false
        AND is_repeat = false
        AND is_conflict = false
                ", schoolYearCbx.Text, semesterCbx.Text, courseTypeCbx.Text);
                #endregion
                DataTable dt = qh.Select(sql);

                // 資料整理
                Dictionary<string, List<string>> dicStuSelectableSubject = new Dictionary<string, List<string>>();
                foreach (DataRow row in dt.Rows)
                {
                    string studentID = "" + row["ref_student_id"];
                    string subjectID = "" + row["ref_subject_id"];
                    if (!dicStuSelectableSubject.ContainsKey(studentID))
                    {
                        dicStuSelectableSubject.Add(studentID, new List<string>());
                    }
                    dicStuSelectableSubject[studentID].Add(subjectID);
                }
                // 3. 分發
                foreach (DataRow row in _DataRowList)
                {
                    string studentID = "" + row["id"];
                    string selectedSubjectID = "" + row["ref_subject_id"];

                    if (string.IsNullOrEmpty(selectedSubjectID) && dicStuSelectableSubject.ContainsKey(studentID))
                    {
                        for (int i = 0; i < dicStuSelectableSubject[studentID].Count; i++)
                        {
                            if (string.IsNullOrEmpty("" + row["ref_subject_id"]))
                            {
                                string subjectID = dicStuSelectableSubject[studentID][i];
                                if (_DicSubjectData[subjectID].StuCount < _DicSubjectData[subjectID].SubjectLimit)
                                {
                                    row["ref_subject_id"] = subjectID;
                                    row["選課課程"] = tool.SubjectNameAndLevel(subjectID);
                                    row["attend_type"] = "指定";
                                    _DicSubjectData[subjectID].StuCount++;
                                }
                            }
                        }
                    }
                }

                ShowDataRow();
            }
            catch (Exception err)
            {
                MsgBox.Show(err.Message);
            }
            dataGridViewX1.ResumeLayout();
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
