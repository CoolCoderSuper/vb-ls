# Description
This is a fork of the Roslyn LSP with support for Visual Basic.

`vb-ls` requires .NET 10 SDK to be installed.

# How it works
We vendor  in the Roslyn LSP and apply a set of patches to it to add support for Visual Basic.

# Installation
`dotnet tool install -g vb-ls`

See [vb-ls nuget page](https://www.nuget.org/packages/vb-ls/)

# Clients

`vb-ls` implements the standard LSP protocol to interact with your editor.
However there are some features that need a non-standard implementation and this
is where editor-specific plugins can be helpful.

## NeoVim

NeoVim 0.11+ can easily be configured using the `vim.lsp.config` api.
```lua
vim.lsp.config['vb_ls'] = {
    cmd = {
        'vb-ls',
        '--logLevel',
        'Information',
        '--extensionLogDirectory',
        vim.fs.joinpath(vim.uv.os_tmpdir(), 'roslyn_ls', 'logs'),
        '--stdio',
    },
    filetypes = { 'cs', 'vbnet' },
    root_dir = function(bufnr, cb)
        local root_dir = vim.fs.root(bufnr, function(fname, _)
            return fname:match('%.sln[x]?$') ~= nil
        end)

        if not root_dir then
            root_dir = vim.fs.root(bufnr, function(fname, _)
                return fname:match('%.csproj$') ~= nil or fname:match('%.vbproj$') ~= nil
            end)
        end

        if root_dir then
            cb(root_dir)
        end
    end,
    on_init = {
        function(client)
            local root_dir = client.config.root_dir

            for entry, type in vim.fs.dir(root_dir) do
                if type == 'file' and (vim.endswith(entry, '.sln') or vim.endswith(entry, '.slnx')) then
                    roslyn_open_solution(client, vim.fs.joinpath(root_dir, entry))
                    return
                end
            end

            local project_files = {}
            for entry, type in vim.fs.dir(root_dir) do
                if type == 'file' and (vim.endswith(entry, '.csproj') or vim.endswith(entry, '.vbproj')) then
                    table.insert(project_files, vim.fs.joinpath(root_dir, entry))
                end
            end

            if not vim.tbl_isempty(project_files) then
                roslyn_open_projects(client, project_files)
            end
        end,
    },
    settings = {
        ['csharp|background_analysis'] = {
            dotnet_analyzer_diagnostics_scope = 'fullSolution',
            dotnet_compiler_diagnostics_scope = 'fullSolution',
        },
        ['csharp|inlay_hints'] = {
            csharp_enable_inlay_hints_for_implicit_object_creation = true,
            csharp_enable_inlay_hints_for_implicit_variable_types = true,
            csharp_enable_inlay_hints_for_lambda_parameter_types = true,
            csharp_enable_inlay_hints_for_types = true,
            dotnet_enable_inlay_hints_for_indexer_parameters = true,
            dotnet_enable_inlay_hints_for_literal_parameters = true,
            dotnet_enable_inlay_hints_for_object_creation_parameters = true,
            dotnet_enable_inlay_hints_for_other_parameters = true,
            dotnet_enable_inlay_hints_for_parameters = true,
            dotnet_suppress_inlay_hints_for_parameters_that_differ_only_by_suffix = true,
            dotnet_suppress_inlay_hints_for_parameters_that_match_argument_name = true,
            dotnet_suppress_inlay_hints_for_parameters_that_match_method_intent = true,
        },
        ['csharp|symbol_search'] = {
            dotnet_search_reference_assemblies = true,
        },
        ['csharp|completion'] = {
            dotnet_show_name_completion_suggestions = true,
            dotnet_show_completion_items_from_unimported_namespaces = true,
            dotnet_provide_regex_completions = true,
        },
        ['csharp|code_lens'] = {
            dotnet_enable_references_code_lens = true,
        },
    },
}

vim.lsp.enable('vb_ls')
```
You will also have to install the [vbnet.nvim](https://github.com/CoolCoderSuper/vbnet.nvim) plugin to register the
vbnet filetype and improved syntax highlighting.

## Visual Studio Code
See [vscode-vb-ls](https://marketplace.visualstudio.com/items?itemName=CoolCoderSuper.vscode-vb-ls).
