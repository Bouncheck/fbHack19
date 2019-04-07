using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using windows_app.data_collection;

namespace windows_app
{
    public partial class CreateNote : Window
    {
        public CreateNote()
        {
            InitializeComponent();

            CaptureScreenShot();
            ScreenshotImage.Source = Screenshot;
            ScreenshotTime = DateTime.Now;
        }

        private void CaptureScreenShot()
        {
            Rectangle bounds = Screen.GetBounds(System.Drawing.Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
                }

                Screenshot = Convert(bitmap);
            }
        }

        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string json = GetNoteJSON();
            Console.WriteLine(json);

            StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            client.PostAsync(CollectionConfiguration.Default.WebService + "/notes/upload", httpContent);

            Close();
        }

        private string GetNoteJSON()
        {
            string base64JPG = ImageToBase64JPG();

            NoteJSON noteJson = new NoteJSON()
            {
                Image = base64JPG,
                Note = NoteTextBox.Text,
                Time = ScreenshotTime.ToString("o", CultureInfo.CurrentCulture),
                User = CollectionConfiguration.Default.Username
            };

            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(NoteJSON));
            jsonSerializer.WriteObject(memoryStream, noteJson);

            memoryStream.Position = 0;
            StreamReader reader = new StreamReader(memoryStream);
            string json = reader.ReadToEnd();
            return json;
        }

        private string ImageToBase64JPG()
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Screenshot));

            MemoryStream ms = new MemoryStream();
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return System.Convert.ToBase64String(ms.ToArray());
        }

        private static readonly HttpClient client = new HttpClient();
        private DateTime ScreenshotTime;
        private BitmapImage Screenshot;
    }
}
