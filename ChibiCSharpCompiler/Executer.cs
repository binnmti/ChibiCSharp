using System.Diagnostics;

namespace ChibiCSharpCompiler;

public class Executer
{
    private const string TempFileName = "BlazorChibiCSharpAssembly";

    public static string Run(string code, string assembly)
    {
        File.WriteAllText($"{TempFileName}.cil", assembly);

        using var ilasm = GetCommandlineProcess("ilasm.exe", $"{TempFileName}.cil");
        ilasm.WaitForExit();
        File.Delete($"{TempFileName}.cil");

        using var exe = GetCommandlineProcess($"{TempFileName}.exe", $"");
        var result = exe.StandardOutput.ReadToEnd().Trim();
        exe.WaitForExit();
        File.Delete($"{TempFileName}.exe");

        return result;
    }

    private static Process GetCommandlineProcess(string exeName, string Arguments)
        => Process.Start(new ProcessStartInfo(exeName)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = Arguments,
            RedirectStandardOutput = true,
        })!;
}
