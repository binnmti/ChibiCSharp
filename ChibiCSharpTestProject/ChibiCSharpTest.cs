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

    private static int GetChibiCsharp(string str)
    {
        const string outputName = "test";
        using var chibiCSharp = GetCommandlineProcess("ChibiCsharp.exe", $"\"{str}\"");
        File.WriteAllText($"{outputName}.cil", chibiCSharp.StandardOutput.ReadToEnd());
        chibiCSharp.WaitForExit();

        using var ilasm = GetCommandlineProcess(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\ilasm.exe", $"{outputName}.cil");
        ilasm.WaitForExit();

        using var exe = GetCommandlineProcess($"{outputName}.exe", $"{str}");
        var result = exe.StandardOutput.ReadToEnd().Trim();
        return int.Parse(result);
    }

    [TestMethod]
    public void TestChibiCSharp()
    {
        Assert.AreEqual(0, GetChibiCsharp("0"));
        Assert.AreEqual(42, GetChibiCsharp("42"));
        Assert.AreEqual(21, GetChibiCsharp("5+20-4"));
        Assert.AreEqual(41, GetChibiCsharp(" 12 + 34 - 5 "));
        Assert.AreEqual(15, GetChibiCsharp(" 5*(9-6) "));
        Assert.AreEqual(4, GetChibiCsharp(" (3+5)/2 "));
    }
}
