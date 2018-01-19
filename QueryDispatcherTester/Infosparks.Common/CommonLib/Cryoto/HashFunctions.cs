using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TenK.InfoSparks.Common.Cryoto
{
    public static class HashFunctions
    {
        public static string ComputerSha1Hash(string data)
        {
            byte[] hashData;
            using (var sha = new SHA1CryptoServiceProvider())
            {
                //convert the input text to array of bytes
                hashData = sha.ComputeHash(Encoding.ASCII.GetBytes(data));

            }

            // return hexadecimal string
            return BitConverter.ToString(hashData);
        }

        public static string ComputeSHAHash(string data)
        {
            string trim = Regex.Replace(data, @"\s", "");
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(trim.ToLowerInvariant());
            SHA256 shaM = new SHA256Managed();
            return Convert.ToBase64String(shaM.ComputeHash(plainTextBytes)) + data.GetHashCode();
        }   
    }
}
