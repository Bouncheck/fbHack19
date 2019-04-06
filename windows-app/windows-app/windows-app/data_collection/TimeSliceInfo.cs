using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_app.data_collection
{
    class TimeSliceInfo
    {
        public List<CurrentWindowCollector.TickInfo> RecordedWindows { get; set; }

        public long TickInMs { get; set; }

        public long LengthInMs { get; set; }

        public DateTime StartTime { get; set; }

        private List<TimeSliceSummary> timeSliceSummary;
        public List<TimeSliceSummary> TimeSliceSummary
        {
            get
            {
                return timeSliceSummary ?? (timeSliceSummary =
                       RecordedWindows.GroupBy(q => q.Window.ProgramName)
                           .Select(g => new TimeSliceSummary()
                           {
                               ProgramName = g.Key,
                               TimeInMs = g.Sum(q => q.Span.Milliseconds),
                               Window = g.First().Window
                           }).ToList());
            }
        }
    }
}
