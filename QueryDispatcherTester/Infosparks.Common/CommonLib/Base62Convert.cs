using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenK.InfoSparks.Common
{
    public class Base62Convert
    {
        private const string CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string CHARS_RANDOM = "BtdCmh2Q7TblZjfPYHFER8wSOuAq3yXszrDKLaM1iJ5UW6pxoIv4ek90nVcNGg";

        /* public static String ConvertToBase(long num, int nbase)
         {            
             // check if we can convert to another base
             if (nbase < 2 || nbase > chars.Length)
                 return "";

             int r;
             String newNumber = "";

             // in r we have the offset of the char that was converted to the new base
             while (num >= nbase)
             {
                 r = (int)(num % (long)nbase);
                 newNumber = chars[(int)r] + newNumber;
                 num = num / nbase;
             }
             // the last number to convert
             newNumber = chars[(int)num] + newNumber;

             return newNumber;
         }*/

        static public long ConvertFromBase(string s, int nbase = 62)
        {
            long result = 0;
            int r = 0;
            while (r < s.Length)
            {
                int pos = CHARS_RANDOM.LastIndexOf(s[s.Length - r - 1]);
                result = result + pos * (long)(Math.Pow(nbase, r));
                r++;
            }

            return result;
        }

        static public String ConvertToBase(long num, int nbase = 62)
        {
            // check if we can convert to another base
            if (nbase < 2 || nbase > CHARS_RANDOM.Length)
                return "";

            String newNumber = "";

            // in r we have the offset of the char that was converted to the new base
            while (num >= nbase)
            {
                int r = (int)(num % nbase);
                newNumber = CHARS_RANDOM[r] + newNumber;
                num = num / nbase;
            }
            // the last number to convert
            newNumber = CHARS_RANDOM[(int)num] + newNumber;

            return newNumber;
        }


    }
}
