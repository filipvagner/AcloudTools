using System.Text;

namespace AcloudTools.Models
{
    class PostUri
    {
        public string SetPostUri(string azResource, string subscriptionId, string resourceGroup, string virtualMachine)
        {
            StringBuilder uriBuild = new StringBuilder();
            uriBuild.Append(azResource);
            uriBuild.Append("subscriptions/");
            uriBuild.Append(subscriptionId);
            uriBuild.Append("/resourceGroups/");
            uriBuild.Append(resourceGroup);
            uriBuild.Append("/providers/Microsoft.Compute/virtualMachines/");
            uriBuild.Append(virtualMachine);
            uriBuild.Append("/runCommand?api-version=2020-12-01");

            return uriBuild.ToString();
        }
    }
}