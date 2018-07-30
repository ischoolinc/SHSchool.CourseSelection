using System;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    [FISCA.UDT.TableName("ischool.course_selection.opening_time")]
    public class OpeningTime : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (OpeningTime.AfterUpdate != null)
                OpeningTime.AfterUpdate(null, EventArgs.Empty);
        }

        internal static event EventHandler AfterUpdate;
        /// <summary>
        /// 開放選課開始時間
        /// </summary>
        [Obsolete]
        [FISCA.UDT.Field(Field = "start_time", Indexed = true)]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 開放選課結束時間
        /// </summary>
        [Obsolete]
        [FISCA.UDT.Field(Field = "end_time", Indexed = true)]
        public DateTime EndTime { get; set; }

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
        /// 選課說明
        /// </summary>
        [FISCA.UDT.Field(Field = "memo", Indexed = false)]
        public string Memo { get; set; }

        /// <summary>
        /// 選課模式(志願序 || 先搶先贏)
        /// </summary>
        [FISCA.UDT.Field(Field = "mode", Indexed = false)]
        public string Mode { get; set; }

        /// <summary>
        /// 開放選課的課程類別xml
        /// </summary>
        [FISCA.UDT.Field(Field = "open_type", Indexed = false)]
        public string OpenType { get; set; }

        /// <summary>
        /// P1 開放選課開始時間
        /// </summary>
        //[FISCA.UDT.Field(Field = "p1_start_time", Indexed = false)]
        //public DateTime P1StartTime { get; set; }

        /// <summary>
        /// P1 開放選課結束時間
        /// </summary>
        //[FISCA.UDT.Field(Field = "P1_end_time", Indexed = false)]
        //public DateTime P1EndTime { get; set; }

        /// <summary>
        /// P1 選課模式 (志願序 || 先搶先贏)
        /// </summary>
        //[FISCA.UDT.Field(Field = "p1_mode", Indexed = false)]
        //public string P1Mode { get; set; }

        /// <summary>
        /// P2 開放選課開始時間
        /// </summary>
        //[FISCA.UDT.Field(Field = "p2_start_time", Indexed = false)]
        //public DateTime P2StartTime { get; set; }

        /// <summary>
        /// P2 開放選課結束時間
        /// </summary>
        //[FISCA.UDT.Field(Field = "P2_end_time", Indexed = false)]
        //public DateTime P2EndTime { get; set; }

        /// <summary>
        /// P2 選課模式 (志願序 || 先搶先贏)
        /// </summary>
        //[FISCA.UDT.Field(Field = "p2_mode", Indexed = false)]
        //public string P2Mode { get; set; }
    }
}