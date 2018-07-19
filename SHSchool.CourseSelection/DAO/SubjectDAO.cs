using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace SHSchool.CourseSelection.DAO
{
    class SubjectDAO
    {
        /// <summary>
        /// 學年度、學期科目資料
        /// </summary>
        private static Dictionary<string, UDT.Subject> dicSubject ;

        private static List<string> listTypeName;

        public SubjectDAO(string schoolYear,string semester)
        {
            listTypeName = new List<string>();

            AccessHelper access = new AccessHelper();
            List<UDT.Subject> listSubject = access.Select<UDT.Subject>(string.Format("school_year = {0} AND semester = {1}",schoolYear,semester));
            foreach (UDT.Subject subject in listSubject)
            {
                if (!listTypeName.Contains(subject.Type))
                {
                    listTypeName.Add(subject.Type);
                }
            }
        }

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

        /// <summary>
        /// 判斷課程類別是否存在系統(呼叫方法前請先建構函式)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckSubjectTypeInSystem(string type)
        {
            if (listTypeName.Contains(type) || type.Trim() == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
