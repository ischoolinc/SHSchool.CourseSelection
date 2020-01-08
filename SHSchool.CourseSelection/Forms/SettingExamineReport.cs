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
using K12.Data;
using Aspose.Cells;
using System.IO;

namespace SHSchool.CourseSelection.Forms
{
    public partial class SettingExamineReport : BaseForm
    {
        /// <summary>
        /// 畫面資料是否初始化完成
        /// </summary>
        private bool initFinish = false;

        /// <summary>
        /// SheetOneData
        /// </summary>
        private Dictionary<string, ClassData> dicSheetOneData = new Dictionary<string, ClassData>();

        /// <summary>
        /// SheetOneTitle : 紀錄欄位索引
        /// </summary>
        private Dictionary<string, int> dicSheetOneTitle;

        /// <summary>
        /// SheetTwoTitle
        /// </summary>
        private List<string> listSheetTwoTitle;

        /// <summary>
        /// SheetThreeTitle
        /// </summary>
        private List<string> listSheetThreeTitle;

        public SettingExamineReport()
        {
            InitializeComponent();
        }

        private void SettingExamineReport_Load(object sender, EventArgs e)
        {
            #region Init SchoolYear Semester
            AccessHelper access = new AccessHelper();
            List<UDT.OpeningTime> listOpenTime = access.Select<UDT.OpeningTime>();
            if (listOpenTime.Count == 0)
            {
                listOpenTime.Add(new UDT.OpeningTime() { SchoolYear = int.Parse(K12.Data.School.DefaultSchoolYear), Semester = int.Parse(K12.Data.School.DefaultSemester) });
                listOpenTime.SaveAll();
            }

            cbxSchoolYear.Items.Add(listOpenTime[0].SchoolYear + 1);
            cbxSchoolYear.Items.Add(listOpenTime[0].SchoolYear);
            cbxSchoolYear.Items.Add(listOpenTime[0].SchoolYear - 1);
            cbxSchoolYear.SelectedIndex = 1;

            cbxSemester.Items.Add(1);
            cbxSemester.Items.Add(2);
            cbxSemester.SelectedIndex = listOpenTime[0].Semester - 1;
            #endregion

            initFinish = true;
            ReloadCbxType();
        }

        private void ReloadCbxType()
        {
            cbxType.Items.Clear();
            string sql = string.Format(@"
SELECT DISTINCT
    type
FROM
    $ischool.course_selection.subject
WHERE
    school_year = {0}
    AND semester = {1}
            ", cbxSchoolYear.Text, cbxSemester.Text);

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql);

            foreach (DataRow row in dt.Rows)
            {
                cbxType.Items.Add("" + row["type"]);
            }

            if (cbxType.Items.Count > 0)
            {
                cbxType.SelectedIndex = 0;
                btnPrint.Enabled = true;
            }
            else
            {
                btnPrint.Enabled = false;
            }
        }

        private void cbxSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (initFinish)
            {
                ReloadCbxType();
            }
        }

        private void cbxSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (initFinish)
            {
                ReloadCbxType();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            dicSheetOneTitle = new Dictionary<string, int>();
            listSheetTwoTitle = new List<string>();
            listSheetThreeTitle = new List<string>();
            QueryHelper qh = new QueryHelper();

            // 總計資料
            Total total = new Total();
            total.DicSubject = new Dictionary<string, SubjectData>();

            #region 設定WorkSheet標題列
            {
                #region SheetOneTitle
                string sql = string.Format(@"
SELECT 
    *
FROM
    $ischool.course_selection.subject
WHERE
	school_year = {0} 
	AND semester = {1}
	AND type = '{2}'
ORDER BY
    type
    , subject_name
    , level
    , credit
            ", cbxSchoolYear.Text, cbxSemester.Text, cbxType.Text);

                DataTable dt = qh.Select(sql);
                int colIndex = 4;
                foreach (DataRow row in dt.Rows)
                {
                    string subjectName = string.Format("{0} {1}", row["subject_name"], Tool.RomanChar("" + row["level"]));
                    dicSheetOneTitle.Add(subjectName, colIndex++);
                }
                #endregion

                #region SheetTwoTitle

                listSheetTwoTitle.Add("班級");
                listSheetTwoTitle.Add("座號");
                listSheetTwoTitle.Add("學號");
                listSheetTwoTitle.Add("姓名");
                listSheetTwoTitle.Add("班級開放科目數");
                listSheetTwoTitle.Add("阻擋科目數");
                listSheetTwoTitle.Add("可選科目數");

                #endregion

                #region SheetThreeTitle

                listSheetThreeTitle.Add("班級");
                listSheetThreeTitle.Add("座號");
                listSheetThreeTitle.Add("學號");
                listSheetThreeTitle.Add("姓名");
                listSheetThreeTitle.Add("擋修名單科目");
                listSheetThreeTitle.Add("擋修名單科目級別");
                listSheetThreeTitle.Add("擋修名單原因");

                #endregion
            }
            #endregion

            Workbook template = new Workbook(new MemoryStream(Properties.Resources.選課設定檢查報表樣板));
            Workbook wb = new Workbook(new MemoryStream(Properties.Resources.選課設定檢查報表樣板));

            #region SheetOne
            {
                DataTable sheetOneDt;

                #region 取得班級可選科目資料
                {
                    string sql = string.Format(@"
WITH class_student_count AS(
	SELECT
        class.id
		, count(student.*)
	FROM
		class
        LEFT OUTER JOIN student
            ON student.ref_class_id = class.id
    WHERE
        class.id IN(
            SELECT DISTINCT
                scs.ref_class_id
            FROM
                $ischool.course_selection.subject AS subject
                LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs
                    ON subject.uid = scs.ref_subject_id
            WHERE
                subject.school_year = {0}
                AND subject.semester = {1}
                AND subject.type = '{2}'
        )
        AND student.status IN( 1 , 2)
    GROUP BY
        class.id
) 
SELECT 
	class.class_name
    , class.grade_year
	, subject.subject_name
	, subject.level
	, subject.limit
	, class_student_count.count AS 班級人數
FROM  
	$ischool.course_selection.subject AS subject
	LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs
		ON scs.ref_subject_id = subject.uid
	LEFT OUTER JOIN class
		ON class.id = scs.ref_class_id
	LEFT OUTER JOIN class_student_count
		ON class_student_count.id = class.id
WHERE
	subject.school_year = {0} 
	AND subject.semester = {1}
	AND subject.type = '{2}'
	AND class.id IS NOT NULL
ORDER BY
	class.grade_year
	, class.display_order
	, class.class_name
            ", cbxSchoolYear.Text, cbxSemester.Text, cbxType.Text);
                    sheetOneDt = qh.Select(sql);
                }
                #endregion

                #region 資料整理
                dicSheetOneData.Clear();

                foreach (DataRow row in sheetOneDt.Rows)
                {
                    string subjectName = string.Format("{0} {1}", "" + row["subject_name"], Tool.RomanChar("" + row["level"]));
                    string className = "" + row["class_name"];
                    int subjectLimit = ("" + row["limit"]) == "" ? 0 : int.Parse("" + row["limit"]);
                    int classStudentCount = ("" + row["班級人數"]) == "" ? 0 : int.Parse("" + row["班級人數"]);

                    #region 班級資料整理
                    if (!dicSheetOneData.ContainsKey(className))
                    {
                        ClassData data = new ClassData();
                        data.ClassName = className;
                        data.ClassStudentCount = classStudentCount;
                        data.dicSubject = new Dictionary<string, SubjectData>();
                        data.ClassSubjectQuota += subjectLimit;
                        data.GradeYear = "" + row["grade_year"];

                        SubjectData subjectData = new SubjectData();
                        subjectData.SubjectName = subjectName;
                        subjectData.Level = "" + row["level"];
                        subjectData.Limit = subjectLimit;
                        data.dicSubject.Add(subjectName, subjectData);

                        dicSheetOneData.Add(className, data);

                        // 計算班級人數加總
                        total.TotalClassStudent += data.ClassStudentCount;
                    }
                    else
                    {
                        if (!dicSheetOneData[className].dicSubject.ContainsKey(subjectName))
                        {
                            SubjectData subjectData = new SubjectData();
                            subjectData.SubjectName = subjectName;
                            subjectData.Level = "" + row["level"];
                            subjectData.Limit = subjectLimit;

                            dicSheetOneData[className].dicSubject.Add(subjectName, subjectData);
                            dicSheetOneData[className].ClassSubjectQuota += subjectLimit;
                        }
                    }
                    #endregion

                    #region 總計資料整理

                    // 計算所有科目名額
                    if (!total.DicSubject.ContainsKey(subjectName))
                    {
                        SubjectData subjectData = new SubjectData();
                        subjectData.SubjectName = subjectName;
                        subjectData.Level = "" + row["level"];
                        subjectData.Limit = subjectLimit;

                        // 紀錄科目可選學生數
                        subjectData.TotalStudent += classStudentCount;

                        total.DicSubject.Add(subjectName, subjectData);
                        // 所有科目名額總計
                        total.TotalSubjectQuota += subjectData.Limit;

                    }
                    else if (total.DicSubject.ContainsKey(subjectName))
                    {
                        // 紀錄科目可選學生數
                        total.DicSubject[subjectName].TotalStudent += classStudentCount;
                    }

                    #endregion

                }
                #endregion

                // 填WorkSheet1科目表頭
                int col = 0;
                foreach (string subjectName in dicSheetOneTitle.Keys)
                {
                    if (col == 0)
                    {
                        wb.Worksheets[0].Cells.CopyColumn(template.Worksheets[0].Cells, 4, 4);
                    }
                    else if (col + 1 < dicSheetOneTitle.Count)
                    {
                        wb.Worksheets[0].Cells.CopyColumn(template.Worksheets[0].Cells, 5, 4 + col);
                    }
                    else
                    {
                        wb.Worksheets[0].Cells.CopyColumn(template.Worksheets[0].Cells, 6, 4 + col);
                    }
                    col++;
                }
                template.Worksheets[0].Cells.CopyRows(wb.Worksheets[0].Cells, 0, 0, 6);
                wb.Worksheets[0].Cells.DeleteRows(4, 2);
                foreach (string subjectName in dicSheetOneTitle.Keys)
                {
                    wb.Worksheets[0].Cells[0, dicSheetOneTitle[subjectName]].PutValue(subjectName);
                }

                #region 填欄位資料
                int rowIndex = 1;
                foreach (string className in dicSheetOneData.Keys)
                {
                    // 複製樣板格式
                    wb.Worksheets[0].Cells.CopyRow(template.Worksheets[0].Cells, 2, rowIndex);

                    int colIndex = 0;

                    ClassData classData = dicSheetOneData[className];
                    // 年級
                    wb.Worksheets[0].Cells[rowIndex, colIndex++].PutValue(dicSheetOneData[className].GradeYear);
                    // 班級
                    wb.Worksheets[0].Cells[rowIndex, colIndex++].PutValue(className);
                    // 班級人數
                    wb.Worksheets[0].Cells[rowIndex, colIndex++].PutValue(classData.ClassStudentCount);
                    // 選課名額
                    wb.Worksheets[0].Cells[rowIndex, colIndex++].PutValue(classData.ClassSubjectQuota);

                    // 填入科目資料
                    foreach (string subjectName in dicSheetOneData[className].dicSubject.Keys)
                    {
                        SubjectData subject = dicSheetOneData[className].dicSubject[subjectName];
                        //colIndex = dicSheetOneTitle.IndexOf(subject.SubjectName);
                        colIndex = dicSheetOneTitle[subjectName];
                        wb.Worksheets[0].Cells[rowIndex, colIndex].PutValue(subject.Limit);
                    }

                    rowIndex++;
                }
                #endregion

                #region 填WorkSheet 總計欄位
                wb.Worksheets[0].Cells.CopyRows(template.Worksheets[0].Cells, 4, rowIndex, 2);

                foreach (string subjectName in total.DicSubject.Keys)
                {
                    col = dicSheetOneTitle[subjectName];
                    //wb.Worksheets[0].Cells[rowIndex + 1, col].PutValue("(可選學生數/人數上限)");
                    wb.Worksheets[0].Cells[rowIndex + 1, col].PutValue(string.Format("{0} / {1}", total.DicSubject[subjectName].TotalStudent, total.DicSubject[subjectName].Limit));
                }

                wb.Worksheets[0].Cells.Merge(rowIndex, 4, 1, dicSheetOneTitle.Count);
                // 填入所有科目名額總計
                //col = dicSheetOneTitle.IndexOf("可選科目名額總計");
                //wb.Worksheets[0].Cells[rowIndex + 1, col].PutValue("所有科目名額總計");
                wb.Worksheets[0].Cells[rowIndex + 1, 3].PutValue(total.TotalSubjectQuota);

                // 填入班級人數加總
                //col = dicSheetOneTitle.IndexOf("班級人數");
                //wb.Worksheets[0].Cells[rowIndex + 1, col].PutValue("班級人數加總");
                wb.Worksheets[0].Cells[rowIndex + 1, 2].PutValue(total.TotalClassStudent);
                #endregion
            }
            #endregion

            #region SheetTwo

            DataTable dt2;

            #region SQL
            {
                string sql = string.Format(@"
WITH target_subject AS(
	SELECT
		subject.*
	FROM
		$ischool.course_selection.subject AS subject
		--LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs 
			--ON scs.ref_subject_id = subject.uid
	WHERE
		subject.school_year = {0}
		AND subject.semester = {1}
		AND subject.type = '{2}'
) ,target_class AS(
	SELECT DISTINCT
		class.*
	FROM 
		class
		LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs 
			ON scs.ref_class_id = class.id
		LEFT OUTER JOIN $ischool.course_selection.subject AS subject 
			ON subject.uid = scs.ref_subject_id
	WHERE
		subject.uid IN (SELECT uid FROM target_subject)
		AND subject.uid IS NOT NULL
), tareget_student AS (
	SELECT 
		student.*
	FROM
		student
		INNER JOIN target_class
			ON student.ref_class_id	 = target_class.id
	WHERE
		student.status IN (1 , 2)
), target_class_subject_count AS(
	SELECT
		target_class.id AS ref_class_id
		, count(target_subject.*)
	FROM
		target_class
		LEFT OUTER JOIN $ischool.course_selection.subject_class_selection AS scs
			ON scs.ref_class_id = target_class.id
		LEFT OUTER JOIN target_subject
			ON target_subject.uid = scs.ref_subject_id
	GROUP BY
		target_class.id
), student_block_count AS (
	SELECT
		tareget_student.id
		, count(*)
	FROM
		tareget_student
		LEFT OUTER JOIN $ischool.course_selection.subject_block AS block
			ON tareget_student.id = block.ref_student_id
	WHERE
	 	block.ref_subject_id in (SELECT uid FROM target_subject)
 	GROUP BY
 		tareget_student.id
)
SELECT
	target_class.class_name
	, tareget_student.seat_no
	, tareget_student.student_number
	, tareget_student.name
	, target_class_subject_count.count AS 班級開放科目數
	, student_block_count.count AS 阻擋科目數
FROM
	target_class
	LEFT OUTER JOIN tareget_student
		ON tareget_student.ref_class_id = target_class.id
	LEFT OUTER JOIN target_class_subject_count
		ON target_class_subject_count.ref_class_id = target_class.id
	LEFT OUTER JOIN student_block_count
		ON student_block_count.id = tareget_student.id
ORDER BY
	target_class.grade_year
	, target_class.display_order
	, target_class.class_name
    , tareget_student.seat_no
            ", cbxSchoolYear.Text, cbxSemester.Text, cbxType.Text);

                dt2 = qh.Select(sql);
            }
            #endregion

            int rowIndex2 = 0;

            // 填入資料
            rowIndex2++;
            foreach (DataRow row in dt2.Rows)
            {
                wb.Worksheets["班級學生資料"].Cells.CopyRows(template.Worksheets["班級學生資料"].Cells, 1, rowIndex2, 1);
                int a = ("" + row["班級開放科目數"]) == "" ? 0 : int.Parse("" + row["班級開放科目數"]);
                int b = ("" + row["阻擋科目數"]) == "" ? 0 : int.Parse("" + row["阻擋科目數"]);

                for (int i = 0; i < listSheetTwoTitle.Count; i++)
                {
                    switch (listSheetTwoTitle[i])
                    {
                        case "班級":
                            wb.Worksheets["班級學生資料"].Cells[rowIndex2, 0].PutValue("" + row["class_name"]);
                            break;
                        case "座號":
                            wb.Worksheets["班級學生資料"].Cells[rowIndex2, 1].PutValue("" + row["seat_no"]);
                            break;
                        case "學號":
                            wb.Worksheets["班級學生資料"].Cells[rowIndex2, 2].PutValue("" + row["student_number"]);
                            break;
                        case "姓名":
                            wb.Worksheets["班級學生資料"].Cells[rowIndex2, 3].PutValue("" + row["name"]);
                            break;
                        case "班級開放科目數":
                            wb.Worksheets["班級學生資料"].Cells[rowIndex2, 4].PutValue("" + row["班級開放科目數"]);
                            break;
                        case "阻擋科目數":
                            wb.Worksheets["班級學生資料"].Cells[rowIndex2, 5].PutValue("" + row["阻擋科目數"]);
                            break;
                        case "可選科目數":
                            wb.Worksheets["班級學生資料"].Cells[rowIndex2, 6].PutValue(a - b);
                            break;
                    }
                }
                rowIndex2++;
            }
            #endregion

            #region SheetThree

            DataTable dt3;
            #region sql
            {
                string sql = string.Format(@"
SELECT 
    class.class_name
    , student.seat_no
    , student.student_number
    , student.name
    , subject.subject_name
    , subject.level
    , block.reason
FROM
    $ischool.course_selection.subject_block AS block
    LEFT OUTER JOIN student
        ON student.id = block.ref_student_id
    LEFT OUTER JOIN class
        ON class.id = student.ref_class_id
    LEFT OUTER JOIN $ischool.course_selection.subject AS subject
        ON subject.uid = block.ref_subject_id
WHERE
    subject.school_year = {0}
    AND subject.semester = {1}
    AND subject.type = '{2}'
    AND student.id IS NOT NULL
                ", cbxSchoolYear.Text, cbxSemester.Text, cbxType.Text);

                dt3 = qh.Select(sql);
            }
            #endregion

            int rowIndex3 = 0;

            // 填入資料
            rowIndex3++;
            foreach (DataRow row in dt3.Rows)
            {

                wb.Worksheets["擋修名單資料"].Cells.CopyRows(template.Worksheets["擋修名單資料"].Cells, 1, rowIndex3, 1);
                for (int i = 0; i < listSheetThreeTitle.Count; i++)
                {
                    switch (listSheetThreeTitle[i])
                    {
                        case "班級":
                            wb.Worksheets["擋修名單資料"].Cells[rowIndex3, 0].PutValue("" + row["class_name"]);
                            break;
                        case "座號":
                            wb.Worksheets["擋修名單資料"].Cells[rowIndex3, 1].PutValue("" + row["seat_no"]);
                            break;
                        case "學號":
                            wb.Worksheets["擋修名單資料"].Cells[rowIndex3, 2].PutValue("" + row["student_number"]);
                            break;
                        case "姓名":
                            wb.Worksheets["擋修名單資料"].Cells[rowIndex3, 3].PutValue("" + row["name"]);
                            break;
                        case "擋修名單科目":
                            wb.Worksheets["擋修名單資料"].Cells[rowIndex3, 4].PutValue("" + row["subject_name"]);
                            break;
                        case "擋修名單科目級別":
                            wb.Worksheets["擋修名單資料"].Cells[rowIndex3, 5].PutValue("" + row["level"]);
                            break;
                        case "擋修名單原因":
                            wb.Worksheets["擋修名單資料"].Cells[rowIndex3, 6].PutValue("" + row["reason"]);
                            break;
                    }
                }
                rowIndex3++;
            }

            #endregion

            Save(wb);
        }

        private void Save(Workbook workbook)
        {
            #region 儲存資料

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "選課設定檢查報表";
            saveFileDialog.FileName = "選課設定檢查報表.xlsx";
            saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|所有檔案 (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                DialogResult result = new DialogResult();
                try
                {
                    workbook.Save(saveFileDialog.FileName);
                    result = MsgBox.Show("檔案儲存完成，是否開啟檔案?", "是否開啟", MessageBoxButtons.YesNo);
                }
                catch (Exception ex)
                {
                    MsgBox.Show(ex.Message);
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show("開啟檔案發生失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                this.Close();
            }

            #endregion
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    class ClassData
    {
        /// <summary>
        /// 班級名稱
        /// </summary>
        public string ClassName { get; set; }

        public string GradeYear { get; set; }

        /// <summary>
        /// 班級可選科目資料
        /// </summary>
        public Dictionary<string, SubjectData> dicSubject { get; set; }

        /// <summary>
        /// 班級學生人數
        /// </summary>
        public int ClassStudentCount { get; set; }

        /// <summary>
        /// 班級可選科目名額總計
        /// </summary>
        public int ClassSubjectQuota { get; set; }
    }
    class SubjectData
    {
        public string SubjectName { get; set; }
        public string Level { get; set; }
        public int Limit { get; set; }
        public int TotalStudent { get; set; }
    }
    class Total
    {
        /// <summary>
        /// 科目清單
        /// </summary>
        public Dictionary<string, SubjectData> DicSubject { get; set; }

        /// <summary>
        /// 所有科目名額加總
        /// </summary>
        public int TotalSubjectQuota { get; set; }

        /// <summary>
        /// 班級人數加總
        /// </summary>
        public int TotalClassStudent { get; set; }
    }
}
