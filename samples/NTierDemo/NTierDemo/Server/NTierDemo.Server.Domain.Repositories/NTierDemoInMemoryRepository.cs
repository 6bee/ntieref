using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories;
using NTierDemo.Common.Domain.Model;
using NTierDemo.Common.Domain.Model.NTierDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NTierDemo.Server.Domain.Repositories
{
    public partial class NTierDemoInMemoryRepository : InMemoryRepository, INTierDemoRepository
    {
        #region InMemoryDataStore

        private sealed class InMemoryDataStore
        {
            private static long _id = 0;
            private readonly ISet<Author> _authors;
            private readonly ISet<Blog> _blogs;
            private readonly ISet<Post> _posts;

            public ISet<Author> Authors { get { return _authors; } }
            public ISet<Post> Posts { get { return _posts; } }
            public ISet<Blog> Blogs { get { return _blogs; } }
            public ISet<PostInfo> PostInfos
            {
                get
                {
                    return Posts
                        .Select(p =>
                        {
                            var info = new PostInfo
                            {
                                Id = p.Id,
                                BlogId = p.BlogId,
                                Title = p.Title,
                                Abstract = p.Abstract,
                                CreatedDate = p.CreatedDate,
                                ModifiedDate = p.ModifiedDate,
                            };
                            info.AcceptChanges();
                            return info;
                        })
                        .ToSet();
                }
            }
            
            public InMemoryDataStore()
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
            public long GetNextId()
            {
                return Interlocked.Increment(ref _id);
            }
        }

        private static readonly InMemoryDataStore _store = new InMemoryDataStore();

        #endregion InMemoryDataStore

        #region EntitySets

        public IEntitySet<Author> Authors { get { return GetEntitySet<Author>(_store.Authors); } }

        public IEntitySet<Blog> Blogs { get { return GetEntitySet<Blog>(_store.Blogs); } }

        public IEntitySet<Post> Posts { get { return GetEntitySet<Post>(_store.Posts); } }

        public IEntitySet<PostInfo> PostInfos { get { return GetEntitySet<PostInfo>(_store.PostInfos); } }

        #endregion EntitySets

        #region insert update triggers

        protected override void OnInsert(Entity entity)
        {
            if (entity is IUpdatableEntity)
            {
                ((IUpdatableEntity)entity).Id = _store.GetNextId();
            }

            FixeUpReferencesAndState(entity);
        }

        protected override void OnUpdate(Entity entity)
        {
            FixeUpReferencesAndState(entity);
        }

        protected override void OnDelete(Entity entity)
        {
            CleanUpReferences(entity);
        }

        private void FixeUpReferencesAndState(Entity entity)
        {
            if (entity is Blog)
            {
                var blog = (Blog)entity;
                var owner = Authors.FirstOrDefault(i => i.Id == blog.OwnerId);
                if (owner != null)
                {
                    blog.Author = owner;
                    owner.AcceptChanges();
                }
            }
        }

        private void CleanUpReferences(Entity entity)
        {
            if (entity is Blog)
            {
                var blog = (Blog)entity;

                foreach (var author in Authors)
                {
                    foreach (var referencesBlog in author.Blogs.Where(b => b.Id == blog.Id).ToList())
                    {
                        author.Blogs.Remove(referencesBlog);
                    }
                    author.AcceptChanges();
                }

                foreach(var post in Posts.Where(x => x.BlogId == blog.Id).ToList())
                {
                    ((IInMemoryEntitySet<Post>)Posts).Remove(post);
                }
            }
        }
        
        #endregion insert update triggers
    }
}
