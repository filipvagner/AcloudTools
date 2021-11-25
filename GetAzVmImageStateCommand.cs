using AcloudTools.Models;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcloudTools
{
    [Cmdlet(VerbsCommon.Get, "AzVmImageState")]
    [OutputType(typeof(string))]
    public class GetAzVmImageStateCommand : Cmdlet
    {
        #region public parameters

        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "The virtual machine name.")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return vmName; }
            set { vmName = value; }
        }

        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Resource group where is placed virtual machine.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName
        {
            get { return resourceGroupName; }
            set { resourceGroupName = value; }
        }

        [Parameter]
        public string ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }

        [Parameter]
        public string ClientSecret
        {
            get { return clientSecret; }
            set { clientSecret = value; }
        }

        [Parameter]
        public string SubscriptionId
        {
            get { return subscriptionId; }
            set { subscriptionId = value; }
        }

        [Parameter]
        public string TenantId
        {
            get { return tenantId; }
            set { tenantId = value; }
        }

        [Parameter(
           Mandatory = false,
           HelpMessage = "Requires token (Get-AzAccessToken).")]
        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        #endregion public parameters

        #region private parameters

        private string vmName;
        private string resourceGroupName;
        private string clientId;
        private string clientSecret;
        private string subscriptionId;
        private string tenantId;
        private string token;
        readonly string azResource = "https://management.azure.com/";
        readonly string commandToSend = @"{
                commandId: ""RunPowerShellScript"",
                script:[
                    ""(Get-ItemProperty HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Setup\\State\\).ImageState""
                ]
                }";
        PostUri setPostUri = new PostUri();
        HttpClient clientToCall = new HttpClient();
        ImageStateResult imageStateObj = new ImageStateResult();

        #endregion private parameters

        protected override void ProcessRecord()
        {
            var imageStateObjList = GetImageStateAsync();
            imageStateObjList.Wait();
            var imgageStateResultObj = imageStateObjList.Result;
            WriteObject(imgageStateResultObj, true);
        }

        private async Task<ImageStateResult> GetImageStateAsync()
        {
            Task<AuthenticationResult> authTask = AuthOps.GetAuthenticatedAsync(tenantId, clientId, clientSecret);
            authTask.Wait();
            var authResult = authTask.Result;
            Uri postUri = new Uri(setPostUri.SetPostUri(azResource, subscriptionId, resourceGroupName, vmName));
            var postBody = new StringContent(commandToSend, Encoding.UTF8, "application/json");

            clientToCall.DefaultRequestHeaders.Add("Authorization", "Bearer " + authResult.AccessToken);
            clientToCall.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage postResponse = await clientToCall.PostAsync(postUri, postBody);

            HttpResponseMessage apiResponse = new HttpResponseMessage();
            int reposnseCounter = 0;
            bool responseSuccess = true;
            TimeSpan counterTime = new TimeSpan(0, 0, 10);
            do
            {
                apiResponse = await clientToCall.GetAsync(postResponse.Headers.Location);
                Thread.Sleep(counterTime);
                reposnseCounter++;
                if (reposnseCounter == 5)
                {
                    responseSuccess = false;
                    break;
                }
            } while (!apiResponse.ReasonPhrase.Equals("OK"));

            if (!responseSuccess)
            {
                imageStateObj.Code = "Failed";
                imageStateObj.DisplayStatus = "Provisioning failed";
                imageStateObj.Level = "Info";
                imageStateObj.Message = null;

                return imageStateObj;
            }
            else
            {
                var apiResponseString = apiResponse.Content.ReadAsStringAsync().Result;
                int firstSqrBracketIndex = apiResponseString.IndexOf("[");
                apiResponseString = apiResponseString.Remove(0, firstSqrBracketIndex);
                int lastCrlyBracketIndex = apiResponseString.LastIndexOf("}");
                apiResponseString = apiResponseString.Remove(lastCrlyBracketIndex, 1);

                List<ImageStateResult> imageStateResults = JsonConvert.DeserializeObject<List<ImageStateResult>>(apiResponseString);
                imageStateObj =
                    (from imgRes in imageStateResults
                     where imgRes.Message == "IMAGE_STATE_COMPLETE"
                     select imgRes).First();

                return imageStateObj;
            }

        }
    }
}