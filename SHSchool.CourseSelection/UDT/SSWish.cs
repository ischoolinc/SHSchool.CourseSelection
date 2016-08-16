using System;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    [FISCA.UDT.TableName("ischool.course_selection.ss_wish")]
    public class SSWish : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (SSWish.AfterUpdate != null)
                SSWish.AfterUpdate(null, EventArgs.Empty);
        }

        internal static event EventHandler AfterUpdate;
        /// <summary>
        /// 學生系統編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_student_id", Indexed = true)]
        public int StudentID { get; set; }

        /// <summary>
        /// 科目系統編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_subject_id", Indexed = true)]
        public int SubjectID { get; set; }

        /// <summary>
        /// 順位
        /// </summary>
        [FISCA.UDT.Field(Field = "sequence", Indexed = true)]
        public int Order { get; set; }
    }
}