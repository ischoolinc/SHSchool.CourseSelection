namespace SHSchool.CourseSelection.Forms
{
    partial class frmSubject_Management
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
            this.Exit = new DevComponents.DotNetBar.ButtonX();
            this.dgvData = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.Delete = new DevComponents.DotNetBar.ButtonX();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.Addd = new DevComponents.DotNetBar.ButtonX();
            this.Update = new DevComponents.DotNetBar.ButtonX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.Add = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Institute = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.SubjectName = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Level = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Credit = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Type = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Limit = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.PreSubject = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.PreSubjectLevel = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.CrossType1 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.CrossType2 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // Exit
            // 
            this.Exit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Exit.BackColor = System.Drawing.Color.Transparent;
            this.Exit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Exit.Location = new System.Drawing.Point(898, 527);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(75, 23);
            this.Exit.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Exit.TabIndex = 5;
            this.Exit.Text = "離  開";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvData.BackgroundColor = System.Drawing.Color.White;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Add,
            this.Institute,
            this.SubjectName,
            this.Level,
            this.Credit,
            this.Type,
            this.Limit,
            this.PreSubject,
            this.PreSubjectLevel,
            this.CrossType1,
            this.CrossType2});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvData.Location = new System.Drawing.Point(12, 44);
            this.dgvData.MultiSelect = false;
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowTemplate.Height = 24;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(961, 477);
            this.dgvData.TabIndex = 6;
            this.dgvData.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellContentClick);
            this.dgvData.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellDoubleClick);
            this.dgvData.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvData_DataBindingComplete);
            // 
            // Delete
            // 
            this.Delete.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Delete.BackColor = System.Drawing.Color.Transparent;
            this.Delete.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Delete.Location = new System.Drawing.Point(817, 527);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(75, 23);
            this.Delete.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Delete.TabIndex = 4;
            this.Delete.Text = "刪  除";
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(65, 13);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(60, 25);
            this.cboSchoolYear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSchoolYear.TabIndex = 0;
            this.cboSchoolYear.SelectedIndexChanged += new System.EventHandler(this.cboSchoolYear_SelectedIndexChanged);
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(192, 13);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(50, 25);
            this.cboSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSemester.TabIndex = 1;
            this.cboSemester.SelectedIndexChanged += new System.EventHandler(this.cboSemester_SelectedIndexChanged);
            // 
            // Addd
            // 
            this.Addd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Addd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Addd.BackColor = System.Drawing.Color.Transparent;
            this.Addd.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Addd.Location = new System.Drawing.Point(655, 527);
            this.Addd.Name = "Addd";
            this.Addd.Size = new System.Drawing.Size(75, 23);
            this.Addd.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Addd.TabIndex = 2;
            this.Addd.Text = "新  增";
            this.Addd.Click += new System.EventHandler(this.Addd_Click);
            // 
            // Update
            // 
            this.Update.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Update.BackColor = System.Drawing.Color.Transparent;
            this.Update.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Update.Location = new System.Drawing.Point(736, 527);
            this.Update.Name = "Update";
            this.Update.Size = new System.Drawing.Size(75, 23);
            this.Update.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Update.TabIndex = 3;
            this.Update.Text = "修  改";
            this.Update.Click += new System.EventHandler(this.Update_Click);
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(12, 14);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(47, 23);
            this.labelX2.TabIndex = 36;
            this.labelX2.Text = "學年度";
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(152, 14);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(34, 23);
            this.labelX1.TabIndex = 37;
            this.labelX1.Text = "學期";
            // 
            // Add
            // 
            this.Add.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Add.Frozen = true;
            this.Add.HeaderText = "加入";
            this.Add.Name = "Add";
            this.Add.ReadOnly = true;
            this.Add.Text = "";
            this.Add.Width = 40;
            // 
            // Institute
            // 
            this.Institute.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Institute.Frozen = true;
            this.Institute.HeaderText = "教學單位";
            this.Institute.Name = "Institute";
            this.Institute.ReadOnly = true;
            this.Institute.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Institute.Width = 85;
            // 
            // SubjectName
            // 
            this.SubjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.SubjectName.Frozen = true;
            this.SubjectName.HeaderText = "科目名稱";
            this.SubjectName.Name = "SubjectName";
            this.SubjectName.ReadOnly = true;
            this.SubjectName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SubjectName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SubjectName.Width = 85;
            // 
            // Level
            // 
            this.Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Level.HeaderText = "級別";
            this.Level.Name = "Level";
            this.Level.ReadOnly = true;
            this.Level.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Level.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Level.Width = 59;
            // 
            // Credit
            // 
            this.Credit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Credit.HeaderText = "學分數";
            this.Credit.Name = "Credit";
            this.Credit.ReadOnly = true;
            this.Credit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Credit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Credit.Width = 72;
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Type.HeaderText = "課程類別";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Type.Width = 85;
            // 
            // Limit
            // 
            this.Limit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Limit.HeaderText = "修課人數上限";
            this.Limit.Name = "Limit";
            this.Limit.ReadOnly = true;
            this.Limit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Limit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Limit.Width = 111;
            // 
            // PreSubject
            // 
            this.PreSubject.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.PreSubject.HeaderText = "前導課程科目";
            this.PreSubject.Name = "PreSubject";
            this.PreSubject.ReadOnly = true;
            this.PreSubject.Width = 92;
            // 
            // PreSubjectLevel
            // 
            this.PreSubjectLevel.HeaderText = "前導課程級別";
            this.PreSubjectLevel.Name = "PreSubjectLevel";
            this.PreSubjectLevel.ReadOnly = true;
            // 
            // CrossType1
            // 
            this.CrossType1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.CrossType1.HeaderText = "跨課程類別1";
            this.CrossType1.Name = "CrossType1";
            this.CrossType1.ReadOnly = true;
            this.CrossType1.Width = 87;
            // 
            // CrossType2
            // 
            this.CrossType2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.CrossType2.HeaderText = "跨課程類別2";
            this.CrossType2.Name = "CrossType2";
            this.CrossType2.ReadOnly = true;
            this.CrossType2.Width = 87;
            // 
            // frmSubject_Management
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 562);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.Update);
            this.Controls.Add(this.Addd);
            this.Controls.Add(this.cboSchoolYear);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.Delete);
            this.DoubleBuffered = true;
            this.MaximizeBox = true;
            this.Name = "frmSubject_Management";
            this.Text = "科目管理";
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX Exit;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgvData;
        private DevComponents.DotNetBar.ButtonX Delete;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.ButtonX Addd;
        private DevComponents.DotNetBar.ButtonX Update;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.DataGridViewLinkColumn Add;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Institute;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn SubjectName;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Level;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Credit;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Type;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Limit;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn PreSubject;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn PreSubjectLevel;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn CrossType1;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn CrossType2;
    }
}