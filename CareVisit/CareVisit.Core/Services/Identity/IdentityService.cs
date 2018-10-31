using CareVisit.Core.Services.RestClients;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CareVisit.Core.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IRestClient restClient;
        private string _codeVerifier;

        public IdentityService(IRestClient restClient)
        {
            this.restClient = restClient;
        }
    }
}
