// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.


namespace NTier.Common.Domain.Model
{
    // The interface is implemented by the self tracking entities that EF will generate.
    // We will have an Adapter that converts this interface to the interface that the EF expects.
    // The Adapter will live on the server side.
    public interface IObjectWithChangeTracker
    {
        // Has all the change tracking information for the subgraph of a given object.
        ObjectChangeTracker ChangeTracker { get; set; }
    }
}
