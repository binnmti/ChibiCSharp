using System.Diagnostics;

namespace ChibiCSharpCompiler;

public class Executer
{
    public static string Run(string arg, string cil)
    {
        File.WriteAllText($"temp.cil", cil);
        using var ilasm = GetCommandlineProcess(@"ilasm.exe", $"temp.cil");
        ilasm.WaitForExit();

        using var exe = GetCommandlineProcess($"temp.exe", $"{arg}");
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
