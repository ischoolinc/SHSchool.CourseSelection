using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Data;
using FISCA.UDT;

namespace SHSchool.CourseSelection.DAO
{
    class BlackListDAO
    {
        /// <summary>
        /// 透過學年度學期取得黑名單
        /// </summary>
        /// <param name="schoolYear"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public static DataTable GetBlackListBySchoolYearSemester(string schoolYear,string semester)
        {
            AccessHelper access = new AccessHelper();
            access.Select<UDT.SubjectBlock>();

            string sql = string.Format(@"
WITH target_subject_type AS(
    SELECT DISTINCT
        type
    FROM
        $ischool.course_selection.subject
    WHERE
        school_year = {0}
        AND semester = {1}
        AND type IS NOT NULL
) 
SELECT
    target_subject_type.type
    , count(subject_block.*)
FROM
    target_subject_type
    LEFT OUTER JOIN $ischool.course_selection.subject AS subject
        ON subject.type = target_subject_type.type 
    LEFT OUTER JOIN $ischool.course_selection.subject_block AS subject_block
        ON subject.uid = subject_block.ref_subject_id
 GROUP BY
    target_subject_type.type
                ",schoolYear,semester);

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql);

            return dt;
        }
    }
}
