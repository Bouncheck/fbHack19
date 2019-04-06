using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace windows_app.data_collection
{
    class TimeSliceUploader : ITimeSliceConsumer
    {
        [DataContract]
        internal class TimeSliceJSON
        {
            [DataMember]
            internal string Username;

            [DataMember]
            internal string StartTime;

            [DataMember]
            internal long TimeLength;

            [DataMember]
            internal Dictionary<string, long> Data;

            public static TimeSliceJSON FromTimeSliceInfo(TimeSliceInfo timeSlice)
            {
                TimeSliceJSON timeSliceJson = new TimeSliceJSON
                {
                    StartTime = timeSlice.StartTime.ToString("o", CultureInfo.CurrentCulture),
                    TimeLength = timeSlice.LengthInMs,
                    Username = CollectionConfiguration.Default.Username,
                    Data = timeSlice.TimeSliceSummary.ToDictionary(p => p.ProgramName, p => p.TimeInMs)
                };
                return timeSliceJson;
            }
        }

        public void Consume(TimeSliceInfo timeSlice)
        {
            TimeSliceJSON timeSliceJson = TimeSliceJSON.FromTimeSliceInfo(timeSlice);

            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(TimeSliceJSON));
            jsonSerializer.WriteObject(memoryStream, timeSliceJson);

            memoryStream.Position = 0;
            StreamReader reader = new StreamReader(memoryStream);
            string json = reader.ReadToEnd();
            Console.WriteLine(json);

            StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            client.PostAsync(CollectionConfiguration.Default.WebService + "/activities/upload", httpContent);
        }

        private static readonly HttpClient client = new HttpClient();
    }
}
