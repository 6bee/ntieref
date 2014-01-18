using System;

namespace NTierDemo.Common.Domain.Model
{
    internal interface IUpdatableEntity
    {
        long Id { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime ModifiedDate { get; set; }
    }
}
