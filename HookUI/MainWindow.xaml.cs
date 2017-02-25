using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace QiDiTu.Hook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string filterClassNamePath = "Resources/FilterClassName.txt";

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
            check();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;
            if (File.Exists(filterClassNamePath))
            {
                using (FileStream stream =
                    new FileStream(filterClassNamePath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        classNameFilterSet.Add(reader.ReadLine());
                    }
                }
            }
            dataGrid.ItemsSource = dataList;
            refresh();
        }

        
        private void check()
        {
            hasX86 = File.Exists("x86\\" + HookCore.dllFileName) 
                        && File.Exists("x86\\" + HookCore.hookFileName);
            hasX64 = File.Exists("x64\\" + HookCore.dllFileName) 
                        && File.Exists("x64\\" + HookCore.hookFileName);
            if (!hasX86 && !hasX64)
            {
                MessageBox.Show("x86和x64版本文件缺失");
                Application.Current.Shutdown();
            }
            else if (!hasX86)
            {
                MessageBox.Show("x86版本文件缺失，x86进程将不能注入");
            }
            else if (!hasX64 && Environment.Is64BitOperatingSystem)
            {
                MessageBox.Show("x64版本文件缺失，x64进程将不能注入");
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private ObservableCollection<Data> dataList = new ObservableCollection<Data>();
        private bool hasX86 = false;
        private bool hasX64 = false;

        private void refresh()
        {
            Dictionary<int, Data> map = new Dictionary<int, Data>();
            foreach (var d in enumWindows())
            {
                if (!map.ContainsKey(d.ProcessID))
                {
                    map.Add(d.ProcessID, d);
                }
            }
            for (int i = dataList.Count - 1; i >= 0; --i)
            {
                Data d = dataList[i];
                if (!map.ContainsKey(d.ProcessID))
                {
                    dataList.RemoveAt(i);
                }
                else
                {
                    d.WindowName = map[d.ProcessID].WindowName;
                    map.Remove(d.ProcessID);
                }
            }
            foreach (var d in map.Values)
            {
                dataList.Add(d);
            }
        }

        private List<Data> enumWindows()
        {
            List<Data> array = new List<Data>();
            PInvoke.EnumWindows(new PInvoke.EnumWindowsProc(
                (IntPtr hwnd, ref IntPtr lParam) => {
                    if (!isConformFilter(hwnd))
                    {
                        Data data = new Data(hwnd);
                        array.Add(data);
                    }
                    return true;
                }), IntPtr.Zero);
            return array;
        }

        private HashSet<string> classNameFilterSet = new HashSet<string>();

        private bool isConformFilter(IntPtr hwnd)
        {
            //窗口是否可视
            if (!PInvoke.IsWindowVisible(hwnd))
            {
                return true;
            }
            //窗口是否可激活
            if (!PInvoke.IsWindowEnabled(hwnd))
            {
                return true;
            }
            //标题为空
            int length = PInvoke.GetWindowTextLength(hwnd);
            if (length == 0)
            {
                return true;
            }
            StringBuilder stringBuilder = new StringBuilder(length + 1);
            PInvoke.GetWindowText(hwnd, stringBuilder, stringBuilder.Capacity);
            string windowName = stringBuilder.ToString();
            if (windowName.isEmpty())
            {
                return true;
            }

            int style = PInvoke.GetWindowLong(hwnd, PInvoke.WindowLongFlags.GWL_EXSTYLE);
            if ((style & PInvoke.WS_POPUP) != 0 && (style & PInvoke.WS_CAPTION) == 0)
            {
                return true;
            }

            IntPtr parent = PInvoke.GetWindowLongPtr(hwnd, PInvoke.WindowLongFlags.GWL_HWNDPARENT);
            //窗口是否具有父窗口
            if (parent != IntPtr.Zero)
            {
                return true;
            }
            ////父窗口是否可激活
            //if (PInvoke.IsWindowVisible(parent))
            //{
            //    return true;
            //}
            ////父窗口是否可视
            //if (PInvoke.IsWindowEnabled(parent))
            //{
            //    return true;
            //}

            //过滤win10uwp后台不可视应用
            if (isInvisibleWin10BackgroundAppWindow(hwnd))
            {
                return true;
            }
            //过滤指定ClassName
            // Pre-allocate 256 characters, since this is the maximum class name length.
            StringBuilder stringBuilder1 = new StringBuilder(256);
            PInvoke.GetClassName(hwnd, stringBuilder1, stringBuilder1.Capacity);
            string className = stringBuilder1.ToString();
            if (classNameFilterSet.Contains(className))
            {
                return true;
            }

            return false;
        }

        private static bool isInvisibleWin10BackgroundAppWindow(IntPtr hwnd)
        {
            bool cloakedVal;
            int res = PInvoke.DwmGetWindowAttribute(hwnd, PInvoke.DWMWINDOWATTRIBUTE.Cloaked, out cloakedVal, sizeof(int));
            if (res != 0)
            {
                cloakedVal = false;
            }
            return cloakedVal;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int index = dataGrid.SelectedIndex;
            if (index < 0)
            {
                return;
            }
            CheckBox checkBox = (CheckBox)sender;

            if (dataList[index].Is64BitProcess)
            {
                if(!hasX64)
                {
                    MessageBox.Show("x64版本文件缺失，无法注入");
                    checkBox.IsChecked = false;
                    return;
                }
            }
            else if(!hasX86)
            {
                MessageBox.Show("x86版本文件缺失，无法注入");
                checkBox.IsChecked = false;
                return;
            }
            
            if (checkBox.IsChecked == true)
            {
                if (!dataList[index].install())
                {
                    checkBox.IsChecked = false;
                }
            }
            else
            {
                if (!dataList[index].uninstall())
                {
                    checkBox.IsChecked = true;
                }
            }
        }
    }

    static class HookCore
    {
        public const string dllFileName = "hook.dll";
        public const string hookFileName = "hook.exe";

        public static bool install(Process target)
        {
            Process process = new Process();
            string dir = Path.Combine(Environment.CurrentDirectory, (target.is64() ? "x64\\" : "x86\\"));
            process.StartInfo.FileName = Path.Combine(dir, hookFileName);
            process.StartInfo.Arguments = "install " + target.Id;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = dir;
            process.Start();
            process.WaitForExit();
            switch (process.ExitCode)
            {
                case 0:
                {
                    return true;
                }
                default:
                {
                    MessageBox.Show("进程异常退出，" + process.ExitCode);
                } break;
            }
            return false;
        }

        public static bool uninstall(Process target)
        {
            Process process = new Process();
            string dir = Path.Combine(Environment.CurrentDirectory, (target.is64() ? "x64\\" : "x86\\"));
            process.StartInfo.FileName = Path.Combine(dir, hookFileName);
            process.StartInfo.Arguments = "uninstall " + target.Id;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = dir;
            process.Start();
            process.WaitForExit();
            switch (process.ExitCode)
            {
                case 0:
                {
                    return true;
                }
                default:
                {
                    MessageBox.Show("进程异常退出，" + process.ExitCode);
                }  break;
            }
            return false;
        }
    }

    class Data
    {
        public Data(IntPtr hwnd)
        {
            int length = PInvoke.GetWindowTextLength(hwnd);
            StringBuilder stringBuilder = new StringBuilder(length + 1);
            PInvoke.GetWindowText(hwnd, stringBuilder, stringBuilder.Capacity);
            WindowName = stringBuilder.ToString();
            uint processId;
            PInvoke.GetWindowThreadProcessId(hwnd, out processId);
            process = Process.GetProcessById((int)processId);
        }

        ~Data()
        {
            if (IsRegister)
            {
                uninstall();
            }
        }

        public string WindowName
        {
            get;
            internal set;
        }

        private Process process;
        public int ProcessID
        {
            get
            {
                return process.Id;
            }
        }

        public bool IsRegister
        {
            get;
            private set;
        }

        public bool Is64BitProcess
        {
            get
            {
                return process.is64();
            }
        }

        public bool install()
        {
            if (HookCore.install(process))
            {
                IsRegister = true;
                return true;
            }
            return false;
        }

        public bool uninstall()
        {
            if (HookCore.uninstall(process))
            {
                IsRegister = false;
                return true;
            }
            return false;
        }

    }
}
