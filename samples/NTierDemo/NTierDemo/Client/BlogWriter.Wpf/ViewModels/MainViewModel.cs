using NTierDemo.Client.Domain;
using NTierDemo.Common.Domain.Model.NTierDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlogWriter.Wpf.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly Func<INTierDemoDataContext> _dataContextFactory;

        public MainViewModel(Func<INTierDemoDataContext> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
            OnPropertyChanged(() => IsUserAuthenticated);
        }


        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case "CurrentUser":
                    OnPropertyChanged(() => IsUserAuthenticated);
                    break;

                case "IsUserAuthenticated":
                    if (IsUserAuthenticated)
                    {
                        if (UserBlogsViewModel == null) UserBlogsViewModel = new UserBlogsViewModel(_dataContextFactory, CurrentUser.Username, () => CurrentUser = null);
                        LoginViewModel = null;
                        RegistrationViewModel = null;
                    }
                    else
                    {
                        if (LoginViewModel == null) LoginViewModel = new LoginViewModel(_dataContextFactory, x => CurrentUser = x);
                        if (RegistrationViewModel == null) RegistrationViewModel = new RegistrationViewModel(_dataContextFactory, x => CurrentUser = x);
                        UserBlogsViewModel = null;
                    }

                    break;
            }
        }


        internal INTierDemoDataContext DataContext { get; private set; }


        #region View models

        public LoginViewModel LoginViewModel
        {
            get { return _loginViewModel; }
            private set { _loginViewModel = value; OnPropertyChanged(() => LoginViewModel); }
        }
        private LoginViewModel _loginViewModel;

        public RegistrationViewModel RegistrationViewModel
        {
            get { return _registrationViewModel; }
            private set { _registrationViewModel = value; OnPropertyChanged(() => RegistrationViewModel); }
        }
        private RegistrationViewModel _registrationViewModel;

        public UserBlogsViewModel UserBlogsViewModel
        {
            get { return _userBlogsViewModel; }
            private set { _userBlogsViewModel = value; OnPropertyChanged(() => UserBlogsViewModel); }
        }
        private UserBlogsViewModel _userBlogsViewModel;
        
        #endregion View models


        #region User

        public bool IsUserAuthenticated
        {
            get { return CurrentUser != null; }
        }

        public Author CurrentUser
        {
            get { return _currentUser; }
            private set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged(() => CurrentUser);
                }
            }
        }
        private Author _currentUser;

        #endregion User
    }
}
