using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories;
using NTierDemo.Common.Domain.Model;
using NTierDemo.Common.Domain.Model.NTierDemo;

namespace NTierDemo.Server.Domain.Repositories
{
    public partial class NTierDemoInMemoryRepository : InMemoryRepository, INTierDemoRepository
    {
        #region InMemoryDataStore

        private static class InMemoryDataStore
        {
            private static long _id = 0;
            private static readonly ISet<Author> _authors;
            private static readonly ISet<Blog> _blogs;
            private static readonly ISet<Post> _posts;

            public static ISet<Author> Authors { get { return _authors; } }
            public static ISet<Post> Posts { get { return _posts; } }
            public static ISet<Blog> Blogs { get { return _blogs; } }
            public static ISet<PostInfo> PostInfos
            {
                get
                {
                    return Posts
                        .Select(p =>
                        {
                            var info = new PostInfo
                            {

                            };
                            info.AcceptChanges();
                            return info;
                        })
                        .ToSet();
                }
            }
            
            static InMemoryDataStore()
            {
                // Here we create the initial data
                var authors = new[]
                {
                    new Author { Id = GetNextId(), Username = "thuber", Password = "****", FirstName = "Thomas", LastName = "Huber", Description = "MVP Client Development", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                    new Author { Id = GetNextId(), Username = "nmueggler", Password = "****", FirstName = "Nicolas", LastName = "Mueggler", Description = "TFS Guru", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                    new Author { Id = GetNextId(), Username = "anobbmann", Password = "****", FirstName = "Andreas", LastName = "Nobbmann", Description = "All about ORACLE BI", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                    new Author { Id = GetNextId(), Username = "gschmutz", Password = "****", FirstName = "Guido", LastName = "Schmutz", Description = "Knows everything about SOA with Oracle", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                    new Author { Id = GetNextId(), Username = "mmeyer", Password = "****", FirstName = "Manuel", LastName = "Meyer", Description = ".NET, Azure, everything", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                };

                var blogs = new[]
                {
                    new Blog { Id = GetNextId(), Title = ".NET Development, Performance Management and the Windows Azure Cloud", Author = authors.Single(i=>i.Username=="mmeyer"), CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now },
                    new Blog { Id = GetNextId(), Title = "User Interface Rocker", Author = authors.Single(i=>i.Username=="thuber"), CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now },
                };

                // TODO: create posts
                var posts = new Post[0];

                _authors = Initialize(authors);
                _blogs = Initialize(blogs);
                _posts = Initialize(posts);
            }

            /// <summary>
            /// primary key generator
            /// </summary>
            public static long GetNextId()
            {
                return Interlocked.Increment(ref _id);
            }
        }

        #endregion InMemoryDataStore

        #region EntitySets

        public IEntitySet<Author> Authors { get { return GetEntitySet<Author>(InMemoryDataStore.Authors); } }

        public IEntitySet<Blog> Blogs { get { return GetEntitySet<Blog>(InMemoryDataStore.Blogs); } }

        public IEntitySet<Post> Posts { get { return GetEntitySet<Post>(InMemoryDataStore.Posts); } }

        public IEntitySet<PostInfo> PostInfos { get { return GetEntitySet<PostInfo>(InMemoryDataStore.PostInfos); } }

        #endregion EntitySets

        #region insert update triggers

        protected override void OnInsert(Entity entity)
        {
            if (entity is IUpdatableEntity)
            {
                ((IUpdatableEntity)entity).Id = InMemoryDataStore.GetNextId();
            }
        }
        
        #endregion insert update triggers
    }
}
