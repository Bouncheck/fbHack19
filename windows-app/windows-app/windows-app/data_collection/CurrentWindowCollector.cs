using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace windows_app.data_collection
{
    class CurrentWindowCollector
    {
        private const int CollectTickMs = 15;

        private List<CurrentWindow> CurrentTimeSlice;
        private DateTime CurrentTimeSliceStart;

        private ITimeSliceConsumer Consumer;

        private Timer Timer;

        public CurrentWindowCollector(ITimeSliceConsumer consumer)
        {
            CurrentTimeSlice = new List<CurrentWindow>();
            CurrentTimeSliceStart = DateTime.Now;

            Consumer = consumer;

            Timer = new Timer();
            Timer.Elapsed += DoCollect;
            Timer.Interval = CollectTickMs;

            Timer.Start();
        }

        private void DoCollect(object sender, ElapsedEventArgs e)
        {
            if (ShouldEndTimeSlice()) StartNewTimeSlice();

            CollectTick();
        }

        private void CollectTick()
        {
            CurrentWindow current = CurrentWindow.GetActiveWindow();
            if (current.IsEmpty()) return;

            CurrentTimeSlice.Add(current);
        }

        private void StartNewTimeSlice()
        {
            CurrentTimeSliceStart = DateTime.Now;

            TimeSliceInfo timeSliceInfo = new TimeSliceInfo
            {
                RecordedWindows = CurrentTimeSlice,
                TickInMs = CollectTickMs
            };

            Consumer.Consume(timeSliceInfo);
            CurrentTimeSlice.Clear();
        }

        private bool ShouldEndTimeSlice()
        {
            DateTime currentTime = DateTime.Now;

            bool withinSameTimeSlice = currentTime.Hour == CurrentTimeSliceStart.Hour &&
                                       currentTime.Minute == CurrentTimeSliceStart.Minute;

            return !withinSameTimeSlice;
        }
    }
}
