namespace SHSchool.CourseSelection.Forms
{
    partial class frmError
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
            this.btnMarkCourse = new DevComponents.DotNetBar.ButtonX();
            this.btnMarkStudent = new DevComponents.DotNetBar.ButtonX();
            this.btnLeave = new DevComponents.DotNetBar.ButtonX();
            this.lbMessage = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(458, 120);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "\r\n轉入失敗:\r\n\r\n部分學生已有修課紀錄，請先到「課程」頁面\r\n刪除重複修課學生再進行選課「轉入修課學生」動作。";
            this.labelX1.TextLineAlignment = System.Drawing.StringAlignment.Near;
            // 
            // btnMarkCourse
            // 
            this.btnMarkCourse.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnMarkCourse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMarkCourse.BackColor = System.Drawing.Color.Transparent;
            this.btnMarkCourse.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnMarkCourse.Location = new System.Drawing.Point(13, 183);
            this.btnMarkCourse.Name = "btnMarkCourse";
            this.btnMarkCourse.Size = new System.Drawing.Size(185, 23);
            this.btnMarkCourse.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnMarkCourse.TabIndex = 1;
            this.btnMarkCourse.Text = "重複課程加入代處理";
            this.btnMarkCourse.Click += new System.EventHandler(this.btnMarkCourse_Click);
            // 
            // btnMarkStudent
            // 
            this.btnMarkStudent.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnMarkStudent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMarkStudent.BackColor = System.Drawing.Color.Transparent;
            this.btnMarkStudent.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnMarkStudent.Location = new System.Drawing.Point(204, 183);
            this.btnMarkStudent.Name = "btnMarkStudent";
            this.btnMarkStudent.Size = new System.Drawing.Size(185, 23);
            this.btnMarkStudent.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnMarkStudent.TabIndex = 2;
            this.btnMarkStudent.Text = "重複學生加入代處理";
            this.btnMarkStudent.Click += new System.EventHandler(this.btnMarkStudent_Click);
            // 
            // btnLeave
            // 
            this.btnLeave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnLeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeave.BackColor = System.Drawing.Color.Transparent;
            this.btnLeave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnLeave.Location = new System.Drawing.Point(395, 183);
            this.btnLeave.Name = "btnLeave";
            this.btnLeave.Size = new System.Drawing.Size(75, 23);
            this.btnLeave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnLeave.TabIndex = 3;
            this.btnLeave.Text = "離開";
            this.btnLeave.Click += new System.EventHandler(this.btnLeave_Click);
            // 
            // lbMessage
            // 
            this.lbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbMessage.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbMessage.BackgroundStyle.Class = "";
            this.lbMessage.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbMessage.Location = new System.Drawing.Point(13, 154);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(457, 23);
            this.lbMessage.TabIndex = 4;
            // 
            // frmError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 218);
            this.Controls.Add(this.lbMessage);
            this.Controls.Add(this.btnLeave);
            this.Controls.Add(this.btnMarkStudent);
            this.Controls.Add(this.btnMarkCourse);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(500, 265);
            this.MinimumSize = new System.Drawing.Size(500, 265);
            this.Name = "frmError";
            this.Text = "錯誤訊息";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.ButtonX btnMarkCourse;
        private DevComponents.DotNetBar.ButtonX btnMarkStudent;
        private DevComponents.DotNetBar.ButtonX btnLeave;
        private DevComponents.DotNetBar.LabelX lbMessage;
    }
}