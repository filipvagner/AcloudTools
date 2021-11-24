using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcloudTools.Models
{
    class AuthOps
    {
        public static async Task<AuthenticationResult> GetAuthenticatedAsync(string tenantId, string clientId, string clientSecret)
        {
            List<string> scopes = new List<string>
            {
                "https://management.azure.com/.default"
            };

            ConfidentialClientApplication app;
            app = (ConfidentialClientApplication)ConfidentialClientApplicationBuilder.Create(clientId)
                                                      .WithClientSecret(clientSecret)
                                                      .WithAuthority("https://login.microsoftonline.com/" + tenantId)
                                                      .Build();
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            return result;
        }
    }
}