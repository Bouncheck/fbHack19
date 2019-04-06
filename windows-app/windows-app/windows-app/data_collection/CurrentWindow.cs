using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace windows_app.data_collection
{
    class CurrentWindow
    {
        public string WindowTitle { get; set; }
        public string ProgramName { get; set; }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public string Test()
        {
            try
            {
                IntPtr handle = GetActiveWindow();

                string windowTitle = GetWindowTitle(handle);
                string programName = GetProgramName(handle);

                return programName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static IntPtr GetActiveWindow()
        {
            return GetForegroundWindow();
        }

        private static string GetWindowTitle(IntPtr handle)
        {
            int BUFFER_SIZE = 4096;
            StringBuilder buffer = new StringBuilder(BUFFER_SIZE);

            int readBytes = GetWindowText(handle, buffer, BUFFER_SIZE);
            return readBytes > 0 ? buffer.ToString() : String.Empty;
        }

        private static string GetProgramName(IntPtr handle)
        {
            GetWindowThreadProcessId(handle, out var processId);

            Process process = Process.GetProcessById((int) processId);
            string processPath = process.MainModule.FileName;
            
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(processPath);

            return info.ProductName;
        }
    }
}
