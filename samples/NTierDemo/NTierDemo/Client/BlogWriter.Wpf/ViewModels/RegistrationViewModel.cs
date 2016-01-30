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
    public class RegistrationViewModel:ViewModel
    {
        private readonly Func<INTierDemoDataContext> _dataContextFactory;
        private readonly Action<User> _setUser;

        public RegistrationViewModel(Func<INTierDemoDataContext> dataContextFactory, Action<User> setUser, Action cancel)
        {
            _dataContextFactory = dataContextFactory;
            _setUser = setUser;

            User = new User();
            RegisterCommand = new AsyncRelayCommand(RegisterAsync, () => User.IsValid);
            CancelCommand = new RelayCommand(cancel);
        }

        public ICommand RegisterCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private async Task RegisterAsync()
        {
            // ensure user is valid
            if (!User.IsValid) throw new Exception("User is not valid!");

            var dataContext = _dataContextFactory();
            dataContext.MergeOption = MergeOption.NoTracking;

            // ensure user does not exists
            var query =
                from author in dataContext.Users.AsQueryable()
                where author.Username == User.Username
                select author;
            if ((await query.ExecuteAsync()).ResultSet.Any()) throw new Exception(string.Format("User with user name '{0}' already exists!", User.Username));

            // save as new user
            dataContext.Add(User);
            await dataContext.SaveChangesAsync();

            // set as current user
            _setUser(User);
        }

        public User User { get; private set; }
    }
}
