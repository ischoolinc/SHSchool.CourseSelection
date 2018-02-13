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

namespace SHSchool.CourseSelection.Forms
{
    public partial class AdjustSSAttendForm : BaseForm
    {
        public AdjustSSAttendForm()
        {
            InitializeComponent();

            AccessHelper access = new AccessHelper();
            QueryHelper qh = new QueryHelper();

            #region Init SchoolYearLb、SemesterLb
            {
                List<UDT.OpeningTime> timeList = access.Select<UDT.OpeningTime>();

                schoolYearLb.Text = "" + timeList[0].SchoolYear;
                semesterLb.Text = "" + timeList[0].Semester;
            }
            #endregion

            #region Init CourseTypeCbx
            {
                string sql = string.Format(@"
                    SELECT DISTINCT 
                        type 
                    FROM 
                        $ischool.course_selection.subject
                    WHERE school_year = {0} AND semester = {1}
                    ",schoolYearLb.Text,semesterLb.Text);

                DataTable dt = qh.Select(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    courseTypeCbx.Items.Add(dr["type"]);
                }
            }
            #endregion

            #region 未分發人數
            {
                string sql = "SELECT count(*) FROM $ischool.course_selection.ss_attend WHERE ref_subject_id IS NULL";
                DataTable dt = qh.Select(sql);

                stuCountLb.Text = "" + dt.Rows[0][0];
            }
            #endregion

        }

        public void ReloadDataGridView()
        {
            dataGridViewX1.Rows.Clear();

            #region SQL
            string sql = string.Format(@"
SELECT 
	subject.uid AS ref_subject_id,
	subject.subject_name,
	sbc.ref_class_id,
	class.class_name,
	sb.count AS subject_student_count,
	stu_count.count AS class_student_count
FROM 
	$ischool.course_selection.subject AS subject
	LEFT OUTER JOIN(
		SELECT
			ref_subject_id,
			ref_class_id
		FROM
			$ischool.course_selection.subject_class_selection
	) sbc ON sbc.ref_subject_id = uid
	LEFT OUTER JOIN(
		SELECT 
			ref_class_id,
			count(id)
		FROM 
			student
		WHERE ref_class_id IS NOT NULL
		GROUP BY ref_class_id
	)stu_count ON stu_count.ref_class_id = sbc.ref_class_id
	LEFT OUTER JOIN(
		SELECT 
			id,
			class_name
		FROM 
			class
	)class ON class.id = sbc.ref_class_id
	LEFT OUTER JOIN(
		SELECT 
			count(ref_student_id),
			ref_subject_id
		FROM
			$ischool.course_selection.ss_attend
		GROUP BY ref_subject_id
	)sb ON sb.ref_subject_id = subject.uid 
WHERE 
	school_year = {0} AND semester = {1} AND type = '{2}'
                ", schoolYearLb.Text, semesterLb.Text, courseTypeCbx.Text);
            #endregion

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql);
            // key: subjecyID
            Dictionary<string, List<_Class>> classDic = new Dictionary<string, List<_Class>>();
            Dictionary<string, List<_Subject>> subjectDic = new Dictionary<string, List<_Subject>>();
            // 計算該選課類別 應選課學生總人數
            Dictionary<string, int> _ClassDic = new Dictionary<string, int>();
            int ssaCount = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (classDic.ContainsKey("" + dr["ref_subject_id"]))
                {
                    _Class _class = new _Class();
                    _class.RefClassID = "" + dr["ref_class_id"];
                    _class.StudentCount = "" + dr["class_student_count"];
                    _class.ClassName = "" + dr["class_name"];
                    classDic["" + dr["ref_subject_id"]].Add(_class);
                }

                if (!classDic.ContainsKey("" + dr["ref_subject_id"]))
                {
                    _Class _class = new _Class();
                    _class.RefClassID = "" + dr["ref_class_id"];
                    _class.StudentCount = "" + dr["class_student_count"];
                    _class.ClassName = "" + dr["class_name"];
                    List<_Class> classList = new List<_Class>();

                    classList.Add(_class);

                    classDic.Add("" + dr["ref_subject_id"],classList);
                }

                if (!subjectDic.ContainsKey("" + dr["ref_subject_id"]))
                {
                    List<_Subject> sbList = new List<_Subject>();
                    _Subject sb = new _Subject();
                    sb.SubjectID = "" + dr["ref_subject_id"];
                    sb.SubjectName = "" + dr["subject_name"];
                    sb.SubjectStudentCount = "" + dr["subject_student_count"];
                    sbList.Add(sb);
                    subjectDic.Add("" + dr["ref_subject_id"], sbList);
                }

                if (!_ClassDic.ContainsKey("" + dr["ref_class_id"]))
                {
                    _ClassDic.Add("" + dr["ref_class_id"],int.Parse("" + dr["class_student_count"]));
                }
            }

            int totalSSACount = _ClassDic.Values.Sum();

            foreach (string subjectID in classDic.Keys)
            {
                int index = 0;
                DataGridViewRow datarow = new DataGridViewRow();
                datarow.CreateCells(dataGridViewX1);

                datarow.Cells[index++].Value = subjectDic[subjectID][0].SubjectName;

                string selectClass = "";
                foreach (_Class c in classDic[subjectID])
                {
                    selectClass += " " + c.ClassName;
                }
                datarow.Cells[index++].Value = selectClass;
                datarow.Cells[index++].Value = subjectDic[subjectID][0].SubjectStudentCount;

                ssaCount += (subjectDic[subjectID][0].SubjectStudentCount == null || subjectDic[subjectID][0].SubjectStudentCount == "" ? 0 : int.Parse(subjectDic[subjectID][0].SubjectStudentCount));

                dataGridViewX1.Rows.Add(datarow);
            }

            ReloadStuCountLb(totalSSACount - ssaCount);
        }

        public void ReloadStuCountLb(int c)
        {
            stuCountLb.Text = "" + c;
        }

        private void dataGridViewX1_Click(object sender, EventArgs e)
        {

        }

        private void courseTypeCbx_TextChanged(object sender, EventArgs e)
        {
            ReloadDataGridView();
        }
    }
    class _Class
    {
        public string RefClassID { get; set; }
        public string ClassName { get; set; }
        public string StudentCount { get; set; }
    }

    class _Subject
    {
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string SubjectStudentCount { get; set; }
    }
}


