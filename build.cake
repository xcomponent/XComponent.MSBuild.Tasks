#tool "nuget:?package=NUnit.Runners&version=2.6.4"
#load "cake.scripts/utilities.cake"
#addin "Cake.FileHelpers"
#addin "Cake.Incubator&version=1.0.56"

var target = Argument("target", "Build");
var buildConfiguration = Argument("Configuration", "Release");
var version = Argument("Version", "1.0.0-build1");
var apiKey = Argument("NugetKey", "");

Task("Clean")
.Does(() =>
{
    if (DirectoryExists("nuget"))
    {
        CleanDirectory("nuget");
    }

    if (DirectoryExists("packages"))
    {
        CleanDirectory("packages");
    }

    CleanSolution(@"./XComponent.MSBuild.Tasks.sln", buildConfiguration);
});

Task("RestoreNugetPackages")
.Does(() =>
{
    NuGetRestore("XComponent.MSBuild.Tasks.sln", new NuGetRestoreSettings { NoCache = true });
});

Task("Build")
.Does(() =>
{
    BuildSolution(@"./XComponent.MSBuild.Tasks.sln", buildConfiguration);
});

Task("Test")
.Does(() =>
{
    var testAssembliesPattern = 
        "./XComponent.MSBuild.Tasks.Test/bin/" + buildConfiguration + "/XComponent.MSBuild.Tasks.Test.dll";
    
    var testAssemblies = GetFiles(testAssembliesPattern);
    var nunitSettings = new NUnitSettings(){ ResultsFile = "TestResults.xml" };
    NUnit(testAssemblies, nunitSettings);
});

Task("CreatePackage")
.Does(() =>
{
    EnsureDirectoryExists("nuget");

    var formattedNugetVersion = FormatNugetVersion(version);

    var filesToPackPatterns = new string[]
        {
            "./XComponent.MSBuild.Tasks/bin/"+ buildConfiguration + "/*.dll",
            "./XComponent.MSBuild.Tasks/bin/"+ buildConfiguration + "/*.pdb",
            "./XComponent.MSBuild.Tasks/bin/"+ buildConfiguration + "/*.xml"
        };

    var filesToPack = GetFiles(filesToPackPatterns);

    var nuSpecContents = new List<NuSpecContent>();

    foreach (var file in filesToPack)
    {
        if (!file.FullPath.Contains("CodeAnalysisLog.xml"))
        {
            nuSpecContents.Add(new NuSpecContent {Source = file.FullPath, Target = @"lib\net451"});
        }
    }

    var nugetPackSettings = new NuGetPackSettings()
    { 
        OutputDirectory = @"./nuget",
        Files = nuSpecContents,
        Version = formattedNugetVersion,
        IncludeReferencedProjects = true
    };

    NuGetPack("XComponent.MSBuild.Tasks.nuspec", nugetPackSettings);
});

Task("PushPackage")
.IsDependentOn("All")
.Does(() =>
{
    var formattedNugetVersion = FormatNugetVersion(version);
    if (FileExists("./nuget/XComponent.MSBuild.Tasks." + formattedNugetVersion + ".nupkg")
        && !string.IsNullOrEmpty(apiKey))
    {
        var package = "./nuget/XComponent.MSBuild.Tasks." + formattedNugetVersion + ".nupkg";
        var nugetPushSettings = new NuGetPushSettings 
        {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = apiKey
        };

        NuGetPush(package, nugetPushSettings);
    }
});

Task("All")
  .IsDependentOn("Clean")
  .IsDependentOn("RestoreNugetPackages")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("CreatePackage")
  .Does(() =>
  {
  });

RunTarget(target);