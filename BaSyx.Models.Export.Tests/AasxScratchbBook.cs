using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Export;

namespace BaSyx.Models.Export.ScratchBook;

public class AasxScratchbBook
{
    public void AasToAasx()
    {
        const string aasxPath = "path\\to\\test.aasx";

        Identifier demoIdentifier = new Identifier("demoIdentifier", KeyType.Custom);
        AssetAdministrationShell shell = new AssetAdministrationShell("demoIdShort", demoIdentifier);
        
        AssetAdministrationShell[] shellArray = { shell };
        AssetAdministrationShellEnvironment_V2_0 shellEnvironment = new AssetAdministrationShellEnvironment_V2_0(shellArray);
        

        AASX aasx = new AASX(aasxPath);
        aasx.AddEnvironment(demoIdentifier, shellEnvironment, ExportType.Xml);
    }
}