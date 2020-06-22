namespace SHSchool.CourseSelection.Forms
{
    partial class frmWishEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.cbxSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cbxSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cbxCoursePeriod = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.btnUnOpen = new DevComponents.DotNetBar.ButtonX();
            this.btnSignDisabled = new DevComponents.DotNetBar.ButtonItem();
            this.btnCancelDisabled = new DevComponents.DotNetBar.ButtonItem();
            this.btnSelected = new DevComponents.DotNetBar.ButtonX();
            this.btnSignSelected = new DevComponents.DotNetBar.ButtonItem();
            this.btnCancelSelected = new DevComponents.DotNetBar.ButtonItem();
            this.btnBlock = new DevComponents.DotNetBar.ButtonX();
            this.btnSignBlock = new DevComponents.DotNetBar.ButtonItem();
            this.btnCancelBlock = new DevComponents.DotNetBar.ButtonItem();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnLeave = new DevComponents.DotNetBar.ButtonX();
            this.btnRecover = new DevComponents.DotNetBar.ButtonX();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressBarX1 = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.lb4 = new DevComponents.DotNetBar.LabelX();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
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
            this.labelX1.Location = new System.Drawing.Point(12, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(45, 23);
            this.labelX1.TabIndex = 0;
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
            this.labelX2.Location = new System.Drawing.Point(129, 12);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(35, 23);
            this.labelX2.TabIndex = 1;
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
            this.labelX3.Location = new System.Drawing.Point(671, 12);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(60, 23);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "課程時段";
            // 
            // cbxSchoolYear
            // 
            this.cbxSchoolYear.DisplayMember = "Text";
            this.cbxSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSchoolYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSchoolYear.FormattingEnabled = true;
            this.cbxSchoolYear.ItemHeight = 19;
            this.cbxSchoolYear.Location = new System.Drawing.Point(63, 11);
            this.cbxSchoolYear.Name = "cbxSchoolYear";
            this.cbxSchoolYear.Size = new System.Drawing.Size(60, 25);
            this.cbxSchoolYear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbxSchoolYear.TabIndex = 4;
            this.cbxSchoolYear.SelectedIndexChanged += new System.EventHandler(this.cbxSchoolYear_SelectedIndexChanged);
            // 
            // cbxSemester
            // 
            this.cbxSemester.DisplayMember = "Text";
            this.cbxSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSemester.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSemester.FormattingEnabled = true;
            this.cbxSemester.ItemHeight = 19;
            this.cbxSemester.Location = new System.Drawing.Point(170, 11);
            this.cbxSemester.Name = "cbxSemester";
            this.cbxSemester.Size = new System.Drawing.Size(55, 25);
            this.cbxSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbxSemester.TabIndex = 5;
            this.cbxSemester.SelectedIndexChanged += new System.EventHandler(this.cbxSemester_SelectedIndexChanged);
            // 
            // cbxCoursePeriod
            // 
            this.cbxCoursePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxCoursePeriod.DisplayMember = "Text";
            this.cbxCoursePeriod.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxCoursePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCoursePeriod.FormattingEnabled = true;
            this.cbxCoursePeriod.ItemHeight = 19;
            this.cbxCoursePeriod.Location = new System.Drawing.Point(737, 11);
            this.cbxCoursePeriod.Name = "cbxCoursePeriod";
            this.cbxCoursePeriod.Size = new System.Drawing.Size(140, 25);
            this.cbxCoursePeriod.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbxCoursePeriod.TabIndex = 6;
            this.cbxCoursePeriod.SelectedIndexChanged += new System.EventHandler(this.cbxCoursePeriod_SelectedIndexChanged);
            // 
            // btnUnOpen
            // 
            this.btnUnOpen.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnUnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUnOpen.BackColor = System.Drawing.Color.Transparent;
            this.btnUnOpen.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnUnOpen.Location = new System.Drawing.Point(12, 531);
            this.btnUnOpen.Name = "btnUnOpen";
            this.btnUnOpen.Size = new System.Drawing.Size(100, 23);
            this.btnUnOpen.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnUnOpen.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnSignDisabled,
            this.btnCancelDisabled});
            this.btnUnOpen.TabIndex = 9;
            this.btnUnOpen.Text = "不開課課程";
            // 
            // btnSignDisabled
            // 
            this.btnSignDisabled.Name = "btnSignDisabled";
            this.btnSignDisabled.Text = "標示不開課課程";
            this.btnSignDisabled.Click += new System.EventHandler(this.btnSignDisabled_Click);
            // 
            // btnCancelDisabled
            // 
            this.btnCancelDisabled.Name = "btnCancelDisabled";
            this.btnCancelDisabled.Text = "清除不開課課程";
            this.btnCancelDisabled.Click += new System.EventHandler(this.btnCancelDisabled_Click);
            // 
            // btnSelected
            // 
            this.btnSelected.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelected.BackColor = System.Drawing.Color.Transparent;
            this.btnSelected.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSelected.Location = new System.Drawing.Point(118, 531);
            this.btnSelected.Name = "btnSelected";
            this.btnSelected.Size = new System.Drawing.Size(100, 23);
            this.btnSelected.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSelected.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnSignSelected,
            this.btnCancelSelected});
            this.btnSelected.TabIndex = 10;
            this.btnSelected.Text = "已選課程";
            // 
            // btnSignSelected
            // 
            this.btnSignSelected.Name = "btnSignSelected";
            this.btnSignSelected.Text = "標示已選課程";
            this.btnSignSelected.Click += new System.EventHandler(this.btnSignSelected_Click);
            // 
            // btnCancelSelected
            // 
            this.btnCancelSelected.Name = "btnCancelSelected";
            this.btnCancelSelected.Text = "清除已選課程";
            this.btnCancelSelected.Click += new System.EventHandler(this.btnCancelSelected_Click);
            // 
            // btnBlock
            // 
            this.btnBlock.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBlock.BackColor = System.Drawing.Color.Transparent;
            this.btnBlock.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnBlock.Location = new System.Drawing.Point(224, 531);
            this.btnBlock.Name = "btnBlock";
            this.btnBlock.Size = new System.Drawing.Size(100, 23);
            this.btnBlock.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnBlock.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnSignBlock,
            this.btnCancelBlock});
            this.btnBlock.TabIndex = 11;
            this.btnBlock.Text = "黑名單";
            // 
            // btnSignBlock
            // 
            this.btnSignBlock.Name = "btnSignBlock";
            this.btnSignBlock.Text = "標示黑名單課程";
            this.btnSignBlock.Click += new System.EventHandler(this.btnSignBlock_Click);
            // 
            // btnCancelBlock
            // 
            this.btnCancelBlock.Name = "btnCancelBlock";
            this.btnCancelBlock.Text = "清除黑名單課程";
            this.btnCancelBlock.Click += new System.EventHandler(this.btnCancelBlock_Click);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(721, 531);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLeave
            // 
            this.btnLeave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnLeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeave.BackColor = System.Drawing.Color.Transparent;
            this.btnLeave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnLeave.Location = new System.Drawing.Point(802, 531);
            this.btnLeave.Name = "btnLeave";
            this.btnLeave.Size = new System.Drawing.Size(75, 23);
            this.btnLeave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnLeave.TabIndex = 13;
            this.btnLeave.Text = "離開";
            this.btnLeave.Click += new System.EventHandler(this.btnLeave_Click);
            // 
            // btnRecover
            // 
            this.btnRecover.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRecover.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRecover.BackColor = System.Drawing.Color.Transparent;
            this.btnRecover.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRecover.Location = new System.Drawing.Point(330, 531);
            this.btnRecover.Name = "btnRecover";
            this.btnRecover.Size = new System.Drawing.Size(100, 23);
            this.btnRecover.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnRecover.TabIndex = 14;
            this.btnRecover.Text = "復原取消志願";
            this.btnRecover.Click += new System.EventHandler(this.btnRecover_Click);
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
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column11});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(12, 41);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.ReadOnly = true;
            this.dataGridViewX1.RowHeadersVisible = false;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.Size = new System.Drawing.Size(865, 484);
            this.dataGridViewX1.TabIndex = 8;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "班級";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column2.HeaderText = "座號";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 59;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "姓名";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "志願一";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 120;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "志願二";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 120;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "志願三";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Width = 120;
            // 
            // Column10
            // 
            this.Column10.HeaderText = "志願四";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.Width = 120;
            // 
            // Column11
            // 
            this.Column11.HeaderText = "志願五";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            this.Column11.Width = 120;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn1.HeaderText = "班級";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn2.HeaderText = "座號";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 60;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn3.HeaderText = "姓名";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "鎖定";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 60;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn5.HeaderText = "選修課程";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn6.HeaderText = "志願一";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn7.HeaderText = "志願二";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn8.HeaderText = "志願三";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn9.HeaderText = "志願四";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn10.HeaderText = "志願五";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.HeaderText = "分發志願";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.Width = 90;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn12.HeaderText = "修課方式";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn13.HeaderText = "驗證訊息";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            // 
            // progressBarX1
            // 
            this.progressBarX1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBarX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.progressBarX1.BackgroundStyle.Class = "";
            this.progressBarX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.progressBarX1.Location = new System.Drawing.Point(436, 531);
            this.progressBarX1.Name = "progressBarX1";
            this.progressBarX1.Size = new System.Drawing.Size(279, 23);
            this.progressBarX1.TabIndex = 15;
            this.progressBarX1.Text = "progressBarX1";
            this.progressBarX1.Visible = false;
            // 
            // lb4
            // 
            this.lb4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lb4.BackgroundStyle.Class = "";
            this.lb4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lb4.ForeColor = System.Drawing.Color.Black;
            this.lb4.Location = new System.Drawing.Point(407, 272);
            this.lb4.Name = "lb4";
            this.lb4.Size = new System.Drawing.Size(75, 23);
            this.lb4.TabIndex = 16;
            this.lb4.Text = "isLoading";
            this.lb4.Visible = false;
            // 
            // frmWishEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 566);
            this.Controls.Add(this.lb4);
            this.Controls.Add(this.progressBarX1);
            this.Controls.Add(this.btnRecover);
            this.Controls.Add(this.btnLeave);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnBlock);
            this.Controls.Add(this.btnSelected);
            this.Controls.Add(this.btnUnOpen);
            this.Controls.Add(this.dataGridViewX1);
            this.Controls.Add(this.cbxCoursePeriod);
            this.Controls.Add(this.cbxSemester);
            this.Controls.Add(this.cbxSchoolYear);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.Name = "frmWishEditor";
            this.Text = "選課志願調整";
            this.Load += new System.EventHandler(this.frmWishEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSemester;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxCoursePeriod;
        private DevComponents.DotNetBar.ButtonX btnUnOpen;
        private DevComponents.DotNetBar.ButtonX btnSelected;
        private DevComponents.DotNetBar.ButtonX btnBlock;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnLeave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private DevComponents.DotNetBar.ButtonItem btnSignDisabled;
        private DevComponents.DotNetBar.ButtonItem btnCancelDisabled;
        private DevComponents.DotNetBar.ButtonItem btnCancelSelected;
        private DevComponents.DotNetBar.ButtonItem btnSignBlock;
        private DevComponents.DotNetBar.ButtonItem btnSignSelected;
        private DevComponents.DotNetBar.ButtonItem btnCancelBlock;
        private DevComponents.DotNetBar.ButtonX btnRecover;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private DevComponents.DotNetBar.Controls.ProgressBarX progressBarX1;
        private DevComponents.DotNetBar.LabelX lb4;
    }
}