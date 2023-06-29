using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Rpa.Util
{
    #region "キーコード"
    /*
     * https://docs.microsoft.com/ja-jp/previous-versions/windows/scripting/cc364423(v=msdn.10)?redirectedfrom=MSDN
     キー	コード
BACKSPACE	{BACKSPACE}、{BS}、または {BKSP}
BREAK	{BREAK}
CAPS LOCK	{CAPSLOCK}
DEL or DELETE	{DELETE} または {DEL}
DOWN ARROW	{DOWN}
END	{END}
ENTER	{ENTER} または ~
ESC	{ESC}
HELP	{HELP}
HOME	{HOME}
INS or INSERT	{INSERT} または {INS}
LEFT ARROW	{LEFT}
NUM LOCK	{NUMLOCK}
PAGE DOWN	{PGDN}
PAGE UP	{PGUP}
PRINT SCREEN	{PRTSC}
RIGHT ARROW	{RIGHT}
SCROLL LOCK	{SCROLLLOCK}
TAB	{TAB}
UP ARROW	{UP}
F1	{F1}
F2	{F2}
F3	{F3}
F4	{F4}
F5	{F5}
F6	{F6}
F7	{F7}
F8	{F8}
F9	{F9}
F10	{F10}
F11	{F11}
F12	{F12}
F13	{F13}
F14	{F14}
F15	{F15}
F16	{F16}
通常のキーと Shift キー、Ctrl キー、または Alt キーとの組み合わせを送信するには、キーの組み合わせを表現する文字列を引数に指定します。これを行うには、通常のキーの前に次の特殊文字を 1 つまたは複数個付加します。

テーブル 2
キー	コード
SHIFT	+
CTRL	^
ALT	%
     */
    #endregion

    static class MyRich
    {
        private static List<string> _keyword = new List<string>();


        private static List<string> _keyword2 = new List<string>();
        private static List<string> _keyword3 = new List<string>();

        public static List<string> keyword { get { return _keyword; } set { _keyword = value; } }

        public static List<string> keyword2 { get { return _keyword2; } set { _keyword2 = value; } }
        public static List<string> keyword3 { get { return _keyword3; } set { _keyword3 = value; } }
        public static void setkeyword()
        {

            keyword.Add("{F1");
            keyword.Add("{F2");
            keyword.Add("{F3");
            keyword.Add("{F4");
            keyword.Add("{F5");
            keyword.Add("{F6");
            keyword.Add("{F7");
            keyword.Add("{F8");
            keyword.Add("{F9");
            keyword.Add("{F10");
            keyword.Add("{F11");
            keyword.Add("{F12");
            keyword.Add("{F13");
            keyword.Add("{F14");
            keyword.Add("{F15");
            keyword.Add("{F16");
            keyword.Add("{BS");
            keyword.Add("{BREAK");
            keyword.Add("{CAPSLOCK");
            keyword.Add("{DEL");
            keyword.Add("{END");
            keyword.Add("{ENTER");
            keyword.Add("{ESC");
            keyword.Add("{HOME");
            keyword.Add("{INS");
            keyword.Add("{NUMLOCK");
            keyword.Add("{PGDN");
            keyword.Add("{PGUP");
            keyword.Add("{PRTSC");
            keyword.Add("{TAB");
            keyword.Add("{UP");
            keyword.Add("{DOWN");
            keyword.Add("{LEFT");
            keyword.Add("{RIGHT");
            keyword.Add("{A");
            keyword.Add("{B");
            keyword.Add("{C");
            keyword.Add("{D");
            keyword.Add("{E");
            keyword.Add("{F");
            keyword.Add("{G");
            keyword.Add("{H");
            keyword.Add("{I");
            keyword.Add("{J");
            keyword.Add("{K");
            keyword.Add("{L");
            keyword.Add("{M");
            keyword.Add("{N");
            keyword.Add("{O");
            keyword.Add("{P");
            keyword.Add("{Q");
            keyword.Add("{R");
            keyword.Add("{S");
            keyword.Add("{T");
            keyword.Add("{U");
            keyword.Add("{V");
            keyword.Add("{W");
            keyword.Add("{X");
            keyword.Add("{Y");
            keyword.Add("{Z");
            keyword.Add("{(");
            keyword.Add("{)");
            keyword.Add("{WIN");

            keyword2.Add(":wait");
            keyword2.Add(":stop");
            keyword2.Add(":loop");
            keyword2.Add(":active");
            keyword2.Add(":getpid");
            keyword2.Add(":clear");
            keyword2.Add(":password");

            //keyword2.Add(":if"); //if()


            keyword3.Add("^");
            keyword3.Add("%");
            keyword3.Add("+");

        }

        public static string keyword_color(string text)
        {
            setkeyword();


            text = text.Replace("\\", "\\\\");
            text = "\\cf3 " + text;
            //text = text.Replace("\n", "\a\n\a");
            //text = text.Replace("\r", "\a\r\a");
            //text = text.Replace("\t", "\a\t\a");
            ////text = text.Replace("-", "\a -\a");
            //text = text.Replace("=", "\a=\a");
            //text = text.Replace("(", "\a(\a");
            //text = text.Replace(")", "\a)\a");
            //text = text.Replace("[", "\a[\a");
            //text = text.Replace("]", "\a]\a");
            ////text = text.Replace("{", "\a{\a");
            text = text.Replace("}", "\a}\a");
            //text = text.Replace("<", "\a <\a");
            //text = text.Replace(">", "\a >\a");
            //text = text.Replace(".", "\a.\a");
            //text = text.Replace(",", "\a,\a");
            //text = text.Replace(";", "\a;\a");
            //text = text.Replace(" ", "\a");
            for (int i = 0; i < keyword.Count; i++)
            {
                string keycmd = keyword[i].Replace("{", "");
                int cnt = keycmd.Count();
                text = text.Replace(keyword[i] + "\a}\a", "{\\cf1 " + keycmd + "\\cf3 }");
                text = text.Replace(keyword[i] + " ", "{\\cf1 " + keycmd + "\\cf3 \a\a");

                keycmd = keyword[i].Replace("{", "").ToLower();
                text = text.Replace("{" + keycmd + "\a}\a", "{\\cf1 " + keycmd + "\\cf3 }");
                text = text.Replace("{" + keycmd + " ", "{\\cf1 " + keycmd + "\\cf3 \a\a");

                if (cnt > 1)
                {
                    keycmd = keycmd.Substring(0, 1).ToUpper() + keycmd.Substring(1, keycmd.Length - 1);
                    text = text.Replace("{" + keycmd + "\a}\a", "{\\cf1 " + keycmd + "\\cf3 }");
                    text = text.Replace("{" + keycmd + " ", "{\\cf1 " + keycmd + "\\cf3 \a\a");
                }
            }

            for (int i = 0; i < keyword2.Count; i++)
            {
                text = text.Replace(keyword2[i], "\\cf4 " + keyword2[i] + "\\cf3 ");
            }


            for (int i = 0; i < keyword3.Count; i++)
            {
                text = text.Replace(keyword3[i], "\\cf4 " + keyword3[i] + "\\cf3 ");
            }

            //text = text.Replace("\a\n\a", "\n");
            //text = text.Replace("\a\r\a", "\r");
            //text = text.Replace("\a\t\a", "\t");
            ////text = text.Replace("\a -\a", "-");
            //text = text.Replace("\a=\a", "=");
            //text = text.Replace("\a(\a", "(");
            //text = text.Replace("\a)\a", ")");
            //text = text.Replace("\a[\a", "[");
            //text = text.Replace("\a]\a", "]");
            ////text = text.Replace("\a{\a", "{");
            text = text.Replace("\a}\a", "\\cf3}");
            //text = text.Replace("\a<\a", "<");
            //text = text.Replace("\a>\a", ">");
            //text = text.Replace("\a.\a", ".");
            //text = text.Replace("\a,\a", ",");
            //text = text.Replace("\a;\a", ";");
            text = text.Replace("{", "\\{");
            text = text.Replace("}", "\\}");
            text = text.Replace("\a\a", " ");



            //コメント
            StringReader sr = new StringReader(text);
            string buff = "";
            List<string> linebuff = new List<string>();
            while (buff != null)
            {
                buff = sr.ReadLine();

                if (buff == null) break;

                int p1 = buff.IndexOf("#");

                string param = "";
                //先頭が　＃
                if (p1 != -1)
                {
                    param = "#";
                    //先頭が　＃
                    if (buff.StartsWith(param, StringComparison.Ordinal))
                    {
                        linebuff.Add("\\cf5 " + buff + "\\cf3 " + "\r\n");
                    }
                    //先頭が　"\\cf5"　Green
                    else if (buff.Length > 5 && buff.Substring(0, 6) == "\\cf5 "+ param)
                    {
                        linebuff.Add(buff + "\r\n");
                    }
                    //先頭が　"\\cf* #"　#が５番目
                    else if (p1 == 5)
                    {
                        buff = "\\cf5 " + buff.Substring(5, buff.Length - 5);

                        linebuff.Add(buff + "\r\n");
                    }
                    else
                    {
                        linebuff.Add(buff + "\r\n");

                    }
                }
                ////先頭が　:
                //else if (p2 != -1)
                //{

                //}
                else
                {
                    linebuff.Add(buff + "\r\n");
                }
            }


            text = header() + string.Concat(linebuff) + "\n}";
            text = text.Replace("\n", "\\par\n");

            return text;
        }

        private static string header()
        {
            string header = "";
            header += "{\\rtf1\\ansi\\ansicpg932\\deff0\\deflang1033\\deflangfe1041{\\fonttbl{\\f0\\fnil";
            header += "\\fcharset128 \\'82\\'6c\\'82\\'72 \\'83\\'53\\'83\\'56\\'83\\'62\\'83\\'4e;}}"; //ＭＳ ゴシック
            header += "{\\colortbl ;\\red255\\green0\\blue0;"; //cf1:keyword Red
            header += "\\red255\\green255\\blue255;"; //cf2:背景 White
            header += "\\red0\\green0\\blue0;"; //cf3:通常文 Black
            header += "\\red255\\green0\\blue255;"; //cf4: リテラル Pink
            header += "\\red0\\green128\\blue0;}"; //cf5: コメント Green
            header += "\\viewkind4\\uc1\\pard\\li75\\tx345\\tx690\\tx1020\\tx1365\\tx1710\\tx2055\\tx2385\\tx2730\\tx3075\\tx3420\\tx3750\\tx4095";
            header += "\\tx4440\\tx4785\\tx5115\\tx5460\\tx5805\\tx6150\\tx6480\\tx6825\\tx7170\\tx7515\\tx7845\\tx8190\\tx8535\\tx8880\\tx9210";
            header += "\\tx9555\\tx9900\\tx10245\\tx10575\\tx10920\\cf1\\highlight2\\lang1041\\f0\\fs24 "; //フォントサイズ: fs24=12pt
            return header;
        }

        //private static string header(){
        //    string header="";
        //    header += "{\\rtf1\\ansi\\deff0{\\fonttbl{\\f0\\fnil\\fcharset128 \\’82\\’6c\\’82\\’72 \\’83\\’53\\’83\\’56\\’83\\’62\\’83\\’4e;}}";
        //    //header += "{\\colortbl ;\\red0\\green0\\blue255;\\red255\\green255\\blue255;\\red0\\green0\\blue0;\\red255\\green0\\blue255;\\red0\\green128\\blue0;}";
        //    header += "{\\colortbl ;\\red255\\green0\\blue0;\\red255\\green255\\blue255;\\red0\\green0\\blue0;\\red255\\green0\\blue255;\\red0\\green128\\blue0;}";
        //    header += "\\viewkind4\\uc1\\pard\\li75\\tx345\\tx690\\tx1020\\tx1365\\tx1710\\tx2055\\tx2385\\tx2730\\tx3075\\tx3420\\tx3750\\tx4095";
        //    header += "\\tx4440\\tx4785\\tx5115\\tx5460\\tx5805\\tx6150\\tx6480\\tx6825\\tx7170\\tx7515\\tx7845\\tx8190\\tx8535\\tx8880\\tx9210";
        //    header += "\\tx9555\\tx9900\\tx10245\\tx10575\\tx10920\\cf1\\highlight2\\lang1041\\f0\\fs20 ";
        //    return header;
        //}
    }
}
