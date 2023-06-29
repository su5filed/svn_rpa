using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rpa.Util
{
    class MyCrypto
    {
        const int REPEAT_NUM = 2;

        // 暗号化(「暗号キーとなる文字列」と「暗号化したい文字列」)
        public static string Encryption(string encryptionKeyStr, string encryptionStr)
        {
            try
            {
                AesManaged aes = SetAes256(encryptionKeyStr);
                // 暗号化するためにはバイトの配列に変換する必要がある
                byte[] byteText = Encoding.UTF8.GetBytes(encryptionStr);
                byte[] encryptText = new byte[] { };
                for (int i = 0; i < REPEAT_NUM; i++)
                {
                    // 最初のみ既に暗号化する文字列を渡してあるので飛ばす
                    if (i != 0)
                    {
                        // byteTextには暗号化したものを入れる
                        byteText = encryptText;
                    }
                    encryptText = aes.CreateEncryptor().TransformFinalBlock(byteText, 0, byteText.Length);
                }
                // Base64形式（64種類の英数字で表現）に変換して返す
                return Convert.ToBase64String(encryptText);
            }
            catch (Exception ex)
            {
                //LogUtil.WriteLineWithDebug("ERROR:", ex);
                return "";
            }
        }

        // 復号(「キー」と「暗号化した文字列」と「回数」)
        public static string Decryption(string encryptionKeyStr, string encryptionStr)
        {
            try
            {
                AesManaged aes = SetAes256(encryptionKeyStr);
                // 暗号化された文字列を復号するためにバイトの配列に変換
                byte[] byteText = Convert.FromBase64String(encryptionStr);
                byte[] decryptText = new byte[] { };
                for (int i = 0; i < REPEAT_NUM; i++)
                {
                    // 最初のみ既に復号する文字列を渡してあるので飛ばす
                    if (i != 0)
                    {
                        // byteTextには復号したものを入れる
                        byteText = decryptText;
                    }
                    decryptText = aes.CreateDecryptor().TransformFinalBlock(byteText, 0, byteText.Length);


                }
                // 復号されたバイトデータを文字列に変換して返す
                return Encoding.UTF8.GetString(decryptText);
            }
            catch (Exception ex)
            {
                //LogUtil.WriteLineWithDebug("ERROR:", ex);
                return "";
            }
        }

        /// <summary>
        /// AES256に応じた設定を返却する
        /// </summary>
        /// <param name="encryptionKeyStr"></param>
        /// <returns></returns>
        private static AesManaged SetAes256(string encryptionKeyStr)
        {
            // 暗号化方式はAES
            AesManaged aes = new AesManaged();
            // 鍵の長さ(256)
            aes.KeySize = 256;
            // ブロックサイズ（何文字単位で処理するか）は必ず128を指定
            aes.BlockSize = 128;
            // 暗号利用モード
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            string keyText = "";
            if (encryptionKeyStr.Length < 32)
            {
                //３２文字に満たない場合は空白埋め
                keyText = encryptionKeyStr.PadLeft(32);
            }
            else
            {
                //３２以上は切り出し
                keyText = encryptionKeyStr.Substring(0, 32);
            }

            //１６文字取得
            string ivText = keyText.Substring(0, 16);

            byte[] keyBytes = ASCIIEncoding.ASCII.GetBytes(keyText);
            byte[] ivBytes = ASCIIEncoding.ASCII.GetBytes(ivText);

            //暗号化Key
            aes.Key = keyBytes;

            //暗号化ベクトルも共通
            aes.IV = ivBytes;

            return aes;
        }

    }
}
