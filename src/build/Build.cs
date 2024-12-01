using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.GitVersion.GitVersionTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    AbsolutePath ArtifactsDirectory => RootDirectory / ".." / "artifacts";

    AbsolutePath CoreLibProject => RootDirectory / "EasyParsing" / "EasyParsing.csproj";
    AbsolutePath JsonParserProject => RootDirectory / "EasyParsing.Samples.Json" / "EasyParsing.Samples.Json.csproj";
    AbsolutePath MarkdownParserProject => RootDirectory / "EasyParsing.Samples.Markdown" / "EasyParsing.Samples.Markdown.csproj";
    
    AbsolutePath[] PublishedProjects =>
    [
        CoreLibProject,
        JsonParserProject,
        MarkdownParserProject
    ];
    
    [Solution] readonly Solution Solution;

    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetClean(options => options.SetProject(Solution));
            
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var (gitVersion, _) = GitVersion(s => s.SetProcessWorkingDirectory(RootDirectory));

            foreach (var project in PublishedProjects)
            {
                DotNetPack(options => options
                    .SetProject(project)
                    .SetConfiguration(Configuration)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .EnableNoBuild()
                    .EnableIncludeSymbols()
                    .SetRepositoryUrl("https://github.com/rflechner/EasyParsing")
                    // .SetPackageReleaseNotes("")
                    .SetVersionSuffix(gitVersion.PreReleaseTag));   
            }
        });

}
