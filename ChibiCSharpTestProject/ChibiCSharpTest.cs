using System.Diagnostics;

namespace ChibiCSharpTestProject;

[TestClass]
public sealed class ChibiCSharpTest
{
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

    private static int GetChibiCsharp(int n)
    {
        const string outputName = "test";
        using var chibiCSharp = GetCommandlineProcess("ChibiCsharp.exe", $"{n}");
        File.WriteAllText($"{outputName}.cil", chibiCSharp.StandardOutput.ReadToEnd());
        chibiCSharp.WaitForExit();

        using var ilasm = GetCommandlineProcess(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\ilasm.exe", $"{outputName}.cil");
        ilasm.WaitForExit();

        using var exe = GetCommandlineProcess($"{outputName}.exe", $"{n}");
        var result = exe.StandardOutput.ReadToEnd().Trim();
        return int.Parse(result);
    }

    [TestMethod]
    public void TestChibiCSharp()
    {
        Assert.AreEqual(0, GetChibiCsharp(0));
        Assert.AreEqual(42, GetChibiCsharp(42));
    }
}
