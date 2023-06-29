using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Rpa.Util
{
    class Window
    {
        public string ClassName;
        public string Title;
        public IntPtr hWnd;
        public int Style;
    }

    #region "キーコード表"
    /*
     Alt + F4は括弧内でのみ機能します

     SendKeys.SendWait("(%{F4})");

    SendWait("^({ESC}E)")またはSend("^({ESC}E)")

     */
    /*
     　A　	　"A"　
　AAAAA　	　"{A 5}"　
　ABC　	　"ABC"　
　[SHIFT] A　	　"+A"　
　[CTRL] A　	　"^A"　
　[ALT] A　	　"%A"　
　[F1]　	　"{F1}"　
　[BACKSPACE]　	　"{BS}"　
　[BREAK]　	　"{BREAK}"　
　[CAPSLOCK]　	　"{CAPSLOCK}"　
　[DEL]　	　"{DEL}"　
　[END]　	　"{END}"　
　[ENTER]　	　"{ENTER}"　
　[ESC]　	　"{ESC}"　
　[HOME]　	　"{HOME}"　
　[INS]　	　"{INS}"　
　[NUMLOCK]　	　"{NUMLOCK}"　
　[PageDown]　	　"{PGDN}"　
　[PageUp]　	　"{PGUP}"　
　[PRTSC]　	　"{PRTSC}"　
　[TAB]　	　"{TAB}"　
　[↑]　	　"{UP}"　
　[↓]　	　"{DOWN}"　
　[←]　	　"{LEFT}"　
　[→]　	　"{RIGHT}"　
　+　	　"{+}"　
　^　	　"{^}"　
　%　	　"{%}"　
　~　	　"{~}"　
　(　	　"{(}"　
　)　	　"{)}"　
　{　	　"{{}"　
　}　	　"{}}"　
　[　	　"{[}"　
　]　	　"{]}"　


        キーコード表
　BACKSPACE　	　0x08　
　TAB　	　0x09　
　ENTER　	　0x0D　
　SHIFT　	　0x10　
　CTRL　	　0x11　
　ALT　	　0x12　
　PAUSE　	　0x13　
　CAPSLOCK　	　0x14　
　ESC　	　0x1B　
　SPACE　	　0x20　
　PGUP　	　0x21　
　PGDN　	　0x22　
　END　	　0x23　
　HOME　	　0x24　
　←　	　0x25　
　↑　	　0x26　
　→　	　0x27　
　↓　	　0x28　
　PRTSC　	　0x2C　
　INS　	　0x2D　
　DEL　	　0x2E　
　0 ～ 9　	　0x30 ～ 0x39　
　A ～ Z　	　0x41 ～ 0x5A　
　F1 ～ F24　	　0x70 ～ 0x87
         */

    #endregion

    class MyKey:MyKeyMap
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern int VkKeyScan(char ch);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        private extern static int GetWindowThreadProcessId(int hwnd, out int lpdwprocessid);

        #region "window"
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int MK_LBUTTON = 0x0001;
        public static int GWL_STYLE = -16;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        //[DllImport("user32.dll")]
        //public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        #region " http://pgcenter.web.fc2.com/contents/csharp_sendinput.html"

        // マウスイベント(mouse_eventの引数と同様のデータ)
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        // キーボードイベント(keybd_eventの引数と同様のデータ)
        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        // ハードウェアイベント
        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        };

        // 各種イベント(SendInputの引数データ)
        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)] public int type;
            [FieldOffset(4)] public MOUSEINPUT mi;
            [FieldOffset(4)] public KEYBDINPUT ki;
            [FieldOffset(4)] public HARDWAREINPUT hi;
        };

        [DllImport("user32.dll")]
        static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

        // 仮想キーコードをスキャンコードに変換
        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        private extern static int MapVirtualKey(int wCode, int wMapType);
        #endregion

        #endregion

        #region "マウスAPI"
        /////// APIの利用に必要な構造体・共用体の定義　ここから /////
        //[StructLayout(LayoutKind.Sequential)]
        //public struct Win32Point
        //{
        //    public Int32 X;
        //    public Int32 Y;
        //};

        //[StructLayout(LayoutKind.Sequential)]
        //public struct MOUSEINPUT
        //{
        //    public int dx;
        //    public int dy;
        //    public int mouseData;
        //    public int dwFlags;
        //    public int time;
        //    public IntPtr dwExtraInfo;
        //};

        //[StructLayout(LayoutKind.Sequential)]
        //public struct KEYBDINPUT
        //{
        //    public short wVk;
        //    public short wScan;
        //    public int dwFlags;
        //    public int time;
        //    public IntPtr dwExtraInfo;
        //};

        //[StructLayout(LayoutKind.Sequential)]
        //public struct HARDWAREINPUT
        //{
        //    public int uMsg;
        //    public short wParamL;
        //    public short wParamH;
        //};

        //[StructLayout(LayoutKind.Explicit)]
        //public struct INPUT_UNION
        //{
        //    [FieldOffset(0)] public MOUSEINPUT mouse;
        //    [FieldOffset(0)] public KEYBDINPUT keyboard;
        //    [FieldOffset(0)] public HARDWAREINPUT hardware;
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //public struct INPUT
        //{
        //    public int type;
        //    public INPUT_UNION ui;
        //};
        /////// APIの利用に必要な構造体・共用体の定義　ここまで /////

        //public static class NativeMethods
        //{

        //    // 定数の定義

        //    public const int INPUT_MOUSE = 0;
        //    public const int INPUT_KEYBOARD = 1;
        //    public const int INPUT_HARDWARE = 2;

        //    public const int MOUSEEVENTF_MOVE = 0x1;
        //    public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        //    public const int MOUSEEVENTF_LEFTDOWN = 0x2;
        //    public const int MOUSEEVENTF_LEFTUP = 0x4;
        //    public const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        //    public const int MOUSEEVENTF_RIGHTUP = 0x10;
        //    public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        //    public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        //    public const int MOUSEEVENTF_WHEEL = 0x800;
        //    public const int WHEEL_DELTA = 120;

        //    public const int KEYEVENTF_KEYDOWN = 0x0;
        //    public const int KEYEVENTF_KEYUP = 0x2;
        //    public const int KEYEVENTF_EXTENDEDKEY = 0x1;



        //    // APIの読み込み

        //    [DllImport("user32.dll")]
        //    public static extern bool GetCursorPos(ref Win32Point pt);

        //    [DllImport("user32.dll")]
        //    public static extern void SendInput(int nInputs, ref INPUT pInputs, int cbsize);
        //}

        #endregion


        private static IntPtr _hwnd;

        private static int _pid;

        private static bool _stopflag;

        public static IntPtr hwnd { get { return _hwnd; } set { _hwnd = value; } }

        public static int pid { get { return _pid; } set { _pid = value; } }

        public static bool stopflag { 
            get { return _stopflag; } 
            set 
            {
                if (value) endCallback("stop.");
                _stopflag = value; 
            } 
        }

        static System.Timers.Timer timer;

        #region "コールバック"
        //どこの処理やっているか
        public delegate void EndCallback(string line);

        private static EndCallback _endCallback;

        public static EndCallback endCallback { get { return _endCallback; } set { _endCallback = value; } }

        //Clear
        //どこの処理やっているか
        public delegate void ClearCallback();

        private static ClearCallback _clearCallback;

        public static ClearCallback clearCallback { get { return _clearCallback; } set { _clearCallback = value; } }


        #endregion

        struct SENDKEY_STATUS {
            public int waittime;
            public bool activeflag;
        }
        
        public MyKey(LineCallback call, EndCallback call2, ClearCallback call3)
        {
            lineCallback = call;
            endCallback = call2;
            clearCallback = call3;

            stopflag = false;
        }

        public void stop()
        {
            stopflag = true;
        }

        public static void getprocess()
        {

            // デスクトップのウインドウハンドル取得
            hwnd = GetDesktopWindow();
            // メモ帳のウインドウハンドル取得
            hwnd = FindWindowEx(hwnd, IntPtr.Zero, "notepad", null);
            // メモ帳ウインドウ内の「edit」ウインドウのハンドル取得
            //hwnd = FindWindowEx(hwnd, IntPtr.Zero, "edit", null);


            // ウィンドウハンドル(hwnd)をプロセスID(pid)に変換する
            //int ppid = GetPidFromHwnd(hwnd);

        }

        // ウィンドウハンドル(hwnd)をプロセスID(pid)に変換する
        public int GetPidFromHwnd(int hwnd)
        {
            int pid;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;
        }

        public static void active(string strpid)
        {
            // 選択しているプロセスをアクティブ
            pid = int.Parse(strpid);
            Process p = Process.GetProcessById(pid);
            SetForegroundWindow(p.MainWindowHandle);
        }

        public static void active()
        {
            if (pid == 0) return;
            // 選択しているプロセスをアクティブ
            Process p = Process.GetProcessById(pid);
            SetForegroundWindow(p.MainWindowHandle);
        }

        public void starttimer()
        {
            timer = new System.Timers.Timer(60 * 1000);
            timer.Elapsed += MyClock;
            timer.Interval = 60 * 1000;//60秒
            timer.Enabled = false; 
            
        }

        public static void sendkey(string buf)
        {

            // 「edit」ウインドウに「aab」を送信
            //PostMessage(hwnd, 0x0100, VkKeyScan('a'), 0);
            //PostMessage(hwnd, 0x0100, 0x41, 0);
            //PostMessage(hwnd, 0x0100, 0x42, 0);


        }

        public void read(string buf)
        {
            MyFile.read(buf);

            //読み取り
            //lineCallback("read　comp.");
        }

        public void MyClock(object sender, EventArgs e)
        {
            if (stopflag) return;
            work();
        }

        /// <summary>
        /// キーストローク実行
        /// </summary>
        public async void work()
        {
            SENDKEY_STATUS status;
            status.activeflag = true;//アクティブにする
            status.waittime = 2 * 1000;//時間

            stopflag = false;

            MyKeyMap.setEnumKey2Str();

            pid = 0;

            //----------
            //MyProcess myp = new MyProcess();
            //pid = myp.getpid("desktop", "");
            //lineCallback("(desktop)" + pid);

            try
            {
                //[main]がない場合
                if (MyFile.funcl.Count == 0)
                {
                    foreach (var dd in MyFile.d)
                    {
                        if (stopflag) return;

                        //subloop(waittime, dd.Key);
                        await Task.Run(() => subloop(status, dd.Key));
                    }
                }
                //[main]が有り
                else
                {
                    for (int i = 0; MyFile.funcl.Count > i; i++)
                    {

                        string func = MyFile.funcl.ElementAt(i);
                        if (stopflag) return;

                        if (func == ":loop")
                        {
                            i = 0;
                            await Task.Run(() => subloop(status, func));
                        }
                        else if (func == ":stop")
                        {
                            break;
                        }
                        else
                        {
                            await Task.Run(() => subloop(status, func));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // lineCallback(e.ToString());
                //endCallback("comp");

            }
            //読み取り
            //lineCallback("end.");

            endCallback("comp");

        }


        private static void subloop(SENDKEY_STATUS status, string key)
        {
            if (stopflag) return;
            //Console.WriteLine("GROUP=" + key);
            //lineCallback("GROUP=" + key);
            try
            {
                foreach (var pair in MyFile.d[key])
                {
                    if (stopflag) return;
                    
                    //wait=1000 
                    //print コメント
                    //active=on       アクティブon,off
                    string[] ws = pair.Value.Split('=');
                    string[] wss;

                    Console.WriteLine("({0}){1}", pair.Key,  pair.Value);

                    if (!pair.Value.StartsWith(":", StringComparison.Ordinal))
                    {
                        //コマンド送信
                        lineCallback("(" + key + ")" + pair.Value, 0);
                        if (status.activeflag)
                        {
                            active();
                        }

                        send(pair.Value);
                    }
                    else
                    {
                        #region "独自コマンド 先頭に[:]"
                        //時間
                        if (ws[0] == ":wait")
                        {
                            lineCallback("(" + key + ")" + pair.Value, 1);
                            status.waittime = int.Parse(ws[1]);
                        }
                        //コメント表示
                        else if (ws[0] == ":print")
                        {
                            lineCallback("(" + key + ")" + pair.Value, 1);
                            //lineCallback("(" + key + ")" + ws[1]);
                        }
                        //指定PIDでアクティブにする
                        else if (ws[0] == ":active")
                        {
                            lineCallback("(" + key + ")" + pair.Value, 1);
                            if (ws[1].ToUpper().Trim() == "ON" || ws[1] == "1")
                            {
                                status.activeflag = true;

                                active();
                            }
                            if (ws[1].ToUpper().Trim() == "OFF" || ws[1] == "0")
                            {
                                status.activeflag = false;
                            }
                        }
                        //PID取得
                        else if (ws[0] == ":clear")
                        {
                            clearCallback();
                        }
                        //PID取得
                        else if (ws[0] == ":getpid")
                        {
                            wss = ws[1].Split(',');
                            if (wss.Count() == 2)
                            {
                                MyProcess myp = new MyProcess();
                                int cnt = 0;
                                pid = 0;
                                while (pid == 0)
                                {
                                    pid = myp.getpid(wss[0], wss[1]);
                                    Thread.Sleep(1000);
                                    cnt++;
                                    if (cnt > 10)
                                    {
                                        lineCallback("(" + key + ") pid 取得失敗", 2);

                                        //ストップ
                                        stopflag = true;

                                        break;
                                    }
                                }

                                active();
                                if (!stopflag)
                                {
                                    lineCallback("(" + key + ")" + ws[1] + " pid = " + pid.ToString(), 1);
                                }
                            }
                        }
                    }
                    //await Task.Delay(waittime);

                    #endregion

                    Thread.Sleep(status.waittime);
                }
            }
            catch (Exception e)
            {
                lineCallback(e.ToString(), 0);
            }
        }

        public static void send(string key)
        {
            try
            {
                int p1 = key.IndexOf("{LWin}");
                int p2 = key.IndexOf("{RWin}");
                Keys[] k;
                if (p1 != -1 || p2 != -1 )
                {
                    string keysub = "";
                    k = new Keys[2];

                    if (p1 != -1)
                    {
                        keysub = key.Replace("{LWin}", "");
                        k[0] = MyKeyMap.d2["LWin"];
                    }

                    if (p2 != -1)
                    {
                        keysub = key.Replace("{RWin}", "");
                        k[0] = MyKeyMap.d2["RWin"];
                    }
                    if(keysub.Length > 0)
                    {
                        k[1] = MyKeyMap.d2[keysub];
                    }

                    MyKeyMap.send(k);
                }
                //else if (key.IndexOf("{ENTER}") != -1)
                //{
                //    k = new Keys[1];

                //    k[0] = MyKeyMap.d2["Enter"];

                //    MyKeyMap.send(k);
                //}
                else
                {
                    //MyKeybord.send(key);
                    SendKeys.SendWait(key);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                lineCallback(e.ToString(), 0);
            }

        }


        /// <summary>
        /// test マウス
        /// </summary>
        public static void testmaouse()
        {
            //Win32Point mousePosition = new Win32Point
            //{
            //    X = 0,
            //    Y = 0
            //};

            //// マウスポインタの現在位置を取得する。
            //NativeMethods.GetCursorPos(ref mousePosition);

            //// マウスポインタの現在位置でマウスの左ボタンの押し下げ・押し上げを連続で行うためのパラメータを設定する。
            //INPUT[] inputs = new INPUT[] {
            //    new INPUT {
            //        type = NativeMethods.INPUT_MOUSE,
            //        ui = new INPUT_UNION {
            //            mouse = new MOUSEINPUT {
            //                dwFlags = NativeMethods.MOUSEEVENTF_LEFTDOWN,
            //                dx = mousePosition.X,
            //                dy = mousePosition.Y,
            //                mouseData = 0,
            //                dwExtraInfo = IntPtr.Zero,
            //                time = 0
            //            }
            //        }
            //    },
            //    new INPUT {
            //        type = NativeMethods.INPUT_MOUSE,
            //        ui = new INPUT_UNION {
            //            mouse = new MOUSEINPUT {
            //                dwFlags = NativeMethods.MOUSEEVENTF_LEFTUP,
            //                dx = mousePosition.X,
            //                dy = mousePosition.Y,
            //                mouseData = 0,
            //                dwExtraInfo = IntPtr.Zero,
            //                time = 0
            //            }
            //        }
            //    }
            //};

            //// 設定したパラメータに従ってマウス動作を行う。
            //NativeMethods.SendInput(2, ref inputs[0], Marshal.SizeOf(inputs[0]));

            //System.Windows.Forms.Screen.PrimaryScreen
            
           // Screen screen = new Screen();

            MyMouse.SendMouseMove(150, 150, System.Windows.Forms.Screen.PrimaryScreen);

        }



        #region "window"

        public static void testcalc()
        {

            // 電卓のトップウィンドウのウィンドウハンドル（※見つかることを前提としている）
            //var mainWindowHandle = Process.GetProcessesByName("calc")[0].MainWindowHandle;
            var mainWindowHandle = Process.GetProcessById(pid).MainWindowHandle;

            // 対象のボタンを探す
            var hWnd = FindTargetButton(GetWindow(mainWindowHandle));

            // マウスを押して放す
            SendMessage(hWnd, WM_LBUTTONDOWN, MK_LBUTTON, 0x000A000A);
            SendMessage(hWnd, WM_LBUTTONUP, 0x00000000, 0x000A000A);
        }

        // 全てのボタンを列挙し、その10番目のボタンのウィンドウハンドルを返す
        public static IntPtr FindTargetButton(Window top)
        {
            var all = GetAllChildWindows(top, new List<Window>());
            return all.Where(x => x.ClassName == "Button").Skip(9).First().hWnd;
        }


        // 指定したウィンドウの全ての子孫ウィンドウを取得し、リストに追加する
        public static List<Window> GetAllChildWindows(Window parent, List<Window> dest)
        {
            dest.Add(parent);
            EnumChildWindows(parent.hWnd).ToList().ForEach(x => GetAllChildWindows(x, dest));
            return dest;
        }

        // 与えた親ウィンドウの直下にある子ウィンドウを列挙する（孫ウィンドウは見つけてくれない）
        public static IEnumerable<Window> EnumChildWindows(IntPtr hParentWindow)
        {
            IntPtr hWnd = IntPtr.Zero;
            while ((hWnd = FindWindowEx(hParentWindow, hWnd, null, null)) != IntPtr.Zero) { yield return GetWindow(hWnd); }
        }

        // ウィンドウハンドルを渡すと、ウィンドウテキスト（ラベルなど）、クラス、スタイルを取得してWindowsクラスに格納して返す
        public static Window GetWindow(IntPtr hWnd)
        {
            int textLen = GetWindowTextLength(hWnd);
            string windowText = null;
            if (0 < textLen)
            {
                //ウィンドウのタイトルを取得する
                StringBuilder windowTextBuffer = new StringBuilder(textLen + 1);
                GetWindowText(hWnd, windowTextBuffer, windowTextBuffer.Capacity);
                windowText = windowTextBuffer.ToString();
            }

            //ウィンドウのクラス名を取得する
            StringBuilder classNameBuffer = new StringBuilder(256);
            GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity);

            // スタイルを取得する
            int style = GetWindowLong(hWnd, GWL_STYLE);
            return new Window() { hWnd = hWnd, Title = windowText, ClassName = classNameBuffer.ToString(), Style = style };
        }

        #endregion

    }
}
