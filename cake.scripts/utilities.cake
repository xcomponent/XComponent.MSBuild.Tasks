var FormatNugetVersion = new Func<string, string>(currentVersion =>
{
    var result = currentVersion;
    var versionType = string.Empty;

    if (currentVersion.Contains("build"))
    {
        versionType = "build";
    }
    else if (currentVersion.Contains("rc"))
    {
        versionType = "rc";
    }

    if (!string.IsNullOrEmpty(versionType))
    {
        var versionComponents = currentVersion.Split('-');
        var majorVersion = versionComponents[0];
        var buildVersion = versionComponents[1];
        var buildNumberStr = buildVersion.Substring(
                                    versionType.Length,
                                    buildVersion.Length - versionType.Length);

        var buildNumber = int.Parse(buildNumberStr);
        var buildNumberPrefix = string.Empty;
        if (buildNumber < 10)
        {
            buildNumberPrefix = "00";
        }
        else if (buildNumber < 100)
        {
            buildNumberPrefix = "0";
        }
        else if (buildNumber > 1000)
        {
            buildNumberPrefix = string.Empty;
        }

        result = majorVersion + "-" + versionType + "v" + buildNumberPrefix + buildNumberStr;
    }

    return result;
});

var CleanSolution = new Action<string, string>((solutionPath, configuration) =>
{
    if (IsRunningOnUnix())
    {
        var xBuildSettings = new XBuildSettings();
        xBuildSettings.SetConfiguration(configuration)
        .WithTarget("Clean");

        XBuild(solutionPath, xBuildSettings);
    }
    else
    {
        var msBuildSettings = new MSBuildSettings();
        msBuildSettings.SetConfiguration(configuration)
        .WithTarget("Clean");

        MSBuild(solutionPath, msBuildSettings);
    }
});

var BuildSolution = new Action<string, string>((solutionPath, configuration) =>
{
    if (IsRunningOnUnix())
    {
        var xBuildSettings = new XBuildSettings() { Configuration = configuration };
        xBuildSettings.WithTarget("Rebuild");
        
        XBuild(solutionPath, xBuildSettings);
    }
    else
    {
        var msBuildSettings = new MSBuildSettings() { Configuration = configuration };
        msBuildSettings.WithTarget("Rebuild");
        
        MSBuild(solutionPath, msBuildSettings);
    }
});