// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;

namespace NTier.Common.Domain.Model
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ServerGenerationAttribute : Attribute
    {
        public readonly ServerGenerationTypes Type;

        public ServerGenerationAttribute(ServerGenerationTypes type)
        {
            this.Type = type;
        }

        public override string ToString()
        {
            var serverGenerationTypesEnumName = Type.GetType().FullName;
            var serverGenerationTypes = Type.ToString().Split(',').Select(s => string.Format("global::{0}.{1}", serverGenerationTypesEnumName, s.Trim()));

            return string.Format("{0}({1})", base.ToString(), string.Join(" | ", serverGenerationTypes));
        }
    }
}
