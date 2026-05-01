import { ExtensionContext, Uri, workspace } from "vscode";
import {
  LanguageClient,
  LanguageClientOptions,
  ServerOptions
} from "vscode-languageclient/node";

let client: LanguageClient | undefined;

export async function activate(context: ExtensionContext) {
  const config = workspace.getConfiguration("vb-ls");
  const configuredServerPath = config.get<string>("server.path", "vb-ls").trim();
  const serverPath = configuredServerPath.length > 0 ? configuredServerPath : "vb-ls";
  const solutionPath = config.get<string>("solution.path");
  const autoOpenProjects = config.get<boolean>("autoOpenProjects", true);
  const extensionLogDirectory = config.get<string>("extensionLogDirectory");

  const args = ["--stdio"];
  if (extensionLogDirectory) {
    args.push("--extensionLogDirectory", extensionLogDirectory);
  }

  const serverOptions: ServerOptions = {
    command: serverPath,
    args
  };

  const clientOptions: LanguageClientOptions = {
    documentSelector: [
      { scheme: "file", language: "vbnet" },
      { scheme: "file", language: "csharp" }
    ],
    uriConverters: {
      code2Protocol: uri => uri.toString(true),
      protocol2Code: value => Uri.parse(value)
    }
  };

  client = new LanguageClient(
    "vb-ls",
    "Visual Basic Language Server",
    serverOptions,
    clientOptions
  );

  context.subscriptions.push(client);
  await client.start();

  if (solutionPath) {
    await client.sendNotification("solution/open", { solution: Uri.file(solutionPath).toString(true) });
  } else if (autoOpenProjects) {
    await openWorkspaceProjects();
  }
}

export function deactivate(): Thenable<void> | undefined {
  return client?.stop();
}

async function openWorkspaceProjects() {
  if (!client) {
    return;
  }

  const solutionFiles = await workspace.findFiles("{*.sln,*.slnx}", "**/{bin,obj,node_modules,roslyn}/**", 2);
  if (solutionFiles.length === 1) {
    await client.sendNotification("solution/open", { solution: solutionFiles[0].toString(true) });
    return;
  }

  const projectFiles = await workspace.findFiles("**/*.{csproj,vbproj}", "**/{bin,obj,node_modules,roslyn}/**");
  if (projectFiles.length > 0) {
    await client.sendNotification("project/open", { projects: projectFiles.map(project => project.toString(true)) });
  }
}
