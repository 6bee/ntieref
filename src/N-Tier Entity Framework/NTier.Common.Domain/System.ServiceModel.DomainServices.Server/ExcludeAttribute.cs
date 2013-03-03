// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

#if CLIENT_PROFILE
//copied from Assembly System.ServiceModel.DomainServices.Server.dll, v4.0.30319
//to be used in client profile
namespace System.ServiceModel.DomainServices.Server
{
    /// <summary>
    /// Indicates that an entity member should not exist in the code generated client view of the entity, and that the value should never be sent to the client.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ExcludeAttribute : Attribute
    {
    }
}
#endif
