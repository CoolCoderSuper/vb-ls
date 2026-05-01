# vb-ls for Zed

This is a local Zed extension for Visual Basic .NET support using the `vb-ls`
language server.

## Requirements

- Zed
- Rust installed via rustup
- .NET 10 SDK/runtime
- `vb-ls` on your PATH

Install the server with:

```sh
dotnet tool install -g vb-ls
```

## Local install

In Zed, run `zed: install dev extension` from the command palette and select
this `zed` directory.

The extension starts:

```sh
vb-ls --stdio --autoLoadProjects --logLevel Information
```

`--autoLoadProjects` lets Roslyn discover a workspace solution or project files
without editor-specific `solution/open` or `project/open` notifications.
