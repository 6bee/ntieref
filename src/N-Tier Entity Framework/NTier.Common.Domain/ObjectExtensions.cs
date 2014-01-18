// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NTier.Common.Domain
{
    internal static class ObjectExtensions
    {
        public static T Clone<T>(this T obj)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                stream.Seek(0, SeekOrigin.Begin);
                T clone = (T)formatter.Deserialize(stream);
                return clone;
            }
        }
    }
}
