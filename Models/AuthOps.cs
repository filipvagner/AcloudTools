using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AcloudTools.Models
{
    class AuthOps
    {
        static async Task<AuthenticationResult> GetAuthenticatedAsync(string tenantId, string clientId, string clientSecret)
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

        public static string GetAadToken(string tenantId, string clientId, string clientSecret)
        {
            Task<AuthenticationResult> authTask = GetAuthenticatedAsync(tenantId, clientId, clientSecret);
            authTask.Wait();
            var authResult = authTask.Result;

            return authResult.AccessToken;
        }

        public static string GetLocalToken()
        {
            PowerShell psCommand = PowerShell.Create();
            psCommand.AddCommand("Get-AzAccessToken");
            PSObject accessToken = (from tokenItem in psCommand.Invoke()
                                    where tokenItem.Members["Token"] != null
                                    select tokenItem).First();

            return accessToken.Members["Token"].Value.ToString();
        }
    }
}