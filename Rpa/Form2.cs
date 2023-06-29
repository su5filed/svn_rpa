using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rpa.Util;

namespace Rpa
{
    //https://docs.microsoft.com/ja-jp/dotnet/api/system.windows.forms.control.keypress?view=netcore-3.1
    public partial class Form2 : Form
    {
        //public event System.Windows.Forms.KeyPressEventHandler KeyPress;

        // Boolean flag used to determine when a character other than a number is entered.
        private bool nonNumberEntered = false;

        public Form2()
        {
            InitializeComponent();

            init();
        }

        private void init()
        {
            
            textBox2.Multiline = true;
            textBox2.ScrollBars = ScrollBars.Both;

            //Setup events that listens on keypress
            textBox1.KeyDown += TextBox1_KeyDown;
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox1.KeyUp += TextBox1_KeyUp;
        }

        #region "イベント"
        // Handle the KeyUp event to print the type of character entered into the control.
        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            textBox2.AppendText($"KeyUp code: {e.KeyCode}, value: {e.KeyValue}, modifiers: {e.Modifiers}" + "\r\n");
        }

        // Handle the KeyPress event to print the type of character entered into the control.
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBox2.AppendText($"KeyPress keychar: {e.KeyChar}" + "\r\n");
        }

        // Handle the KeyDown event to print the type of character entered into the control.
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            textBox2.AppendText($"KeyDown code: {e.KeyCode}, value: {e.KeyValue}, modifiers: {e.Modifiers}" + "\r\n");
        }
        #endregion


        // Handle the KeyDown event to determine the type of character entered into the control.
        private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Initialize the flag to false.
            nonNumberEntered = false;

            // Determine whether the keystroke is a number from the top of the keyboard.
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (e.KeyCode != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }
            //If shift key was pressed, it's not a number.
            //if (Control.ModifierKeys == Keys.Shift)
            //{
            //    nonNumberEntered = true;
            //}
        }

        // This event occurs after the KeyDown event and can be used to prevent
        // characters from entering the control.
        private void textBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Check for the flag being set in the KeyDown event.
            if (nonNumberEntered == true)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_DoubleClick(object sender, EventArgs e)
        {

            //SendKeys.Send("{LWin}");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            MyProcess pr = new MyProcess();

            // プロセス一覧を更新
            comboBox1.DataSource = pr.ProcessTable();
            comboBox1.ValueMember = "PID";
            comboBox1.DisplayMember = "VALUE";

        }
    }
}
