using System.Windows.Forms;
using FISCA;
using FISCA.UDT;
using FISCA.Presentation;
using FISCA.Permission;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SHSchool.CourseSelection
{
    public static class Program
    {
        [MainMethod("選課模組2.0")]
        static public void Main()
        {
            #region 模組啟用先同步Schema

            SchemaManager Manager = new SchemaManager(FISCA.Authentication.DSAServices.DefaultConnection);

            Manager.SyncSchema(new UDT.Identity());
            Manager.SyncSchema(new UDT.OpeningTime());
            Manager.SyncSchema(new UDT.SIRelation());
            Manager.SyncSchema(new UDT.SSAttend());
            Manager.SyncSchema(new UDT.Subject());
            Manager.SyncSchema(new UDT.SSWish());
            Manager.SyncSchema(new UDT.SSLog());

            //ServerModule.AutoManaged("https://module.ischool.com.tw/module/4923/shinmin.tc.edu.tw/udm.xml");
            //ServerModule.AutoManaged("http://module.ischool.com.tw/module/140/Course_Selection/udm.xml");
            #endregion

            #region 教務作業/設定與管理
            InitRibbonBar();
            #endregion
        }

        public static void InitRibbonBar()
        {
            #region 教務作業

            #region 選修科目設定

            Catalog button_SelectableSubject_Management = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_SelectableSubject_Management.Add(new RibbonFeature("Button_SelectableSubject_Management", "選修科目設定"));

            var vSelectableSubject_Management = MotherForm.RibbonBarItems["教務作業", "選課作業"]["設定"];
            vSelectableSubject_Management.Size = RibbonBarButton.MenuButtonSize.Large;
            vSelectableSubject_Management.Image = Properties.Resources.network_lock_64;
            vSelectableSubject_Management["選修科目"].Enable = UserAcl.Current["Button_SelectableSubject_Management"].Executable;
            vSelectableSubject_Management["選修科目"].Click += delegate
            {
                (new Forms.frmSelectableSubject_Management()).ShowDialog();
            };

            #endregion

            #region 選課時間設定

            Catalog button_OpeningTime_Management = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_OpeningTime_Management.Add(new RibbonFeature("Button_OpeningTime_Management", "選課時間設定"));

            var vOpeningTime_Management = MotherForm.RibbonBarItems["教務作業", "選課作業"]["設定"];
            vOpeningTime_Management.Size = RibbonBarButton.MenuButtonSize.Large;
            vOpeningTime_Management["選課時間"].Enable = UserAcl.Current["Button_OpeningTime_Management"].Executable;
            vOpeningTime_Management["選課時間"].Click += delegate
            {
                (new Forms.frmOpeningTime()).ShowDialog();
            };

            #endregion

            #region 學生選修科目設定

            Catalog button_SSAttend_Management = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_SSAttend_Management.Add(new RibbonFeature("Button_SSAttend_Management", "學生選修科目設定"));

            var vSSAttend_Management = MotherForm.RibbonBarItems["教務作業", "選課作業"]["設定"];
            vSSAttend_Management.Size = RibbonBarButton.MenuButtonSize.Large;
            vSSAttend_Management["學生選修科目"].Enable = UserAcl.Current["Button_SSAttend_Management"].Executable;
            vSSAttend_Management["學生選修科目"].Click += delegate
            {
                (new Forms.frmSSAttend_Management()).ShowDialog();
            };

            #endregion

            #region 科目管理

            Catalog button_Subject_Management = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_Subject_Management.Add(new RibbonFeature("Button_Subject_Management", "科目管理"));

            var vSubject_Management = MotherForm.RibbonBarItems["教務作業", "選課作業"]["管理"];
            vSubject_Management.Size = RibbonBarButton.MenuButtonSize.Large;
            vSubject_Management.Image = Properties.Resources.sandglass_unlock_64;
            vSubject_Management["科目"].Enable = UserAcl.Current["Button_Subject_Management"].Executable;
            vSubject_Management["科目"].Click += delegate
            {
                (new Forms.frmSubject_Management()).ShowDialog();
            };

            #endregion

            button_Subject_Management.Add(new RibbonFeature("Sequence_distribution_Management", "志願序分發作業"));

            vSubject_Management["志願序分發作業"].Enable = UserAcl.Current["Sequence_distribution_Management"].Executable;
            vSubject_Management["志願序分發作業"].Click += delegate
            {
                (new  Forms.Sequence_distribution_Management()).ShowDialog();
            };

            // 2017/10/11。羿均新增項目
            #region 選課課程設定
            button_Subject_Management.Add(new RibbonFeature("7140A14A-F26B-4DEA-8C87-D71A1FDCF6BE", "轉入修課學生"));

            vSubject_Management["轉入修課學生"].Enable = UserAcl.Current["7140A14A-F26B-4DEA-8C87-D71A1FDCF6BE"].Executable;
            vSubject_Management["轉入修課學生"].Click += delegate
            {
                (new Forms.CourseCorrespond()).ShowDialog();
            };

            #endregion

            #region 產生隨機選課志願的Code 備而不用
            //button_Subject_Management.Add(new RibbonFeature("Test_distribution_Management", "產生隨機選課志願"));

            //vSubject_Management["產生隨機選課志願"].Enable = UserAcl.Current["Test_distribution_Management"].Executable;
            //vSubject_Management["產生隨機選課志願"].Click += delegate
            //{
            //    List<SHSchool.CourseSelection.UDT.SSWish> Wish_list = new List<SHSchool.CourseSelection.UDT.SSWish>();


            //    AccessHelper AH = new AccessHelper();

            //    //Wish_list = AH.Select<SHSchool.CourseSelection.UDT.SSWish>("ref_student_id in(" + string.Join(",", CourseSelectedStuID_list.ToArray()) + ")");

            //    Wish_list = AH.Select<SHSchool.CourseSelection.UDT.SSWish>();

            //    //每次都把舊的刪光
            //    Wish_list.Clear();

            //    List<int> StuList = new List<int>();

            //    var Stu_list = K12.Data.Student.SelectAll();

            //    foreach (var Item in Stu_list)
            //    {
            //        if (Item.Class != null)
            //        {
            //            if (Item.Status == K12.Data.StudentRecord.StudentStatus.一般 && Item.Class.GradeYear == 3)
            //            {

            //                StuList.Add(int.Parse(Item.ID));

            //            }
            //        }

            //    }


            //    //int[] StuList = {885,887,888,889,890,1253,892,894,895,896};

            //    int[] CourseIDList = { 127489, 128645, 127491, 128642, 128646, 128634, 128633, 127490, 128644, 128643 };

            //    //int[] OrderList = { 1, 2, 3, 4, 5,6,7,8,9,10};

            //    List<int> OrderList = new List<int>();

            //    for (int i = 1; i <= 10; i++)
            //    {
            //        OrderList.Add(i);

            //    }

            //    Random rand = new Random();


            //    foreach (var StuID in StuList)
            //    {

            //        OrderList.Sort((x, y) => { return x.CompareTo(rand.Next(0, 10)); });


            //        int i = 0;

            //        foreach (var CouID in CourseIDList)
            //        {
            //            SHSchool.CourseSelection.UDT.SSWish NewWish = new UDT.SSWish();

            //            NewWish.StudentID = StuID;

            //            NewWish.SubjectID = CouID;



            //            NewWish.Order = OrderList[i];

            //            Wish_list.Add(NewWish);

            //            i++;
            //        }


            //    }


            //    Wish_list.SaveAll();

            //}; 
            #endregion


            #region 匯出科目

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"].Image = Properties.Resources.Export_Image;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_exportSubject = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_exportSubject.Add(new RibbonFeature("Button_Subject_Export", "匯出科目資料"));
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出科目資料"].Enable = UserAcl.Current["Button_Subject_Export"].Executable;

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出科目資料"].Click += delegate
            {
                (new Export.Subject_Export()).ShowDialog();
            };

            #endregion

            #region 匯出學生選修科目

            //Catalog button_exportSSAttend_Student = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            //button_exportSSAttend_Student.Add(new RibbonFeature("Button_SSAttend_Student_Export", "匯出學生選修科目"));
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出學生選修科目"].Enable = UserAcl.Current["Button_SSAttend_Student_Export"].Executable;

            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出學生選修科目"].Click += delegate
            //{
            //    (new Forms.frmSSAttend_Student()).ShowDialog();
            //};

            #endregion

            #region 匯出尚未選課學生

            Catalog button_exportSSAttend_Student_SelectNone = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_exportSSAttend_Student_SelectNone.Add(new RibbonFeature("Button_SSAttend_Student_SelectNone_Export", "匯出尚未選課學生"));
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出尚未選課學生"].Enable = UserAcl.Current["Button_SSAttend_Student_SelectNone_Export"].Executable;

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出尚未選課學生"].Click += delegate
            {
                (new Export.SSAttend_NoneSelectStudent_Export()).ShowDialog();
            };

            #endregion

            #region 匯出科目選課學生

            Catalog button_exportSSAttend_Subject = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_exportSSAttend_Subject.Add(new RibbonFeature("Button_SSAttend_Subject_Export", "匯出科目選課學生"));
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出科目選課學生"].Enable = UserAcl.Current["Button_SSAttend_Subject_Export"].Executable;

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出科目選課學生"].Click += delegate
            {
                (new Export.SSAttend_Student_Export()).ShowDialog();
            };

            #endregion

            #region 匯入科目

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯入"].Image = Properties.Resources.Import_Image;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_importSubject = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_importSubject.Add(new RibbonFeature("Button_Subject_Import", "匯入科目資料"));
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯入"]["匯入科目資料"].Enable = UserAcl.Current["Button_Subject_Import"].Executable;

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯入"]["匯入科目資料"].Click += delegate
            {
                (new Import.Subject_Import()).Execute();
            };

            #endregion

            #region 查詢科目選課學生

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["查詢"].Image = Properties.Resources.searchHistory;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["查詢"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_Query_SSAttend_Subject = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_Query_SSAttend_Subject.Add(new RibbonFeature("Button_SSAttend_Subject_Query", "查詢科目選課學生"));
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["查詢"]["科目選課學生"].Enable = UserAcl.Current["Button_SSAttend_Subject_Query"].Executable;

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["查詢"]["科目選課學生"].Click += delegate
            {
                (new Forms.frmSSAttend_Subject()).ShowDialog();
            };

            #endregion

            #region 列印班級學生選修科目清單

            MotherForm.RibbonBarItems["班級", "選課"]["報表"].Image = Properties.Resources.paste_64;
            MotherForm.RibbonBarItems["班級", "選課"]["報表"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_Report_SSAttend_Student = RoleAclSource.Instance["班級"]["功能按鈕"];
            button_Report_SSAttend_Student.Add(new RibbonFeature("Button_SSAttend_Student_Report", "列印班級學生選修科目清單"));
            MotherForm.RibbonBarItems["班級", "選課"]["報表"]["學生選修科目清單"].Enable = UserAcl.Current["Button_SSAttend_Student_Report"].Executable;

            MotherForm.RibbonBarItems["班級", "選課"]["報表"]["學生選修科目清單"].Click += delegate
            {
                (new Report.Class_SSAttend()).Execute();
            };

            #endregion

            #region 列印科目選修學生名單

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["報表"].Image = Properties.Resources.paste_64;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["報表"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_Report_SSAttend_Subject = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_Report_SSAttend_Subject.Add(new RibbonFeature("Button_SSAttend_Subject_Report", "列印科目選修學生名單"));
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["報表"]["科目選修學生名單"].Enable = UserAcl.Current["Button_SSAttend_Subject_Report"].Executable;

            MotherForm.RibbonBarItems["教務作業", "選課作業"]["報表"]["科目選修學生名單"].Click += delegate
            {
                (new Report.Subject_SSAttend()).Execute();
            };

            #endregion

            #endregion
        }
    }
}
