namespace SHSchool.CourseSelection.Forms
{
    partial class Sequence_distribution_Management
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.GradeCbox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.DepartmentCbox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.GroupCbox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.SchoolYearCbox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.SemesterCbox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.buttonX2 = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 64);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "科別:";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(187, 62);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "群組:";
            // 
            // GradeCbox
            // 
            this.GradeCbox.DisplayMember = "Text";
            this.GradeCbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.GradeCbox.FormattingEnabled = true;
            this.GradeCbox.ItemHeight = 19;
            this.GradeCbox.Location = new System.Drawing.Point(228, 16);
            this.GradeCbox.Name = "GradeCbox";
            this.GradeCbox.Size = new System.Drawing.Size(121, 25);
            this.GradeCbox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.GradeCbox.TabIndex = 2;
            // 
            // DepartmentCbox
            // 
            this.DepartmentCbox.DisplayMember = "Text";
            this.DepartmentCbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DepartmentCbox.FormattingEnabled = true;
            this.DepartmentCbox.ItemHeight = 19;
            this.DepartmentCbox.Location = new System.Drawing.Point(50, 62);
            this.DepartmentCbox.Name = "DepartmentCbox";
            this.DepartmentCbox.Size = new System.Drawing.Size(131, 25);
            this.DepartmentCbox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.DepartmentCbox.TabIndex = 3;
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.BackColor = System.Drawing.Color.Transparent;
            this.buttonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1.Location = new System.Drawing.Point(89, 119);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(75, 23);
            this.buttonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1.TabIndex = 4;
            this.buttonX1.Text = "分發";
            this.buttonX1.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(187, 16);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(35, 23);
            this.labelX3.TabIndex = 5;
            this.labelX3.Text = "年級:";
            // 
            // GroupCbox
            // 
            this.GroupCbox.DisplayMember = "Text";
            this.GroupCbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.GroupCbox.FormattingEnabled = true;
            this.GroupCbox.ItemHeight = 19;
            this.GroupCbox.Location = new System.Drawing.Point(228, 60);
            this.GroupCbox.Name = "GroupCbox";
            this.GroupCbox.Size = new System.Drawing.Size(121, 25);
            this.GroupCbox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.GroupCbox.TabIndex = 6;
            // 
            // labelX4
            // 
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(12, 16);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(45, 23);
            this.labelX4.TabIndex = 7;
            this.labelX4.Text = "學年:";
            // 
            // labelX5
            // 
            this.labelX5.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Location = new System.Drawing.Point(106, 16);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(45, 23);
            this.labelX5.TabIndex = 8;
            this.labelX5.Text = "學期:";
            // 
            // SchoolYearCbox
            // 
            this.SchoolYearCbox.DisplayMember = "Text";
            this.SchoolYearCbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.SchoolYearCbox.FormattingEnabled = true;
            this.SchoolYearCbox.ItemHeight = 19;
            this.SchoolYearCbox.Location = new System.Drawing.Point(50, 14);
            this.SchoolYearCbox.Name = "SchoolYearCbox";
            this.SchoolYearCbox.Size = new System.Drawing.Size(50, 25);
            this.SchoolYearCbox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.SchoolYearCbox.TabIndex = 9;
            this.SchoolYearCbox.SelectedIndexChanged += new System.EventHandler(this.SchoolYearCbox_SelectedIndexChanged);
            // 
            // SemesterCbox
            // 
            this.SemesterCbox.DisplayMember = "Text";
            this.SemesterCbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.SemesterCbox.FormattingEnabled = true;
            this.SemesterCbox.ItemHeight = 19;
            this.SemesterCbox.Location = new System.Drawing.Point(138, 14);
            this.SemesterCbox.Name = "SemesterCbox";
            this.SemesterCbox.Size = new System.Drawing.Size(43, 25);
            this.SemesterCbox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.SemesterCbox.TabIndex = 10;
            this.SemesterCbox.SelectedIndexChanged += new System.EventHandler(this.SemesterCbox_SelectedIndexChanged);
            // 
            // buttonX2
            // 
            this.buttonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX2.BackColor = System.Drawing.Color.Transparent;
            this.buttonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX2.Location = new System.Drawing.Point(203, 119);
            this.buttonX2.Name = "buttonX2";
            this.buttonX2.Size = new System.Drawing.Size(75, 23);
            this.buttonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX2.TabIndex = 11;
            this.buttonX2.Text = "取消";
            this.buttonX2.Click += new System.EventHandler(this.buttonX2_Click);
            // 
            // Sequence_distribution_Management
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 154);
            this.Controls.Add(this.buttonX2);
            this.Controls.Add(this.SemesterCbox);
            this.Controls.Add(this.SchoolYearCbox);
            this.Controls.Add(this.labelX5);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.GroupCbox);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.buttonX1);
            this.Controls.Add(this.DepartmentCbox);
            this.Controls.Add(this.GradeCbox);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(388, 193);
            this.MinimumSize = new System.Drawing.Size(388, 193);
            this.Name = "Sequence_distribution_Management";
            this.Text = "志願序分發作業";
            this.Load += new System.EventHandler(this.Sequence_distribution_Management_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx GradeCbox;
        private DevComponents.DotNetBar.Controls.ComboBoxEx DepartmentCbox;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx GroupCbox;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.Controls.ComboBoxEx SchoolYearCbox;
        private DevComponents.DotNetBar.Controls.ComboBoxEx SemesterCbox;
        private DevComponents.DotNetBar.ButtonX buttonX2;
    }
}