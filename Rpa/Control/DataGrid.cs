using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rpa.Control
{
    public partial class DataGrid : UserControl
    {
        const int DATA_ROW_MAX = 100;
        public DataGrid()
        {
            InitializeComponent();

            init();
        }

        private void init()
        {
            // カラム数を指定
            dataGridView1.ColumnCount = 2;

            // カラム名を指定
            dataGridView1.Columns[0].HeaderText = "パスワード名";

            dataGridView1.Columns[1].HeaderText = "パスワード";


            //列をテーブルスタイルに追加する
            dataGridView1.Columns[1].Width = 300;

            // データを追加
            for (int i=0; DATA_ROW_MAX>i; i++)
            {
                dataGridView1.Rows.Add("password"+i, "");

            }



        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
