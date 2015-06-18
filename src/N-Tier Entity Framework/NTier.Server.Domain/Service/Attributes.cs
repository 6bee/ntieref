// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Server.Domain.Service
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class QueryInterceptorAttribute : Attribute
    {
        public readonly Type Type;

        public QueryInterceptorAttribute(Type targetType)
        {
            this.Type = targetType;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ChangeInterceptorAttribute : Attribute
    {
        public readonly Type Type;

        public ChangeInterceptorAttribute(Type targetType)
        {
            this.Type = targetType;
        }
    }
}
