using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Diagnostics;

namespace ChibiCSharpTestProject
{



    [TestClass]
    public sealed class Test1
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

        [TestMethod]
        public void TestMethod1()
        {
            int n = 100;
            string exeName = "test";

            using var chibiCSharp = GetCommandlineProcess("ChibiCsharp.exe", $"{n}");
            File.WriteAllText($"{exeName}.cil", chibiCSharp.StandardOutput.ReadToEnd());
            chibiCSharp.WaitForExit();

            using var ilasm = GetCommandlineProcess(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\ilasm.exe""", $"{exeName}.cil");
            ilasm.WaitForExit();

            using var exe = GetCommandlineProcess($"{exeName}.exe", $"{n}");
            var o = exe.StandardOutput.ReadToEnd().Trim();
            Assert.AreEqual(n.ToString(), o);


        }
    }
}
