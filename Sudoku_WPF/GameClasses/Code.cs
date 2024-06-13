using System;
using System.Security.Cryptography;
using System.Text;
using System.Reflection; // for Assembly

namespace Sudoku_WPF.GameClasses
{
    internal class Code
    {
        public static string Protect(string str)
        {
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            byte[] data = Encoding.ASCII.GetBytes(str);
            string protectedData = Convert.ToBase64String(ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser));
            return protectedData;
        }

        public static string Unprotect(string str)
        {
            try
            {
                byte[] protectedData = Convert.FromBase64String(str);
                byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
                string data = Encoding.ASCII.GetString(ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser));
                return data;
            }
            catch
            {
                return null;
            }
        }
    }


}
