Imports System.Diagnostics
Imports System.IO

Module Program
    Function Main(args As String()) As Integer
        Dim baseDirectory = AppContext.BaseDirectory
        Dim languageServerAssembly = Path.Combine(baseDirectory, "roslyn-lsp", "Microsoft.CodeAnalysis.LanguageServer.dll")

        If Not File.Exists(languageServerAssembly) Then
            Console.Error.WriteLine($"Roslyn language server was not found at '{languageServerAssembly}'.")
            Return 1
        End If

        Dim startInfo = New ProcessStartInfo With {
            .FileName = "dotnet",
            .UseShellExecute = False
        }

        startInfo.ArgumentList.Add(languageServerAssembly)

        For Each arg In args
            startInfo.ArgumentList.Add(arg)
        Next

        Using serverProcess = Process.Start(startInfo)
            If serverProcess Is Nothing Then
                Console.Error.WriteLine("Failed to start the Roslyn language server.")
                Return 1
            End If

            serverProcess.WaitForExit()
            Return serverProcess.ExitCode
        End Using
    End Function
End Module
