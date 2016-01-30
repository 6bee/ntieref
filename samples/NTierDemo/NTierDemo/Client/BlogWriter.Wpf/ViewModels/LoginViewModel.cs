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
using BlogWriter.Wpf.Extensions;
using System.Security;

namespace BlogWriter.Wpf.ViewModels
{
    public class LoginViewModel:ViewModel
    {
        private readonly Func<INTierDemoDataContext> _dataContextFactory;
        private readonly Action<User> _setUser;

        public LoginViewModel(Func<INTierDemoDataContext> dataContextFactory, Action<User> setUser, Action openRegistration)
        {
            _dataContextFactory = dataContextFactory;
            _setUser = setUser;

            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
            OpenRegistrationCommand = new RelayCommand(openRegistration);

            // demo purpose: set existung user and password
            Username = "mmeyer";
            Password = "****".ToSecureString();
        }

        private async Task LoginAsync(object pw)
        {
            var dataContext = _dataContextFactory();
            dataContext.MergeOption = MergeOption.NoTracking;

            // retrieve user from backend
            var password = Password.ToUnsecureString();
            var query =
                from author in dataContext.Users.AsQueryable()
                where author.Username == Username && author.Password == password
                select author;

            var user = (await query.ExecuteAsync()).ResultSet.FirstOrDefault();

            // check if user found
            ErrorMessage = user == null ? "Username or password invalid." : null;

            // set as current user
            _setUser(user);
        }

        private bool CanLogin(object pw)
        {
            return !string.IsNullOrWhiteSpace(Username) 
                && Password != null
                && Password.Length > 0;
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(() => Username); }
        }
        private string _username;

        public SecureString Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(() => Password); }
        }
        private SecureString _password;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(() => ErrorMessage); }
        }
        private string _errorMessage;

        public ICommand LoginCommand { get; private set; }
        public ICommand OpenRegistrationCommand { get; private set; }
    }
}
