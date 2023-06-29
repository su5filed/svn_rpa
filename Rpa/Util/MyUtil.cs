using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpa.Util
{
    class MyUtil
    {
        private const string FILENAME = "\\debuglocal.log";

        //排他
        public static object regedit = new object();

        ///// <summary>
        ///// 実行ファイル直下にログを出力(デバッグ用)
        ///// </summary>
        ///// <param name="log">出力する内容</param>
        //public static void DebugLocalLog(string log)
        //{
        //    DateTime dt = DateTime.Now;
        //    //1を渡しているのは、「一つ前の」スタックを参照する
        //    StackFrame CallStack = new StackFrame(1, true);
        //    //メソッド名　ライン
        //    string method_line = " " + CallStack.GetMethod().Name + "(" + CallStack.GetFileLineNumber() + ") ";

        //    string path = System.Environment.CurrentDirectory + FILENAME;
        //    string output = dt.ToString("yyyy-MM-dd HH:mm:ss") + method_line + log + Environment.NewLine;
        //    File.AppendAllText(path, output);
        //}

        public static string read(string fname)
        {
            string path = System.Environment.CurrentDirectory + "\\" + fname;

            //存在しない
            if (!File.Exists(path)) return string.Empty;
            string readText = "";
            lock (regedit)
            {
                readText = File.ReadAllText(path);
            }
            return readText;
        }

        public static void write(string fname, string buff)
        {
            lock (regedit) {
                string path = System.Environment.CurrentDirectory + "\\" + fname;
                //string readText = File.ReadAllText(path);
                File.WriteAllText(path, buff, Encoding.UTF8);
            }
        }


        public static void debuglog(string buf)
        {

            string path = Directory.GetCurrentDirectory() + "debuglog.txt";
            DateTime dt = DateTime.Now;

            string timebuf = dt.ToString("yyyy/MM/dd HH:mm:ss ");

            //「一つ前の」スタックを参照する
            StackFrame CallStack = new StackFrame(1, true);

            string sourceline = "";
            string fname = "";
            if (CallStack.GetFileName().Length > 0)
            {
                fname = Path.GetFileName(CallStack.GetFileName());

            }
            sourceline = fname + ":" + CallStack.GetMethod().Name + "(" + CallStack.GetFileLineNumber() + ")";


            string linebuf = timebuf + sourceline + buf + "\r\n";

            File.AppendAllText(path, linebuf);
        }
    }
}
