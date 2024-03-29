﻿using System;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    [FISCA.UDT.TableName("ischool.course_selection.ss_attend")]
    public class SSAttend : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (SSAttend.AfterUpdate != null)
                SSAttend.AfterUpdate(null, EventArgs.Empty);
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
        /// 科目課程系統編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_subject_course_id", Indexed = true)]
        public int? SubjectCourseID { get; set; }

        /// <summary>
        /// 退選鎖定
        /// </summary>
        [FISCA.UDT.Field(Field = "lock", Indexed = false)]
        public bool Lock { get; set; }

        /// <summary>
        /// 修課類別[志願分發、先搶先贏、指定]
        /// </summary>
        [FISCA.UDT.Field(Field = "attend_type", Indexed = false)]
        public string AttendType { get; set; }
    }
}