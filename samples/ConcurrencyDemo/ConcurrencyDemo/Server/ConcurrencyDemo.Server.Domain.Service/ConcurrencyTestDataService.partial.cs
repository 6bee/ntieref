using ConcurrencyDemo.Common.Domain.Model.ConcurrencyTest;
using NTier.Server.Domain.Service;
using System;

namespace ConcurrencyDemo.Server.Domain.Service
{
    partial class ConcurrencyTestDataService
    {
        [ChangeInterceptor(typeof(ARecord))]
        public void OnChange(ARecord entity, UpdateOperations operation)
        {
            switch (operation)
            {
                case UpdateOperations.Add:
                case UpdateOperations.Change:
                    entity.ChangedDate = DateTime.Now;
                    entity.Key = Guid.NewGuid(); // setting next value for concurrency column, check is based on original value
                    break;
            }
        }

        [ChangeInterceptor(typeof(BRecord))]
        public void OnChange(BRecord entity, UpdateOperations operation)
        {
            switch (operation)
            {
                case UpdateOperations.Add:
                case UpdateOperations.Change:
                    entity.ChangedDate = DateTime.Now;
                    break;
            }
        }

        [ChangeInterceptor(typeof(CRecord))]
        public void OnChange(CRecord entity, UpdateOperations operation)
        {
            switch (operation)
            {
                case UpdateOperations.Add:
                case UpdateOperations.Change:
                    entity.ChangedDate = DateTime.Now;
                    break;
            }
        }
    }
}

