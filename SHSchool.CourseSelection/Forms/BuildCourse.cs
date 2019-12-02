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
using FISCA.Data;
using SHSchool.Data;
using FISCA.UDT;

namespace SHSchool.CourseSelection.Forms
{
    public partial class BuildCourse : BaseForm
    {
        private int _schoolYear, _semester;

        private Dictionary<string, Dictionary<string, _SubjectCourse>> _subjectCourseDic = new Dictionary<string, Dictionary<string, _SubjectCourse>>();
        private Dictionary<string, _Subject> _subjectDic = new Dictionary<string, _Subject>();

        public BuildCourse(DataGridView dgv, int sy, int s, string type)
        {
            InitializeComponent();

            _schoolYear = sy;
            _semester = s;

            // Init Lb
            schoolYearLb.Text = "" + sy;
            semesterLb.Text = "" + s;
            courseTypeLb.Text = type;

            #region SQL
            string sql = string.Format(@"
                SELECT
                    subject.uid AS ref_subject_id
	                , subject.school_year
                    , subject.semester
                    , subject.subject_name
                    , subject.level
                    , subject.credit
                    , subject_course.uid AS ref_subject_course_id
                    , subject_course.class_type
	                , subject_course.ref_course_id
	                , course.course_name
                FROM
	                $ischool.course_selection.subject AS subject
	                INNER JOIN $ischool.course_selection.subject_course AS subject_course
		                ON subject.uid = subject_course.ref_subject_id
	                LEFT OUTER JOIN course
		                ON  course.id = subject_course.ref_course_id
                WHERE
                    subject.school_year = {0}
                    AND subject.semester = {1}
                    AND subject.type = '{2}'
            ", sy, s, type);
            #endregion

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql);

            #region 整理科目課程資料
            foreach (DataRow row in dt.Rows)
            {
                string subjectID = "" + row["ref_subject_id"];
                string subjectCourseID = "" + row["ref_subject_course_id"];

                if (_subjectCourseDic.ContainsKey(subjectID))
                {
                    _subjectCourseDic[subjectID].Add(subjectCourseID, new _SubjectCourse());
                    _subjectCourseDic[subjectID][subjectCourseID].UID = subjectCourseID;
                    _subjectCourseDic[subjectID][subjectCourseID].CourseID = "" + row["ref_course_id"];
                    _subjectCourseDic[subjectID][subjectCourseID].CourseName = "" + row["course_name"];
                    _subjectCourseDic[subjectID][subjectCourseID].ClassType = "" + row["class_type"];
                }
                if (!_subjectCourseDic.ContainsKey(subjectID))
                {
                    _subjectCourseDic.Add(subjectID, new Dictionary<string, _SubjectCourse>());
                    _subjectCourseDic[subjectID].Add(subjectCourseID, new _SubjectCourse());
                    _subjectCourseDic[subjectID][subjectCourseID].UID = subjectCourseID;
                    _subjectCourseDic[subjectID][subjectCourseID].CourseID = "" + row["ref_course_id"];
                    _subjectCourseDic[subjectID][subjectCourseID].CourseName = "" + row["course_name"];
                    _subjectCourseDic[subjectID][subjectCourseID].ClassType = "" + row["class_type"];
                }
                if (!_subjectDic.ContainsKey(subjectID))
                {
                    _subjectDic.Add(subjectID, new _Subject());
                    _subjectDic[subjectID].SubjectName = "" + row["subject_name"];
                    _subjectDic[subjectID].Level = "" + row["level"];
                    _subjectDic[subjectID].Credit = "" + row["credit"];
                }
            }
            #endregion

            #region Init DGV
            {
                foreach (DataGridViewRow dr in dgv.Rows)
                {
                    string subjectID = "" + dr.Tag;
                    int 設定開班數 = 0;
                    int.TryParse("" + dr.Cells["buildCourseCount"].Value, out 設定開班數);
                    int 已開班數 = 0;
                    int.TryParse("" + dr.Cells["courseCount"].Value, out 已開班數);

                    // Update 修改
                    if (設定開班數 == 已開班數 && 設定開班數 != 0)
                    {
                        foreach (_SubjectCourse sbc in _subjectCourseDic[subjectID].Values)
                        {
                            int index = 0;
                            DataGridViewRow datarow = new DataGridViewRow();
                            datarow.CreateCells(dataGridViewX1);
                            datarow.Cells[index++].Value = "修改";
                            datarow.Cells[index++].Value = sbc.CourseName;
                            datarow.Cells[index].Tag = subjectID;
                            datarow.Cells[index++].Value = _subjectDic[subjectID].SubjectName;
                            datarow.Cells[index++].Value = _subjectDic[subjectID].Level;
                            datarow.Cells[index++].Value = sbc.ClassType;
                            datarow.Cells[index++].Value = _subjectDic[subjectID].Credit;
                            datarow.Tag = sbc.UID;
                            dataGridViewX1.Rows.Add(datarow);
                        }
                    }

                    // 已開班數 > 設定開班數 : 刪除
                    if (設定開班數 < 已開班數)
                    {
                        int n = 1;
                        foreach (_SubjectCourse sbc in _subjectCourseDic[subjectID].Values)
                        {
                            int index = 0;
                            DataGridViewRow datarow = new DataGridViewRow();
                            datarow.CreateCells(dataGridViewX1);

                            //if (設定開班數 == 1)
                            //{
                            //    sbc.ClassType = "";
                            //}
                            if (n > 設定開班數)
                            {
                                datarow.Cells[index++].Value = "刪除";
                                datarow.DefaultCellStyle.ForeColor = Color.Red;
                            }
                            if (n <= 設定開班數)
                            {
                                datarow.Cells[index++].Value = "修改";
                            }
                            datarow.Cells[index++].Value = sbc.CourseName;
                            datarow.Cells[index].Tag = subjectID;
                            datarow.Cells[index++].Value = _subjectDic[subjectID].SubjectName;
                            datarow.Cells[index++].Value = _subjectDic[subjectID].Level;
                            datarow.Cells[index++].Value = sbc.ClassType;
                            datarow.Cells[index++].Value = _subjectDic[subjectID].Credit;
                            datarow.Tag = sbc.UID;
                            dataGridViewX1.Rows.Add(datarow);
                            n++;
                        }
                    }

                    // 已開班數 < 設定開班數 : 新增
                    if (已開班數 < 設定開班數)
                    {
                        int n = 0;
                        if (n < 已開班數)
                        {
                            foreach (_SubjectCourse sbc in _subjectCourseDic[subjectID].Values)
                            {
                                DataGridViewRow datarow = new DataGridViewRow();
                                datarow.CreateCells(dataGridViewX1);
                                int index = 0;
                                datarow.Cells[index++].Value = "修改";
                                datarow.Cells[index++].Value = sbc.CourseName;
                                datarow.Cells[index].Tag = subjectID;
                                datarow.Cells[index++].Value = _subjectDic[subjectID].SubjectName;
                                datarow.Cells[index++].Value = _subjectDic[subjectID].Level;
                                datarow.Cells[index++].Value = sbc.ClassType;
                                datarow.Cells[index++].Value = _subjectDic[subjectID].Credit;
                                datarow.Tag = sbc.UID;

                                dataGridViewX1.Rows.Add(datarow);
                                n++;
                            }
                        }
                        if (n > 已開班數 || n < 設定開班數)
                        {
                            for (int i = 1; i <= 設定開班數 - n; i++)
                            {
                                InitDataGridView("新增", i + n, 設定開班數, subjectID);
                            }
                        }
                    }
                }
            }
            #endregion

            if (dt.Rows.Count > 0)
            {
                this.buildCourseBtn.Enabled = true;
            }
            else
            {
                this.buildCourseBtn.Enabled = false;
            }
        }

        public void InitDataGridView(string _dataType, int i, int count, string subjectID)
        {
            #region Switch 級別、班別
            string[] mark = new string[10];
            string level = ""; // 羅馬數字
            switch (i)
            {
                case 1:
                    mark[i] = "A";
                    break;
                case 2:
                    mark[i] = "B";
                    break;
                case 3:
                    mark[i] = "C";
                    break;
                case 4:
                    mark[i] = "D";
                    break;
                case 5:
                    mark[i] = "E";
                    break;
                case 6:
                    mark[i] = "F";
                    break;
                case 7:
                    mark[i] = "G";
                    break;
                case 8:
                    mark[i] = "H";
                    break;
                case 9:
                    mark[i] = "I";
                    break;
                case 10:
                    mark[i] = "J";
                    break;
                default:
                    mark[i] = "";
                    break;
            }
            int tryParseInt = -1;
            if (int.TryParse(_subjectDic[subjectID].Level, out tryParseInt))
            {
                switch (tryParseInt)
                {
                    case 1:
                        level = "I";
                        break;
                    case 2:
                        level = "II";
                        break;
                    case 3:
                        level = "III";
                        break;
                    case 4:
                        level = "IV";
                        break;
                    case 5:
                        level = "V";
                        break;
                    case 6:
                        level = "VI";
                        break;
                    default:
                        level = "" + tryParseInt;
                        break;
                }
            }
            else
            {
                level = "";
            }
            #endregion

            //如果 設定開班數 count = 1，不加班別
            if (count == 1)
            {
                mark[i] = "";
            }

            DataGridViewRow datarow = new DataGridViewRow();
            datarow.CreateCells(dataGridViewX1);
            int index = 0;
            datarow.Cells[index++].Value = _dataType;
            datarow.Cells[index++].Value = courseTypeLb.Text + " " + _subjectDic[subjectID].SubjectName + " " + level + " " + mark[i]; // 課程名稱: 課程類別 + 科目名稱 + 級別 + 班別
            datarow.Cells[index].Tag = subjectID;
            datarow.Cells[index++].Value = _subjectDic[subjectID].SubjectName;
            datarow.Cells[index++].Value = _subjectDic[subjectID].Level; // INTERGER
            datarow.Cells[index++].Value = mark[i];
            datarow.Cells[index++].Value = _subjectDic[subjectID].Credit;
            if (_dataType == "刪除")
            {
                //drX1.DefaultCellStyle.BackColor = Color.Red;
                datarow.DefaultCellStyle.ForeColor = Color.Red;
            }
            dataGridViewX1.Rows.Add(datarow);
        }

        private void dataGridViewX1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                string level = "";
                int tryParseInt = 0;
                if (int.TryParse("" + dataGridViewX1.Rows[e.RowIndex].Cells[4].Value, out tryParseInt))
                {
                    switch (tryParseInt)
                    {
                        case 1:
                            level = "I";
                            break;
                        case 2:
                            level = "II";
                            break;
                        case 3:
                            level = "III";
                            break;
                        case 4:
                            level = "IV";
                            break;
                        case 5:
                            level = "V";
                            break;
                        case 6:
                            level = "VI";
                            break;
                        default:
                            level = "" + dataGridViewX1.Rows[e.RowIndex].Cells[4].Value;
                            break;
                    }
                }
                dataGridViewX1.Rows[e.RowIndex].Cells[1].Value = dataGridViewX1.Rows[e.RowIndex].Cells[2].Value + " " +
                                                                 dataGridViewX1.Rows[e.RowIndex].Cells[3].Value + " " +
                                                                 level + " " +
                                                                 dataGridViewX1.Rows[e.RowIndex].Cells[5].Value;
            }
        }

        // 欄位驗證 : 判斷課程名稱是否重複
        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Dictionary<string, string> courseNameDic = new Dictionary<string, string>();
            foreach (DataGridViewRow dr in dataGridViewX1.Rows)
            {
                if (courseNameDic.ContainsKey("" + dr.Cells[1].Value))
                {
                    dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "error";
                    return;
                }
                if (!courseNameDic.ContainsKey("" + dr.Cells[1].Value))
                {
                    courseNameDic.Add("" + dr.Cells["courseName"].Value, "" + dr.Cells["subjectName"].Value);
                    dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = string.Empty;
                }

            }
        }

        // 開課
        private void buildCourseBtn_Click(object sender, EventArgs e)
        {
            List<string> courseNameList = new List<string>();
            bool repeat = false;
            foreach (DataGridViewRow datarow in dataGridViewX1.Rows)
            {
                if (courseNameList.Contains("" + datarow.Cells["courseName"].Value))
                {
                    repeat = true;
                    MessageBox.Show("課程名稱重複!!");
                    return;
                }
                if (!courseNameList.Contains("" + datarow.Cells["courseName"].Value))
                {
                    courseNameList.Add("" + datarow.Cells["courseName"].Value);
                }
            }

            List<string> dataList = new List<string>();
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                string dataType = "" + row.Cells["dataType"].Value;
                string subjectID = "" + row.Cells["subjectName"].Tag;
                string subjectCourseID = (("" + row.Tag) == "" ? "NULL" : ("" + row.Tag));
                string courseID = subjectCourseID == "NULL" ? "NULL" : _subjectCourseDic[subjectID][subjectCourseID].CourseID;
                string subjectName = _subjectDic[subjectID].SubjectName;
                string classType = "" + row.Cells["classType"].Value;
                string level = _subjectDic[subjectID].Level;
                int tryParseInt = 0;
                if (int.TryParse("" + row.Cells["level"].Value, out tryParseInt))
                {
                    switch (tryParseInt)
                    {
                        case 1:
                            level = "I";
                            break;
                        case 2:
                            level = "II";
                            break;
                        case 3:
                            level = "III";
                            break;
                        case 4:
                            level = "IV";
                            break;
                        case 5:
                            level = "V";
                            break;
                        case 6:
                            level = "VI";
                            break;
                        default:
                            level = "" + row.Cells["level"].Value;
                            break;
                    }
                }

                string courseName = (courseTypeLb.Text + " " + subjectName + " " + level + " " + classType).Trim();
                string credit = _subjectDic[subjectID].Credit == "" ? "NULL" : _subjectDic[subjectID].Credit;

                string data = string.Format(@"
SELECT
	'{0}'::TEXT AS data_type
	, {1}::BIGINT AS ref_subject_id
	, {2}::BIGINT AS ref_subject_course_id
	, {3}::BIGINT AS ref_course_id
	, '{4}'::TEXT AS subject_name
	, '{5}'::TEXT AS class_type
	, {6}::INT AS subject_level
	, '{7}'::TEXT AS course_name
	, {8}::REAL AS credit
	, {9} AS school_year
	, {10} AS semester   
                    "
                    , dataType
                    , subjectID
                    , subjectCourseID
                    , courseID
                    , subjectName
                    , classType
                    , int.TryParse(_subjectDic[subjectID].Level, out tryParseInt) ? ("" + tryParseInt) : "NULL"
                    , courseName
                    , credit
                    , schoolYearLb.Text
                    , semesterLb.Text
                );

                dataList.Add(data);

                #region
                #endregion
            }

            string dataRow = string.Join("\r UNION ALL", dataList);

            #region SQL
            string sql = string.Format(@"
WITH data_row AS(
	{0}
) ,insert_course AS(
	INSERT INTO course(
	 		course_name
	 		, subject
	 		, subj_level
	 		, credit
	 		, school_year
	 		, semester
	)
	SELECT
		course_name
		, subject_name
		, subject_level
		, credit
		, school_year
		, semester
	FROM
	 	data_row
 	WHERE
 		data_type = '新增'
 	RETURNING *
) ,insert_subject_course AS(
	INSERT INTO $ischool.course_selection.subject_course (
		ref_subject_id
		, ref_course_id
		, class_type	
	)
	SELECT
		data_row.ref_subject_id
		, insert_course.id
		, data_row.class_type
	FROM
		data_row
		LEFT OUTER JOIN insert_course 
			ON insert_course.course_name = data_row.course_name
	WHERE
		data_row.data_type = '新增'
	RETURNING *
) ,update_subject_course AS(
	UPDATE $ischool.course_selection.subject_course SET
		class_type = data_row.class_type 
	FROM
		data_row
	WHERE
		data_row.data_type = '更新'
		AND $ischool.course_selection.subject_course.uid = data_row.ref_subject_course_id
	RETURNING $ischool.course_selection.subject_course.uid AS ref_subject_course_id
) ,update_course AS( 
	UPDATE course SET 
		course_name = data_row.course_name
	FROM
		data_row
	WHERE
		course.id = data_row.ref_course_id
) ,delete_course AS(
	DELETE 
	FROM
		course
	WHERE
		id IN (
				SELECT
					ref_course_id
				FROM
					data_row
				WHERE
					data_type = '刪除'
			)
	RETURNING *
) ,delete_subject_course AS(
    DELETE
    FROM
	    $ischool.course_selection.subject_course
    WHERE
	    uid IN(
			    SELECT
				    ref_subject_course_id
			    FROM
				    data_row
			    WHERE
				    data_type = '刪除'
		)    
    RETURNING *
)
UPDATE 
    $ischool.course_selection.ss_attend AS ss_attend
SET
    ref_subject_course_id = NULL
FROM
    data_row
WHERE
    data_row.ref_subject_course_id = ss_attend.ref_subject_course_id
    AND data_row.data_type = '刪除'
                ", dataRow);
            #endregion

            UpdateHelper up = new UpdateHelper();
            up.Execute(sql);

            if (repeat == false)
            {
                MessageBox.Show("課程建立成功!");
                this.Close();
            }
            if (repeat == true)
            {
                MessageBox.Show("課程建立失敗!");
            }

        }
    }
}

class _SubjectCourse
{
    public string UID { get; set; }
    public string DataType { get; set; }
    public string CourseName { get; set; }
    public string CourseID { get; set; }
    public string ClassType { get; set; }

}
class _Subject
{
    public string SubjectName { get; set; }
    public string Level { get; set; }
    public string Credit { get; set; }
}
