﻿namespace SHSchool.CourseSelection.Forms
{
    partial class ManualDisClass
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.stuNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.className = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saetNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.attendNow = new SHSchool.CourseSelection.Forms.DataGridViewColorBallTextColumn();
            this.attendAfter = new SHSchool.CourseSelection.Forms.DataGridViewColorBallTextColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.autoDisClassBtn = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.schoolYearCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.semesterCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.subjectCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.saveBtn = new DevComponents.DotNetBar.ButtonX();
            this.closeBtn = new DevComponents.DotNetBar.ButtonX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.courseTypeCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.groupPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupPanel1
            // 
            this.groupPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.pictureBox1);
            this.groupPanel1.Controls.Add(this.dataGridViewX1);
            this.groupPanel1.Controls.Add(this.flowLayoutPanel1);
            this.groupPanel1.Location = new System.Drawing.Point(12, 41);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Padding = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.groupPanel1.Size = new System.Drawing.Size(750, 479);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.Class = "";
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.Class = "";
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.Class = "";
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 0;
            this.groupPanel1.Text = "學生分班";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = global::SHSchool.CourseSelection.Properties.Resources.loading;
            this.pictureBox1.Location = new System.Drawing.Point(354, 208);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(37, 37);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToResizeColumns = false;
            this.dataGridViewX1.AllowUserToResizeRows = false;
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.stuNumber,
            this.name,
            this.gender,
            this.className,
            this.saetNo,
            this.attendNow,
            this.attendAfter});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewX1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(2, 35);
            this.dataGridViewX1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.ReadOnly = true;
            this.dataGridViewX1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(740, 412);
            this.dataGridViewX1.TabIndex = 0;
            // 
            // stuNumber
            // 
            this.stuNumber.HeaderText = "學號";
            this.stuNumber.Name = "stuNumber";
            this.stuNumber.ReadOnly = true;
            this.stuNumber.Width = 59;
            // 
            // name
            // 
            this.name.HeaderText = "姓名";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Width = 59;
            // 
            // gender
            // 
            this.gender.HeaderText = "性別";
            this.gender.Name = "gender";
            this.gender.ReadOnly = true;
            this.gender.Width = 59;
            // 
            // className
            // 
            this.className.HeaderText = "班級";
            this.className.Name = "className";
            this.className.ReadOnly = true;
            this.className.Width = 59;
            // 
            // saetNo
            // 
            this.saetNo.HeaderText = "座號";
            this.saetNo.Name = "saetNo";
            this.saetNo.ReadOnly = true;
            this.saetNo.Width = 59;
            // 
            // attendNow
            // 
            this.attendNow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.attendNow.HeaderText = "原修課";
            this.attendNow.Name = "attendNow";
            this.attendNow.ReadOnly = true;
            this.attendNow.Visible = false;
            // 
            // attendAfter
            // 
            this.attendAfter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.attendAfter.HeaderText = "分班";
            this.attendAfter.Name = "attendAfter";
            this.attendAfter.ReadOnly = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 5);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(0, 30);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(740, 30);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // autoDisClassBtn
            // 
            this.autoDisClassBtn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.autoDisClassBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.autoDisClassBtn.BackColor = System.Drawing.Color.Transparent;
            this.autoDisClassBtn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.autoDisClassBtn.Location = new System.Drawing.Point(18, 526);
            this.autoDisClassBtn.Margin = new System.Windows.Forms.Padding(550, 3, 3, 3);
            this.autoDisClassBtn.Name = "autoDisClassBtn";
            this.autoDisClassBtn.Size = new System.Drawing.Size(75, 23);
            this.autoDisClassBtn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.autoDisClassBtn.TabIndex = 0;
            this.autoDisClassBtn.Text = "自動分班";
            this.autoDisClassBtn.Click += new System.EventHandler(this.autoDisClassBtn_Click);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(18, 10);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(47, 23);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "學年度";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(151, 10);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(34, 23);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "學期";
            // 
            // labelX3
            // 
            this.labelX3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(566, 10);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(35, 23);
            this.labelX3.TabIndex = 3;
            this.labelX3.Text = "科目";
            // 
            // schoolYearCbx
            // 
            this.schoolYearCbx.DisplayMember = "Text";
            this.schoolYearCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.schoolYearCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.schoolYearCbx.FormattingEnabled = true;
            this.schoolYearCbx.ItemHeight = 19;
            this.schoolYearCbx.Location = new System.Drawing.Point(71, 9);
            this.schoolYearCbx.Name = "schoolYearCbx";
            this.schoolYearCbx.Size = new System.Drawing.Size(60, 25);
            this.schoolYearCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.schoolYearCbx.TabIndex = 4;
            this.schoolYearCbx.SelectedIndexChanged += new System.EventHandler(this.schoolYearCbx_TextChanged);
            // 
            // semesterCbx
            // 
            this.semesterCbx.DisplayMember = "Text";
            this.semesterCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.semesterCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.semesterCbx.FormattingEnabled = true;
            this.semesterCbx.ItemHeight = 19;
            this.semesterCbx.Location = new System.Drawing.Point(192, 9);
            this.semesterCbx.Name = "semesterCbx";
            this.semesterCbx.Size = new System.Drawing.Size(50, 25);
            this.semesterCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.semesterCbx.TabIndex = 5;
            this.semesterCbx.SelectedIndexChanged += new System.EventHandler(this.semesterCbx_TextChanged);
            // 
            // subjectCbx
            // 
            this.subjectCbx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.subjectCbx.DisplayMember = "Text";
            this.subjectCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.subjectCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.subjectCbx.FormattingEnabled = true;
            this.subjectCbx.ItemHeight = 19;
            this.subjectCbx.Location = new System.Drawing.Point(606, 9);
            this.subjectCbx.Name = "subjectCbx";
            this.subjectCbx.Size = new System.Drawing.Size(156, 25);
            this.subjectCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.subjectCbx.TabIndex = 6;
            this.subjectCbx.SelectedIndexChanged += new System.EventHandler(this.subjectCbx_TextChanged);
            // 
            // saveBtn
            // 
            this.saveBtn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.BackColor = System.Drawing.Color.Transparent;
            this.saveBtn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.saveBtn.Location = new System.Drawing.Point(606, 526);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.saveBtn.TabIndex = 7;
            this.saveBtn.Text = "儲存";
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // closeBtn
            // 
            this.closeBtn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.BackColor = System.Drawing.Color.Transparent;
            this.closeBtn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.closeBtn.Location = new System.Drawing.Point(687, 526);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.closeBtn.TabIndex = 8;
            this.closeBtn.Text = "關閉";
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // labelX4
            // 
            this.labelX4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(360, 10);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(61, 23);
            this.labelX4.TabIndex = 9;
            this.labelX4.Text = "課程類別";
            // 
            // courseTypeCbx
            // 
            this.courseTypeCbx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.courseTypeCbx.DisplayMember = "Text";
            this.courseTypeCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.courseTypeCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.courseTypeCbx.FormattingEnabled = true;
            this.courseTypeCbx.ItemHeight = 19;
            this.courseTypeCbx.Location = new System.Drawing.Point(427, 9);
            this.courseTypeCbx.Name = "courseTypeCbx";
            this.courseTypeCbx.Size = new System.Drawing.Size(110, 25);
            this.courseTypeCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.courseTypeCbx.TabIndex = 10;
            this.courseTypeCbx.SelectedIndexChanged += new System.EventHandler(this.courseTypeCbx_TextChanged);
            // 
            // ManualDisClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 561);
            this.Controls.Add(this.autoDisClassBtn);
            this.Controls.Add(this.courseTypeCbx);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.subjectCbx);
            this.Controls.Add(this.semesterCbx);
            this.Controls.Add(this.schoolYearCbx);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.groupPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new System.Drawing.Size(680, 600);
            this.Name = "ManualDisClass";
            this.Text = "選課分班";
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx schoolYearCbx;
        private DevComponents.DotNetBar.Controls.ComboBoxEx semesterCbx;
        private DevComponents.DotNetBar.Controls.ComboBoxEx subjectCbx;
        private DevComponents.DotNetBar.ButtonX saveBtn;
        private DevComponents.DotNetBar.ButtonX closeBtn;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevComponents.DotNetBar.ButtonX autoDisClassBtn;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.ComboBoxEx courseTypeCbx;
        private System.Windows.Forms.DataGridViewTextBoxColumn stuNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn gender;
        private System.Windows.Forms.DataGridViewTextBoxColumn className;
        private System.Windows.Forms.DataGridViewTextBoxColumn saetNo;
        private DataGridViewColorBallTextColumn attendNow;
        private DataGridViewColorBallTextColumn attendAfter;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}