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
using FISCA.UDT;
using FISCA.Authentication;
using FISCA.LogAgent;
using System.Threading.Tasks;

namespace SHSchool.CourseSelection.Forms
{
    public partial class frmWishEditor : BaseForm
    {
        private AccessHelper _access = new AccessHelper();
        private QueryHelper _qh = new QueryHelper();
        private UpdateHelper _up = new UpdateHelper();
        private bool _loading = true;
        private string _userAccount = DSAServices.UserAccount;
        private string _clientInfo = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;
        BackgroundWorker _bgw = new BackgroundWorker();

        public frmWishEditor()
        {
            InitializeComponent();

            _bgw.WorkerReportsProgress = true;
            _bgw.DoWork += new DoWorkEventHandler(BGW_DoWork);
            _bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            _bgw.ProgressChanged += new ProgressChangedEventHandler(BGW_ProgressChanged);
        }

        private void frmWishEditor_Load(object sender, EventArgs e)
        {
            #region Init SchoolYear、Semester
            {
                List<UDT.OpeningTime> listOpenTime = this._access.Select<UDT.OpeningTime>();

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
            }
            #endregion

            #region Init CoursePeriod
            {
                ReloadCoursePeriod(cbxSchoolYear.SelectedItem.ToString(), cbxSemester.SelectedItem.ToString());
            }
            #endregion
            
            #region Init DataGridView 
            {
                if (cbxCoursePeriod.Items.Count > 0)
                {
                    ReloadDataGridView(cbxSchoolYear.SelectedItem.ToString(), cbxSemester.SelectedItem.ToString(), cbxCoursePeriod.SelectedItem.ToString());
                }
            }
            #endregion
                
            this._loading = false;
        }

        /// <summary>
        /// 重新取得課程時段清單
        /// </summary>
        private void ReloadCoursePeriod(string schoolYear, string semester)
        {
            DataTable dt = this._qh.Select(string.Format(@"
SELECT DISTINCT
    type
FROM
    $ischool.course_selection.subject
WHERE
    school_year = {0}
    AND semester = {1}
            ", schoolYear, semester));


            if (dt.Rows.Count <= 0)
            {
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                cbxCoursePeriod.Items.Add("" + row["type"]);
            }

            cbxCoursePeriod.SelectedIndex = 0;
        }

        /// <summary>
        /// 重新取得志願清單資料
        /// Cell.Tag => Wish Object
        /// dgvrow.Tag => Attend Object
        /// </summary>
        private void ReloadDataGridView(string schoolYear, string semester, string coursePeriod)
        {
            lb4.Visible = true;
            //this.SuspendLayout();
            {
                dataGridViewX1.Rows.Clear();

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
) ,target_class AS(
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
  SELECT 
    ref_student_id
    , ref_subject_id
    , string_agg(reason,'; ') AS reason
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
) , student_attend AS(
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
) ,wish_row AS(
  SELECT
    wish.ref_student_id
    , wish.ref_subject_id
    , wish.sequence
    , subject.subject_name
    , subject.disabled 
    , subject.level
    , wish.is_cancel
    , wish.cancel_reason
    , wish.cancel_by
    , wish.cancel_time
  FROM
    $ischool.course_selection.ss_wish AS wish
    LEFT OUTER JOIN $ischool.course_selection.subject AS subject
        ON subject.uid = wish.ref_subject_id
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
) ,wish AS(
  SELECT
    ref_student_id
    , ROW_NUMBER() OVER(PARTITION BY ref_student_id ORDER BY sequence) AS sequence  
    , ref_subject_id
    , subject_name
    , disabled
    , level
    , is_cancel
    , cancel_reason
    , cancel_by
    , cancel_time
  FROM
    wish_row
) 
SELECT
  target_student.*
  , student_attend.lock
  , student_attend.attend_type
  , student_attend.ref_subject_id
  , student_attend.subject_name
  , student_attend.跨課程時段 AS is_cross_type
  , wish1.subject_name AS wish1
  , wish2.subject_name AS wish2
  , wish3.subject_name AS wish3
  , wish4.subject_name AS wish4
  , wish5.subject_name AS wish5
  , wish6.subject_name AS wish6
  , wish7.subject_name AS wish7
  , wish8.subject_name AS wish8
  , wish9.subject_name AS wish9
  , wish10.subject_name AS wish10
  , wish1.ref_subject_id AS wish1ref_subject_id
  , wish2.ref_subject_id AS wish2ref_subject_id
  , wish3.ref_subject_id AS wish3ref_subject_id
  , wish4.ref_subject_id AS wish4ref_subject_id
  , wish5.ref_subject_id AS wish5ref_subject_id
  , wish6.ref_subject_id AS wish6ref_subject_id
  , wish7.ref_subject_id AS wish7ref_subject_id
  , wish8.ref_subject_id AS wish8ref_subject_id
  , wish9.ref_subject_id AS wish9ref_subject_id
  , wish10.ref_subject_id AS wish10ref_subject_id
  , wish1.level AS wish1_level
  , wish2.level AS wish2_level
  , wish3.level AS wish3_level
  , wish4.level AS wish4_level
  , wish5.level AS wish5_level
  , wish6.level AS wish6_level
  , wish7.level AS wish7_level
  , wish8.level AS wish8_level
  , wish9.level AS wish9_level
  , wish10.level AS wish10_level
  , wish1.disabled AS wish1_disabled
  , wish2.disabled AS wish2_disabled
  , wish3.disabled AS wish3_disabled
  , wish4.disabled AS wish4_disabled
  , wish5.disabled AS wish5_disabled
  , wish6.disabled AS wish6_disabled
  , wish7.disabled AS wish7_disabled
  , wish8.disabled AS wish8_disabled
  , wish9.disabled AS wish9_disabled
  , wish10.disabled AS wish10_disabled
  , wish1.is_cancel AS wish1_is_cancel
  , wish2.is_cancel AS wish2_is_cancel
  , wish3.is_cancel AS wish3_is_cancel
  , wish4.is_cancel AS wish4_is_cancel
  , wish5.is_cancel AS wish5_is_cancel
  , wish6.is_cancel AS wish6_is_cancel
  , wish7.is_cancel AS wish7_is_cancel
  , wish8.is_cancel AS wish8_is_cancel
  , wish9.is_cancel AS wish9_is_cancel
  , wish10.is_cancel AS wish10_is_cancel
  , wish1.cancel_reason AS wish1_cancel_reason
  , wish2.cancel_reason AS wish2_cancel_reason
  , wish3.cancel_reason AS wish3_cancel_reason
  , wish4.cancel_reason AS wish4_cancel_reason
  , wish5.cancel_reason AS wish5_cancel_reason
  , wish6.cancel_reason AS wish6_cancel_reason
  , wish7.cancel_reason AS wish7_cancel_reason
  , wish8.cancel_reason AS wish8_cancel_reason
  , wish9.cancel_reason AS wish9_cancel_reason
  , wish10.cancel_reason AS wish10_cancel_reason
  , wish1.cancel_by AS wish1_cancel_by
  , wish2.cancel_by AS wish2_cancel_by
  , wish3.cancel_by AS wish3_cancel_by
  , wish4.cancel_by AS wish4_cancel_by
  , wish5.cancel_by AS wish5_cancel_by
  , wish6.cancel_by AS wish6_cancel_by
  , wish7.cancel_by AS wish7_cancel_by
  , wish8.cancel_by AS wish8_cancel_by
  , wish9.cancel_by AS wish9_cancel_by
  , wish10.cancel_by AS wish10_cancel_by
  , CASE WHEN block1.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish1_is_block
  , block1.reason AS block1_reason
  , CASE WHEN block2.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish2_is_block
  , block2.reason AS block2_reason
  , CASE WHEN block3.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish3_is_block
  , block3.reason AS block3_reason
  , CASE WHEN block4.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish4_is_block
  , block4.reason AS block4_reason
  , CASE WHEN block5.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish5_is_block
  , block5.reason AS block5_reason
 , CASE WHEN block6.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish6_is_block
  , block6.reason AS block6_reason
 , CASE WHEN block7.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish7_is_block
  , block7.reason AS block7_reason
 , CASE WHEN block8.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish8_is_block
  , block8.reason AS block8_reason
 , CASE WHEN block9.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish9_is_block
  , block9.reason AS block9_reason
 , CASE WHEN block10.ref_student_id IS NOT null
        THEN true
        ELSE false
    END AS wish10_is_block
  , block10.reason AS block10_reason
FROM
  target_student
  LEFT OUTER JOIN student_attend
    ON student_attend.ref_student_id = target_student.id
  LEFT OUTER JOIN wish as wish1
    ON wish1.ref_student_id = target_student.id
    AND wish1.sequence = 1
  LEFT OUTER JOIN target_subject_block AS block1
    ON block1.ref_student_id = target_student.id
    AND block1.ref_subject_id = wish1.ref_subject_id
  LEFT OUTER JOIN wish as wish2
    ON wish2.ref_student_id = target_student.id
    AND wish2.sequence = 2
  LEFT OUTER JOIN target_subject_block AS block2
    ON block2.ref_student_id = target_student.id
    AND block2.ref_subject_id = wish2.ref_subject_id
  LEFT OUTER JOIN wish as wish3
    ON wish3.ref_student_id = target_student.id
    AND wish3.sequence = 3
  LEFT OUTER JOIN target_subject_block AS block3
    ON block3.ref_student_id = target_student.id
    AND block3.ref_subject_id = wish3.ref_subject_id
  LEFT OUTER JOIN wish as wish4
    ON wish4.ref_student_id = target_student.id
    AND wish4.sequence = 4
  LEFT OUTER JOIN target_subject_block AS block4
    ON block4.ref_student_id = target_student.id
    AND block4.ref_subject_id = wish4.ref_subject_id
  LEFT OUTER JOIN wish as wish5
    ON wish5.ref_student_id = target_student.id
    AND wish5.sequence = 5
  LEFT OUTER JOIN target_subject_block AS block5
    ON block5.ref_student_id = target_student.id
    AND block5.ref_subject_id = wish5.ref_subject_id
  LEFT OUTER JOIN wish as wish6
    ON wish6.ref_student_id = target_student.id
    AND wish6.sequence = 6
  LEFT OUTER JOIN target_subject_block AS block6
    ON block6.ref_student_id = target_student.id
    AND block6.ref_subject_id = wish6.ref_subject_id
  LEFT OUTER JOIN wish as wish7
    ON wish7.ref_student_id = target_student.id
    AND wish7.sequence = 7
  LEFT OUTER JOIN target_subject_block AS block7
    ON block7.ref_student_id = target_student.id
    AND block7.ref_subject_id = wish7.ref_subject_id
  LEFT OUTER JOIN wish as wish8
    ON wish8.ref_student_id = target_student.id
    AND wish8.sequence = 8
  LEFT OUTER JOIN target_subject_block AS block8
    ON block8.ref_student_id = target_student.id
    AND block8.ref_subject_id = wish8.ref_subject_id
  LEFT OUTER JOIN wish as wish9
    ON wish9.ref_student_id = target_student.id
    AND wish9.sequence = 9
  LEFT OUTER JOIN target_subject_block AS block9
    ON block9.ref_student_id = target_student.id
    AND block9.ref_subject_id = wish9.ref_subject_id
  LEFT OUTER JOIN wish as wish10
    ON wish10.ref_student_id = target_student.id
    AND wish10.sequence = 10
  LEFT OUTER JOIN target_subject_block AS block10
    ON block10.ref_student_id = target_student.id
    AND block10.ref_subject_id = wish10.ref_subject_id
ORDER BY 
  target_student.grade_year
  , target_student.display_order
  , target_student.class_name
  , target_student.seat_no
  , target_student.id
            ", schoolYear, semester, coursePeriod);
                #endregion

                DataTable dt = new DataTable();

                try
                {
                    Task t = Task.Factory.StartNew(() => {
                        dt = _qh.Select(sql);
                    });

                    t.Wait();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                
                foreach (DataRow row in dt.Rows)
                {
                    DataGridViewRow dgvrow = new DataGridViewRow();
                    dgvrow.CreateCells(dataGridViewX1);

                    Attend attend = new Attend();
                    Wish wish1 = new Wish();
                    Wish wish2 = new Wish();
                    Wish wish3 = new Wish();
                    Wish wish4 = new Wish();
                    Wish wish5 = new Wish();
                    Wish wish6 = new Wish();
                    Wish wish7 = new Wish();
                    Wish wish8 = new Wish();
                    Wish wish9 = new Wish();
                    Wish wish10 = new Wish();

                    #region Attend
                    {
                        attend.RefStudentID = "" + row["id"];
                        attend.RefSubjectID = "" + row["ref_subject_id"];
                        attend.SubjectName = "" + row["subject_name"];
                        attend.IsCrossType = ("" + row["is_cross_type"]) == "true" ? true : false;
                        attend.IsLock = ("" + row["lock"]) == "true" ? true : false;
                        attend.AttendType = "" + row["attend_type"];
                    }
                    #endregion

                    #region wish1
                    {
                        wish1.RefSubjectID = "" + row["wish1ref_subject_id"];
                        wish1.Level = "" + row["wish1_level"];
                        wish1.Disabled = ("" + row["wish1_disabled"]) == "true" ? true : false;
                        wish1.IsCancel = ("" + row["wish1_is_cancel"]) == "true" ? true : false;
                        wish1.CancelReason = "" + row["wish1_cancel_reason"];
                        wish1.CancelBy = "" + row["wish1_cancel_by"];
                        wish1.IsBlock = ("" + row["wish1_is_block"]) == "true" ? true : false;
                        wish1.BlockReason = "" + row["block1_reason"];
                    }
                    #endregion

                    #region wish2
                    {
                        wish2.RefSubjectID = "" + row["wish2ref_subject_id"];
                        wish2.Level = "" + row["wish2_level"];
                        wish2.Disabled = ("" + row["wish2_disabled"]) == "true" ? true : false;
                        wish2.IsCancel = ("" + row["wish2_is_cancel"]) == "true" ? true : false;
                        wish2.CancelReason = "" + row["wish2_cancel_reason"];
                        wish2.CancelBy = "" + row["wish2_cancel_by"];
                        wish2.IsBlock = ("" + row["wish2_is_block"]) == "true" ? true : false;
                        wish2.BlockReason = "" + row["block2_reason"];
                    }
                    #endregion

                    #region wish3
                    {
                        wish3.RefSubjectID = "" + row["wish3ref_subject_id"];
                        wish3.Level = "" + row["wish3_level"];
                        wish3.Disabled = ("" + row["wish3_disabled"]) == "true" ? true : false;
                        wish3.IsCancel = ("" + row["wish3_is_cancel"]) == "true" ? true : false;
                        wish3.CancelReason = "" + row["wish3_cancel_reason"];
                        wish3.CancelBy = "" + row["wish3_cancel_by"];
                        wish3.IsBlock = ("" + row["wish3_is_block"]) == "true" ? true : false;
                        wish3.BlockReason = "" + row["block3_reason"];
                    }
                    #endregion

                    #region wish4
                    {
                        wish4.RefSubjectID = "" + row["wish4ref_subject_id"];
                        wish4.Level = "" + row["wish4_level"];
                        wish4.Disabled = ("" + row["wish4_disabled"]) == "true" ? true : false;
                        wish4.IsCancel = ("" + row["wish4_is_cancel"]) == "true" ? true : false;
                        wish4.CancelReason = "" + row["wish4_cancel_reason"];
                        wish4.CancelBy = "" + row["wish4_cancel_by"];
                        wish4.IsBlock = ("" + row["wish4_is_block"]) == "true" ? true : false;
                        wish4.BlockReason = "" + row["block4_reason"];
                    }
                    #endregion

                    #region wish5
                    {
                        wish5.RefSubjectID = "" + row["wish5ref_subject_id"];
                        wish5.Level = "" + row["wish5_level"];
                        wish5.Disabled = ("" + row["wish5_disabled"]) == "true" ? true : false;
                        wish5.IsCancel = ("" + row["wish5_is_cancel"]) == "true" ? true : false;
                        wish5.CancelReason = "" + row["wish5_cancel_reason"];
                        wish5.CancelBy = "" + row["wish5_cancel_by"];
                        wish5.IsBlock = ("" + row["wish5_is_block"]) == "true" ? true : false;
                        wish5.BlockReason = "" + row["block5_reason"];
                    }
                    #endregion

                    #region wish6
                    {
                        wish6.RefSubjectID = "" + row["wish6ref_subject_id"];
                        wish6.Level = "" + row["wish6_level"];
                        wish6.Disabled = ("" + row["wish6_disabled"]) == "true" ? true : false;
                        wish6.IsCancel = ("" + row["wish6_is_cancel"]) == "true" ? true : false;
                        wish6.CancelReason = "" + row["wish6_cancel_reason"];
                        wish6.CancelBy = "" + row["wish6_cancel_by"];
                        wish6.IsBlock = ("" + row["wish6_is_block"]) == "true" ? true : false;
                        wish6.BlockReason = "" + row["block6_reason"];
                    }
                    #endregion

                    #region wish7
                    {
                        wish7.RefSubjectID = "" + row["wish7ref_subject_id"];
                        wish7.Level = "" + row["wish7_level"];
                        wish7.Disabled = ("" + row["wish7_disabled"]) == "true" ? true : false;
                        wish7.IsCancel = ("" + row["wish7_is_cancel"]) == "true" ? true : false;
                        wish7.CancelReason = "" + row["wish7_cancel_reason"];
                        wish7.CancelBy = "" + row["wish7_cancel_by"];
                        wish7.IsBlock = ("" + row["wish7_is_block"]) == "true" ? true : false;
                        wish7.BlockReason = "" + row["block7_reason"];
                    }
                    #endregion

                    #region wish8
                    {
                        wish8.RefSubjectID = "" + row["wish8ref_subject_id"];
                        wish8.Level = "" + row["wish8_level"];
                        wish8.Disabled = ("" + row["wish8_disabled"]) == "true" ? true : false;
                        wish8.IsCancel = ("" + row["wish8_is_cancel"]) == "true" ? true : false;
                        wish8.CancelReason = "" + row["wish8_cancel_reason"];
                        wish8.CancelBy = "" + row["wish8_cancel_by"];
                        wish8.IsBlock = ("" + row["wish8_is_block"]) == "true" ? true : false;
                        wish8.BlockReason = "" + row["block8_reason"];
                    }
                    #endregion

                    #region wish9
                    {
                        wish9.RefSubjectID = "" + row["wish9ref_subject_id"];
                        wish9.Level = "" + row["wish9_level"];
                        wish9.Disabled = ("" + row["wish9_disabled"]) == "true" ? true : false;
                        wish9.IsCancel = ("" + row["wish9_is_cancel"]) == "true" ? true : false;
                        wish9.CancelReason = "" + row["wish9_cancel_reason"];
                        wish9.CancelBy = "" + row["wish9_cancel_by"];
                        wish9.IsBlock = ("" + row["wish9_is_block"]) == "true" ? true : false;
                        wish9.BlockReason = "" + row["block9_reason"];
                    }
                    #endregion

                    #region wish10
                    {
                        wish10.RefSubjectID = "" + row["wish10ref_subject_id"];
                        wish10.Level = "" + row["wish10_level"];
                        wish10.Disabled = ("" + row["wish10_disabled"]) == "true" ? true : false;
                        wish10.IsCancel = ("" + row["wish10_is_cancel"]) == "true" ? true : false;
                        wish10.CancelReason = "" + row["wish10_cancel_reason"];
                        wish10.CancelBy = "" + row["wish10_cancel_by"];
                        wish10.IsBlock = ("" + row["wish10_is_block"]) == "true" ? true : false;
                        wish10.BlockReason = "" + row["block10_reason"];
                    }
                    #endregion

                    int i = 0;

                    dgvrow.Cells[i++].Value = "" + row["class_name"];
                    dgvrow.Cells[i++].Value = "" + row["seat_no"];
                    dgvrow.Cells[i++].Value = "" + row["name"];

                    #region wish1
                    {
                        dgvrow.Cells[i].Tag = wish1;
                        if (wish1.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish1.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish1"], Tool.RomanChar(wish1.Level));
                    }
                    #endregion

                    #region wish2
                    {
                        dgvrow.Cells[i].Tag = wish2;
                        if (wish2.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish2.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish2"], Tool.RomanChar(wish2.Level));
                    }
                    #endregion

                    #region wish3
                    {
                        dgvrow.Cells[i].Tag = wish3;
                        if (wish3.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish3.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish3"], Tool.RomanChar(wish3.Level));
                    }
                    #endregion

                    #region wish4
                    {
                        dgvrow.Cells[i].Tag = wish4;
                        if (wish4.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish4.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish4"], Tool.RomanChar(wish4.Level));
                    }
                    #endregion

                    #region wish5
                    {
                        dgvrow.Cells[i].Tag = wish5;
                        if (wish5.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish5.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish5"], Tool.RomanChar(wish5.Level));
                    }
                    #endregion

                    #region wish6
                    {
                        dgvrow.Cells[i].Tag = wish6;
                        if (wish6.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish6.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish6"], Tool.RomanChar(wish6.Level));
                    }
                    #endregion

                    #region wish7
                    {
                        dgvrow.Cells[i].Tag = wish7;
                        if (wish7.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish7.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish7"], Tool.RomanChar(wish7.Level));
                    }
                    #endregion

                    #region wish8
                    {
                        dgvrow.Cells[i].Tag = wish8;
                        if (wish8.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish8.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish8"], Tool.RomanChar(wish8.Level));
                    }
                    #endregion

                    #region wish9
                    {
                        dgvrow.Cells[i].Tag = wish9;
                        if (wish9.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish9.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish9"], Tool.RomanChar(wish9.Level));
                    }
                    #endregion

                    #region wish10
                    {
                        dgvrow.Cells[i].Tag = wish10;
                        if (wish10.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish10.CancelReason;
                        }
                        dgvrow.Cells[i++].Value = string.Format("{0} {1}", row["wish10"], Tool.RomanChar(wish10.Level));
                    }
                    #endregion

                    dgvrow.Tag = attend;
                    dataGridViewX1.Rows.Add(dgvrow);
                }
            }
            //this.ResumeLayout();
            lb4.Visible = false;
        }

        private void cbxSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this._loading)
            {
                ReloadCoursePeriod(cbxSchoolYear.SelectedItem.ToString(), cbxSemester.SelectedItem.ToString());
            }
        }

        private void cbxSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this._loading)
            {
                ReloadCoursePeriod(cbxSchoolYear.SelectedItem.ToString(), cbxSemester.SelectedItem.ToString());
            }
        }

        private void cbxCoursePeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this._loading)
            {
                ReloadDataGridView(cbxSchoolYear.SelectedItem.ToString(),cbxSemester.SelectedItem.ToString(),cbxCoursePeriod.SelectedItem.ToString());
            }
        }

        private void btnSignDisabled_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            {
                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    for (int i = 3; i <= 12; i++)
                    {
                        Wish wish = dgvrow.Cells[i].Tag as Wish;
                        if (wish.Disabled)
                        {
                            dgvrow.Cells[i].Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
            this.ResumeLayout();
        }

        private void btnCancelDisabled_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            {
                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    for (int i = 3; i <= 12; i++)
                    {
                        Wish wish = dgvrow.Cells[i].Tag as Wish;
                        if (wish.Disabled && !wish.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = "不開課";
                            wish.IsCancel = true;
                            wish.CancelReason = "不開課";
                        }
                    }
                }
            }
            this.ResumeLayout();
        }

        private void btnSignBlock_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            {
                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    for (int i = 3; i <= 12; i++)
                    {
                        Wish wish = dgvrow.Cells[i].Tag as Wish;
                        if (wish.IsBlock)
                        {
                            dgvrow.Cells[i].Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
            this.ResumeLayout();
        }

        private void btnCancelBlock_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            {
                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    for (int i = 3; i <= 12; i++)
                    {
                        Wish wish = dgvrow.Cells[i].Tag as Wish;
                        if (wish.IsBlock && !wish.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = wish.BlockReason;
                            wish.IsCancel = true;
                            wish.CancelReason = wish.BlockReason;
                        }
                    }
                }
            }
            this.ResumeLayout();
        }

        private void btnSignSelected_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            {
                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    Attend attend = dgvrow.Tag as Attend;
                    for (int i = 3; i <= 12; i++)
                    {
                        Wish wish = dgvrow.Cells[i].Tag as Wish;
                        if (attend.RefSubjectID == wish.RefSubjectID)
                        {
                            dgvrow.Cells[i].Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
            this.ResumeLayout();
        }

        private void btnCancelSelected_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            {
                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    Attend attend = dgvrow.Tag as Attend;
                    for (int i = 3; i <= 12; i++)
                    {
                        Wish wish = dgvrow.Cells[i].Tag as Wish;
                        if (attend.RefSubjectID == wish.RefSubjectID && !wish.IsCancel)
                        {
                            dgvrow.Cells[i].Style.Font = new Font(dataGridViewX1.Font, FontStyle.Strikeout);
                            dgvrow.Cells[i].ToolTipText = "已選修課程";
                            wish.IsCancel = true;
                            wish.CancelReason = "已選修課程";
                        }
                    }
                }
            }
            this.ResumeLayout();
        }

        private void btnRecover_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            {
                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    for (int i = 3; i <= 12; i++)
                    {
                        dgvrow.Cells[i].Style = new DataGridViewCellStyle(dataGridViewX1.DefaultCellStyle);
                        Wish wish = dgvrow.Cells[i].Tag as Wish;
                        wish.IsCancel = false;
                        wish.CancelReason = "";
                    }
                }
            }
            this.ResumeLayout();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region Button
            {
                cbxSchoolYear.Enabled = false;
                cbxSemester.Enabled = false;
                cbxCoursePeriod.Enabled = false;
                btnUnOpen.Enabled = false;
                btnSelected.Enabled = false;
                btnBlock.Enabled = false;
                btnRecover.Enabled = false;
                btnLeave.Enabled = false;
                btnSave.Enabled = false;
                progressBarX1.Visible = true;
            }
            #endregion

            BgwArgument arg = new BgwArgument();
            arg.SchoolYear = cbxSchoolYear.SelectedItem.ToString();
            arg.Semester = cbxSemester.SelectedItem.ToString();
            arg.Type = cbxCoursePeriod.SelectedItem.ToString();

            this._bgw.RunWorkerAsync(arg);
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = sender as BackgroundWorker;
            BgwArgument arg = (BgwArgument)e.Argument;

            List<string> listDataRow = new List<string>();

            #region 資料整理
            {
                foreach (DataGridViewRow dgvrow in dataGridViewX1.Rows)
                {
                    Attend attend = dgvrow.Tag as Attend;

                    for (int i = 3; i <= 12; i++)
                    {
                        Wish wish = dgvrow.Cells[i].Tag as Wish;
                        // 如果有志願
                        if (!string.IsNullOrEmpty(wish.RefSubjectID))
                        {
                            string data = string.Format(@"
SELECT
    {0}::BIGINT AS ref_student_id
    , {1}::BIGINT AS ref_subject_id
    , {2}::BOOLEAN AS is_cancel
    , '{3}'::TEXT AS cancel_reason
    , '{4}'::TEXT AS cancel_by 
    , now() AS cancel_time
                    ", attend.RefStudentID, wish.RefSubjectID, wish.IsCancel, wish.CancelReason, this._userAccount);

                            listDataRow.Add(data);
                        }

                    }
                }
            }
            #endregion

            #region SQL
            string sql = string.Format(@"
WITH data_row AS(
    {0}
), origin_data AS(
    SELECT
        ss_wish.*
    FROM
        $ischool.course_selection.ss_wish AS ss_wish
        INNER JOIN data_row
            ON data_row.ref_student_id = ss_wish.ref_student_id
            AND data_row.ref_subject_id = ss_wish.ref_subject_id
), update_data AS(
    UPDATE $ischool.course_selection.ss_wish SET
        is_cancel = data_row.is_cancel
        , cancel_reason = data_row.cancel_reason
        , cancel_by = data_row.cancel_by
        , cancel_time = data_row.cancel_time
    FROM
        data_row
    WHERE
        $ischool.course_selection.ss_wish.ref_student_id = data_row.ref_student_id
        AND $ischool.course_selection.ss_wish.ref_subject_id = data_row.ref_subject_id
    RETURNING $ischool.course_selection.ss_wish.*
)
SELECT
    student.id
    , class.class_name
    , student.seat_no
    , student.name 
    , subject.subject_name
    , origin_data.sequence
    , origin_data.is_cancel AS origin_is_cancel
    , origin_data.cancel_reason AS origin_cancel_reason
    , update_data.is_cancel AS update_is_cancel
    , update_data.cancel_reason AS update_cancel_reason
FROM
    origin_data
    LEFT OUTER JOIN $ischool.course_selection.subject AS subject
        ON subject.uid = origin_data.ref_subject_id
    LEFT OUTER JOIN update_data
        ON update_data.ref_student_id = origin_data.ref_student_id
        AND update_data.ref_subject_id = origin_data.ref_subject_id
    LEFT OUTER JOIN student 
        ON student.id = origin_data.ref_student_id
    LEFT OUTER JOIN class
        ON class.id = student.ref_class_id
ORDER BY
    class.class_name
    , student.seat_no
    , origin_data.sequence
            ", string.Join(" UNION ALL ", listDataRow), this._userAccount, this._clientInfo, arg.SchoolYear, arg.Semester);

            #endregion

            try
            {
                bgw.ReportProgress(10);

                // 取回更新資料
                DataTable dt = this._qh.Select(sql);
                bgw.ReportProgress(50);

                Dictionary<string, Log> dicLogByStudentID = new Dictionary<string, Log>();

                #region 整理LOG紀錄
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        // 已學生為單位收集LOG資料
                        if (!dicLogByStudentID.ContainsKey("" + row["id"]))
                        {
                            dicLogByStudentID.Add("" + row["id"], new Log());
                            dicLogByStudentID["" + row["id"]].ListWishLog = new List<DataRow>();
                        }
                        dicLogByStudentID["" + row["id"]].ClassName = "" + row["class_name"];
                        dicLogByStudentID["" + row["id"]].SeatNo = "" + row["seat_no"];
                        dicLogByStudentID["" + row["id"]].StudentName = "" + row["name"];
                        dicLogByStudentID["" + row["id"]].ListWishLog.Add(row);
                    }
                }
                #endregion
                bgw.ReportProgress(60);

                StringBuilder description = new StringBuilder();
                description.AppendLine(string.Format("學年度{0} 學期{1} 「學生選課調整」: "
                    , arg.SchoolYear
                    , arg.Semester));

                foreach (string studentID in dicLogByStudentID.Keys)
                {
                    Log log = dicLogByStudentID[studentID];

                    description.AppendLine(string.Format("班級「{0}」座號「{1}」姓名「{2}」:  ", log.ClassName, log.SeatNo, log.StudentName));
                    foreach (DataRow row in dicLogByStudentID[studentID].ListWishLog)
                    {
                        description.AppendLine(string.Format("志願「{0}」 科目名稱「{1}」 取消「{2}」變更為「{3}」  取消原因「{4}」變更為「{5}」",
                            "" + row["sequence"], "" + row["subject_name"], "" + row["origin_is_cancel"], "" + row["update_is_cancel"]
                            , "" + row["origin_cancel_reason"], "" + row["update_cancel_reason"]));
                    }
                    description.AppendLine("");
                }

                #region SQL
                sql = string.Format(@"
INSERT INTO log(
    actor
    , action_type
    , action
    , target_category
    , server_time
    , client_info
    , action_by
    , description
)
VALUES(
    '{0}'
    , 'Record'
    , '選課志願調整'
    , 'ss_wish'
    , now()
    , '{1}'
    , '選課志願調整'
    , '{2}'
)                   ", this._userAccount, this._clientInfo, description.ToString());
                #endregion
                bgw.ReportProgress(70);

                this._up.Execute(sql);
                bgw.ReportProgress(100);

                arg.Success = true;
                arg.Message = "儲存成功!";

                e.Result = arg;
            }
            catch (Exception ex)
            {
                arg.Success = false;
                arg.Message = "儲存失敗:" + ex.Message;

                e.Result = arg;
            }
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BgwArgument arg = (BgwArgument)e.Result;

            if (arg.Success)
            {
                MsgBox.Show(arg.Message);
                ReloadDataGridView(arg.SchoolYear, arg.Semester, arg.Type);
            }
            else
            {
                MsgBox.Show(arg.Message);
            }

            #region Button
            {
                cbxSchoolYear.Enabled = true;
                cbxSemester.Enabled = true;
                cbxCoursePeriod.Enabled = true;
                btnUnOpen.Enabled = true;
                btnSelected.Enabled = true;
                btnBlock.Enabled = true;
                btnRecover.Enabled = true;
                btnLeave.Enabled = true;
                btnSave.Enabled = true;
                progressBarX1.Visible = false;
            }
            #endregion
        }

        private void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarX1.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// 儲存學生志願資料
        /// </summary>
        private class Wish
        {
            public string RefSubjectID { get; set; }
            public string Level { get; set; }
            public bool Disabled { get; set; }
            public bool IsCancel { get; set; }
            public string CancelReason { get; set; }
            public string CancelBy { get; set; }
            public bool IsBlock { get; set; }
            public string BlockReason { get; set; }
        }

        /// <summary>
        /// 儲存學生修課紀錄資料
        /// </summary>
        private class Attend
        {
            public string RefStudentID { get; set; }
            public string RefSubjectID { get; set; }
            public string SubjectName { get; set; }
            public string AttendType { get; set; }
            public bool IsLock { get; set; }
            public bool IsCrossType { get; set; }
        }

        /// <summary>
        /// 資料更新後資料，新增Log紀錄用
        /// </summary>
        private class Log
        {
            public string ClassName { get; set; }
            public string SeatNo { get; set; }
            public string StudentName { get; set; }
            public List<DataRow> ListWishLog { get; set; }
        }

        /// <summary>
        /// 背景執行序參數
        /// </summary>
        private class BgwArgument
        {
            public string SchoolYear { get; set; }
            public string Semester { get; set; }
            public string Type { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}
