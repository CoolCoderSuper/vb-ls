// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.LanguageServer.HostWorkspace;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.Extensions.Logging.Abstractions;
using Roslyn.LanguageServer.Protocol;
using Xunit.Abstractions;

namespace Microsoft.CodeAnalysis.LanguageServer.UnitTests;

public sealed class LanguageServerCompositionTests(ITestOutputHelper testOutputHelper)
    : AbstractLanguageServerHostTests(testOutputHelper)
{
    [Fact]
    public async Task LanguageServerHostServicesIncludeCSharpAndVisualBasic()
    {
        await using var testLspServer = await CreateLanguageServerWithoutTestOutputAsync();

        var workspaceFactory = testLspServer.ExportProvider.GetExportedValue<LanguageServerWorkspaceFactory>();
        var solutionServices = workspaceFactory.HostWorkspace.Services.SolutionServices;

        Assert.NotNull(solutionServices.GetLanguageServices(LanguageNames.CSharp).GetService<ICommandLineParserService>());
        Assert.NotNull(solutionServices.GetLanguageServices(LanguageNames.VisualBasic).GetService<ICommandLineParserService>());
    }

    [Theory]
    [InlineData("Program.cs", "csharp", LanguageNames.CSharp, "class Program { }")]
    [InlineData("Program.vb", "vb", LanguageNames.VisualBasic, "Class Program" + "\r\n" + "End Class")]
    public async Task LooseSourceFilesAreTrackedWithExpectedLanguage(string fileName, string languageId, string expectedLanguage, string sourceText)
    {
        await using var testLspServer = await CreateLanguageServerWithoutTestOutputAsync();

        var sourceFile = TempRoot.CreateDirectory().CreateFile(fileName).WriteAllText(sourceText);
        var documentUri = ProtocolConversions.CreateAbsoluteDocumentUri(sourceFile.Path);

        await OpenDocumentAsync(testLspServer, documentUri, languageId, sourceText);

        var (_, _, textDocument) = await testLspServer.LanguageServerHost.GetRequiredLspService<LspWorkspaceManager>()
            .GetLspDocumentInfoAsync(new TextDocumentIdentifier { DocumentUri = documentUri }, CancellationToken.None);
        var document = Assert.IsType<Document>(textDocument);

        Assert.Equal(expectedLanguage, document.Project.Language);
    }

    [Fact]
    public async Task VisualBasicProjectLoadsThroughProjectOpen()
    {
        await using var testLspServer = await CreateLanguageServerWithoutTestOutputAsync();

        var projectDirectory = TempRoot.CreateDirectory();
        var projectFile = projectDirectory.CreateFile("Test.vbproj").WriteAllText("""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Library</OutputType>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);
        var sourceFile = projectDirectory.CreateFile("Program.vb").WriteAllText("""
            Public Class Program
            End Class
            """);

        var projectInitialized = new TaskCompletionSource();
        testLspServer.AddClientLocalRpcTarget(ProjectInitializationHandler.ProjectInitializationCompleteName, () => projectInitialized.SetResult());

#pragma warning disable RS0030 // Do not use banned APIs
        await testLspServer.OpenProjectsAsync([new Uri(projectFile.Path)]);
#pragma warning restore RS0030 // Do not use banned APIs
        await projectInitialized.Task.WaitAsync(TimeSpan.FromMinutes(2));

        var workspaceFactory = testLspServer.ExportProvider.GetExportedValue<LanguageServerWorkspaceFactory>();
        var project = Assert.Single(workspaceFactory.HostWorkspace.CurrentSolution.Projects);

        Assert.Equal(LanguageNames.VisualBasic, project.Language);
        Assert.Contains(project.Documents, document => document.FilePath == sourceFile.Path);
        Assert.NotNull(project.ParseOptions);
        Assert.NotNull(project.CompilationOptions);
    }

    [Theory]
    [InlineData("Microsoft.CodeAnalysis.VisualBasic.dll")]
    [InlineData("Microsoft.CodeAnalysis.VisualBasic.Workspaces.dll")]
    [InlineData("Microsoft.CodeAnalysis.VisualBasic.Features.dll")]
    public void LanguageServerOutputIncludesVisualBasicAssemblies(string assemblyName)
    {
        var assemblyPath = Path.Combine(TestPaths.GetLanguageServerDirectory(), assemblyName);
        Assert.True(File.Exists(assemblyPath), $"Expected '{assemblyPath}' to exist.");
    }

    private static Task OpenDocumentAsync(TestLspServer testLspServer, DocumentUri documentUri, string languageId, string sourceText)
        => testLspServer.ExecuteRequestAsync<DidOpenTextDocumentParams, object>(Methods.TextDocumentDidOpenName, new DidOpenTextDocumentParams
        {
            TextDocument = new TextDocumentItem
            {
                DocumentUri = documentUri,
                LanguageId = languageId,
                Text = sourceText,
                Version = 0
            }
        }, CancellationToken.None);

    private Task<TestLspServer> CreateLanguageServerWithoutTestOutputAsync()
        => TestLspServer.CreateAsync(new ClientCapabilities(), NullLoggerFactory.Instance, MefCacheDirectory.Path, includeDevKitComponents: false);
}
