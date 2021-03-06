﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_app.data_collection
{
    class SummarizedConsumer : ITimeSliceConsumer
    {
        public void Consume(TimeSliceInfo timeSlice)
        {
            List<Summary> summarized = timeSlice.RecordedWindows.GroupBy(q => q.ProgramName)
                .Select(g => new Summary()
                {
                    ProgramName = g.Key,
                    TimeInMs = g.Count() * timeSlice.TickInMs
                }).ToList();
        }
    }
}
