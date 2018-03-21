using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    // UDT 選修科目課程
    [TableName("ischool.course_selection.subject_course")]

    public class SubjectCourse: ActiveRecord
    {
        /// <summary>
        /// 課程ID
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_course_id", Indexed = false)]
        public int? RefCourseID { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_subject_id", Indexed = false)]
        public int? RefSubjectID { get; set; }

        ///<summary>
        /// 班別
        ///</summary>
        [FISCA.UDT.Field(Field = "class_type", Indexed = false)]
        public string ClassType { get; set; }
    }
}