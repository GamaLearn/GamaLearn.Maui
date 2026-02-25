// GAMALEARN.MAUI BUILD SCRIPT
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

// TASKS
Task("Clean")
    .Does(() =>
{
    CleanDirectory("./Artifacts");
    CleanDirectory("./Binlog");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetRestore("GamaLearn.Maui.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetBuildSettings
    {
        Configuration = configuration,
        NoRestore = true,
        MSBuildSettings = new DotNetMSBuildSettings()
    };

    // Enable binary logging
    settings.MSBuildSettings.BinaryLogger = new MSBuildBinaryLoggerSettings
    {
        Enabled = true,
        FileName = "./Binlog/build.binlog"
    };

    DotNetBuild("GamaLearn.Maui.sln", settings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Uncomment when you have test projects
    // var testSettings = new DotNetTestSettings
    // {
    //     Configuration = configuration,
    //     NoBuild = true,
    //     NoRestore = true
    // };
    // DotNetTest("GamaLearn.Maui.sln", testSettings);

    Information("No tests configured yet - skipping test task");
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    var packSettings = new DotNetPackSettings
    {
        Configuration = configuration,
        NoBuild = true,
        NoRestore = true,
        OutputDirectory = "./Artifacts"
    };

    DotNetPack("GamaLearn.Maui.pack.slnf", packSettings);
});

Task("Default")
    .IsDependentOn("Pack");

// EXECUTION
RunTarget(target);