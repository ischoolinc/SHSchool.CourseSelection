﻿using System;
using System.Collections.Generic;
using System.Xml;
using FISCA.UDT;
using FISCA.Data;
using System.Data;

namespace SHSchool.CourseSelection.Export
{
    public partial class Subject_Export : EMBA.Export.ExportProxyForm
    {
        public Subject_Export()
        {
            InitializeComponent();

            this.Load += new EventHandler(Subject_Export_Load);
        }

        private void Subject_Export_Load(object sender, EventArgs e)
        {
            InitSchoolYear();
            InitSemester();

            InitializeData();
        }

        private void InitSchoolYear()
        {
            this.cboSchoolYear.Items.Clear();

            int school_year;

            if (!int.TryParse(K12.Data.School.DefaultSchoolYear, out school_year))
                school_year = DateTime.Today.Year - 1911;

            this.cboSchoolYear.Items.Add(school_year - 2);
            this.cboSchoolYear.Items.Add(school_year - 1);
            this.cboSchoolYear.Items.Add(school_year);
            this.cboSchoolYear.Items.Add(school_year + 1);
            this.cboSchoolYear.Items.Add(school_year + 2);

            this.cboSchoolYear.SelectedIndex = 2;
        }

        private void InitSemester()
        {
            this.cboSemester.Items.Clear();

            this.cboSemester.Items.Add("1");
            this.cboSemester.Items.Add("2");
            if (K12.Data.School.DefaultSemester == "1")
                this.cboSemester.SelectedIndex = 0;
            else
                this.cboSemester.SelectedIndex = 1;
        }

        private void InitializeData()
        {
            this.AutoSaveFile = true;
            this.AutoSaveLog = true;
            this.KeyField = "科目系統編號";
            this.InvisibleFields = null;
            this.ReplaceFields = null;
            this.Text = "匯出科目資料";
            this.Tag = this.Text;
            this.QuerySQL = SetQueryString();
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            this.QuerySQL = SetQueryString();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.QuerySQL = SetQueryString();
        }

        private string SetQueryString()
        {
            string school_year = string.IsNullOrEmpty(this.cboSchoolYear.Text) ? "0" : this.cboSchoolYear.Text;
            string semester = string.IsNullOrEmpty(this.cboSemester.Text) ? "0" : this.cboSemester.Text;
            string querySQL = string.Empty;

            querySQL = string.Format(@"
SELECT 
    uid as 科目系統編號
    , school_year as 學年度
    , semester as 學期
    , institute as 教學單位
    , subject_name as 科目名稱
    , level as 級別
    , credit as 學分數
    , type as 課程時段
    , ""limit"" as 修課人數上限
    , goal as 教學目標
    , content as 教學內容
    , memo as 備註 
    , pre_subject as 前導課程科目
    , pre_subject_level as 前導課程級別
    , pre_subject_block_mode as 前導課程採計方式
    , rejoin_block_mode as 重複修課採計方式
    , CASE 
        WHEN disabled = true THEN '是'
        ELSE '否'
        END as 不開課
    , cross_type1 as 跨課程時段1
    , cross_type2 as 跨課程時段2
FROM
    $ischool.course_selection.subject 
WHERE
    school_year='{0}' 
    AND semester='{1}' 
ORDER BY
    subject_name
    , level", school_year, semester);


            return querySQL;
        }

        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.QuerySQL = SetQueryString();
        }

        private void cboSemester_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            this.QuerySQL = SetQueryString();
        }
    }
}
