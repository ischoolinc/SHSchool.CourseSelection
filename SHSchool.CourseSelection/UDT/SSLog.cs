using System;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    [FISCA.UDT.TableName("ischool.course_selection.ss_log")]
    public class SSLog : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (SSLog.AfterUpdate != null)
                SSLog.AfterUpdate(null, EventArgs.Empty);
        }

        internal static event EventHandler AfterUpdate;
        /// <summary>
        /// 學生系統編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_student_id", Indexed = true)]
        public int StudentID { get; set; }

        /// <summary>
        /// 開放選課學年度
        /// </summary>
        [FISCA.UDT.Field(Field = "school_year", Indexed = true)]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 開放選課學期
        /// </summary>
        [FISCA.UDT.Field(Field = "semester", Indexed = true)]
        public int Semester { get; set; }

        /// <summary>
        /// 時間
        /// </summary>
        [FISCA.UDT.Field(Field = "event_time", Indexed = false)]
        public DateTime EventTime { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        [FISCA.UDT.Field(Field = "ip_address", Indexed = false)]
        public string IPAddress { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        [FISCA.UDT.Field(Field = "action", Indexed = false)]
        public string Action { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [FISCA.UDT.Field(Field = "content", Indexed = false)]
        public string Content { get; set; }
    }
}