using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TenK.InfoSparks.Common.Cryoto
{

    public class TripleDESEncryption
    {
        private static readonly byte[] GlobalKey = Convert.FromBase64String("+r7AvulHPZpe8zPMgxfuObivRhF6siKT");

        public static byte[] EncryptData(string value)
        {
            byte[] key = GlobalKey;
            // Convert the passed string to a byte array. 
            byte[] source = new ASCIIEncoding().GetBytes(value);
            byte[] target = new byte[0];

            // Use this to generate new IV
            TripleDESCryptoServiceProvider tDESalg = new TripleDESCryptoServiceProvider();
            try
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] iv = tDESalg.IV;

                    ms.Write(iv, 0, iv.Length);
                    // Create a CryptoStream using the FileStream 
                    // and the passed key and initialization vector (IV).
                    using (CryptoStream cStream = new CryptoStream(ms,
                                                                   new TripleDESCryptoServiceProvider().CreateEncryptor(
                                                                       key, iv),
                                                                   CryptoStreamMode.Write))
                    {


                        // Write the byte array to the crypto stream and flush it.
                        cStream.Write(source, 0, source.Length);
                        cStream.FlushFinalBlock();

                        target = ms.ToArray();
                    }
                }
            }
            catch (CryptographicException e)
            {

            }

            return target;
        }

        public static string DecryptData(byte[] value)
        {
            byte[] key = GlobalKey;
            //byte[] IV_192 = Encoding.ASCII.GetBytes(Configuration.Element("IV_192").Value);

            string result = "";
            try
            {
                using (MemoryStream ms = new MemoryStream(value))
                {
                    byte[] iv = new byte[8];
                    ms.Read(iv, 0, 8);

                    // Create a CryptoStream using the FileStream 
                    // and the passed key and initialization vector (IV).
                    using (CryptoStream cStream = new CryptoStream(ms,
                                                                   new TripleDESCryptoServiceProvider().CreateDecryptor(
                                                                       key, value),
                                                                   CryptoStreamMode.Read))
                    {

                        // Create a StreamReader using the CryptoStream.
                        using (StreamReader sReader = new StreamReader(cStream))
                        {
                            // Read the data from the stream to decrypt it.
                            result = sReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (CryptographicException e)
            {

            }

            // Return the string. 
            return result;
        }

    }
}
