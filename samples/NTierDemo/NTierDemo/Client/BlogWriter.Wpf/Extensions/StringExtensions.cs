using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlogWriter.Wpf.Extensions
{
    public static class StringExtensions
    {
        public static string ToUnsecureString(this SecureString source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(source);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ToSecureString(this string source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var secureString = new SecureString();
            foreach (var c in source)
                secureString.AppendChar(c);
            secureString.MakeReadOnly();
            return secureString;
        }
    }
}
