using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using K12.Presentation;
using FISCA.Presentation.Controls;

namespace SHSchool.CourseSelection.Forms
{
    public partial class frmError : BaseForm
    {
        private List<string> listScAttendID = new List<string>();
        private List<string> listCourseID = new List<string>();
        private List<string> listStudentID = new List<string>();

        public frmError(List<string>listSc, List<string>listCourse, List<string>listStudent)
        {
            InitializeComponent();
            listScAttendID = listSc;
            listCourseID = listCourse;
            listStudentID = listStudent;
        }

        private void btnMarkCourse_Click(object sender, EventArgs e)
        {
            List<string> listTargetCourse = new List<string>();
            foreach (string id in listCourseID)
            {
                if (!NLDPanels.Course.TempSource.Contains(id))
                {
                    listTargetCourse.Add(id);
                }
            }
            
            NLDPanels.Course.AddToTemp(listTargetCourse);
            lbMessage.Text = string.Format("已加入{0}筆待處理課程", listTargetCourse.Count);
        }

        private void btnMarkStudent_Click(object sender, EventArgs e)
        {
            List<string> listTargetStudent = new List<string>();
            foreach (string id in listStudentID)
            {
                if (!NLDPanels.Student.TempSource.Contains(id))
                {
                    listTargetStudent.Add(id);
                }
            }
            
            NLDPanels.Student.AddToTemp(listTargetStudent);
            lbMessage.Text = string.Format("已加入{0}筆待處理學生", listTargetStudent.Count);
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
