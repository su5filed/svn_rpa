using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpa.Util
{
    static class MyRegistry
    {
        private static Dictionary<string, string> _d = new Dictionary<string, string>();
        public static Dictionary<string, string> d { get { return _d; }set { d = value; } }

        //キー（HKEY_CURRENT_USER\Software\test\sub）を開く
        const string REG_PATH = @"Software\RpaKeyStork\password";

        public static void readlist(List<string> list)
        {
            //キーを読み取り専用で開く
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REG_PATH, false);

            if (regkey == null) return;

            //subキーにあるキーの数を表示
            Console.WriteLine("サブキーの数:{0}", regkey.SubKeyCount);

            //subキーにあるすべてのキー名を取得
            string[] keyNames = regkey.GetSubKeyNames();
            //表示する
            foreach (string key in keyNames)
            {

                //list.Add(k);
                d[key]  = (string)regkey.GetValue(key);
            }

            ////subキーにある値の数を表示
            //Console.WriteLine("キーの値の数:{0}", regkey.ValueCount);
            ////subキーにあるすべての値の名前を取得
            //string[] valueNames = regkey.GetValueNames();
            ////表示する
            //foreach (string v in valueNames)
            //{
            //    Console.WriteLine(v);
            //}

            //閉じる
            regkey.Close();
        }

        
        public static string read(string name)
        {
            //キー（HKEY_CURRENT_USER\Software\test\sub）を開く
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REG_PATH);
            
            //キーが存在しないときは null が返される
            if (regkey == null) return "";
            
            //指定した名前の値が存在しないときは null が返される
            string stringValue = (string)regkey.GetValue(name);


            //上のコードでは、指定したキーが存在しないときは新しく作成される。
            //作成されないようにするには、次のようにする。
            //Microsoft.Win32.RegistryKey regkey =
            //    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\test\sub", true);

            ////レジストリに書き込み
            ////文字列を書き込む（REG_SZで書き込まれる）
            //regkey.SetValue("string", "StringValue");
            ////整数（Int32）を書き込む（REG_DWORDで書き込まれる）
            //regkey.SetValue("int", 100);
            ////文字列配列を書き込む（REG_MULTI_SZで書き込まれる）
            //string[] s = new string[] { "1", "2", "3" };
            //regkey.SetValue("StringArray", s);
            ////バイト配列を書き込む（REG_BINARYで書き込まれる）
            //byte[] bs = new byte[] { 0, 1, 2 };
            //regkey.SetValue("Bytes", bs);

            //閉じる
            regkey.Close();

            return stringValue;

        }


        public static void write(string name, string value)
        {
            //キー（HKEY_CURRENT_USER\Software\test\sub）を開く
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REG_PATH);

            //REG_EXPAND_SZで書き込む
            regkey.SetValue(name, value, Microsoft.Win32.RegistryValueKind.ExpandString);

            ////REG_QWORDで書き込む
            //regkey.SetValue("QWord", 1000, Microsoft.Win32.RegistryValueKind.QWord);

            //閉じる
            regkey.Close();
        }
    }
}
