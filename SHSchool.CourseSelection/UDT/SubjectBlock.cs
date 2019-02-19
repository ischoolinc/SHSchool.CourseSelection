using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    [FISCA.UDT.TableName("ischool.course_selection.subject")]
    class SubjectBlock : ActiveRecord
    {
        /// <summary>
        /// 學生編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_student_id", Indexed = false)]
        public string RefStudentID { get; set; }
        
        /// <summary>
        /// 科目編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_subject_id", Indexed = false)]
        public string RefSubjectID { get; set; }

        /// <summary>
        /// 不能加選原因
        /// </summary>
        [FISCA.UDT.Field(Field = "reason", Indexed = false)]
        public string Reason { get; set; }
    }
}
