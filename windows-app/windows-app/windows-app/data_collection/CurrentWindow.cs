using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace windows_app.data_collection
{
    class CurrentWindow
    {
        public string WindowTitle { get; private set; }
        public string ProgramPath { get; private set; }
        public string ProgramName { get; private set; }

        public static CurrentWindow Empty => new CurrentWindow();

        public static CurrentWindow GetActiveWindow()
        {
            try
            {
                IntPtr handle = GetActiveWindowHandle();

                CurrentWindow window = new CurrentWindow();
                window.WindowTitle = GetWindowTitle(handle);
                window.ProgramPath = GetProgramPath(handle);
                window.ProgramName = GetProgramName(window.ProgramPath);

                return window;
            }
            catch (Exception ex)
            {
                return CurrentWindow.Empty;
            }
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(WindowTitle);
        }

        private static IntPtr GetActiveWindowHandle()
        {
            return GetForegroundWindow();
        }

        private static string GetWindowTitle(IntPtr handle)
        {
            int BUFFER_SIZE = 4096;
            StringBuilder buffer = new StringBuilder(BUFFER_SIZE);

            int readBytes = GetWindowText(handle, buffer, BUFFER_SIZE);
            return readBytes > 0 ? buffer.ToString() : string.Empty;
        }

        private static string GetProgramPath(IntPtr handle)
        {
            GetWindowThreadProcessId(handle, out var processId);

            Process process = Process.GetProcessById((int)processId);
            string processPath = process.MainModule.FileName;

            return processPath;
        }

        private static string GetProgramName(string programPath)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(programPath);
            return info.ProductName;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}
