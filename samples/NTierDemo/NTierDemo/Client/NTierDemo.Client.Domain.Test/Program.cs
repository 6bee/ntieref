using System;
using System.Diagnostics;
using System.Linq;
using NTierDemo.Common.Domain.Model.NTierDemo;
using NTierDemo.Common.Domain.Service.Contracts;
using NTier.Common.Domain.Model;
using NTierDemo.Server.Domain.Repositories;
using NTierDemo.Server.Domain.Service;

namespace NTierDemo.Client.Domain.Test
{
    class Program
    {
        private static readonly Func<ClientInfo, INTierDemoRepository> _repositoryFactory;
        private static readonly Func<INTierDemoDataService> _dataServiceFactory;
        private static readonly Func<INTierDemoDataContext> _dataContextFactory;

        static Program()
        {
            // using an in-memory data store and repository for demo purpose. see NTierDemo.Server.Domain.Repositories.NTierDemoInMemoryRepository
            _repositoryFactory = (x) => new NTierDemoInMemoryRepository();

            // using a direct instance of the data service for demo purpose rather than connecting through WCF. 
            _dataServiceFactory = () => new NTierDemoDataService(_repositoryFactory);

            // call the data context constructor with no parameters to connect backend via WCF.
            _dataContextFactory = () => new NTierDemoDataContext(_dataServiceFactory);
        }


        static void Main(string[] args)
        {
            // create a client data context
            var dataContext = _dataContextFactory();


            // query data using filtes, paging, sorting
            var authorsQuery =
                from author in dataContext.Users
                                          .AsQueryable() // <-- AsQueryable() specifies the query to be executed remotely, without AsQueryable you only process in-memory data of the local data context
                                          .Include("Blogs.Posts") // <-- indicates to load related blogs and blog posts for each author within one single request
                where author.Blogs.Any()
                orderby author.FirstName, author.LastName
                select author;

            var authors1 = authorsQuery.ToList(); // <-- this executes the query (remotely!)


            // modify existing data and relations
            var theAuthor = dataContext.Users.First(); // <-- previousely queried data by default remains in data context and is available for local processing
            theAuthor.Description = "Have you seen my new blog about N-Tier Entity Framework?";
            var newBlog = new Blog
            {
                Title = "N-Tier Entity Framework is fun!"
            };
            theAuthor.Blogs.Add(newBlog);

            Debug.Assert(dataContext.Blogs.Contains(newBlog)); // <-- new blog was automatically added to data context


            // create and store new data
            var newAuthor = new User
            {
                //Id = 1  <-- id is readonly and will be set by backend
                FirstName = "Michael",
                LastName = "Könings",
                Description = "Chef de blog",
                Username = "mkoenigs",
                Password = "****"
                //CreatedDate = DateTime.Now  <-- readonly timestamp set by backend upon insert
                //ModifiedDate = DateTime.Now <-- readonly timestamp set by backend upon insert and update
            };
            new Blog
            {
                Owner = newAuthor, // <-- automatically creates 2-way association Author <--> Blog
                Title = "All you need to know about http://blog.trivadis.com",
            };
            dataContext.Users.Add(newAuthor); // <-- adds author and relates blogs to data context


            // save all changes in one unit of work
            dataContext.SaveChanges(); // <-- performs insert, update, delete and refreshes local in-memory data with server generated values (IDs, timestamps, etc.)


            // local data is automatically refreshed after save
            Debug.Assert(newAuthor.Id != default(long));              // <-- ServerGeneration attribute generated automatically for primary key
            Debug.Assert(newAuthor.CreatedDate != default(DateTime)); // <-- ServerGeneration attribute specified in entity metatdata
            Debug.Assert(newAuthor.ModifiedDate != default(DateTime));// <-- ServerGeneration attribute specified in entity metatdata


            // run the old query again and see what you get...
            var authors2 = authorsQuery.ToList();
        }
    }
}
