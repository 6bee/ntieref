using System;

namespace NTierDemo.Common.Domain.Model.NTierDemo
{
    partial class Post : IUpdatableEntity
    {
        long IUpdatableEntity.Id
        {
            get { return Id; }
            set { Id = value; }
        }

        DateTime IUpdatableEntity.CreatedDate
        {
            get { return CreatedDate; }
            set { CreatedDate = value; }
        }

        DateTime IUpdatableEntity.ModifiedDate
        {
            get { return ModifiedDate; }
            set { ModifiedDate = value; }
        }
    }
}
