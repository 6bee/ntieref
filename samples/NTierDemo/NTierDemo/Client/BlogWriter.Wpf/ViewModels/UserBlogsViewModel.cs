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
        private readonly User _user;

        public UserBlogsViewModel(Func<INTierDemoDataContext> dataContextFactory, User user, Action logout)
        {
            _dataContext = dataContextFactory();
            _user = user;

            LogoutCommand = new RelayCommand(logout);
            CreateNewBlogCommand = new RelayCommand(CreateNewBlog, () => OpenedBlog == null);
            CreateNewPostCommand = new RelayCommand(CreateNewPost, () => OpenedBlog != null && OpenedPost == null);
            SaveCommand = new AsyncRelayCommand(SaveAsync, () => _dataContext.HasChanges && _dataContext.Blogs.All(x => x.IsValid));
            CancelCommand = new RelayCommand(Cancel, () => _dataContext.HasChanges);
            OpenBlogCommand = new RelayCommand(OpenBlog, (o) => o is Blog);
            OpenPostCommand = new RelayCommand(OpenPost, (o) => o is Post);
            CloseBlogCommand = new RelayCommand(CloseBlog, () => OpenedBlog != null);
            ClosePostCommand = new RelayCommand(ClosePost, () => OpenedPost != null);
            DeleteBlogCommand = new RelayCommand(DeleteBlog, (o) => o is Blog);
            DeletePostCommand = new RelayCommand(DeletePost, (o) => o is Post);

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            using (new BusyCursor())
            {
                var query = _dataContext.Blogs
                    .AsQueryable()
                    .Include("Posts")
                    .Where(x => x.OwnerId == _user.Id);

                var result = query.ExecuteAsync();

                // creating a collection view based on the user's blogs
                UserBlogs = await result;
                OnPropertyChanged(() => UserBlogs);
            }
        }

        public NTier.Client.Domain.IEntitySet<Blog> UserBlogs { get; private set; }

        public Blog OpenedBlog { get; private set; }

        public Post OpenedPost { get; private set; }

        public ICommand LogoutCommand { get; private set; }
        public ICommand CreateNewBlogCommand { get; private set; }
        public ICommand CreateNewPostCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OpenBlogCommand { get; private set; }
        public ICommand OpenPostCommand { get; private set; }
        public ICommand CloseBlogCommand { get; private set; }
        public ICommand ClosePostCommand { get; private set; }
        public ICommand DeleteBlogCommand { get; private set; }
        public ICommand DeletePostCommand { get; private set; }


        private void CreateNewBlog()
        {
            var userBlogs = UserBlogs;
            if (userBlogs == null) return;

            // add new blog the user's blogs collection
            userBlogs.Add(new Blog() { OwnerId = _user.Id });
        }

        private void CreateNewPost()
        {
            var blog = OpenedBlog;
            if (blog == null) return;

            var post = new Post();
            blog.Posts.Add(post);
            OpenedPost = post;
            OnPropertyChanged(() => OpenedPost);
        }

        private async Task SaveAsync()
        {
            await _dataContext.SaveChangesAsync();
        }

        private void Cancel()
        {
            _dataContext.RevertChanges();

            var openedPost = OpenedPost;
            if (openedPost != null && !_dataContext.Posts.Any(i => Equals(i, openedPost)))
            {
                OpenedPost = null;
                OnPropertyChanged(() => OpenedPost);
            }

            var openedBlog = OpenedBlog;
            if (openedBlog != null && !_dataContext.Blogs.Any(b => Equals(b, openedBlog)))
            {
                OpenedBlog = null;
                OnPropertyChanged(() => OpenedBlog);
            }
        }

        private void OpenBlog(object o)
        {
            OpenedBlog = o as Blog;
            OnPropertyChanged(() => OpenedBlog);
        }

        private void OpenPost(object o)
        {
            OpenedPost = o as Post;
            OnPropertyChanged(() => OpenedPost);
        }

        private void CloseBlog()
        {
            OpenedBlog = null;
            OnPropertyChanged(() => OpenedBlog);
        }

        private void ClosePost()
        {
            OpenedPost = null;
            OnPropertyChanged(() => OpenedPost);
        }

        private void DeleteBlog(object o)
        {
            var blogs = UserBlogs;
            if (blogs == null) return;

            var blog = o as Blog;
            if (blog == null) return;

            // instead of just removing from user's blogs collection we want to actually remove the blog instance altogether
            blog.Owner = null;
            _dataContext.Delete(blog);
        }

        private void DeletePost(object o)
        {
            var blog = OpenedBlog;
            if (blog == null) return;

            var post = o as Post;
            if (post == null) return;

            if (blog.Posts.Contains(post))
            {
                blog.Posts.Remove(post);
            }
            _dataContext.Delete(post);
        }
    }
}
