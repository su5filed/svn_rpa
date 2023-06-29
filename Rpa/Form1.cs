using Rpa.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

using Rpa.Control;

namespace Rpa
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("USER32.dll")]
        private static extern IntPtr SendMessage(System.IntPtr hWnd, Int32 Msg, Int32 wParam, ref Point lParam);

        private static List<string> buf;
        System.Timers.Timer timer;
        private UserText usertext;// = new UserText();
        private TabText TabText;// = new UserText();

        private static bool richtextfirst = false;

        //Form3 f3;

        public Form1()
        {
            buf = new List<string>();

            timer = new System.Timers.Timer(1 * 1000);
            timer.Elapsed += MyClockTextView;
            timer.Enabled = true;

            richtextfirst = false;

            //user = null;
            InitializeComponent();

            //f3 = new Form3();

            init();

        }

        private void init()
        {
            //UserText
            usertext = new UserText();
            Point pos = new Point(0, 0);
            usertext.Location = pos;
            Size s = new Size(230, 335);
            usertext.Size = s;
            usertext.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            //TabText
            TabText = new TabText();
            pos = new Point(0, 0);
            TabText.Location = pos;
            s = new Size(splitContainer1.Panel1.Width, splitContainer1.Panel1.Height);
            TabText.Size = s;
            TabText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            //TabText.tab_add();

            //フォームにコントロールを追加
            this.splitContainer1.Panel2.Controls.Add(usertext);
            //フォームにコントロールを追加
            this.splitContainer1.Panel1.Controls.Add(TabText);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //richTextBox1.Text = MyFile.read_str("setting.txt");
            //richTextBox1.Rtf = MyRich.keyword_color(richTextBox1.Text);


            TabText.tab_add();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 選択しているプロセスをアクティブ
            //MyKey.active(comboBox1.SelectedValue.ToString());
            // キーストロークを送信
            //SendKeys.Send(textBox1.Text);
            //MyKey.sendkey(textBox1.Text);

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// タイマー処理、定期的にログ表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyClockTextView(object sender, EventArgs e)
        {
            //string bufstr = MyUtil.read();

            //if (bufstr.Length == 0) return;

            //try
            //{
            //    //textBox2.Text = bufstr;

            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}
        }

        //マウス
        private void button4_Click(object sender, EventArgs e)
        {

            //MyKey.testmaouse();
            //MyKey.testcalc();
        }

        private void button5_Click(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //キー一覧を取得
            usertext.get_keymap();

            //MyKeyMap.MapKeyData();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //this.Visible = false;
            //SendKeys.SendWait(Keys.LWin.ToString());
            //SendKeys.Send("+" + Keys.LWin.ToString() + "{LEFT}");
            //this.Visible = true;

            MyKey.getprocess();


        }

        private void button9_Click(object sender, EventArgs e)
        {
            //MyFile.write_str("setting.txt", richTextBox1.Text);
        }

        /// <summary>
        /// https://social.msdn.microsoft.com/Forums/ja-JP/66e306f3-5bda-4c11-b9b7-ff30c5cb8f39/richtextboxrtf?forum=csharpgeneralja
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //this.Focus();
                //Enabled = false; //なのでコメントにする
                ////現在の選択状態を覚えておく
                //int currentSelectionStart = richTextBox1.SelectionStart;
                //int currentSelectionLength = richTextBox1.SelectionLength;
                //// 文字キーワードの色（マジェンタ）
                ////Rtf = TextColorSet.keyword(Text, keyword);

                ////選択状態を元に戻す
                //richTextBox1.Select(currentSelectionStart, currentSelectionLength);
                ////textBox.Font = font;
                //Enabled = true;
                //Focus();

                if (!richtextfirst) return;

                //richTextBox1.Rtf = MyRich.keyword_color(richTextBox1.Text);

                //// 指定のスクロール位置を復元
                //SendMessage(this.richTextBox1.Handle, 0x04DE, 0, ref currentScrolPos);

                //richTextBox1.SelectionStart = currentPos;

            }
            catch { }
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {


        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void richTextBox1_KeyUp_1(object sender, KeyEventArgs e)
        {

        }

        private void richTextBox1_KeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            //コマンド実行
            usertext.main(TabText.getRichText(), "");

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

            //MyKey.stopflag = true;
            usertext.stop();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //OpenDialog.open();

            TabText.tab_add();


        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            
            TabText.open();

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            //TabText.tab_add();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            //名前つけて保存
            TabText.save();

        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {

            //上書き保存
            TabText.save_sub("");
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {

            Form2 f2 = new Form2();
            f2.Show();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            this.Height = 80;
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {

            this.Height = 423;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            TabText.undo();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {

            TabText.redo();
        }

        private void toolStripButton4_MouseHover(object sender, EventArgs e)
        {
            //toolStripButton4.Text = TabText.undo_cnt();

        }

        private void toolStripButton13_MouseHover(object sender, EventArgs e)
        {
            //toolStripButton13.Text = TabText.redo_cnt();

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            TabText.f3.Show();
        }
    }


}
