using System;
namespace CareVisit.Core.Services
{
    public interface IAccountService
    {
        bool Login(string username, string password);
    }
}
