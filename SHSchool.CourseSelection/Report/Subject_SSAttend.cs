﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Data;
using System.Windows.Forms;
using Aspose.Cells;
using System.IO;
using FISCA.UDT;

namespace SHSchool.CourseSelection.Report
{
    class Subject_SSAttend
    {
        private int CurrentSchoolYear;
        private int CurrentSemester;

        public Subject_SSAttend() { }

        private void InitCurrentSchoolYearSemester()
        {
            List<UDT.OpeningTime> openingTimes = (new AccessHelper()).Select<UDT.OpeningTime>();

            if (openingTimes == null || openingTimes.Count == 0)
            {
                throw new Exception("請先設定選課時間！");
            }

            this.CurrentSchoolYear = openingTimes[0].SchoolYear;
            this.CurrentSemester = openingTimes[0].Semester;
        }

        private string RomanChar(string level)
        {
            switch (level)
            {
                case "1":
                    return "Ⅰ";
                case "2":
                    return "Ⅱ";
                case "3":
                    return "Ⅲ";
                case "4":
                    return "Ⅳ";
                case "5":
                    return "Ⅴ";
                case "6":
                    return "Ⅵ";
                default:
                    return "";
            }
        }

        public void Execute()
        {
            try
            {
                InitCurrentSchoolYearSemester();

                string strSQL = string.Format(@"select class_name, seat_no, student_number, student.name as student_name, student_dept.name as student_dept_name, class_dept.name as class_dept_name, teacher.teacher_name, subject.subject_name, subject.level, subject.credit, subject.type from $ischool.course_selection.ss_attend as ss join student on student.id=ss.ref_student_id
join $ischool.course_selection.subject as subject on subject.uid=ss.ref_subject_id
left join class on class.id=student.ref_class_id
left join teacher on teacher.id=class.ref_teacher_id
left join dept as student_dept on student_dept.id=student.ref_dept_id
left join dept as class_dept on class_dept.id=class.ref_dept_id
where student.status in (1, 2) and subject.school_year={0} and subject.semester={1}
order by subject_name, level, student_dept_name, class_dept_name, class_name, seat_no, student_name", this.CurrentSchoolYear, this.CurrentSemester);

                DataTable dataTable = (new QueryHelper()).Select(strSQL);

                if (dataTable.Rows.Count == 0)
                {
                    throw new Exception("沒有資料！");
                }

                Workbook workbook = MakeReport(dataTable);
                if (workbook == null)
                {
                    throw new Exception("沒有資料！");
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "另存新檔";
                sfd.FileName = "科目選修學生名單.xls";
                sfd.Filter = "Excel 2003 相容檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                DialogResult dr = sfd.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    workbook.Save(sfd.FileName);
                    if (System.IO.File.Exists(sfd.FileName))
                        System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Workbook MakeReport(DataTable dataTable)
        {
            List<DataRow> lstSSAttends = dataTable.Rows.Cast<DataRow>().ToList();
            Workbook workbook = new Workbook();
            try
            {
                //  讀取樣版檔
                workbook.Open(new MemoryStream(Properties.Resources.科目選修學生名單樣版));
                //  讀取樣版工作表
                Worksheet templateSheet = workbook.Worksheets[0];
                Dictionary<string, List<DataRow>> dicSSAttends = new Dictionary<string, List<DataRow>>();
                lstSSAttends.ForEach((x) =>
                {
                    //  subject.subject_name, subject.level
                    if (!dicSSAttends.ContainsKey(x["subject_name"] + "" + RomanChar(x["level"] + "")))
                        dicSSAttends.Add(x["subject_name"] + "" + RomanChar(x["level"] + ""), new List<DataRow>());

                    dicSSAttends[x["subject_name"] + "" + RomanChar(x["level"] + "")].Add(x);
                });
                foreach (string key in dicSSAttends.Keys)
                {
                    //  Copy樣版工作表
                    int instanceSheetIndex = workbook.Worksheets.AddCopy(templateSheet.Name);
                    Worksheet instanceSheet = workbook.Worksheets[instanceSheetIndex];
                    IEnumerable<DataRow> filterSSAttends = dicSSAttends[key];
                    if (filterSSAttends.Count() == 0)
                        continue;
                    string subject_name = filterSSAttends.ElementAt(0)["subject_name"] + "" + RomanChar(filterSSAttends.ElementAt(0)["level"] + "");
                    instanceSheet.Name = subject_name;
                    string reportTitle = this.CurrentSchoolYear + "學年度第" + this.CurrentSemester + "學期【" + subject_name + "】選修學生名單";
                    instanceSheet.Cells[0, 0].PutValue(reportTitle);
                    instanceSheet.Cells[1, 0].PutValue("學分：" + (filterSSAttends.ElementAt(0)["credit"] + "") + "                " + "課程類別：" + (filterSSAttends.ElementAt(0)["type"] + ""));
                    instanceSheet.PageSetup.PrintTitleRows = "$1:$3";
                    //  填入學生選修科目資料：序號	科別			班級	座號	學號	姓名
                    int i = 3;
                    foreach (DataRow row in filterSSAttends)
                    {
                        //instanceSheet.Cells[i, 0].PutValue(row["seat_no"] + "");
                        instanceSheet.Cells[i, 1].PutValue(string.IsNullOrEmpty(row["student_dept_name"] + "") ? (row["class_dept_name"] + "") : (row["student_dept_name"] + ""));
                        instanceSheet.Cells[i, 4].PutValue(row["class_name"] + "");
                        instanceSheet.Cells[i, 5].PutValue(row["seat_no"] + "");
                        instanceSheet.Cells[i, 6].PutValue(row["student_number"] + "");
                        instanceSheet.Cells[i, 7].PutValue(row["student_name"] + "");

                        i++;
                    }
                    instanceSheet.Cells.DeleteRows(i, 302);
                }
                //  匯出資料
                workbook.Worksheets.RemoveAt(templateSheet.Name);
                return workbook;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
