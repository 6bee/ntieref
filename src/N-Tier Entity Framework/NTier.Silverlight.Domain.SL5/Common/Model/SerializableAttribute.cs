﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Common.Domain.Model
{
    /// <summary>
    /// Placeholder attribute as a NONFUNCTIONAL placeholder of it's .NET framework version
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    internal sealed class SerializableAttribute : Attribute
    {
    }
}