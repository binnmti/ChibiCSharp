using ChibiCSharpCompiler;

namespace ChibiCSharpTestProject;

[TestClass]
public sealed class ChibiCSharpTest
{
    private static string Run(string code)
        => Executer.Run(code, Compiler.Compile(code));

    [TestMethod]
    public void TestChibiCSharp()
    {
        Assert.AreEqual("0", Run("0;"));
        Assert.AreEqual("42", Run("42;"));
        Assert.AreEqual("21", Run("5+20-4;"));
        Assert.AreEqual("41", Run(" 12 + 34 - 5 ;"));
        Assert.AreEqual("15", Run(" 5 *(9-6) ;"));
        Assert.AreEqual("4", Run("(3+5)/2 ;"));

        Assert.AreEqual("10", Run(" - 10+20;"));
        Assert.AreEqual("10", Run(" - -10;"));
        Assert.AreEqual("10", Run(" - -  +10;"));

        Assert.AreEqual("0", Run("0==1;"));
        Assert.AreEqual("1", Run("1==1;"));
        Assert.AreEqual("1", Run("42==42;"));
        Assert.AreEqual("1", Run("0!=1;"));
        Assert.AreEqual("0", Run("42!=42;"));

        Assert.AreEqual("1", Run("0<1;"));
        Assert.AreEqual("0", Run("1<1;"));
        Assert.AreEqual("0", Run("2<1;"));
        Assert.AreEqual("1", Run("0<=1;"));
        Assert.AreEqual("1", Run("1<=1;"));
        Assert.AreEqual("0", Run("2<=1;"));

        Assert.AreEqual("1", Run("1>0;"));
        Assert.AreEqual("0", Run("1>1;"));
        Assert.AreEqual("0", Run("1>2;"));
        Assert.AreEqual("1", Run("1>=0;"));
        Assert.AreEqual("1", Run("1>=1;"));
        Assert.AreEqual("0", Run("1>=2;"));
        //Assert.AreEqual("3", Run("a = 3; return a;"));
    }
}
