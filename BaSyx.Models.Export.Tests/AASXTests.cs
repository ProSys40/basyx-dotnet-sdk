using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Export;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using System.IO.Compression;
using File = System.IO.File;

namespace BaSyx.Models.Export.Tests;

public class AASXTests
{
    private const string TEST_RESOURCES_DIR = "../../../Resources";
    
    [Fact]
    // TODO: figure out how to write a file into ./Resources/tmp/
    public void MetamodelToAASX()
    { /* FIXME: further elaborate this test!
         * add Asset
         * add Submodel(s)
         * add SubmodelElement(s)
         * add SubmodelElementCollection(s)
         */

        string tmpAasxDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        string expectedAasxPath = Path.Combine(TEST_RESOURCES_DIR, "test.aasx");
        string actualAasxPath = Path.Combine(tmpAasxDir, "tmp.aasx");

        Directory.CreateDirectory(tmpAasxDir);
        Identifier demoIdentifier = new Identifier("demoIdentifier", KeyType.Custom);
        AssetAdministrationShell shell = new AssetAdministrationShell("demoIdShort", demoIdentifier);
        AasToAasx(shell, actualAasxPath);
        

        string tempExpectedDestinationDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string tempActualDestinationDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempExpectedDestinationDir);
        Directory.CreateDirectory(tempActualDestinationDir);

        ZipFile.ExtractToDirectory(expectedAasxPath, tempExpectedDestinationDir);
        ZipFile.ExtractToDirectory(actualAasxPath, tempActualDestinationDir);

        Assert.True(DirectoriesAreEqual(tempExpectedDestinationDir, tempActualDestinationDir));

        Directory.Delete(tmpAasxDir, true);
        Directory.Delete(tempExpectedDestinationDir, true);
        Directory.Delete(tempActualDestinationDir, true);

    }

    private bool DirectoriesAreEqual(string tempExpectedDestinationDir, string tempActualDestinationDir)
    {
        var dir1 = new DirectoryInfo(tempExpectedDestinationDir);
        var dir2 = new DirectoryInfo(tempActualDestinationDir);

        return DirectoriesAreEqualRecursive(dir1, dir2);
    }

    private bool DirectoriesAreEqualRecursive(DirectoryInfo dir1, DirectoryInfo dir2)
    {

        if (dir1.Name.Equals("_rels") || dir1.Name.Equals("_rels"))
        {
            /* FIXME: "_rels" directory contains content with generated Ids that differ with each creation. -> will currently this _rels directory is be ignored
             * Consider special treatment for content of this directory
             */
            return true;
        }

        var files1 = dir1.GetFiles();
        var files2 = dir2.GetFiles();

        if (files1.Length != files2.Length)
            return false;

        foreach (var file1 in files1)
        {
            var matchingFile2 = files2.FirstOrDefault(f => f.Name == file1.Name);
            if (matchingFile2 == null || !FileContentsAreEqual(file1.FullName, matchingFile2.FullName))
                return false;
        }

        var subdirs1 = dir1.GetDirectories();
        var subdirs2 = dir2.GetDirectories();

        if (subdirs1.Length != subdirs2.Length)
            return false;

        foreach (var subdir1 in subdirs1)
        {
            var matchingSubdir2 = subdirs2.FirstOrDefault(d => d.Name == subdir1.Name);
            if (matchingSubdir2 == null || !DirectoriesAreEqualRecursive(subdir1, matchingSubdir2))
                return false;
        }

        return true;
    }

    private bool FileContentsAreEqual(string filePath1, string filePath2)
    {
        byte[] file1Contents = File.ReadAllBytes(filePath1);
        byte[] file2Contents = File.ReadAllBytes(filePath2);

        if (file1Contents.Length != file2Contents.Length)
            return false;

        for (int i = 0; i < file1Contents.Length; i++)
        {
            if (file1Contents[i] != file2Contents[i])
                return false;
        }

        return true;
    }

    private static void AasToAasx(AssetAdministrationShell shell, string aasxPath)
    {
        AssetAdministrationShell[] shellArray = { shell };
        AssetAdministrationShellEnvironment_V2_0 shellEnvironment = new AssetAdministrationShellEnvironment_V2_0(shellArray);

        using (AASX aasx = new AASX(aasxPath))
        {
            aasx.AddEnvironment(shell.Identification, shellEnvironment, ExportType.Xml);
        }
    }
}