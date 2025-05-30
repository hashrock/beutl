﻿using System.Text;
using Nuke.Common.Tools.InnoSetup;
using Serilog;
using static Nuke.Common.Tools.InnoSetup.InnoSetupTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter] Configuration Configuration = Configuration.Release;

    [Parameter] RuntimeIdentifier Runtime = null;

    [Parameter] bool SelfContained = false;

    [Parameter] string Version = "1.0.0";

    [Parameter] string AssemblyVersion = "1.0.0.0";

    [Parameter] string InformationalVersion = "1.0.0.0";

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;


    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(p => p.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(p => p.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution)
                .When(_ => Runtime != null, s => s.SetRuntime(Runtime)));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetVersions(Version, AssemblyVersion, InformationalVersion)
                .EnableNoRestore());
        });

    private string GetTFM()
    {
        AbsolutePath mainProj = SourceDirectory / "Beutl" / "Beutl.csproj";
        using IProcess proc = StartProcess(DotNetPath, $"msbuild --getProperty:TargetFrameworks {mainProj}");
        proc.WaitForExit();
        return proc.Output.First().Text.Split(';')[0];
    }

    Target Publish => _ => _
        //.DependsOn(Compile)
        .DependsOn(Restore)
        .Executes(() =>
        {
            AbsolutePath mainProj = SourceDirectory / "Beutl" / "Beutl.csproj";
            AbsolutePath mainOutput = OutputDirectory / "Beutl";

            string tfm = GetTFM();

            DotNetPublish(s => s
                .EnableNoRestore()
                .When(_ => Runtime != null, s => s
                    .SetRuntime(Runtime)
                    .SetSelfContained(SelfContained)
                    .SetProperty("RuntimeIdentifiers", Runtime.ToString()))
                .When(_ => Runtime == RuntimeIdentifier.win_x64, s => s.SetFramework($"{tfm}-windows"))
                .When(_ => Runtime != RuntimeIdentifier.win_x64, s => s.SetFramework(tfm))
                .SetConfiguration(Configuration)
                .SetVersions(Version, AssemblyVersion, InformationalVersion)
                .SetProject(mainProj)
                .SetOutput(mainOutput)
                .SetProperty("NukePublish", true));

            string[] subProjects =
            [
                "Beutl.ExceptionHandler",
                "Beutl.PackageTools.UI",
                "Beutl.WaitingDialog",
            ];
            foreach (string item in subProjects)
            {
                AbsolutePath output = OutputDirectory / item;
                DotNetPublish(s => s
                    .When(_ => Runtime != null, s => s
                        .SetRuntime(Runtime)
                        .SetSelfContained(SelfContained)
                        .SetProperty("RuntimeIdentifiers", Runtime.ToString()))
                    .When(_ => Runtime == RuntimeIdentifier.win_x64, s => s.SetFramework($"{tfm}-windows"))
                    .When(_ => Runtime != RuntimeIdentifier.win_x64, s => s.SetFramework(tfm))
                    .EnableNoRestore()
                    .SetConfiguration(Configuration)
                    .SetVersions(Version, AssemblyVersion, InformationalVersion)
                    .SetProject(SourceDirectory / item / $"{item}.csproj")
                    .SetOutput(output));

                output.GlobFiles($"**/{item}*")
                    .Select(p => (Source: p, Target: mainOutput / output.GetRelativePathTo(p)))
                    .ForEach(t => t.Source.Copy(t.Target));
            }
        });

    Target Zip => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            AbsolutePath mainOutput = OutputDirectory / "Beutl";

            // Eg: Beutl-0.0.0+0000.zip
            var fileName = new StringBuilder();
            fileName.Append("Beutl");
            if (Runtime != null)
            {
                fileName.Append('-');
                fileName.Append(Runtime.ToString());
            }

            if (SelfContained && Runtime != null)
            {
                fileName.Append("-standalone");
            }

            fileName.Append('-');
            fileName.Append(Version);
            fileName.Append(".zip");

            mainOutput.CompressTo(ArtifactsDirectory / fileName.ToString());
        });

    Target BuildInstaller => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            InnoSetup(c => c
                .SetKeyValueDefinition("MyAppVersion", AssemblyVersion)
                .SetKeyValueDefinition("MyOutputDir", ArtifactsDirectory)
                .SetKeyValueDefinition("MyLicenseFile", RootDirectory / "LICENSE")
                .SetKeyValueDefinition("MySetupIconFile", RootDirectory / "assets/logos/logo.ico")
                .SetKeyValueDefinition("MySource", OutputDirectory / "Beutl")
                .SetKeyValueDefinition("MyOutputBaseFilename", $"beutl{(SelfContained ? "-standalone" : "")}-setup")
                .SetScriptFile(RootDirectory / "nukebuild/beutl-setup.iss"));
        });

    Target BundleApp => _ => _
        .Executes(() =>
        {
            // dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-arm64 -p:TargetFramework=net9.0 -p:UseAppHost=true -p:SelfContained=true
            AbsolutePath directory = SourceDirectory / "Beutl";
            AbsolutePath output = OutputDirectory / "AppBundle";
            string tfm = GetTFM();
            DotNetRestore(s => s.SetProjectFile(directory / "Beutl.csproj"));
            DotNetMSBuild(s => s
                .SetProcessWorkingDirectory(directory)
                .SetTargets("BundleApp")
                .SetConfiguration(Configuration)
                .SetVersions(Version, AssemblyVersion, InformationalVersion)
                .SetProperty("PublishDir", output)
                .SetProperty("CFBundleVersion", AssemblyVersion)
                .SetProperty("CFBundleShortVersionString", AssemblyVersion)
                .SetProperty("RuntimeIdentifier", Runtime.ToString())
                .SetProperty("TargetFramework", tfm)
                .SetProperty("UseAppHost", true)
                .SetProperty("SelfContained", true));
        });

    Target NuGetPack => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            string[] projects =
            [
                "Beutl.Configuration",
                "Beutl.Core",
                "Beutl.Extensibility",
                "Beutl.Engine",
                "Beutl.Language",
                "Beutl.Operators",
                "Beutl.ProjectSystem",
                "Beutl.Threading",
                "Beutl.Utilities",
            ];

            string tfm = GetTFM();
            foreach (string proj in projects)
            {
                DotNetBuild(s => s
                    .EnableNoRestore()
                    .SetFramework(tfm)
                    .SetConfiguration(Configuration)
                    .SetVersions(Version, AssemblyVersion, InformationalVersion)
                    .SetProjectFile(SourceDirectory / proj / $"{proj}.csproj"));

                DotNetPack(s => s
                    .EnableNoRestore()
                    .SetConfiguration(Configuration)
                    .SetVersions(Version, AssemblyVersion, InformationalVersion)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetProject(SourceDirectory / proj / $"{proj}.csproj"));
            }
        });
}
