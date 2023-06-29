using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rpa.Util;

namespace Rpa.Control
{
    public partial class UserText : UserControl
    {
        private static string _logbuf;

        private static RichTextBox _text;

        private static List<string> buf;
        MyKey mykey;

        public static string logbuf { get { return _logbuf; } set { _logbuf = value; } }
        public static RichTextBox text { get { return _text; } set { _text = value; } }

        public UserText()
        {
            InitializeComponent();

            buf = new List<string>();

            //コールバック関数紐づけ
            mykey = new MyKey(lineCallback, endCallback, clearCallback);

            //コールバック関数紐づけ
            //mykey.lineCallback = lineCallback;
        }


        public void main(RichTextBox t, string text_pid)
        {
            try
            {

                buf.Clear();
                text = t;
                text.Enabled = false;

                richTextBox1.Text = "";

                //mykey.stop();
                // 選択しているプロセスをアクティブ
                //MyKey.active(text_pid);
                // キーストロークを送信
                mykey.read(text.Text);
                //MyKey.starttimer();
                mykey.work();

            }
            catch (Exception e)
            {
                Console.WriteLine("error >" + e.ToString());
                lineCallback("書式が間違っている可能性があります。", 2);
                lineCallback(e.ToString(), 2);
                text = t;
                text.Enabled = true;
            }

        }

        public void stop()
        {
            mykey.stop();
        }

        public void get_keymap()
        {

            MyKey.MapKeyData();
        }

        /// <summary>
        /// コールバック
        /// </summary>
        /// <param name="str"></param>
        private void lineCallback(string str, int stat)
        {
           // Console.WriteLine("処理中 >" + str);

            try
            {
                // 先頭に挿入
                //buf.Insert(0, str + "\r\n");
                buf.Add(str + "\r\n");

                logbuf = str + "\r\n";

                //Invokeが必要か確認する
                if (this.InvokeRequired)
                {
                    //Invokeメソッドを使ってコントロールへアクセスする
                    this.Invoke(new Action<string, int>(this.UpdateText),str,stat);
                }
                else
                {
                    UpdateText(str, stat);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        /// <summary>
        /// テキストを更新する。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        private void UpdateText(string str, int stat)
        {
            //string bufstr = string.Concat(buf);

            richTextBox1.HideSelection = false;
            
            //バッファ行数を超えたら
            if(buf.Count > 5000)
            {
                //List<string> lines = new List<string>(textBox1.Lines);
                //lines.RemoveAt(0); // 0行目削除
                buf.RemoveAt(0);
                this.richTextBox1.Text = string.Concat(buf);
            }
            else
            {
                if(stat == 1)
                {
                    richTextBox1.SelectionColor = Color.Green;
                }
                else if(stat == 2)
                {
                    richTextBox1.SelectionColor = Color.Red;
                }
                else
                {
                    richTextBox1.SelectionColor = Color.Black;
                }


                this.richTextBox1.AppendText(logbuf);

            }

        }


        /// <summary>
        /// スレッドの終了
        /// </summary>
        /// <param name="str"></param>
        private void endCallback(string str)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    //Invokeメソッドを使ってコントロールへアクセスする
                    this.Invoke(new Action<string>(this.endText), str);
                }
                else
                {
                    endText(str);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        /// <summary>
        /// スレッドの終了。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        private void endText(string str)
        {

            //this.richTextBox1.AppendText(str + "\r\n");
            text.Enabled = true;
        }

        

        /// <summary>
        /// テキストクリア
        /// </summary>
        /// <param name="str"></param>
        private void clearCallback()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    //Invokeメソッドを使ってコントロールへアクセスする
                    this.Invoke(new Action(this.clearText));
                }
                else
                {
                    clearText();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private void clearText()
        {
            richTextBox1.Text = "";
        }

    }
}
