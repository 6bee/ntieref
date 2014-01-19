using BlogWriter.Wpf.Commands;
using NTier.Client.Domain;
using NTierDemo.Client.Domain;
using NTierDemo.Common.Domain.Model.NTierDemo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlogWriter.Wpf.ViewModels
{
    public class UserBlogsViewModel : ViewModel
    {
        private readonly INTierDemoDataContext _dataContext;
        private readonly string _username;

        public UserBlogsViewModel(Func<INTierDemoDataContext> dataContextFactory, string username, Action logout)
        {
            _dataContext = dataContextFactory();
            _username = username;

            LogoutCommand = new RelayCommand(logout);
            CreateNewBlogCommand = new RelayCommand(CreateNewBlog);
            SaveCommand = new AsyncRelayCommand(SaveAsync, () => _dataContext.HasChanges && _dataContext.Blogs.All(x => x.IsValid));
            CancelCommand = new RelayCommand(Cancel, () => _dataContext.HasChanges);

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            using (new BusyCursor())
            {
                var query = _dataContext.Authors
                    .AsQueryable()
                    .Include("Blogs.Posts")
                    .Where(x => x.Username == _username);

                await query.ExecuteAsync();
            }
        }

        public IEntitySet<Blog> Blogs { get { return _dataContext.Blogs; } }

        public Blog SelectedBlog
        {
            get { return _selectedBlog; }
            set { _selectedBlog = value; OnPropertyChanged(() => SelectedBlog); }
        }
        private Blog _selectedBlog;

        public ICommand LogoutCommand { get; private set; }
        public ICommand CreateNewBlogCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private void CreateNewBlog()
        {
            var blog = new Blog();
            _dataContext.Authors.Single().Blogs.Add(blog);
            SelectedBlog = blog;
        }

        private async Task SaveAsync()
        {
            await _dataContext.SaveChangesAsync();
        }

        private void Cancel()
        {
            _dataContext.RevertChanges();
        }
    }
}
