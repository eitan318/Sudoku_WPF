using System;
using System.Security.Cryptography;
using System.Text;
using System.Reflection; // for Assembly

namespace Sudoku_WPF.GameClasses
{
    /// <summary>
    /// Provides methods to protect and unprotect data using the Windows data protection API.
    /// </summary>
    internal class Code
    {
        /// <summary>
        /// Protects a string using Windows data protection API.
        /// </summary>
        /// <param name="str">The string to be protected.</param>
        /// <returns>A base64-encoded string representing the protected data.</returns>
        public static string Protect(string str)
        {
            // Use assembly full name as entropy
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            byte[] data = Encoding.ASCII.GetBytes(str);
            string protectedData = Convert.ToBase64String(ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser));
            return protectedData;
        }

        /// <summary>
        /// Unprotects a base64-encoded string using Windows data protection API.
        /// </summary>
        /// <param name="str">The base64-encoded string to be unprotected.</param>
        /// <returns>The original string if successful, otherwise null.</returns>
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
