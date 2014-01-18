using System;
using NTier.Server.Domain.Service;
using NTierDemo.Common.Domain.Model.NTierDemo;
using NTierDemo.Server.Domain.Repositories;
using NTierDemo.Common.Domain.Model;

namespace NTierDemo.Server.Domain.Service
{
    partial class NTierDemoDataService
    {
        // for demo purpose we set the in-memory repository as defaul repository
        // comment this code out to let Entity Framework connect to a database 
        static NTierDemoDataService()
        {
            _defaultRepositoryFactory = x => new NTierDemoInMemoryRepository();
        }

        #region custom interceptors

        [ChangeInterceptor(typeof(Author))]
        void OnChange(Author entity, UpdateOperations operation)
        {
            SetModoficationTimestamps(entity, operation);
        }
        
        [ChangeInterceptor(typeof(Blog))]
        void OnChange(Blog entity, UpdateOperations operation)
        {
            SetModoficationTimestamps(entity, operation);
        }

        [ChangeInterceptor(typeof(Post))]
        void OnChange(Post entity, UpdateOperations operation)
        {
            SetModoficationTimestamps(entity, operation);
        }

        [ChangeInterceptor(typeof(PostInfo))]
        void OnChange(PostInfo entity, UpdateOperations operation)
        {
            throw new Exception("Blog post info is read-only and may not be modified");
        }

        private void SetModoficationTimestamps(IUpdatableEntity entity, UpdateOperations operation)
        {
            switch (operation)
            {
                case UpdateOperations.Add:
                    entity.CreatedDate = DateTime.Now;
                    entity.ModifiedDate = DateTime.Now;
                    break;

                case UpdateOperations.Change:
                    entity.ModifiedDate = DateTime.Now;
                    break;
            }
        }

        #endregion custom interceptors
    }
}

