use zed_extension_api as zed;

struct VbLsExtension;

impl zed::Extension for VbLsExtension {
    fn new() -> Self {
        Self
    }

    fn language_server_command(
        &mut self,
        language_server_id: &zed::LanguageServerId,
        worktree: &zed::Worktree,
    ) -> zed::Result<zed::Command> {
        let Some(command) = worktree.which("vb-ls") else {
            return Err(format!(
                "Language server '{language_server_id}' requires the vb-ls .NET tool. Install it with: dotnet tool install -g vb-ls"
            ));
        };

        Ok(zed::Command {
            command,
            args: vec![
                "--stdio".into(),
                "--autoLoadProjects".into(),
                "--logLevel".into(),
                "Information".into(),
            ],
            env: worktree.shell_env(),
        })
    }

    fn language_server_workspace_configuration(
        &mut self,
        _language_server_id: &zed::LanguageServerId,
        _worktree: &zed::Worktree,
    ) -> zed::Result<Option<zed::serde_json::Value>> {
        Ok(Some(zed::serde_json::json!({
            "csharp|background_analysis": {
                "dotnet_analyzer_diagnostics_scope": "fullSolution",
                "dotnet_compiler_diagnostics_scope": "fullSolution"
            },
            "csharp|completion": {
                "dotnet_show_name_completion_suggestions": true,
                "dotnet_show_completion_items_from_unimported_namespaces": true,
                "dotnet_provide_regex_completions": true
            },
            "csharp|inlay_hints": {
                "csharp_enable_inlay_hints_for_implicit_object_creation": true,
                "csharp_enable_inlay_hints_for_implicit_variable_types": true,
                "csharp_enable_inlay_hints_for_lambda_parameter_types": true,
                "csharp_enable_inlay_hints_for_types": true,
                "dotnet_enable_inlay_hints_for_indexer_parameters": true,
                "dotnet_enable_inlay_hints_for_literal_parameters": true,
                "dotnet_enable_inlay_hints_for_object_creation_parameters": true,
                "dotnet_enable_inlay_hints_for_other_parameters": true,
                "dotnet_enable_inlay_hints_for_parameters": true,
                "dotnet_suppress_inlay_hints_for_parameters_that_differ_only_by_suffix": true,
                "dotnet_suppress_inlay_hints_for_parameters_that_match_argument_name": true,
                "dotnet_suppress_inlay_hints_for_parameters_that_match_method_intent": true
            },
            "csharp|symbol_search": {
                "dotnet_search_reference_assemblies": true
            },
            "csharp|code_lens": {
                "dotnet_enable_references_code_lens": true
            }
        })))
    }
}

zed::register_extension!(VbLsExtension);
