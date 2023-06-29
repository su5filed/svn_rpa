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
using System.IO;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;

namespace Rpa.Control
{
    public partial class TabText : UserControl
    {

        [DllImport("Imm32.dll", SetLastError = true)]
        static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        static extern int ImmGetCompositionString(IntPtr hIMC, GCS dwIndex, IntPtr pBuff, int dwBufLen);

        [DllImport("Imm32.dll")]
        static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [System.Runtime.InteropServices.DllImport("USER32.dll")]
        private static extern IntPtr SendMessage(System.IntPtr hWnd, Int32 Msg, Int32 wParam, ref Point lParam);

        //RichTextの行数取得
        [System.Runtime.InteropServices.DllImport("User32.Dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int EM_LINEINDEX = 0xBB;
        private const int EM_LINEFROMCHAR = 0xC9;

        private Form3 _f3;

        public Form3 f3 { get { return _f3; } set { _f3 = value;  } }

        #region "undo"
        //private LinkedList<UndoData> undoList;
        //private LinkedList<UndoData> redoList;
        private int UndoMax;

        private static bool richtextundo = false;
        private static bool richtextredo = false;

        #endregion

        struct RICHINFO
        {

            public LinkedList<UndoData> undoList;
            public LinkedList<UndoData> redoList;
            public RichTextBox richTextBox1;
            public string filepath;//ファイルフルパス
        }

        static List<RICHINFO> rich = new List<RICHINFO>();

        static TabControl tabControl = null;
        static int page_cnt = 0;

        private static bool richtextcmd = false;
        private static bool richtextfirst = false;
        private static int currentPos;
        private static Point currentScrolPos;


        public RichTextBox richtext { 
            get 
            { 
                int index = tabControl.SelectedIndex;

                return rich.ElementAt(index).richTextBox1;
            }
            set
            {
                //int index = tabControl1.SelectedIndex;
                //richTextBox.ElementAt(index) = value;
            }
        }

        public TabText()
        {
            InitializeComponent();

            //タブ作成
            tabControl = new TabControl();
            Point pos = new Point(0, 0);
            tabControl.Location = pos;
            Size s = new Size(this.Width, this.Height );
            tabControl.Size = s;
            tabControl.MouseClick += tabControl_MouseClick;
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            //コントロールに追加
            this.Controls.Add(tabControl);

            f3 = new Form3();

            #region "undo"

            UndoMax = 1000;
            #endregion

        }

        

        private void tabControl_MouseClick(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();

        }

        public void tab_add()
        {

            //以下button1のクリックイベント内
            string title = "無題" + page_cnt.ToString();

            tabControl.TabPages.Add(title);
            int index = tabControl.TabCount - 1;
            tabControl.SelectedIndex = index;

            //RichText
            RICHINFO r = new RICHINFO();
            r.richTextBox1 = new RichTextBox();
            Point pos = new Point(0, 0);
            r.richTextBox1.Location = pos;
            Size s = new Size(tabControl.Width , tabControl.Height - 20);
            r.richTextBox1.Size = s;
            r.richTextBox1.TextChanged += richTextBox_TextChanged;   //イベント登録
            r.richTextBox1.KeyDown += richTextBox_KeyDown;
            r.richTextBox1.KeyUp += richTextBox_KeyUp;
            r.richTextBox1.PreviewKeyDown += richTextBox_PreviewKeyDown;
            r.richTextBox1.MouseDown += richTextBox_MouseDown;
            r.richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            r.richTextBox1.Multiline = true;
            r.richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
            r.richTextBox1.WordWrap = false;
            //Tabキーでタブ記号が入力されるようにする
            //r.richTextBox1.AcceptsTab = true;

            //ファイルパス
            r.filepath = "";

            r.undoList = new LinkedList<UndoData>();
            r.redoList = new LinkedList<UndoData>();

            rich.Add(r);

            //コントロールに追加
            tabControl.SelectedTab.Controls.Add(rich.ElementAt(page_cnt).richTextBox1);

            //フォーカス
            rich.ElementAt(page_cnt).richTextBox1.Focus();

            page_cnt++;



        }

        public void tab_delete()
        {
            //閉じる前に変更があれば保存聞く
            //tab
            int index = tabControl.SelectedIndex;
            //タブ名
            string filename = tabControl.TabPages[index].Text;

            //filename = ;
            if (filename.IndexOf('*') == -1)
            {
                //tabControl.TabPages[index].Text = "*" + filename;
                //保存するか？
            }

            //ファイル名登録
            //RICHINFO r;
            //r = rich[index];
            //r.filepath = "";
            //rich[index] = r;

            Console.WriteLine("rich.Count=" + rich.Count + "index=" + index);
            
            if(rich.Count > index)
            {
                rich.RemoveAt(index);
                page_cnt--;
            }

            //button2のクリックイベントに記入
            tabControl.TabPages.Remove(tabControl.SelectedTab);

            if (tabControl.TabCount == 0)
            {
                tab_add();
            }


        }

        public void open()
        {
            string path = OpenDialog.open();

            int index = tabControl.SelectedIndex;

            int findindex = 0;

            if (path.Length == 0) return;

            Console.WriteLine("既に開かれていかーーーーーーーーーーーーーーー");
            //既に開かれていれば開かない
            if (!checkTabname(path, ref findindex))
            {
                Console.WriteLine("既に開かれている");
                tabControl.SelectedIndex = findindex;
                return;
            }

            //ファイル開く
            richtext.Text = MyFile.read_str(path); ;

            richtext.Rtf = MyRich.keyword_color(richtext.Text);

            //ファイル名登録
            RICHINFO r;
            r = rich[index];
            //r.filename = /*path*/;
            r.filepath = path;
            rich[index] = r;

            resetclear();

            //タブ名
            tabControl.TabPages[index].Text = Path.GetFileName(path);

            //------------------------
            //現在の記録
            rich[index].undoList.AddFirst(this.getUndoData());
            //UndoMaxを超えたら古いデータを削除
            if (rich[index].undoList.Count > UndoMax)
            {
                for (int i = UndoMax; i < rich[index].undoList.Count; i++) rich[index].undoList.RemoveLast();
            }

        }


        public void save()
        {
            string path = OpenDialog.save();

            int index = tabControl.SelectedIndex;

            if (path.Length == 0) return;

            //保存
            save_sub(path);

        }


        /// <summary>
        /// 既に開かれているか
        /// </summary>
        /// <returns></returns>
        private bool checkTabname(string filename, ref int findindex)
        {
            for (int i=0; rich.Count>i;  i++)
            {
                Console.WriteLine("filename = " + filename);
                Console.WriteLine("{0}:{1}", i, rich.ElementAt(i).filepath);
                string filepath = rich.ElementAt(i).filepath;
                if ( filepath.Length > 0)
                if ( filepath == filename)
                {
                    findindex = i;
                    return false;
                }
            }

            return true;
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {

            int index = tabControl.SelectedIndex;
            try
            {
                if (!richtextfirst) return;


            }
            catch { }
        }


        private void richTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            int index = tabControl.SelectedIndex;


            if (!richtextfirst)
            {


                //false未確定文字
                ImmData moji = GetIMEString(false);

                //現在の行を取得する
                int row = SendMessage(richtext.Handle, EM_LINEFROMCHAR, -1, 0);

                //現在の列を取得する
                int lineIndex = SendMessage(richtext.Handle, EM_LINEINDEX, -1, 0);
                int col = richtext.SelectionStart - lineIndex + 1;

                Console.WriteLine("行:{0} 列:{1}", row, col);

                //変換中は処理しない
                if (moji.Composition == null || moji.Composition.Length > 0)
                {
                    return;
                }

                //フォーカス位置
                currentPos = richtext.SelectionStart;

                //スクロール位置を取得
                SendMessage(this.richtext.Handle, 0x04DD, 0, ref currentScrolPos);

                 if (richtextundo)
                {
                    Console.WriteLine("up undo currentPos="+ currentPos);

                    undo();
                    


                }
                else if (richtextredo)
                {
                    Console.WriteLine("up redo");

                    redo();

                }
                else
                {
                    richtext.Rtf = MyRich.keyword_color(richtext.Text);

                    // 指定のスクロール位置を復元
                    SendMessage(this.richtext.Handle, 0x04DE, 0, ref currentScrolPos);
                    richtext.SelectionStart = currentPos;


                    ////画面ロック解除
                    //SendMessage(richtext.Handle, WM_SETREDRAW, 1, 0);
                    //richtext.Enabled = true;
                    //richtext.Focus();
                }




                //richtext.Focus();

                //Ctrl + N, O, S 以外
                if ((!richtextcmd))
                {
                    // ↓↑→←以外
                    if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down && e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                    {
                        richtext_kousin();
                    }
                }


            }
            else
            {
                if (!e.Shift)
                {
                    richtextfirst = false;
                }

            }
        }

        private void richTextBox_KeyDown(object sender, KeyEventArgs e)
        {

            int index = tabControl.SelectedIndex;

            if (e.Shift)
            {
                richtextfirst = true;
            }

            richtextcmd = false;

            // Ctrl + N
            if (e.Control && e.KeyCode == Keys.N)
            {
                richtextcmd = true;
                tab_add();
                return;
            }

            // Ctrl + O
            if (e.Control && e.KeyCode == Keys.O)
            {
                richtextcmd = true;
                open();
                return;
            }

            //Ctrl + S
            if (e.Control && e.KeyCode == Keys.S)
            {
                Console.WriteLine("保存");
                richtextcmd = true;
                save_sub("");
                return;
            }

            #region "undo"

            richtextundo = false;

            //Undo
            if (e.KeyCode == Keys.Z && e.Control && rich[index].undoList.Count > 0)
            {
                richtextundo = true;
            }

            richtextredo = false;

            //Redo
            if (e.KeyCode == Keys.Y && e.Control && rich[index].redoList.Count > 0)
            {
                richtextredo = true;
            }

            if (
                e.KeyCode == Keys.Enter || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Space ||
                //(e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down) || 
                //(e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Left) ||
                ((!e.Control) && (e.KeyCode > Keys.A && e.KeyCode < Keys.Z)) ||
                ((!e.Shift) && (e.KeyCode > Keys.A && e.KeyCode < Keys.Z)) ||
                ((!e.Alt) && (e.KeyCode > Keys.A && e.KeyCode < Keys.Z))
                )
            {
                //------------------------
                if (!richtextundo)
                {
                    rich[index].undoList.AddFirst(this.getUndoData());
                    //UndoMaxを超えたら古いデータを削除
                    if (rich[index].undoList.Count > UndoMax)
                    {
                        for (int i = UndoMax; i < rich[index].undoList.Count; i++) rich[index].undoList.RemoveLast();
                    }
                }
            }





            Console.WriteLine("down richtextundo=" + richtextundo + ",undoList.Count=" + rich[index].undoList.Count);
            Console.WriteLine("down richtextredo=" + richtextredo + ",undoList.Count=" + rich[index].undoList.Count);

            #endregion

        }


        //Undo
        private void richTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            int index = tabControl.SelectedIndex;
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Space )
            {
                rich[index].undoList.AddFirst(this.getUndoData());
                //UndoMaxを超えたら古いデータを削除
                if (rich[index].undoList.Count > UndoMax)
                {
                    for (int i = UndoMax; i < rich[index].undoList.Count; i++) rich[index].undoList.RemoveLast();
                }
            }

        }
        private void richTextBox_MouseDown(object sender, MouseEventArgs e)
        {

            int index = tabControl.SelectedIndex;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                rich[index].undoList.AddFirst(this.getUndoData());
                //UndoMaxを超えたら古いデータを削除
                if (rich[index].undoList.Count > UndoMax)
                {
                    for (int i = UndoMax; i < rich[index].undoList.Count; i++) rich[index].undoList.RemoveLast();
                }
            }

        }



        /// <summary>
        /// 戻すのやり直し
        /// </summary>
        public void redo()
        {

            int index = tabControl.SelectedIndex;

            if (rich[index].redoList.First != null)
            {

                //画面ロック
                //SendMessage(richtext.Handle, WM_SETREDRAW, 0, 0);
                //richtext.Enabled = false;

                //Redoに現状保持
                rich[index].undoList.AddFirst(this.getUndoData());
                //復元
                UndoData redo = rich[index].redoList.First.Value;
                richtext.Text = redo.Text;
                richtext.SelectionStart = redo.Caret;

                //フォーカス位置
                currentPos = richtext.SelectionStart;

                //色つけ
                richtext.Rtf = MyRich.keyword_color(richtext.Text);

                // 指定のスクロール位置を復元
                SendMessage(this.richtext.Handle, 0x04DE, 0, ref currentScrolPos);
                richtext.SelectionStart = currentPos;

                //Point point = redo.Scroll;
                //SendMessage(richtext.Handle, 0x04DE, 0, ref point);

                rich[index].redoList.RemoveFirst();
                //UndoMaxを超えたら古いデータを削除
                if (rich[index].undoList.Count > UndoMax)
                {
                    for (int i = UndoMax; i < rich[index].undoList.Count; i++) rich[index].undoList.RemoveLast();
                }

                //SendMessage(richtext.Handle, WM_SETREDRAW, 1, 0);
                //richtext.Enabled = true;
                //richtext.Focus();
            }
        }

        /// <summary>
        /// 戻す
        /// </summary>
        public void undo()
        {

            int index = tabControl.SelectedIndex;
            if (rich[index].undoList.First != null)
            {

                //画面ロック
                //SendMessage(richtext.Handle, WM_SETREDRAW, 0, 0);
                //richtext.Enabled = false;

                //Redoに現状保持
                rich[index].redoList.AddFirst(this.getUndoData());
                //復元
                UndoData undo = rich[index].undoList.First.Value;
                richtext.Text = undo.Text;
                richtext.SelectionStart = undo.Caret;

                //フォーカス位置
                currentPos = richtext.SelectionStart;

                //色つけ
                richtext.Rtf = MyRich.keyword_color(richtext.Text);

                // 指定のスクロール位置を復元
                SendMessage(this.richtext.Handle, 0x04DE, 0, ref currentScrolPos);
                richtext.SelectionStart = currentPos;

                rich[index].undoList.RemoveFirst();
                //UndoMaxを超えたら古いデータを削除
                if (rich[index].redoList.Count > UndoMax)
                {
                    for (int i = UndoMax; i < rich[index].redoList.Count; i++) rich[index].redoList.RemoveLast();
                }

                //ロック解除
                //SendMessage(richtext.Handle, WM_SETREDRAW, 1, 0);
                //richtext.Enabled = true;
            }

        }


        public string undo_cnt()
        {
            int cnt = 0;
            int index = tabControl.SelectedIndex;
            if (rich[index].undoList != null)
            {
                cnt = rich[index].undoList.Count;
            }

            return "Undo :" + cnt.ToString();

        }


        public string redo_cnt()
        {
            int cnt = 0;
            int index = tabControl.SelectedIndex;
            if (rich[index].redoList != null)
            {
                cnt = rich[index].redoList.Count;
            }

            return "Redo :" + cnt.ToString();

        }


        /// <summary>
        /// リセット
        /// </summary>
        private void resetclear()
        {
            //tab
            int index = tabControl.SelectedIndex;

            rich[index].undoList.Clear();
            rich[index].redoList.Clear();
        }


        //選択Richtextを保存する
        public void save_sub(string newpath)
        {

            //tab
            int index = tabControl.SelectedIndex;

            //タブ名
            string filename = tabControl.TabPages[index].Text;

            string path = rich[index].filepath;

            //resetclear();

            //上書き
            if (newpath.Length == 0)
            {
                if(path.Length == 0)
                {
                    path = OpenDialog.save();

                    if (path.Length == 0)
                    {
                        return;
                    }
                }
                newpath = path;
            }

            //ファイル名登録
            RICHINFO r;
            r = rich[index];
            //r.filename = /*path*/;
            r.filepath = newpath;
            rich[index] = r;
            
            //タブ名
            tabControl.TabPages[index].Text = Path.GetFileName(newpath);

            MyFile.write_str(newpath, richtext.Text);
        }


        /// <summary>
        /// 更新されたら、ファイル名に＊をつける
        /// </summary>
        private void richtext_kousin()
        {

            //tab
            int index = tabControl.SelectedIndex;
            //タブ名
            string filename = tabControl.TabPages[index].Text;

            if (!richtext.Modified) return;

            //filename = ;
            if(filename.IndexOf('*') == -1)
            {
                tabControl.TabPages[index].Text = "*" + filename;
            }


        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {

            tab_delete();

        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_sub("");
        }

        private void 上書き保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        /// <summary>
        /// コマンド実行
        /// </summary>
        public RichTextBox getRichText()
        {
            //tab
            int index = tabControl.SelectedIndex;
            //タブ名
            string filename = tabControl.TabPages[index].Text;

            return richtext;


        }

        #region "IME 変換中"

        enum GCS
        {
            GCS_COMPREADSTR = 0x0001,
            GCS_COMPREADATTR = 0x0002,
            GCS_COMPREADCLAUSE = 0x0004,
            GCS_COMPSTR = 0x0008,
            GCS_COMPATTR = 0x0010,
            GCS_COMPCLAUSE = 0x0020,
            GCS_CURSORPOS = 0x0080,
            GCS_DELTASTART = 0x0100,
            GCS_RESULTREADSTR = 0x0200,
            GCS_RESULTREADCLAUSE = 0x0400,
            GCS_RESULTSTR = 0x0800,
            GCS_RESULTCLAUSE = 0x1000,
        }
        // attribute for COMPOSITIONSTRING Structure
        enum ATTR
        {
            ATTR_INPUT = 0x00,
            ATTR_TARGET_CONVERTED = 0x01,
            ATTR_CONVERTED = 0x02,
            ATTR_TARGET_NOTCONVERTED = 0x03,
            ATTR_INPUT_ERROR = 0x04,
            ATTR_FIXEDCONVERTED = 0x05,
        }

        const int WM_SETREDRAW = 0x000B;
        const int WM_PAINT = 0x000F;

        private class ImmData
        {
            public string Composition;
            public int CursorPosition;
            public ATTR[] Attribute;
        }

        /// <summary></summary>
        /// <param name="isResult">trueは確定文字列を取り出す。falseは未確定文字列を取り出す</param>
        /// <param name="position">カーソル位置</param>
        /// <returns>取り出した文字列</returns>
        private ImmData GetIMEString(bool isResult)
        {
            ImmData immdata = new ImmData();

            IntPtr hIMC = ImmGetContext(this.Handle);
            if (hIMC != IntPtr.Zero)
            {
                try
                {
                    GCS gcs = isResult ? GCS.GCS_RESULTSTR : GCS.GCS_COMPSTR;

                    //変換中文字の属性
                    byte[] x = ImmGetCompositionData(hIMC, GCS.GCS_COMPATTR);
                    immdata.Attribute = new ATTR[x.Length];
                    for (int i = 0; i < x.Length; i++)
                    {
                        immdata.Attribute[i] = (ATTR)x[i];
                    }

                    var y = ImmGetCompositionData(hIMC, GCS.GCS_COMPREADATTR);

                    //カーソル位置を取得
                    immdata.CursorPosition = ImmGetCompositionString(hIMC, GCS.GCS_CURSORPOS, IntPtr.Zero, 0);

                    //文字列取出し
                    byte[] buff = ImmGetCompositionData(hIMC, isResult ? GCS.GCS_RESULTSTR : GCS.GCS_COMPSTR);

                    immdata.Composition = System.Text.Encoding.Unicode.GetString(buff);

                    Console.WriteLine("変換中文字:" + immdata.Composition);

                    if(immdata.Composition == ":password")
                    {
                        f3.Show();
                    }
                }
                finally
                {
                    ImmReleaseContext(this.Handle, hIMC);
                }
            }
            return immdata;
        }
        private byte[] ImmGetCompositionData(IntPtr hIMC, GCS gcs)
        {
            //文字バッファを用意
            int len = ImmGetCompositionString(hIMC, gcs, IntPtr.Zero, 0);
            byte[] buff = new byte[len];
            IntPtr pBuff = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);

            //文字列取出し
            ImmGetCompositionString(hIMC, gcs, pBuff, len);
            return buff;
        }


        int startPosition;//上書き開始位置
        string backupText;//上書きされている文字列
        string compositionText;//変換中文字列
        bool isImmProcessing;//IMMで操作中の描画ちらつきを止めるためのフラグ

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_SETREDRAW:
                case WM_PAINT:
                    if (isImmProcessing)
                    {
                        return;
                    }
                    break;
                case 0x010D: //WM_IME_STARTCOMPOSITION
                    {
                        //上書き開始位置を保存
                        startPosition = richtext.SelectionStart;
                        backupText = "";
                        compositionText = "";

                        richtext.SelectionStart = startPosition;
                        richtext.SelectionLength = 0;
                    }
                    return;
                case 0x010E://WM_IME_ENDCOMPOSITION
                    return;
                case 0x010F://WM_IME_COMPOSITION
                    {
                        isImmProcessing = true;
                        var gcs = (GCS)m.LParam.ToInt32();

                        //入力前の状態に戻す
                        richtext.SelectionStart = startPosition;
                        richtext.SelectionLength = compositionText.Length;
                        richtext.SelectedText = backupText;
                        richtext.SelectionStart = startPosition;
                        richtext.SelectionLength = backupText.Length;
                        richtext.SelectionColor = Color.Black;
                        richtext.SelectionLength = 0;

                        backupText = string.Empty;
                        richtext.SelectionStart = startPosition;

                        if ((gcs & GCS.GCS_RESULTSTR) == GCS.GCS_RESULTSTR || (gcs & GCS.GCS_RESULTCLAUSE) == GCS.GCS_RESULTCLAUSE)
                        {//変換終了か部分確定した


                            //確定文字列を取得して挿入
                            var immdata = GetIMEString(true);
                            compositionText = immdata.Composition;

                            richtext.SelectionStart = startPosition;
                            richtext.SelectionLength = compositionText.Length;
                            richtext.SelectedText = compositionText;
                            richtext.SelectionStart = startPosition;
                            richtext.SelectionLength = compositionText.Length;
                            richtext.SelectionColor = Color.Black;

                            startPosition += compositionText.Length;
                            richtext.SelectionStart = startPosition;
                            richtext.SelectionLength = 0;
                        }

                        if ((gcs & GCS.GCS_COMPSTR) == GCS.GCS_COMPSTR)
                        {//変換中文字列がある

                            //未確定文字列を取得
                            var immdata = GetIMEString(false);
                            compositionText = immdata.Composition;

                            //未確定文字列が上書きしてしまう文字列を一時退避
                            richtext.SelectionStart = startPosition;
                            richtext.SelectionLength = compositionText.Length;
                            backupText = richtext.SelectedText;

                            //未確定文字を上書き
                            richtext.SelectedText = compositionText;

                            //変換状態によって文字の色を変更
                            richtext.SelectionStart = startPosition;
                            richtext.SelectionLength = 1;
                            foreach (ATTR attr in immdata.Attribute)
                            {
                                switch (attr)
                                {
                                    case ATTR.ATTR_INPUT:
                                        richtext.SelectionColor = Color.Purple;
                                        break;
                                    case ATTR.ATTR_TARGET_CONVERTED:
                                        richtext.SelectionColor = Color.Red;
                                        break;
                                    case ATTR.ATTR_CONVERTED:
                                        richtext.SelectionColor = Color.Blue;
                                        break;
                                    case ATTR.ATTR_TARGET_NOTCONVERTED:
                                        break;
                                    case ATTR.ATTR_INPUT_ERROR:
                                        break;
                                    case ATTR.ATTR_FIXEDCONVERTED:
                                        break;
                                    default:
                                        break;
                                }
                                richtext.SelectionStart = richtext.SelectionStart + 1;
                                richtext.SelectionLength = 1;
                            }

                            richtext.SelectionStart = startPosition + immdata.CursorPosition;
                            richtext.SelectionLength = 0;
                        }
                        isImmProcessing = false;
                        Invalidate();
                    }
                    return;
                default:
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion


        #region "Undo"



        private UndoData getUndoData()
        {
            Point point = new Point(0, 0);
            SendMessage(richtext.Handle, 0x04DD, 0, ref point);
            return new UndoData(richtext.Text, point, richtext.SelectionStart);
        }

        public class UndoData
        {
            //保持するUndo情報はテキスト、キャレット位置、スクロール位置
            public UndoData(string text, Point scroll, int caret)
            {
                this.Text = text;
                this.Scroll = scroll;
                this.Caret = caret;
            }

            public string Text { get; set; }
            public Point Scroll { get; set; }
            public int Caret { get; set; }
        }

        #endregion
    }
}
