using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevComponents.DotNetBar.Controls;
using System.ComponentModel;

namespace SHSchool.CourseSelection
{
    class Tool
    {
        private static Dictionary<string, UDT.Subject> dicSubject;

        //private static Tool _instance;

        //public static Tool GetInstance(string schoolYear,string semester)
        //{
        //    if (_instance == null)
        //    {
        //        _instance = new Tool(schoolYear, semester);
        //    }
        //    return _instance;
        //}

        public Tool(string schoolYear,string semester)
        {
            dicSubject = DAO.SubjectDAO.GetSubjectBySchoolYearSemester(schoolYear, semester);
        }
        
        /// <summary>
        /// 選修科目排序規則，順序: 課程類別、科目名稱、級別、學分數
        /// </summary>
        /// <param name="ob1"></param>
        /// <param name="ob2"></param>
        /// <returns></returns>
        public static int SubjectSortRule(UDT.Subject ob1, UDT.Subject ob2)
        {
            if (ob1.Type.CompareTo(ob2.Type) != 0)
            {
                return ob1.Type.CompareTo(ob2.Type);
            }
            else if (ob1.SubjectName.CompareTo(ob2.SubjectName) != 0)
            {
                return ob1.SubjectName.CompareTo(ob2.SubjectName);
            }
            else if (ob1.Level.ToString().CompareTo(ob2.Level.ToString()) != 0)
            {
                return ob1.Level.ToString().CompareTo(ob2.Level.ToString());
            }
            else if (ob1.Credit.ToString().CompareTo(ob2.Credit.ToString()) != 0)
            {
                return ob1.Credit.ToString().CompareTo(ob2.Credit.ToString());
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 級別轉換羅馬數字
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string RomanChar(string level)
        {
            switch (level)
            {
                case "1":
                    return "I";
                case "2":
                    return "II";
                case "3":
                    return "III";
                case "4":
                    return "IV";
                case "5":
                    return "V";
                case "6":
                    return "VI";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 透過科目編號回傳: "科目名稱 羅馬級別"
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        public static string SubjectNameAndLevel(string subjectID)
        {
            if (subjectID == "")
            {
                return "";
            }
            else
            {
                return string.Format("{0} {1}", dicSubject[subjectID].SubjectName, RomanChar("" + dicSubject[subjectID].Level));
            }
            
        }
    }
}
