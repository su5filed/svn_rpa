using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rpa.Util
{
    class MyProcess
    {

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32")]
        private extern static int GetWindowThreadProcessId(int hwnd, out int lpdwprocessid);

        private DataTable _table = new DataTable();

        public DataTable table { get { return _table; } set { _table = value; } }


        // プロセス名でソート ... for Array.Sort
        public class ProcComparator : IComparer<Process>
        {
            public int Compare(Process p, Process q)
            {
                return p.ProcessName.CompareTo(q.ProcessName);
            }
        }


        public int getDesk()
        {

            // デスクトップのウインドウハンドル取得
            IntPtr hwnd = GetDesktopWindow();

            return GetPidFromHwnd(hwnd);

        }


        // ウィンドウハンドル(hwnd)をプロセスID(pid)に変換する
        public int GetPidFromHwnd(IntPtr hwnd)
        {
            int pid;
            GetWindowThreadProcessId((int)hwnd, out pid);
            return pid;
        }


        /// <summary>
        /// 指定　name のPIDを取得
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public int getpid(string name, string title)
        {
            int rlt = 0;
            if(name == "desktop")
            {
                return getDesk();
            }
            else
            {
                DataTable data = ProcessTable(name, title);

                // LINQを使ってデータを抽出
                DataRow[] dRows = table.AsEnumerable()
                    .Where(row => row.Field<string>("NAME") == name &&
                                  row.Field<string>("TITLE").IndexOf(title) != -1
                                     ).ToArray();

                if(dRows.Count() > 0)
                {
                    rlt = int.Parse( dRows[0].Field<string>("PID") );
                }

            }

            return rlt;
        }



        public DataTable ProcessTable()
        {
            return ProcessTable("", "");
        }


        public DataTable ProcessTable(string name, string title)
        {
            // プロセスのリストを取得
            // http://d.hatena.ne.jp/tomoemon/20080430/p2
            Process[] ps = Process.GetProcesses();
            Array.Sort(ps, new ProcComparator());

            if(table != null)
            {
                table.Clear();
            }

            table = new DataTable();
            table.Columns.Add("PID");
            table.Columns.Add("NAME");
            table.Columns.Add("TITLE");
            table.Columns.Add("VALUE");

            foreach (Process p in ps)
            {
                DataRow row = table.NewRow();
                row.SetField<int>("PID", p.Id);
                row.SetField<string>("NAME", p.ProcessName);
                row.SetField<string>("TITLE", p.MainWindowTitle);
                row.SetField<string>("VALUE", p.ProcessName + ","+ p.MainWindowTitle);


                //if(name.Length > 0)
                //{
                //    if(title.Length > 0)
                //    {
                //        //プロセス名　と　タイトルの一部
                //        if (name == p.ProcessName && p.MainWindowTitle.IndexOf(title) != -1)
                //        {
                //            table.Rows.Add(row);
                //            break;
                //        }
                //    }
                //    else
                //    {
                //        //プロセス名　と　タイトルの一部
                //        if (name == p.ProcessName )
                //        {
                //            table.Rows.Add(row);
                //            break;
                //        }
                //    }
                //}
                //else
                //{
                    table.Rows.Add(row);
                //}
            }
            table.AcceptChanges();

            return table;
        }

    }
}
