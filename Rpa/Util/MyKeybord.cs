using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rpa.Util
{
    static class MyKeybord
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_EXTENDEDKEY = 1;
        private const int KEYEVENTF_KEYUP = 2;

        #region "キーボード"
        public static void KeyDown(Keys vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
        }

        public static void KeyUp(Keys vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        #endregion

        //KeyboardSend.KeyDown(Keys.LWin);
        //KeyboardSend.KeyDown(Keys.D4);
        //KeyboardSend.KeyUp(Keys.LWin);
        //KeyboardSend.KeyUp(Keys.D4);

        public static void send(string key)
        {


        }

    }
}
