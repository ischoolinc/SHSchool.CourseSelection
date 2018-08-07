namespace SHSchool.CourseSelection.Test
{
    partial class AutoSelectStuWish
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.schoolYearCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.semesterCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.subCountLb = new DevComponents.DotNetBar.LabelX();
            this.stuCountLb = new DevComponents.DotNetBar.LabelX();
            this.AutoSelStuWishBtn = new DevComponents.DotNetBar.ButtonX();
            this.deleteBtn = new DevComponents.DotNetBar.ButtonX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.courseTypeCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.SuspendLayout();
            // 
            // schoolYearCbx
            // 
            this.schoolYearCbx.DisplayMember = "Text";
            this.schoolYearCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.schoolYearCbx.FormattingEnabled = true;
            this.schoolYearCbx.ItemHeight = 19;
            this.schoolYearCbx.Location = new System.Drawing.Point(70, 10);
            this.schoolYearCbx.Name = "schoolYearCbx";
            this.schoolYearCbx.Size = new System.Drawing.Size(71, 25);
            this.schoolYearCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.schoolYearCbx.TabIndex = 0;
            this.schoolYearCbx.TextChanged += new System.EventHandler(this.schoolYearCbx_TextChanged);
            // 
            // semesterCbx
            // 
            this.semesterCbx.DisplayMember = "Text";
            this.semesterCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.semesterCbx.FormattingEnabled = true;
            this.semesterCbx.ItemHeight = 19;
            this.semesterCbx.Location = new System.Drawing.Point(210, 10);
            this.semesterCbx.Name = "semesterCbx";
            this.semesterCbx.Size = new System.Drawing.Size(50, 25);
            this.semesterCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.semesterCbx.TabIndex = 1;
            this.semesterCbx.TextChanged += new System.EventHandler(this.semesterCbx_TextChanged);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(52, 23);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "學年度:";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(165, 12);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(39, 23);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "學期:";
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(13, 58);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(75, 23);
            this.labelX3.TabIndex = 4;
            this.labelX3.Text = "選修科目數:";
            // 
            // labelX4
            // 
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(13, 98);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(111, 23);
            this.labelX4.TabIndex = 5;
            this.labelX4.Text = "學生人數(一般生):";
            // 
            // subCountLb
            // 
            this.subCountLb.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.subCountLb.BackgroundStyle.Class = "";
            this.subCountLb.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.subCountLb.Location = new System.Drawing.Point(88, 58);
            this.subCountLb.Name = "subCountLb";
            this.subCountLb.Size = new System.Drawing.Size(170, 23);
            this.subCountLb.TabIndex = 6;
            this.subCountLb.UseWaitCursor = true;
            // 
            // stuCountLb
            // 
            this.stuCountLb.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.stuCountLb.BackgroundStyle.Class = "";
            this.stuCountLb.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.stuCountLb.Location = new System.Drawing.Point(122, 98);
            this.stuCountLb.Name = "stuCountLb";
            this.stuCountLb.Size = new System.Drawing.Size(134, 23);
            this.stuCountLb.TabIndex = 7;
            // 
            // AutoSelStuWishBtn
            // 
            this.AutoSelStuWishBtn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.AutoSelStuWishBtn.BackColor = System.Drawing.Color.Transparent;
            this.AutoSelStuWishBtn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.AutoSelStuWishBtn.Location = new System.Drawing.Point(235, 136);
            this.AutoSelStuWishBtn.Name = "AutoSelStuWishBtn";
            this.AutoSelStuWishBtn.Size = new System.Drawing.Size(118, 23);
            this.AutoSelStuWishBtn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.AutoSelStuWishBtn.TabIndex = 8;
            this.AutoSelStuWishBtn.Text = "自動填入學生志願";
            this.AutoSelStuWishBtn.Click += new System.EventHandler(this.AutoSelStuWishBtn_Click);
            // 
            // deleteBtn
            // 
            this.deleteBtn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.deleteBtn.BackColor = System.Drawing.Color.Transparent;
            this.deleteBtn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.deleteBtn.Location = new System.Drawing.Point(359, 136);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(104, 23);
            this.deleteBtn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.deleteBtn.TabIndex = 10;
            this.deleteBtn.Text = "清空學生志願";
            this.deleteBtn.Click += new System.EventHandler(this.deleteBtn_Click);
            // 
            // labelX5
            // 
            this.labelX5.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Location = new System.Drawing.Point(280, 12);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(62, 23);
            this.labelX5.TabIndex = 11;
            this.labelX5.Text = "課程時段:";
            // 
            // courseTypeCbx
            // 
            this.courseTypeCbx.DisplayMember = "Text";
            this.courseTypeCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.courseTypeCbx.FormattingEnabled = true;
            this.courseTypeCbx.ItemHeight = 19;
            this.courseTypeCbx.Location = new System.Drawing.Point(348, 10);
            this.courseTypeCbx.Name = "courseTypeCbx";
            this.courseTypeCbx.Size = new System.Drawing.Size(115, 25);
            this.courseTypeCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.courseTypeCbx.TabIndex = 12;
            // 
            // AutoSelectStuWish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 171);
            this.Controls.Add(this.courseTypeCbx);
            this.Controls.Add(this.labelX5);
            this.Controls.Add(this.deleteBtn);
            this.Controls.Add(this.AutoSelStuWishBtn);
            this.Controls.Add(this.stuCountLb);
            this.Controls.Add(this.subCountLb);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.semesterCbx);
            this.Controls.Add(this.schoolYearCbx);
            this.DoubleBuffered = true;
            this.Name = "AutoSelectStuWish";
            this.Text = "(開發工具)自動填入學生選課志願";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx schoolYearCbx;
        private DevComponents.DotNetBar.Controls.ComboBoxEx semesterCbx;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX subCountLb;
        private DevComponents.DotNetBar.LabelX stuCountLb;
        private DevComponents.DotNetBar.ButtonX AutoSelStuWishBtn;
        private DevComponents.DotNetBar.ButtonX deleteBtn;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.Controls.ComboBoxEx courseTypeCbx;
    }
}