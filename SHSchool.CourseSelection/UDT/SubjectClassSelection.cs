using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace SHSchool.CourseSelection.UDT
{
    // 2018/01/29 羿均 新增:班級選課管理用
    [TableName("ischool.course_selection.subject_class_selection")]

    public class SubjectClassSelection : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (SubjectClassSelection.AfterUpdate != null)
                SubjectClassSelection.AfterUpdate(null, EventArgs.Empty);
        }

        internal static event EventHandler AfterUpdate;

        /// <summary>
        /// 選課科目系統編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_subject_id", Indexed = false)]
        public int? RefSubjectID { get; set; }

        /// <summary>
        /// 班級系統編號
        /// </summary>
        [FISCA.UDT.Field(Field = "ref_class_id", Indexed = false)]
        public int? RefClassID { get; set; }

    }
}