using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panic
{
    public static class HiScore
    {
        //https://script.google.com/a/tiffin.kingston.sch.uk/macros/d/MkMVa193RM0iuXjo5N0twfhsG36trlRMW/edit?uiv=2&mid=ACjPJvELX1UiQ8YkkqE99-_7-z_mMMj9YCToFBdfAOwNPflltmYDPUQdOw4lWhCh7FU5YwI3MrySQiKCu3EwIa35apSfGm5S2n6k4icUw0baMewVYJlMViCqOcpcW4YLtbv89lxCl7ZLksDS
        public static char SEPARATOR = (char)7;

        public static string SALT = "3iou2ioxue32dji";
        public static string generateCodeFromUTF8(string name, string score)
        {
            var retVal = name + SEPARATOR + score + SEPARATOR + doMD5(name + score);
            return Base64Encode(retVal);
        }

        public static string doMD5(string str)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(str + SALT);

            return System.Convert.ToBase64String(md5.ComputeHash(inputBytes));
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
