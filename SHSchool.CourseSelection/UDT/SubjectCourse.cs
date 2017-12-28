using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    // UDT 選修科目課程
    [TableName("ischool.course_selection.subjectcourse")]

    public class SubjectCourse: ActiveRecord
    {
        /// <summary>
        /// 課程ID
        /// </summary>
        [FISCA.UDT.Field(Field = "course_id", Indexed = false)]
        public int? CourseID { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        [FISCA.UDT.Field(Field = "subject_id", Indexed = false)]
        public int? SubjectID { get; set; }

        /// <summary>
        /// 課程名稱
        /// </summary>
        [FISCA.UDT.Field(Field = "course_name", Indexed = false)]
        public string CourseName { get; set; }

        /// <summary>
        /// 科目名稱
        /// </summary>
        [FISCA.UDT.Field(Field = "subject_name", Indexed = false)]
        public string SubjectName { get; set; }

        /// <summary>
        /// 學年
        /// </summary>
        [FISCA.UDT.Field(Field = "school_year", Indexed = false)]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [FISCA.UDT.Field(Field = "semester", Indexed = false)]
        public int Semester { get; set; }

        /// <summary>
        /// 級別
        /// </summary>
        [FISCA.UDT.Field(Field = "level", Indexed = false)]
        public int? Level { get; set; }

        /// <summary>
        /// 學分
        /// </summary>
        [FISCA.UDT.Field(Field = "credit", Indexed = false)]
        public int? Credit { get; set; }

        /// <summary>
        /// 分項類別
        /// </summary>
        [FISCA.UDT.Field(Field = "score_type", Indexed = false)]
        public string Sore_type { get; set; }

        ///<summary>
        /// 課程類別
        ///</summary>
        [FISCA.UDT.Field(Field = "course_type", Indexed =  false)]
        public string Course_type { get; set; }

        ///<summary>
        /// 班別
        ///</summary>
        [FISCA.UDT.Field(Field = "class_type", Indexed = false)]
        public string Class_type { get; set; }
    }
}