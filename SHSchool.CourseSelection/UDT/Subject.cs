﻿using System;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    [FISCA.UDT.TableName("ischool.course_selection.subject")]
    public class Subject : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (Subject.AfterUpdate != null)
                Subject.AfterUpdate(null, EventArgs.Empty);
        }

        internal static event EventHandler AfterUpdate;
        /// <summary>
        /// 名稱
        /// </summary>
        [FISCA.UDT.Field(Field = "subject_name", Indexed = false)]
        public string SubjectName { get; set; }

        /// <summary>
        /// 級別
        /// </summary>
        [FISCA.UDT.Field(Field = "level", Indexed = false)]
        public int? Level { get; set; }

        /// <summary>
        /// 教學單位
        /// </summary>
        [FISCA.UDT.Field(Field = "Institute", Indexed = false)]
        public string Institute { get; set; }
        
        /// <summary>
        /// 學年度
        /// </summary>
        [FISCA.UDT.Field(Field = "school_year", Indexed = true)]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [FISCA.UDT.Field(Field = "semester", Indexed = true)]
        public int Semester { get; set; }

        /// <summary>
        /// 學分
        /// </summary>
        [FISCA.UDT.Field(Field = "credit", Indexed = false)]
        public int? Credit { get; set; }

        /// <summary>
        /// 課程類別
        /// </summary>
        [FISCA.UDT.Field(Field = "type", Indexed = false)]
        public string Type { get; set; }

        /// <summary>
        /// 修課人數上限
        /// </summary>
        [FISCA.UDT.Field(Field = "limit", Indexed = false)]
        public int Limit { get; set; }

        /// <summary>
        /// 教學目標
        /// </summary>
        [FISCA.UDT.Field(Field = "goal", Indexed = false)]
        public string Goal { get; set; }

        /// <summary>
        /// 教學內容
        /// </summary>
        [FISCA.UDT.Field(Field = "content", Indexed = false)]
        public string Content { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [FISCA.UDT.Field(Field = "memo", Indexed = false)]
        public string Memo { get; set; }

        /// <summary>
        /// 匯入課程編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_course_id", Indexed = false)]
        public int? RefCourseID { get; set; }

        // 2018/07/18 新增欄位

        /// <summary>
        /// 前導課程科目
        /// </summary>
        [FISCA.UDT.Field(Field = "pre_subject", Indexed = false)]
        public string PreSubject { get; set; }

        /// <summary>
        /// 前導課程級別
        /// </summary>
        [FISCA.UDT.Field(Field = "pre_subject_level", Indexed = false)]
        public int? PreSubjectLevel { get; set; }

        /// <summary>
        /// 前導課程採計方式
        /// </summary>
        [FISCA.UDT.Field(Field = "pre_subject_block_mode", Indexed = false)]
        public string PreSubjectBlockMode { get; set; }

        /// <summary>
        /// 重複修課採計方式
        /// </summary>
        [FISCA.UDT.Field(Field = "rejoin_block_mode", Indexed = false)]
        public string RejoinBlockMode { get; set; }

        /// <summary>
        /// 不開課
        /// </summary>
        [FISCA.UDT.Field(Field = "disabled", Indexed = false)]
        public bool Disabled { get; set; }

        /// <summary>
        /// 跨課程類別1
        /// </summary>
        [FISCA.UDT.Field(Field = "cross_type1", Indexed = false)]
        public string CrossType1 { get; set; }

        /// <summary>
        /// 跨課程類別2
        /// </summary>
        [FISCA.UDT.Field(Field = "cross_type2", Indexed = false)]
        public string CrossType2 { get; set; }
    }
}