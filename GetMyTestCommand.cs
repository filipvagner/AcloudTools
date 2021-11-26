using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AcloudTools
{
    [Cmdlet(VerbsCommon.Get, "MyTestCommand")]
    [OutputType(typeof(string))]
    public class GetMyTestCommand : Cmdlet
    {
        protected override void ProcessRecord()
        {
            PowerShell psCommand = PowerShell.Create();
            Collection<PSObject> psCommandOutput = new Collection<PSObject>();

            psCommand.AddCommand("Get-AzAccessToken");
            PSObject accessToken = (from tokenItem in psCommand.Invoke()
                                    where tokenItem.Members["Token"] != null
                                    select tokenItem).First();

            WriteObject(accessToken.Members["Token"].Value, true);
        }
    }
}
