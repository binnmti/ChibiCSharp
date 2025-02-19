using System.Diagnostics;

namespace ChibiCSharpCompiler;

public class Executer
{
    private const string TempFileName = "BlazorChibiCSharpAssembly";

    // TODO:実行ファイルのパスを指定した方がよさげ。フルパスがおかしい。。
    public static string Run(string assembly)
    {
        File.WriteAllText($"{TempFileName}.cil", assembly);

        using var ilasm = GetCommandlineProcess("ilasm.exe", $"{TempFileName}.cil");
        ilasm.WaitForExit();
        File.Delete($"{TempFileName}.cil");

        using var exe = GetCommandlineProcess($"{TempFileName}.exe", "");
        exe.WaitForExit();
        File.Delete($"{TempFileName}.exe");
        return exe.ExitCode.ToString();
    }

    private static Process GetCommandlineProcess(string exeName, string Arguments)
        => Process.Start(new ProcessStartInfo(exeName)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = Arguments,
        })!;
}
