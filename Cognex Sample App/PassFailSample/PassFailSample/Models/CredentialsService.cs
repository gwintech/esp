﻿using System.Linq;
using Xamarin.Auth;

namespace PassFailSample.Models
{
    public class CredentialsService : ICredentialsService
    {
        private const string _AppName = "Cognex Demo App";
        public string UserName
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(_AppName).FirstOrDefault();
                return (account != null) ? account.Username : null;
            }
        }

        public string Password
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(_AppName).FirstOrDefault();
                return (account != null) ? account.Properties["Password"] : null;
            }
        }

        public void SaveCredentials(string userName, string password)
        {
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                Account account = new Account
                {
                    Username = userName
                };
                account.Properties.Add("Password", password);
                AccountStore.Create().Save(account, _AppName);
            }

        }

        public void DeleteCredentials()
        {
            var account = AccountStore.Create().FindAccountsForService(_AppName).FirstOrDefault();
            if (account != null)
            {
                AccountStore.Create().Delete(account, _AppName);
            }
        }

        public bool DoCredentialsExist()
        {
            return AccountStore.Create().FindAccountsForService(_AppName).Any() ? true : false;
        }
    }
}
