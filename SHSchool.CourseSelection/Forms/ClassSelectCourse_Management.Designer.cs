namespace SHSchool.CourseSelection.Forms
{
    partial class ClassSelectCourse_Management
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.leaveBtn = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.schoolYearCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.setSelectSubjecttBtn = new DevComponents.DotNetBar.ButtonX();
            this.semesterCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.courseTypeCbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToResizeColumns = false;
            this.dataGridViewX1.AllowUserToResizeRows = false;
            this.dataGridViewX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.Column1,
            this.Column3,
            this.Column4});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(12, 43);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.ReadOnly = true;
            this.dataGridViewX1.RowHeadersVisible = false;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(667, 367);
            this.dataGridViewX1.TabIndex = 1;
            // 
            // leaveBtn
            // 
            this.leaveBtn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.leaveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.leaveBtn.BackColor = System.Drawing.Color.Transparent;
            this.leaveBtn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.leaveBtn.Location = new System.Drawing.Point(604, 416);
            this.leaveBtn.Name = "leaveBtn";
            this.leaveBtn.Size = new System.Drawing.Size(75, 23);
            this.leaveBtn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.leaveBtn.TabIndex = 1;
            this.leaveBtn.Text = "離開";
            this.leaveBtn.Click += new System.EventHandler(this.leaveBtn_Click);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 13);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(47, 23);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "學年度";
            // 
            // schoolYearCbx
            // 
            this.schoolYearCbx.DisplayMember = "Text";
            this.schoolYearCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.schoolYearCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.schoolYearCbx.FormattingEnabled = true;
            this.schoolYearCbx.ItemHeight = 19;
            this.schoolYearCbx.Location = new System.Drawing.Point(65, 12);
            this.schoolYearCbx.Name = "schoolYearCbx";
            this.schoolYearCbx.Size = new System.Drawing.Size(60, 25);
            this.schoolYearCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.schoolYearCbx.TabIndex = 2;
            this.schoolYearCbx.TextChanged += new System.EventHandler(this.schoolYearCbx_TextChanged);
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(142, 13);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(34, 23);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "學期";
            // 
            // setSelectSubjecttBtn
            // 
            this.setSelectSubjecttBtn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.setSelectSubjecttBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.setSelectSubjecttBtn.BackColor = System.Drawing.Color.Transparent;
            this.setSelectSubjecttBtn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.setSelectSubjecttBtn.Location = new System.Drawing.Point(12, 416);
            this.setSelectSubjecttBtn.Name = "setSelectSubjecttBtn";
            this.setSelectSubjecttBtn.Size = new System.Drawing.Size(121, 25);
            this.setSelectSubjecttBtn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.setSelectSubjecttBtn.TabIndex = 3;
            this.setSelectSubjecttBtn.Text = "設定班級可選科目";
            this.setSelectSubjecttBtn.Click += new System.EventHandler(this.setSelectSubjecttBtn_Click);
            // 
            // semesterCbx
            // 
            this.semesterCbx.DisplayMember = "Text";
            this.semesterCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.semesterCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.semesterCbx.FormattingEnabled = true;
            this.semesterCbx.ItemHeight = 19;
            this.semesterCbx.Location = new System.Drawing.Point(186, 12);
            this.semesterCbx.Name = "semesterCbx";
            this.semesterCbx.Size = new System.Drawing.Size(50, 25);
            this.semesterCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.semesterCbx.TabIndex = 4;
            this.semesterCbx.TextChanged += new System.EventHandler(this.semesterCbx_TextChanged);
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
            this.labelX3.Location = new System.Drawing.Point(479, 12);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(66, 25);
            this.labelX3.TabIndex = 5;
            this.labelX3.Text = "選課類別:";
            // 
            // courseTypeCbx
            // 
            this.courseTypeCbx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.courseTypeCbx.DisplayMember = "Text";
            this.courseTypeCbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.courseTypeCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.courseTypeCbx.FormattingEnabled = true;
            this.courseTypeCbx.ItemHeight = 19;
            this.courseTypeCbx.Location = new System.Drawing.Point(551, 12);
            this.courseTypeCbx.Name = "courseTypeCbx";
            this.courseTypeCbx.Size = new System.Drawing.Size(128, 25);
            this.courseTypeCbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.courseTypeCbx.TabIndex = 6;
            this.courseTypeCbx.TextChanged += new System.EventHandler(this.courseTypeCbx_TextChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.Frozen = true;
            this.dataGridViewTextBoxColumn1.HeaderText = "科目名稱";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn2.Frozen = true;
            this.dataGridViewTextBoxColumn2.HeaderText = "級別";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn3.Frozen = true;
            this.dataGridViewTextBoxColumn3.HeaderText = "學分數";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.HeaderText = "可選科目";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 500;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.Frozen = true;
            this.Column2.HeaderText = "年級";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 60;
            // 
            // Column1
            // 
            this.Column1.Frozen = true;
            this.Column1.HeaderText = "班級";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 60;
            // 
            // Column3
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column3.Frozen = true;
            this.Column3.HeaderText = "科目數";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 70;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column4.HeaderText = "可選科目";
            this.Column4.MinimumWidth = 500;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 500;
            // 
            // ClassSelectCourse_Management
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 451);
            this.Controls.Add(this.courseTypeCbx);
            this.Controls.Add(this.leaveBtn);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.dataGridViewX1);
            this.Controls.Add(this.semesterCbx);
            this.Controls.Add(this.setSelectSubjecttBtn);
            this.Controls.Add(this.schoolYearCbx);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(649, 490);
            this.Name = "ClassSelectCourse_Management";
            this.Text = "班級選課管理";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DevComponents.DotNetBar.ButtonX leaveBtn;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx schoolYearCbx;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.ButtonX setSelectSubjecttBtn;
        private DevComponents.DotNetBar.Controls.ComboBoxEx semesterCbx;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx courseTypeCbx;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}