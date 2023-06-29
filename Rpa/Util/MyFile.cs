using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rpa.Util
{
    static class MyFile
    {

        private static Dictionary<string, Dictionary<string, string>> _d = new Dictionary<string, Dictionary<string, string>>();

        private static List<string> _l = new List<string>();
        public static List<string> funcl { get { return _l; } set { _l = value; } }
        public static Dictionary<string, Dictionary<string, string>> d { get { return _d; } set { _d = value; } }

        public static void read(string buf)
        {
            //正規表現
            var regexSection = new Regex(@"^\s*\[(?<section>[^\]]+)\].*$", RegexOptions.Singleline | RegexOptions.CultureInvariant);
            //var regexNameValue = new Regex(@"^\s*(?<name>[^=]+)=(?<value>.*?)(\s+;(?<comment>.*))?$", RegexOptions.Singleline | RegexOptions.CultureInvariant);
            var regexName = new Regex(@"^\>\s*(?<name>[^=]+)$", RegexOptions.Singleline | RegexOptions.CultureInvariant);
            string currentSection = "";

            StringReader sr = new StringReader(buf);

            funcl.Clear();
            d.Clear();

            int i = 0;
            int cnt = 0;
            string buff = "";
            bool flag_cmd = false;
            while (buff != null)
            {
                flag_cmd = false;

                buff = sr.ReadLine();
                
                //Console.WriteLine(">>" + buff);

                if (buff == null) break;

                //空行
                if (buff.Length == 0) continue;

                //コメントアウト
                if (buff.StartsWith(";", StringComparison.Ordinal)) continue;

                if (buff.StartsWith("#", StringComparison.Ordinal)) continue;

                if (buff.StartsWith(":", StringComparison.Ordinal)) flag_cmd = true;

                //[ SECTTION ]
                var matchSection = regexSection.Match(buff);

                //[SECTION]以外
                if (!matchSection.Success)
                {
                    if (flag_cmd)
                    {

                        cnt = 1 + d[currentSection].Count;
                        d[currentSection][cnt.ToString()] = buff_remove_start_tab(buff);
                    }
                    else
                    {

                        int p1 = buff.IndexOf(',');
                        int p2 = buff.IndexOf('=');

                        string[] w = buff.Split(',');
                        // ","無の場合
                        if (w.Count() == 1 )
                        {
                            cnt = 1 + d[currentSection].Count;
                            d[currentSection][cnt.ToString()] = buff_remove_start_tab(buff);
                        }
                        // ","有の場合
                        else
                        {
                            string[] ws = w[1].Split('=');  //{TAB},loop=3
                            string[] ws2 = buff.Split('='); //getpid=notepad,無題

                            //{TAB},loop=3
                            if (ws[0] == "loop")
                            {
                                for (i = 0; int.Parse(ws[1]) > i; i++)
                                {
                                    cnt = 1 + d[currentSection].Count;
                                    d[currentSection][cnt.ToString()] = w[0];
                                }
                            }
                        }
                    }

                    //[main]の場合
                    if (currentSection == "main")
                    {
                        funcl.Add(buff_remove_start_tab(buff));
                    }

                    continue;
                }


                //[ SECTTION ]
                if (matchSection.Success)
                {
                    // [section]の行
                    currentSection = matchSection.Groups["section"].Value;

                    if (!d.ContainsKey(currentSection))
                    {
                        d[currentSection] = new Dictionary<string, string>();
                    }

                    continue;
                }

                //l.Add(sWork);
            }



        }

        /// <summary>
        /// 先頭のTABを削除
        /// </summary>
        /// <returns></returns>
        private static string buff_remove_start_tab(string buff)
        {
            string rlt = "";

            //if(buff.StartsWith("\t", StringComparison.Ordinal) && buff.Length > 1)
            //{
            //    rlt = buff.Substring(1, buff.Length - 1);
            //}

            rlt = buff;

            return rlt;
        }

        public static object regedit = new object();

        public static string read_str(string path)
        {
            //string path = System.Environment.CurrentDirectory + "\\" + fname;

            //存在しない
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            string readText = "";
            lock (regedit)
            {
                readText = File.ReadAllText(path);
            }
            return readText;
        }

        public static void write_str(string path, string buff)
        {
            lock (regedit)
            {
                //string path = System.Environment.CurrentDirectory + "\\" + fname;
                //string readText = File.ReadAllText(path);
                File.WriteAllText(path, buff, Encoding.UTF8);
            }
        }



    }
}
