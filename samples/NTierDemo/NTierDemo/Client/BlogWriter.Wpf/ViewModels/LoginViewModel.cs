using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NTierDemo.Client.Domain;
using NTierDemo.Common.Domain.Model.NTierDemo;
using NTier.Client.Domain;
using BlogWriter.Wpf.Commands;

namespace BlogWriter.Wpf.ViewModels
{
    public class LoginViewModel:ViewModel
    {
        private readonly Func<INTierDemoDataContext> _dataContextFactory;
        private readonly Action<Author> _setUser;

        public LoginViewModel(Func<INTierDemoDataContext> dataContextFactory, Action<Author> setUser)
        {
            _dataContextFactory = dataContextFactory;
            _setUser = setUser;

            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);

            // demo purpose: set existung user and password
            Username = "mmeyer";
            Password = "****";
        }

        public ICommand LoginCommand { get; private set; }

        private async Task LoginAsync()
        {
            var dataContext = _dataContextFactory();
            dataContext.MergeOption = MergeOption.NoTracking;

            // retrieve user from backend
            var query =
                from author in dataContext.Authors.AsQueryable()
                where author.Username == Username && author.Password == Password
                select author;

            var user = (await query.ExecuteAsync()).FirstOrDefault();

            // check if user found
            ErrorMessage = user == null ? "Username or password invalid." : null;

            // set as current user
            _setUser(user);
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) 
                && !string.IsNullOrWhiteSpace(Password);
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(() => Username); }
        }
        private string _username;

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(() => Password); }
        }
        private string _password;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(() => ErrorMessage); }
        }
        private string _errorMessage;
    }
}
