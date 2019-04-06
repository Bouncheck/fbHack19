using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using windows_app.data_collection;

namespace windows_app
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public uint AccentFlags;
        public uint GradientColor;
        public uint AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            breakTime = false;

            timeSliceUploader = new TimeSliceUploader();
            currentWindowCollector = new CurrentWindowCollector(timeSliceUploader);
            CompositionTarget.Rendering += UpdateUI;
        }

        private void UpdateUI(object sender, EventArgs e)
        {
            if (breakTime)
            {
                mainLabel.Content = "Break time";
                secondaryLabel.Content = "Enjoy your break!";
                return;
            }

            string programName = currentWindowCollector.CurrentWindow.ProgramName;
            if (string.IsNullOrWhiteSpace(programName)) return;

            mainLabel.Content = programName;
            secondaryLabel.Content = TimeFormatter.FormatMillis(currentWindowCollector.GetSessionTimeSpent(programName));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupBlur();
            SetupPosition();
        }

        private void SetupPosition()
        {
            double height = ActualHeight, width = ActualWidth;

            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double screenWidth = SystemParameters.PrimaryScreenWidth;

            Left = screenWidth - width - 7;
            Top = screenHeight - height - 40 - 7;
        }

        private void SetupBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            accent.GradientColor = (100 << 24) | (0x666666 & 0xFFFFFF);

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            UIElement senderElement = (UIElement)sender;
            senderElement.Opacity = 1.0;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            UIElement senderElement = (UIElement)sender;
            senderElement.Opacity = 0.7;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement senderElement = (UIElement)sender;
            senderElement.Opacity = 0.9;

            breakTime = !breakTime;
            currentWindowCollector.BreakTime = breakTime;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement senderElement = (UIElement)sender;
            senderElement.Opacity = 1.0;
        }

        private CurrentWindowCollector currentWindowCollector;
        private TimeSliceUploader timeSliceUploader;
        private bool breakTime;
    }
}
