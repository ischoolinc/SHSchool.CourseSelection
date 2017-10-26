namespace SHSchool.CourseSelection.Forms
{
    partial class CourseCorrespond
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
            this.schoolyearcbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.semestercbx = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subject_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.credit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.limit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ischool_course_selection_ss_attend = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CorrespondCourseCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ref_course_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.editbtn = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewLinkColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // schoolyearcbx
            // 
            this.schoolyearcbx.DisplayMember = "Text";
            this.schoolyearcbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.schoolyearcbx.FormattingEnabled = true;
            this.schoolyearcbx.ItemHeight = 19;
            this.schoolyearcbx.Location = new System.Drawing.Point(57, 17);
            this.schoolyearcbx.Name = "schoolyearcbx";
            this.schoolyearcbx.Size = new System.Drawing.Size(60, 25);
            this.schoolyearcbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.schoolyearcbx.TabIndex = 0;
            // 
            // semestercbx
            // 
            this.semestercbx.DisplayMember = "Text";
            this.semestercbx.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.semestercbx.FormattingEnabled = true;
            this.semestercbx.ItemHeight = 19;
            this.semestercbx.Location = new System.Drawing.Point(172, 17);
            this.semestercbx.Name = "semestercbx";
            this.semestercbx.Size = new System.Drawing.Size(50, 25);
            this.semestercbx.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.semestercbx.TabIndex = 1;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(16, 17);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(35, 25);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "學年";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(131, 17);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(35, 25);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "學期";
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToDeleteRows = false;
            this.dataGridViewX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.type,
            this.subject_name,
            this.level,
            this.credit,
            this.limit,
            this.ischool_course_selection_ss_attend,
            this.CorrespondCourseCount,
            this.ref_course_id,
            this.editbtn,
            this.Column1});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.HighlightSelectedColumnHeaders = false;
            this.dataGridViewX1.Location = new System.Drawing.Point(12, 57);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.ReadOnly = true;
            this.dataGridViewX1.RowHeadersVisible = false;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.Size = new System.Drawing.Size(760, 340);
            this.dataGridViewX1.TabIndex = 4;
            this.dataGridViewX1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellClick);
            // 
            // type
            // 
            this.type.HeaderText = "課程類別";
            this.type.Name = "type";
            this.type.ReadOnly = true;
            this.type.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.type.Width = 90;
            // 
            // subject_name
            // 
            this.subject_name.HeaderText = "科目";
            this.subject_name.Name = "subject_name";
            this.subject_name.ReadOnly = true;
            this.subject_name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.subject_name.Width = 85;
            // 
            // level
            // 
            this.level.HeaderText = "級別";
            this.level.Name = "level";
            this.level.ReadOnly = true;
            this.level.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.level.Width = 40;
            // 
            // credit
            // 
            this.credit.HeaderText = "學分";
            this.credit.Name = "credit";
            this.credit.ReadOnly = true;
            this.credit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.credit.Width = 40;
            // 
            // limit
            // 
            this.limit.HeaderText = "人數上限";
            this.limit.Name = "limit";
            this.limit.ReadOnly = true;
            this.limit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.limit.Width = 85;
            // 
            // ischool_course_selection_ss_attend
            // 
            this.ischool_course_selection_ss_attend.HeaderText = "選課人數";
            this.ischool_course_selection_ss_attend.Name = "ischool_course_selection_ss_attend";
            this.ischool_course_selection_ss_attend.ReadOnly = true;
            this.ischool_course_selection_ss_attend.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ischool_course_selection_ss_attend.Width = 85;
            // 
            // CorrespondCourseCount
            // 
            this.CorrespondCourseCount.HeaderText = "課程人數";
            this.CorrespondCourseCount.Name = "CorrespondCourseCount";
            this.CorrespondCourseCount.ReadOnly = true;
            this.CorrespondCourseCount.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CorrespondCourseCount.Width = 85;
            // 
            // ref_course_id
            // 
            this.ref_course_id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ref_course_id.HeaderText = "對應課程";
            this.ref_course_id.Name = "ref_course_id";
            this.ref_course_id.ReadOnly = true;
            this.ref_course_id.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // editbtn
            // 
            this.editbtn.HeaderText = "";
            this.editbtn.Name = "editbtn";
            this.editbtn.ReadOnly = true;
            this.editbtn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.editbtn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.editbtn.Width = 40;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 40;
            // 
            // CourseCorrespond
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 409);
            this.Controls.Add(this.dataGridViewX1);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.semestercbx);
            this.Controls.Add(this.schoolyearcbx);
            this.DoubleBuffered = true;
            this.Name = "CourseCorrespond";
            this.Text = "轉入修課學生";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx schoolyearcbx;
        private DevComponents.DotNetBar.Controls.ComboBoxEx semestercbx;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn subject_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn level;
        private System.Windows.Forms.DataGridViewTextBoxColumn credit;
        private System.Windows.Forms.DataGridViewTextBoxColumn limit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ischool_course_selection_ss_attend;
        private System.Windows.Forms.DataGridViewTextBoxColumn CorrespondCourseCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ref_course_id;
        private System.Windows.Forms.DataGridViewLinkColumn editbtn;
        private System.Windows.Forms.DataGridViewLinkColumn Column1;
    }
}