using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using DevComponents.Editors;
using FISCA.Data;
using SHSchool.Data;
using SHSchool.CourseSelection.UDT;

namespace SHSchool.CourseSelection.Forms
{
    //2016/8/17 穎驊開始動工，選課志願分發


    public partial class Sequence_distribution_Management : BaseForm
    {
        //全校學生List
        List<SHStudentRecord> TotalStu_list = new List<SHStudentRecord>();

        //有選課資格的學生ID List
        List<String> CourseSelectedStuID_list = new List<String>();

        //全志願序List
        List<SSWish> Wish_list = new List<SSWish>();

        // Key =   學生ID，Value 其志願， 此為存放同一Group的志願 之Dict
        Dictionary<int, List<SSWish>> Wish_Dict = new Dictionary<int, List<SSWish>>();

        //科別名稱轉科別ID Dict
        Dictionary<String, String> DeptName_to_DetpID = new Dictionary<string, string>();

        //整理在該群組內所有的科目ID清單
        Dictionary<String, List<String>> SubjectGroup_AllSubjectIDs = new Dictionary<string, List<String>>();

        //整理例外沒有選課學生的例外
        List<String> No_Select_Student_list = new List<string>();

        //紀錄在同一Group 可以複數選課科目的數量上限
        Dictionary<String, int> This_groupID_Count_Limit = new Dictionary<String, int>();

        //學生身分的類別ID
        String identity_ID;

        //取得在該分發學年學期期有的學期科目ID，避免選到錯誤的學期科目
        List<String> This_SchoolYearSemester_SubjectIDs = new List<String>();

        //Key = 科目ID ，Value = 人數上限， 此為用來記錄單科目人數上限 之Dict
        Dictionary<String, int> SubjectID_StudentLimit = new Dictionary<string, int>();

        //Key = 科目ID ，Value = 人數上限， 此為用來記錄目前單科目人數 之Dict
        Dictionary<String, int> SubjectID_StudentCounter = new Dictionary<string, int>();


        //Key = 學生ID ，Value 某學生已經選的課，此Dict用來記錄，避免學生選到同樣ID的科目
        Dictionary<String, List<String>> SubjectID_Already_Attend = new Dictionary<string, List<String>>();

        public Sequence_distribution_Management()
        {
            InitializeComponent();

        }

        //預載
        private void Sequence_distribution_Management_Load(object sender, EventArgs e)
        {
            //設定學年學期
            SetSchoolYearSemester();

        }

        private void SetSchoolYearSemester()
        {
            this.SchoolYearCbox.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear) + 1);
            this.SchoolYearCbox.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear));
            this.SchoolYearCbox.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear) - 1);

            if (K12.Data.School.DefaultSemester == "1")
            {
                this.SemesterCbox.Items.Add(int.Parse(K12.Data.School.DefaultSemester));
                this.SemesterCbox.Items.Add(int.Parse(K12.Data.School.DefaultSemester) + 1);

            }

            if (K12.Data.School.DefaultSemester == "2")
            {
                this.SemesterCbox.Items.Add(int.Parse(K12.Data.School.DefaultSemester));
                this.SemesterCbox.Items.Add(int.Parse(K12.Data.School.DefaultSemester) - 1);

            }

            //學年預設為第1項，為學校系統當下的預設學年
            this.SchoolYearCbox.SelectedIndex = 1;

            //預設為第0項，當下的學期
            this.SemesterCbox.SelectedIndex = 0;

        }


        //設定其他項目的Cbox
        private void setCbox()
        {
            //把前次的資料清空
            #region 刪除舊設定資料
            DeptName_to_DetpID.Clear();
            SubjectID_StudentLimit.Clear();
            This_SchoolYearSemester_SubjectIDs.Clear();
            SubjectID_StudentCounter.Clear();

            this.GradeCbox.Items.Clear();
            this.DepartmentCbox.Items.Clear();
            this.GroupCbox.Items.Clear();
            #endregion

            //取得所有科別
            List<SHDepartmentRecord> Dept_list = new List<SHDepartmentRecord>();

            Dept_list = SHSchool.Data.SHDepartment.SelectAll();

            AccessHelper AH = new AccessHelper();

            var SI_Subject_list = AH.Select<SIRelation>();

            var Subject_list = AH.Select<Subject>();

            List<String> Subject_Group_List = new List<string>();

            this.GradeCbox.Items.Add("1");
            this.GradeCbox.Items.Add("2");
            this.GradeCbox.Items.Add("3");


            #region 整理這學年學期有的科目ID
            foreach (var Item in Subject_list)
            {

                if ("" + Item.SchoolYear == this.SchoolYearCbox.Text && "" + Item.Semester == this.SemesterCbox.Text)
                {
                    //取得這學期有的科目ID
                    if (!This_SchoolYearSemester_SubjectIDs.Contains(Item.UID))
                    {
                        This_SchoolYearSemester_SubjectIDs.Add(Item.UID);
                    }

                    //取得該科目ID修課的學生上限
                    if (!SubjectID_StudentLimit.ContainsKey(Item.UID))
                    {
                        SubjectID_StudentLimit.Add(Item.UID, Item.Limit);
                    }

                    //設定本學期每一個科目的起始必帶指定學生人數，起始預設為0，後續還會先讀Attend再增加
                    if (!SubjectID_StudentCounter.ContainsKey(Item.UID))
                    {
                        SubjectID_StudentCounter.Add(Item.UID, 0);
                    }
                }
            }
            #endregion


            #region 建立科別名稱與ID的轉換
            foreach (var Item in Dept_list)
            {
                this.DepartmentCbox.Items.Add(Item.FullName);

                if (!DeptName_to_DetpID.ContainsKey(Item.FullName))
                {
                    DeptName_to_DetpID.Add(Item.FullName, Item.ID);
                }
            }
            #endregion

            foreach (var Item in SI_Subject_list)
            {
                if (This_SchoolYearSemester_SubjectIDs.Contains("" + Item.SubjectID))
                {
                    //整理Group 清單
                    if (!Subject_Group_List.Contains(Item.Group))
                    {
                        Subject_Group_List.Add(Item.Group);
                    }

                    //整理一Group群組內所有的科目ID
                    if (!SubjectGroup_AllSubjectIDs.ContainsKey(Item.Group))
                    {
                        SubjectGroup_AllSubjectIDs.Add(Item.Group, new List<String>());

                        if (!SubjectGroup_AllSubjectIDs[Item.Group].Contains("" + Item.SubjectID))
                        {
                            SubjectGroup_AllSubjectIDs[Item.Group].Add("" + Item.SubjectID);
                        }
                    }
                    else
                    {
                        if (!SubjectGroup_AllSubjectIDs[Item.Group].Contains("" + Item.SubjectID))
                        {
                            SubjectGroup_AllSubjectIDs[Item.Group].Add("" + Item.SubjectID);
                        }

                    }
                }
            }

            //設定Group群組
            foreach (var Item in Subject_Group_List)
            {
                if (Item != "")
                {
                    this.GroupCbox.Items.Add(Item);
                }
            }

            if (This_SchoolYearSemester_SubjectIDs.Count == 0)
            {
                MsgBox.Show("本學年度沒有任何課程可供選課");

            }
        }


        //分發
        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            if (GradeCbox.Text == "" || DepartmentCbox.Text == "" || GroupCbox.Text == "")
            {
                MsgBox.Show("請選擇年級、科別、群組");
                this.Enabled = true;
                return;
            }

            #region 整理選課學生List
            SHSchool.Data.SHClass.SelectAll();

            TotalStu_list = SHSchool.Data.SHStudent.SelectAll();

            foreach (var StuRec in TotalStu_list)
            {

                //穎驊筆記，這邊有個小眉角，不要呼叫StuRec.Class.Department，因為恩正去檢查lib後發現他沒有建立快取，走這條路線會慢一百倍以上(不誇張)
                if (StuRec != null && StuRec.Class != null && StuRec.Class.GradeYear != null && StuRec.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    if ((StuRec.Class.GradeYear == int.Parse(GradeCbox.Text)) && StuRec.DepartmentID == DeptName_to_DetpID[DepartmentCbox.Text])
                    {
                        CourseSelectedStuID_list.Add(StuRec.ID);

                    }
                }

            }
            #endregion


            // 清除舊的志願資料
            Wish_Dict.Clear();

            AccessHelper AH = new AccessHelper();

            //取得所有有選課資格學生的志願序
            Wish_list = AH.Select<SHSchool.CourseSelection.UDT.SSWish>("ref_student_id in(" + string.Join(",", CourseSelectedStuID_list.ToArray()) + ")");

            var SIRelation_list = AH.Select<SIRelation>();

            var Identity_list = AH.Select<Identity>();

            // 用來存放同一group的志願List
            List<SSWish> one_group_wishes = new List<SSWish>();

            Random rand = new Random();

            if (Wish_list.Count == 0)
            {
                MsgBox.Show("目前沒有任何學生在本學年學期有選課志願，請確認選課時間是否未開放。");
                this.Enabled = true;

                return;
            }


            #region 取得身分類別identity
            foreach (var Item in Identity_list)
            {
                if ("" + Item.GradeYear == GradeCbox.Text && "" + Item.DeptID == DeptName_to_DetpID[DepartmentCbox.Text])
                {
                    identity_ID = Item.UID;

                }
            }
            #endregion


            foreach (var Item in SIRelation_list)
            {
                if ("" + Item.IdentityID == identity_ID && !This_groupID_Count_Limit.ContainsKey(Item.Group) && This_SchoolYearSemester_SubjectIDs.Contains("" + Item.SubjectID))
                {
                    This_groupID_Count_Limit.Add(Item.Group, Item.CountLimit);
                }
            }

            #region 將所有志願的學生依ID整理存到Dict，並處理排序
            foreach (var StuID in CourseSelectedStuID_list)
            {
                one_group_wishes.Clear();

                foreach (var wish in Wish_list)
                {
                    if ("" + wish.StudentID == StuID && SubjectGroup_AllSubjectIDs[this.GroupCbox.Text].Contains("" + wish.SubjectID) && This_SchoolYearSemester_SubjectIDs.Contains("" + wish.SubjectID))
                    {
                        one_group_wishes.Add(wish);
                    }
                }


                //排志願序 在同一Group 志願序要按原志願序編排，如原本在同一志願序有7、2、8 志願序，必須排成2、7、8
                one_group_wishes = one_group_wishes.OrderByDescending(x => -x.Order).ToList();

                //.Sort((x, y) => { return x.CompareTo(y.Order); });

                if (!Wish_Dict.ContainsKey(int.Parse(StuID)))
                {
                    Wish_Dict.Add(int.Parse(StuID), new List<SSWish>());
                }


                foreach (var wish in one_group_wishes)
                {
                    Wish_Dict[int.Parse(StuID)].Add(wish);
                }
            }
            #endregion

            if (Wish_Dict.Count == 0)
            {
                MsgBox.Show("沒有任何學生在此群組有選課志願");
                this.Enabled = true;

                return;
            }


            //重要!!
            //重要!!
            //重要!!
            //因為很重要所以要打三行，下面這行程式碼是本次選課志願分發中的核心
            //藉由改變字典每一位學生的排位順序，達成隨機分配志願序抽籤選課的公平性，另外每比完一次志願，就會再重打亂一次，
            //避免贏者恆贏，輸者恆輸的狀況(我的第n志願永遠贏你的第n志願)

            // 穎驊筆記，感覺這邊還不是真亂數，要請教恩正，
            Wish_Dict = Wish_Dict.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value);


            int MaximumWishCount;

            MaximumWishCount = 0;

            #region 整理出此Group中學生最多志願序數量
            foreach (var StuWish in Wish_Dict)
            {
                if (StuWish.Value.Count > MaximumWishCount)
                {
                    MaximumWishCount = StuWish.Value.Count;
                }
            }
            #endregion


            //此學生ID在此Group 可以選擇項目的上限
            Dictionary<int, int> StudentID_In_This_Group_Count_Limit = new Dictionary<int, int>();


            //查詢本學期科目既有的選課結果，就算是在志願分發之前，可能就會有部分學生已經有選課結果了(EX: 必帶、特殊學生)
            var Attend_List = AH.Select<SSAttend>("ref_subject_id in(" + string.Join(",", This_SchoolYearSemester_SubjectIDs.ToArray()) + ")");

            //如果某一學生在某一Group 已有選課紀錄，要幫他在該領域的選課數量+1，避免他多選到課目
            foreach (var Item in Attend_List)
            {
                if (!StudentID_In_This_Group_Count_Limit.ContainsKey(Item.StudentID))
                {
                    if (SubjectGroup_AllSubjectIDs[this.GroupCbox.Text].Contains("" + Item.SubjectID) && This_SchoolYearSemester_SubjectIDs.Contains("" + Item.SubjectID))
                    {
                        StudentID_In_This_Group_Count_Limit.Add(Item.StudentID, 0);

                        StudentID_In_This_Group_Count_Limit[Item.StudentID]++;
                    }

                }
                else
                {
                    if (SubjectGroup_AllSubjectIDs[this.GroupCbox.Text].Contains("" + Item.SubjectID) && This_SchoolYearSemester_SubjectIDs.Contains("" + Item.SubjectID))
                    {
                        StudentID_In_This_Group_Count_Limit[Item.StudentID]++;
                    }
                }
            }

            //假如選課紀錄已經有學生含在某一科目，自動加總該科目以選擇人數，避免後續分發課堂人數爆量
            foreach (var Item in Attend_List)
            {
                if (This_SchoolYearSemester_SubjectIDs.Contains("" + Item.SubjectID))
                {
                    if (!SubjectID_StudentCounter.ContainsKey("" + Item.SubjectID))
                    {
                        SubjectID_StudentCounter.Add("" + Item.SubjectID, 0);

                        SubjectID_StudentCounter["" + Item.SubjectID]++;
                    }
                    else
                    {
                        SubjectID_StudentCounter["" + Item.SubjectID]++;
                    }
                }
            }


            // 整理每一位學生已經有選課紀錄的課程ID，後續才能處理，避免學生選到重覆的科目
            foreach (var Item in Attend_List)
            {
                if (This_SchoolYearSemester_SubjectIDs.Contains("" + Item.SubjectID) && SubjectGroup_AllSubjectIDs[this.GroupCbox.Text].Contains("" + Item.SubjectID))
                {
                    if (!SubjectID_Already_Attend.ContainsKey("" + Item.StudentID))
                    {
                        SubjectID_Already_Attend.Add("" + Item.StudentID, new List<string>());
                        SubjectID_Already_Attend["" + Item.StudentID].Add("" + Item.SubjectID);

                    }
                    else
                    {
                        SubjectID_Already_Attend["" + Item.StudentID].Add("" + Item.SubjectID);
                    }

                }
            }


            // 所有人從第一志願開始篩選
            for (int i = 1; i <= MaximumWishCount; i++)
            {
                foreach (var StuWish in Wish_Dict)
                {
                    // 排除掉完全沒有選課的學生
                    if (StuWish.Value.Count != 0 && StuWish.Value.Count > i - 1)
                    {

                        //在該志願序有志願
                        if (StuWish.Value[i - 1] != null)
                        {
                            // 檢查此志願的學生，使是否已經有同科目ID的選課紀錄
                            if (SubjectID_Already_Attend.ContainsKey("" + StuWish.Key))
                            {
                                if (SubjectID_Already_Attend["" + StuWish.Key].Contains("" + StuWish.Value[i - 1].SubjectID))
                                {
                                    continue;
                                }
                            }


                            //假如尚未建立，建立該學生在此群組的選課數量統計
                            if (!StudentID_In_This_Group_Count_Limit.ContainsKey(StuWish.Key))
                            {
                                StudentID_In_This_Group_Count_Limit.Add(StuWish.Key, 0);
                            }

                            // 該學生在該群組尚未選滿
                            if (StudentID_In_This_Group_Count_Limit[StuWish.Key] < This_groupID_Count_Limit[this.GroupCbox.Text])
                            {

                                // 該科目總人數尚未滿額
                                if (SubjectID_StudentCounter["" + StuWish.Value[i - 1].SubjectID] < SubjectID_StudentLimit["" + StuWish.Value[i - 1].SubjectID])
                                {
                                    //新增選課紀錄
                                    SSAttend attend = new SSAttend();

                                    attend.StudentID = StuWish.Key;

                                    attend.SubjectID = StuWish.Value[i - 1].SubjectID;

                                    // 將該選課結果鎖定
                                    attend.Lock = true;

                                    Attend_List.Add(attend);

                                    //學生在該群組的選的科目數+1
                                    StudentID_In_This_Group_Count_Limit[StuWish.Key]++;
                                    // 該科目人數+1
                                    SubjectID_StudentCounter["" + StuWish.Value[i - 1].SubjectID]++;

                                }
                            }
                        }
                    }
                    else
                    {
                        //2016/8/22，穎驊筆記
                        //例外處理紀錄文字，目前沒有用到，備而不用
                        No_Select_Student_list.Add("學生系統編號:" + StuWish.Key + "在此類別" + GroupCbox.Text + "沒有任何選課志願");
                    }
                }

                //重要!!
                //重要!!
                //重要!!
                //因為很重要所以要打三行，下面這行程式碼是本次選課志願分發中的核心
                //每輪完一次志願，就把每個人彼此之間的次序洗牌，重新公平抽籤掌握優先權，才不會出現贏者恆贏的不公平狀況(我的第n志願永遠贏你的第n志願)

                Wish_Dict = Wish_Dict.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value);

            }

            //儲存選課結果
            Attend_List.SaveAll();
            this.Enabled = true;
            MessageBox.Show("志願分發完成。");

        }

        private void SchoolYearCbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //學年Cbox改變時，假如學期Cbox有值才進行後續的setCbox();
            if (this.SemesterCbox.Text != "")
            {
                setCbox();
            }
        }

        private void SemesterCbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //學期Cbox改變時，假如學年Cbox有值才進行後續的setCbox();
            if (this.SchoolYearCbox.Text != "")
            {
                setCbox();
            }
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
