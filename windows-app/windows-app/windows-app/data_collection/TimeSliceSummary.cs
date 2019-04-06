using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_app.data_collection
{
    class TimeSliceSummary
    {
        public string ProgramName { get; set; }
        public CurrentWindow Window { get; set; }
        public long TimeInMs { get; set; }
    }
}
