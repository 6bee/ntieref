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
            private readonly ISet<User> _users;
            private readonly ISet<Blog> _blogs;
            private readonly ISet<Post> _posts;

            public ISet<User> Users { get { return _users; } }
            public ISet<Post> Posts { get { return _posts; } }
            public ISet<Blog> Blogs { get { return _blogs; } }
            
            public InMemoryDataStore()
            {
                // Here we create the initial data
                var users = new[]
                {
                    new User { Id = GetNextId(), Username = "thuber", Password = "****", FirstName = "Thomas", LastName = "Huber", Description = "MVP Client Development", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                    new User { Id = GetNextId(), Username = "nmueggler", Password = "****", FirstName = "Nicolas", LastName = "Mueggler", Description = "TFS Guru", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                    new User { Id = GetNextId(), Username = "anobbmann", Password = "****", FirstName = "Andreas", LastName = "Nobbmann", Description = "All about ORACLE BI", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                    new User { Id = GetNextId(), Username = "gschmutz", Password = "****", FirstName = "Guido", LastName = "Schmutz", Description = "Knows everything about SOA with Oracle", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                    new User { Id = GetNextId(), Username = "mmeyer", Password = "****", FirstName = "Manuel", LastName = "Meyer", Description = ".NET, Azure, everything", CreatedDate = DateTime.Now, ModifiedDate =DateTime.Now },
                };

                var blogs = new[]
                {
                    new Blog { Id = GetNextId(), Title = ".NET Development, Performance Management and the Windows Azure Cloud", Owner = users.Single(i => i.Username == "mmeyer"), CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now },
                    new Blog { Id = GetNextId(), Title = "User Interface Rocker", Owner = users.Single(i => i.Username == "thuber"), CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now },
                };

                var posts = new[]
                {
                    new Post { Id = GetNextId(), Title = "Get started with the Azure Cloud!", Abstract = "Get started with Azure using Azure Friday, a collection of bite-size (10-15mins) videos on Windows Azure...", Content = "...", CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now },
                    new Post { Id = GetNextId(), Title = "Next Generation Windows Apps", Abstract = "....", Content = "...", CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now },
                };

                blogs[0].Posts.Add(posts[0]);
                blogs[1].Posts.Add(posts[1]);

                _users = Initialize(users);
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

        public IEntitySet<User> Users { get { return GetEntitySet<User>(_store.Users); } }

        public IEntitySet<Blog> Blogs { get { return GetEntitySet<Blog>(_store.Blogs); } }

        public IEntitySet<Post> Posts { get { return GetEntitySet<Post>(_store.Posts); } }

        #endregion EntitySets

        #region insert update triggers

        protected override void OnInsert(Entity entity)
        {
            if (entity is IUpdatableEntity)
            {
                ((IUpdatableEntity)entity).Id = _store.GetNextId();
            }

            FixeUpReferences(entity);
        }

        protected override void OnUpdate(Entity entity)
        {
            FixeUpReferences(entity);
        }

        protected override void OnDelete(Entity entity)
        {
            CleanUpReferences(entity);
        }

        private void FixeUpReferences(Entity entity)
        {
            if (entity is Blog)
            {
                var blog = (Blog)entity;
                var owner = Users.FirstOrDefault(i => i.Id == blog.OwnerId);
                if (owner != null)
                {
                    owner.Blogs.Add(blog);
                    owner.AcceptChanges();
                }
            }

            if (entity is Post)
            {
                var post = (Post)entity;
                var blog = Blogs.FirstOrDefault(i => i.Id == post.BlogId);
                if (blog != null)
                {
                    blog.Posts.Add(post);
                    blog.AcceptChanges();
                }
            }
        }

        private void CleanUpReferences(Entity entity)
        {
            if (entity is Blog)
            {
                var blog = (Blog)entity;

                foreach (var user in Users)
                {
                    foreach (var referencedBlog in user.Blogs.Where(b => b.Id == blog.Id).ToList())
                    {
                        user.Blogs.Remove(referencedBlog);
                    }
                    user.AcceptChanges();
                }

                foreach(var post in Posts.Where(x => x.BlogId == blog.Id).ToList())
                {
                    ((IInMemoryEntitySet<Post>)Posts).Remove(post);
                }
            }

            if (entity is Post)
            {
                var post = (Post)entity;

                foreach (var blog in Blogs)
                {
                    foreach (var referencedPost in blog.Posts.Where(i => i.Id == post.Id).ToList())
                    {
                        blog.Posts.Remove(referencedPost);
                    }
                    blog.AcceptChanges();
                }
            }
        }
        
        #endregion insert update triggers
    }
}
