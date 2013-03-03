using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManager.WPF.Foundation
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendInNewLine(this StringBuilder stringBuilder, string value)
        {
            if (stringBuilder == null) { throw new ArgumentNullException("stringBuilder"); }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(Environment.NewLine);
            }
            stringBuilder.Append(value);

            return stringBuilder;
        }
    }
}
