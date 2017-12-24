using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace SHSchool.CourseSelection.Forms
{
    public partial class AdjustSubjectStudent : BaseForm
    {

        string _subjectID = "";

        public AdjustSubjectStudent(string subjectID)
        {
            InitializeComponent();

            _subjectID = subjectID;

            #region  Init DatagidView
#endregion

        }
    }
}
