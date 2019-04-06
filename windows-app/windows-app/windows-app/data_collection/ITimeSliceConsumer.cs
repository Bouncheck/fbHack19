using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_app.data_collection
{
    interface ITimeSliceConsumer
    {
        void Consume(TimeSliceInfo timeSlice);
    }
}
