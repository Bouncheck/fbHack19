using System;
using System.Collections.Concurrent;
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
        
        private ConcurrentDictionary<string, long> SessionTimeSpent;

        private ITimeSliceConsumer Consumer;

        private Timer Timer;

        public CurrentWindow CurrentWindow { get; set; }

        public CurrentWindowCollector(ITimeSliceConsumer consumer)
        {
            CurrentTimeSlice = new List<CurrentWindow>();
            CurrentTimeSliceStart = DateTime.Now;

            CurrentWindow = CurrentWindow.Empty;
            
            SessionTimeSpent = new ConcurrentDictionary<string, long>();

            Consumer = consumer;

            Timer = new Timer();
            Timer.Elapsed += DoCollect;
            Timer.Interval = CollectTickMs;

            Timer.Start();
        }

        public long GetSessionTimeSpent(string programName)
        {
            return SessionTimeSpent.ContainsKey(programName)
                ? SessionTimeSpent[programName] : 0;
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

            CurrentWindow = current;

            CurrentTimeSlice.Add(current);

            SessionTimeSpent[current.ProgramName]
                = GetSessionTimeSpent(current.ProgramName) + CollectTickMs;
        }

        private void StartNewTimeSlice()
        {
            CurrentTimeSliceStart = DateTime.Now;

            TimeSliceInfo timeSliceInfo = new TimeSliceInfo
            {
                RecordedWindows = CurrentTimeSlice,
                TickInMs = CollectTickMs
            };

            Consumer?.Consume(timeSliceInfo);
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
