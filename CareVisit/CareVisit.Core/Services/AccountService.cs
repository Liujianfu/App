using System;
namespace CareVisit.Core.Services
{
    public class AccountService:IAccountService
    {
        public AccountService()
        {
        }

        public bool Login(string username, string password)
        {
            //TODO:verify it by using SSO service

            return true;
        }
    }
}
