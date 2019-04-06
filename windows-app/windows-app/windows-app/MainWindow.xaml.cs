using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using windows_app.data_collection;

namespace windows_app
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Timer = new DispatcherTimer();
            Timer.Tick += TimeTick;
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Start();
        }

        private void TimeTick(object sender, EventArgs e)
        {
            this.label.Content = new CurrentWindow().Test();
        }

        private DispatcherTimer Timer { get; set; }
    }
}
