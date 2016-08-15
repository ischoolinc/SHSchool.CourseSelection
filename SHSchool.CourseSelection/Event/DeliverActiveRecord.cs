using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHSchool.CourseSelection.Event
{
    class DeliverActiveRecord
    {
        public DeliverActiveRecord()
        {

        }

        internal static void RaiseSendingEvent(object sender, DeliverActiveRecordEventArgs e)
        {
            if (DeliverActiveRecord.Received != null)
                DeliverActiveRecord.Received(sender, e);
        }

        internal static event EventHandler<DeliverActiveRecordEventArgs> Received;
    }
}
