/// <summary>
///     Just a simple build script.
/// </summary>

// Install tools
#tool GitVersion.CommandLine&prerelease

// *********************
//      ARGUMENTS
// *********************
var Target = Argument("target", "default");
var Configuration = Argument("configuration", "release");
var IsPrerelease = HasArgument("pre");

// *********************
//      VARIABLES
// *********************
var Solution = File("engenious.sln");
var BuildVerbosity = Verbosity.Minimal;
CreateDirectory(Directory("./logs"));
var Version = GitVersion(new GitVersionSettings {LogFilePath = new FilePath("./logs/gitver.log")});

// *********************
//      SETUP
// *********************
Setup(context =>
{
    Information("Building version {0} of engenious.", Version.InformationalVersion);
});

// *********************
//      TASKS
// *********************

/// <summary>
///     Task to build the solution. Using MSBuild on Windows and MDToolBuild on OSX / Linux
/// </summary>
Task("build")
    .IsDependentOn("patch-version")
    .Does(() =>
    {
        DotNetBuild(Solution, cfg =>
        {
            cfg.Configuration = Configuration;
            cfg.Verbosity = BuildVerbosity;
        });
    });

/// <summary>
///     Task to clean all obj and bin directories as well as the ./output folder.
///     Commonly called right before build.
/// </summary>
Task("clean")
    .Does(() =>
    {
        CleanDirectories("./output");
        CleanDirectories("./bin");
        CleanDirectories(string.Format("./src/**/obj/{0}", Configuration));
    });

/// <summary>
///     The default task with a predefined flow.
/// </summary>
Task("default")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .IsDependentOn("build")
    .IsDependentOn("pack");

/// <summary>
///     Task to bundle build results into a NuGet package.
/// </summary>
Task("pack")
    .Does(() =>
    {
        var artifacts = Directory("./output/artifacts");
        CreateDirectory(artifacts);

        NuGetPack("./engenious.nuspec", new NuGetPackSettings
        {
            Version                 = IsPrerelease ? Version.NuGetVersion : Version.MajorMinorPatch,
            OutputDirectory         = artifacts
        });
    });

/// <summary>
///     Task to patch the current version into the assembly infos
/// </summary>
Task("patch-version")
    .Does(() =>
    {
        // TODO: modify solution to use a shared assembly info
        //Information("Skipped due to: Modify solution to use a shared assembly info");
        //return;

        // Patch assembly info
        var solutionInfoFile = "./SolutionInfo.cs";
        CreateAssemblyInfo(solutionInfoFile, new AssemblyInfoSettings
        {
            Product = "engenious",
            Company = "jvbsl",
            Configuration = Configuration,
            Version = Version.AssemblySemVer,
            FileVersion = string.Format("{0}.{1}.0", Version.Major, Version.Minor),
            InformationalVersion = Version.InformationalVersion,
            Copyright = "Copyright (c) jvbsl 2016"
        });
    });

/// <summary>
///     Task to rebuild. Nothing else than a clean followed by build.
/// </summary>
Task("rebuild")
    .IsDependentOn("clean")
    .IsDependentOn("build");

/// <summary>
///     Task to restore NuGet packages on solution level for all containing projects.
/// </summary>
Task("restore")
    .Does(() => NuGetRestore(Solution));

// Execution
RunTarget(Target);
