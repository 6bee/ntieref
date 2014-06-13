using BlogWriter.Wpf.Commands;
using NTierDemo.Client.Domain;
using NTierDemo.Common.Domain.Model.NTierDemo;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace BlogWriter.Wpf.ViewModels
{
    public class UserBlogsViewModel : ViewModel
    {
        private readonly INTierDemoDataContext _dataContext;
        private readonly Author _user;

        public UserBlogsViewModel(Func<INTierDemoDataContext> dataContextFactory, Author user, Action logout)
        {
            _dataContext = dataContextFactory();
            _user = user;

            LogoutCommand = new RelayCommand(logout);
            CreateNewBlogCommand = new RelayCommand(CreateNewBlog, _dataContext.Authors.Count == 1);
            SaveCommand = new AsyncRelayCommand(SaveAsync, () => _dataContext.HasChanges && _dataContext.Blogs.All(x => x.IsValid));
            CancelCommand = new RelayCommand(Cancel, () => _dataContext.HasChanges);
            OpenBlogCommand = new RelayCommand(OpenBlog, () => SelectedBlog != null);
            CloseBlogCommand = new RelayCommand(CloseBlog, () => OpenedBlog != null);
            DeleteBlogCommand = new RelayCommand(DeleteBlog, () => SelectedBlog != null);

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            using (new BusyCursor())
            {
                var query = _dataContext.Authors
                    .AsQueryable()
                    .Include("Blogs.Posts")
                    .Where(x => x.Username == _user.Username);

                await query.ExecuteAsync();

                if (_dataContext.Authors.Count == 1)
                {
                    // creating a collection view based on the user's blogs
                    UserBlogs = new ListCollectionView(_dataContext.Authors.Single().Blogs);
                    OnPropertyChanged(() => UserBlogs);
                }
            }
        }

        public ListCollectionView UserBlogs { get; private set; }

        private Blog SelectedBlog
        {
            get
            {
                var blogs = UserBlogs;
                return blogs == null ? null : blogs.CurrentItem as Blog;
            }
        }

        public Blog OpenedBlog { get; private set; }

        public ICommand LogoutCommand { get; private set; }
        public ICommand CreateNewBlogCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OpenBlogCommand { get; private set; }
        public ICommand CloseBlogCommand { get; private set; }
        public ICommand DeleteBlogCommand { get; private set; }

        private void CreateNewBlog()
        {
            var userBlogs = UserBlogs;
            if (userBlogs == null) return;

            // add new blog the user's blogs collection
            userBlogs.AddNewItem(new Blog() { OwnerId = _user.Id });
            userBlogs.CommitNew();
        }

        private async Task SaveAsync()
        {
            await _dataContext.SaveChangesAsync();
        }

        private void Cancel()
        {
            _dataContext.RevertChanges();
            
            var openedBlog = OpenedBlog;
            if (openedBlog != null && !_dataContext.Blogs.Any(b => Equals(b, openedBlog)))
            {
                OpenedBlog = null;
                OnPropertyChanged(() => OpenedBlog);
            }
        }

        private void OpenBlog()
        {
            var selectedBlog = SelectedBlog;
            if (selectedBlog == null) return;

            OpenedBlog = selectedBlog;
            OnPropertyChanged(() => OpenedBlog);
        }

        private void CloseBlog()
        {
            OpenedBlog = null;
            OnPropertyChanged(() => OpenedBlog);
        }

        private void DeleteBlog()
        {
            var blogs = UserBlogs;
            if (blogs == null) return;

            var blog = blogs.CurrentItem as Blog;
            //var blog = SelectedBlog;
            if (blog == null) return;

            // instead of just removing from user's blogs collection we want to actually remove the blog instance altogether
            blog.Author = null;
            _dataContext.Delete(blog);
        }
    }
}
