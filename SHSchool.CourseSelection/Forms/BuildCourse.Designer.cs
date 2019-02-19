﻿namespace SHSchool.CourseSelection.Forms
{
    partial class BuildCourse
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.buildCourseBtn = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.schoolYearLb = new DevComponents.DotNetBar.LabelX();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.dataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.courseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.classType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.credit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.semesterLb = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.courseTypeLb = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // buildCourseBtn
            // 
            this.buildCourseBtn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buildCourseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buildCourseBtn.BackColor = System.Drawing.Color.Transparent;
            this.buildCourseBtn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buildCourseBtn.Location = new System.Drawing.Point(572, 435);
            this.buildCourseBtn.Name = "buildCourseBtn";
            this.buildCourseBtn.Size = new System.Drawing.Size(75, 24);
            this.buildCourseBtn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buildCourseBtn.TabIndex = 1;
            this.buildCourseBtn.Text = "開課";
            this.buildCourseBtn.Click += new System.EventHandler(this.buildCourseBtn_Click);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 11);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(47, 24);
            this.labelX1.TabIndex = 2;
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
            this.labelX2.Location = new System.Drawing.Point(138, 11);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(37, 24);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "學期";
            // 
            // schoolYearLb
            // 
            this.schoolYearLb.BackColor = System.Drawing.Color.Transparent;
            this.schoolYearLb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            // 
            // 
            // 
            this.schoolYearLb.BackgroundStyle.Class = "";
            this.schoolYearLb.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.schoolYearLb.ForeColor = System.Drawing.Color.Blue;
            this.schoolYearLb.Location = new System.Drawing.Point(65, 11);
            this.schoolYearLb.Name = "schoolYearLb";
            this.schoolYearLb.Size = new System.Drawing.Size(51, 24);
            this.schoolYearLb.TabIndex = 4;
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToDeleteRows = false;
            this.dataGridViewX1.AllowUserToResizeColumns = false;
            this.dataGridViewX1.AllowUserToResizeRows = false;
            this.dataGridViewX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataType,
            this.courseName,
            this.subjectName,
            this.level,
            this.classType,
            this.credit});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewX1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.dataGridViewX1.Location = new System.Drawing.Point(12, 42);
            this.dataGridViewX1.MinimumSize = new System.Drawing.Size(565, 320);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowHeadersVisible = false;
            this.dataGridViewX1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.Size = new System.Drawing.Size(635, 387);
            this.dataGridViewX1.TabIndex = 0;
            this.dataGridViewX1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellEndEdit);
            this.dataGridViewX1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellValueChanged);
            // 
            // dataType
            // 
            this.dataType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataType.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataType.HeaderText = "狀態";
            this.dataType.Name = "dataType";
            this.dataType.ReadOnly = true;
            this.dataType.Width = 59;
            // 
            // courseName
            // 
            this.courseName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.courseName.HeaderText = "課程名稱";
            this.courseName.Name = "courseName";
            this.courseName.ReadOnly = true;
            this.courseName.Width = 85;
            // 
            // subjectName
            // 
            this.subjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.subjectName.HeaderText = "科目名稱";
            this.subjectName.Name = "subjectName";
            this.subjectName.ReadOnly = true;
            this.subjectName.Width = 85;
            // 
            // level
            // 
            this.level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.level.DefaultCellStyle = dataGridViewCellStyle2;
            this.level.HeaderText = "級別";
            this.level.Name = "level";
            this.level.ReadOnly = true;
            this.level.Width = 59;
            // 
            // classType
            // 
            this.classType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.classType.DefaultCellStyle = dataGridViewCellStyle3;
            this.classType.HeaderText = "班別";
            this.classType.Name = "classType";
            this.classType.Width = 59;
            // 
            // credit
            // 
            this.credit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.credit.DefaultCellStyle = dataGridViewCellStyle4;
            this.credit.HeaderText = "學分數";
            this.credit.Name = "credit";
            this.credit.ReadOnly = true;
            this.credit.Width = 59;
            // 
            // semesterLb
            // 
            this.semesterLb.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.semesterLb.BackgroundStyle.Class = "";
            this.semesterLb.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.semesterLb.ForeColor = System.Drawing.Color.Blue;
            this.semesterLb.Location = new System.Drawing.Point(183, 11);
            this.semesterLb.Name = "semesterLb";
            this.semesterLb.Size = new System.Drawing.Size(78, 24);
            this.semesterLb.TabIndex = 5;
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(311, 12);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(61, 23);
            this.labelX3.TabIndex = 6;
            this.labelX3.Text = "課程時段";
            // 
            // courseTypeLb
            // 
            this.courseTypeLb.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.courseTypeLb.BackgroundStyle.Class = "";
            this.courseTypeLb.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.courseTypeLb.ForeColor = System.Drawing.Color.Blue;
            this.courseTypeLb.Location = new System.Drawing.Point(378, 12);
            this.courseTypeLb.Name = "courseTypeLb";
            this.courseTypeLb.Size = new System.Drawing.Size(136, 23);
            this.courseTypeLb.TabIndex = 7;
            // 
            // labelX4
            // 
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(12, 437);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(360, 35);
            this.labelX4.TabIndex = 8;
            this.labelX4.Text = "說明: 課程名稱命名規則為 課程時段+科目名稱+級別+班別\r\n也可透過修改班別來調整課程名稱。";
            // 
            // BuildCourse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 472);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.courseTypeLb);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.dataGridViewX1);
            this.Controls.Add(this.semesterLb);
            this.Controls.Add(this.buildCourseBtn);
            this.Controls.Add(this.schoolYearLb);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.labelX2);
            this.DoubleBuffered = true;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new System.Drawing.Size(660, 490);
            this.Name = "BuildCourse";
            this.Text = "建立課程班級";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevComponents.DotNetBar.ButtonX buildCourseBtn;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX schoolYearLb;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private DevComponents.DotNetBar.LabelX semesterLb;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX courseTypeLb;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn courseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn subjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn level;
        private System.Windows.Forms.DataGridViewTextBoxColumn classType;
        private System.Windows.Forms.DataGridViewTextBoxColumn credit;
        private DevComponents.DotNetBar.LabelX labelX4;
    }
}