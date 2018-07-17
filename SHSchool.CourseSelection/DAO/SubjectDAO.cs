using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace SHSchool.CourseSelection.DAO
{
    class SubjectDAO
    {
        private static Dictionary<string, UDT.Subject> dicSubject ;

        /// <summary>
        /// 取得學年度學期科目資料
        /// </summary>
        /// <param name="schoolYear"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public static Dictionary<string, UDT.Subject> GetSubjectBySchoolYearSemester(string schoolYear ,string semester)
        {
            dicSubject = new Dictionary<string, UDT.Subject>();

            AccessHelper access = new AccessHelper();
            List<UDT.Subject> listSubject = access.Select<UDT.Subject>(string.Format("school_year = {0} AND semester = {1}",schoolYear,semester));

            foreach (UDT.Subject subject in listSubject)
            {
                if (!dicSubject.ContainsKey(subject.UID))
                {
                    dicSubject.Add(subject.UID,subject);
                }
            }

            return dicSubject;
        }
    }
}
