using System.Diagnostics;

namespace ChibiCSharpCompiler;

public class Executer
{
    public static string Run(string arg, string cil)
    {
        string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        var cilFile = Path.Combine(exePath, "BlazorChibiCSharpAssembly.cil");

        File.WriteAllText(cilFile, cil);
        using var ilasm = GetCommandlineProcess(Path.Combine(exePath, "ilasm.exe"), cilFile);
        ilasm.WaitForExit();

        using var exe = GetCommandlineProcess($"BlazorChibiCSharpAssembly.exe", $"{arg}");
        return exe.StandardOutput.ReadToEnd().Trim();
    }

    private static Process GetCommandlineProcess(string exeName, string Arguments)
    {
        var p = Process.Start(new ProcessStartInfo(exeName)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = Arguments,
            RedirectStandardOutput = true,
        });
        Debug.Assert(p != null);
        return p;
    }
}
