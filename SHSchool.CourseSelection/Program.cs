using FISCA;
using FISCA.Permission;
using FISCA.Presentation;

namespace SHSchool.CourseSelection
{
    public static class Program
    {
        [MainMethod("選課模組2.0")]
        static public void Main()
        {
            #region 教務作業
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"].Image = Properties.Resources.Export_Image;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯入"].Image = Properties.Resources.Import_Image;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"].Image = Properties.Resources.sandglass_unlock_64;
            
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課結果及分發"].Size = RibbonBarButton.MenuButtonSize.Large;
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課結果及分發"].Image = Properties.Resources.sandglass_unlock_64;
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["開課/分班"].Size = RibbonBarButton.MenuButtonSize.Large;
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["開課/分班"].Image = Properties.Resources.sandglass_unlock_64;

            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["查詢"].Image = Properties.Resources.searchHistory;
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["查詢"].Size = RibbonBarButton.MenuButtonSize.Large;
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["報表"].Image = Properties.Resources.paste_64;
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["報表"].Size = RibbonBarButton.MenuButtonSize.Large;

            #region 選修科目管理 
            {
                var pcode = "Button_Subject_Management";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "科目管理"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["選修科目管理"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["選修科目管理"].Click += delegate
                {
                    (new Forms.frmSubject_Management()).ShowDialog();
                };
            }
            #endregion

            #region 匯出科目資料
            {
                var pcode = "Button_Subject_Export";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "匯出科目資料"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出科目資料"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出科目資料"].Click += delegate
                {
                    (new Export.Subject_Export()).ShowDialog();
                };
            }
            #endregion

            #region 匯入科目資料
            {
                var pcode = "Button_Subject_Import";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "匯入科目資料"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯入"]["匯入科目資料"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯入"]["匯入科目資料"].Click += delegate
                {
                    (new Import.Subject_Import()).Execute();
                };
            }
            #endregion

            #region 班級選課管理 
            {
                var pcode = "Button_SelectableSubject_Management";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "選修科目設定"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["班級選課管理"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["班級選課管理"].Click += delegate
                {
                    // 2018/01/26 羿均 新民選課--班級選課管理
                    (new Forms.ClassSelectCourse_Management()).ShowDialog();
                };
            }
            #endregion

            #region 產生擋修名單
            {
                var pcode = "Button_Subject_BlackList";
                RoleAclSource.Instance["教學作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "產生擋修名單"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["產生擋修名單"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["產生擋修名單"].Click += delegate
                {
                    (new Forms.BlackListForm()).ShowDialog();
                };
            }
            #endregion

            #region 選課設定檢查報表
            {
                var pcode = "E5264203-8CA3-45C0-BF98-ED2278FEF522";
                RoleAclSource.Instance["教學作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "選課設定檢查報表"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["選課設定檢查報表"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["選課設定檢查報表"].Click += delegate
                {
                    (new Forms.SettingExamineReport()).ShowDialog();
                };
            }
            #endregion

            #region 開放選課時間 
            {
                var pcode = "Button_OpeningTime_Management";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "開放選課時間"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["開放選課時間"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課管理"]["開放選課時間"].Click += delegate
                {
                    (new Forms.frmOpeningTime()).ShowDialog();
                };
            }
            #endregion

            #region 選課結果及分發 
            {
                var pcode = "Button_SSAttend_Management";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "選課互動分發"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課結果及分發"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課結果及分發"].Click += delegate
                {
                    (new Forms.AdjustSSAttendForm()).ShowDialog();
                };
            }
            #endregion

            #region 志願調整
            {
                string pcode = "A383A889-8F0D-4EDA-AA18-CE7DD5BE6BB6";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "選課志願調整"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課志願調整"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["選課志願調整"].Click += delegate
                {
                    (new Forms.frmWishEditor()).ShowDialog();
                };
            }
            #endregion

            #region 選修科目開課 
            {
                var pcode = "BE73708F-4721-4BAE-860C-6FFDCE77B08C";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "選修科目開課"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["開課/分班"]["選修科目開課"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["開課/分班"]["選修科目開課"].Click += delegate
                {
                    (new Forms.BuildCourseClass()).ShowDialog();
                };
            }
            #endregion

            #region 選修科目分班 
            {
                var pcode = "A8BBDA37-5FB3-4353-B232-DA2B67F6DC59";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "選修科目分班"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["開課/分班"]["選修科目分班"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["開課/分班"]["選修科目分班"].Click += delegate
                {
                    (new Forms.ManualDisClass()).ShowDialog();
                };
            }
            #endregion

            #region 轉入修課學生
            {
                var pcode = "7140A14A-F26B-4DEA-8C87-D71A1FDCF6BE";
                RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "轉入修課學生"));

                MotherForm.RibbonBarItems["教務作業", "選課作業"]["開課/分班"]["轉入修課學生"].Enable = UserAcl.Current[pcode].Executable;
                MotherForm.RibbonBarItems["教務作業", "選課作業"]["開課/分班"]["轉入修課學生"].Click += delegate
                {
                    (new Forms.TurnIntoCourseStudent()).ShowDialog();
                };
            }
            #endregion

            #region 查詢科目選課學生
            {
                //var pcode = "Button_SSAttend_Subject_Query";
                //RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "查詢科目選課學生"));

                //MotherForm.RibbonBarItems["教務作業", "選課作業"]["查詢"]["科目選課學生"].Enable = UserAcl.Current[pcode].Executable;
                //MotherForm.RibbonBarItems["教務作業", "選課作業"]["查詢"]["科目選課學生"].Click += delegate
                //{
                //    (new Forms.frmSSAttend_Subject()).ShowDialog();
                //};
            }
            #endregion

            #region 列印科目選修學生名單
            {
                //var pcode = "Button_SSAttend_Subject_Report";
                //RoleAclSource.Instance["教務作業"]["功能按鈕"]["選課作業"].Add(new RibbonFeature(pcode, "列印科目選修學生名單"));

                //MotherForm.RibbonBarItems["教務作業", "選課作業"]["報表"]["科目選修學生名單"].Enable = UserAcl.Current[pcode].Executable;
                //MotherForm.RibbonBarItems["教務作業", "選課作業"]["報表"]["科目選修學生名單"].Click += delegate
                //{
                //    (new Report.Subject_SSAttend()).Execute();
                //};
            }
            #endregion

            #region 列印班級學生選修科目清單
            {
                var pcode = "Button_SSAttend_Student_Report";
                RoleAclSource.Instance["班級"]["功能按鈕"].Add(new RibbonFeature(pcode, "學生選修科目清單"));

                //MotherForm.RibbonBarItems["班級", "選課"]["報表"].Image = Properties.Resources.paste_64;
                //MotherForm.RibbonBarItems["班級", "選課"]["報表"].Size = RibbonBarButton.MenuButtonSize.Large;

                K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["選課相關報表"]["學生選修科目清單"].Enable = UserAcl.Current[pcode].Executable;
                K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["選課相關報表"]["學生選修科目清單"].Click += delegate
                {
                    (new Report.Class_SSAttend()).Execute();
                };
            }
            #endregion

            #region 開發工具-自動填入學生選課志願
            //RibbonBarButton mb = MotherForm.RibbonBarItems["教務作業", "開發工具"]["產生學生選課志願"];
            //mb.Size = RibbonBarButton.MenuButtonSize.Medium;
            //mb.Image = Properties.Resources.Import_Image;

            //mb.Click += delegate 
            //{
            //    Test.AutoSelectStuWish at = new Test.AutoSelectStuWish();
            //    at.ShowDialog();
            //};

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

            #region  志願序分發作業 
            // 2018/03/15 選課2.5 hide舊式分發作業

            //button_Subject_Management.Add(new RibbonFeature("Sequence_distribution_Management", "志願序分發作業"));
            //vSubject_Management["志願分發作業"].Enable = UserAcl.Current["Sequence_distribution_Management"].Executable;
            //vSubject_Management["志願分發作業"].Click += delegate
            //{
            //    (new Forms.Sequence_distribution_Management()).ShowDialog();
            //};

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

            //Catalog button_exportSSAttend_Student_SelectNone = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            //button_exportSSAttend_Student_SelectNone.Add(new RibbonFeature("Button_SSAttend_Student_SelectNone_Export", "匯出尚未選課學生"));
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出尚未選課學生"].Enable = UserAcl.Current["Button_SSAttend_Student_SelectNone_Export"].Executable;

            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出尚未選課學生"].Click += delegate
            //{
            //    (new Export.SSAttend_NoneSelectStudent_Export()).ShowDialog();
            //};

            #endregion

            #region 匯出科目選課學生

            //Catalog button_exportSSAttend_Subject = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            //button_exportSSAttend_Subject.Add(new RibbonFeature("Button_SSAttend_Subject_Export", "匯出科目選課學生"));
            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出科目選課學生"].Enable = UserAcl.Current["Button_SSAttend_Subject_Export"].Executable;

            //MotherForm.RibbonBarItems["教務作業", "選課作業"]["匯出"]["匯出科目選課學生"].Click += delegate
            //{
            //    (new Export.SSAttend_Student_Export()).ShowDialog();
            //};

            #endregion

            #endregion
        }

    }
}
