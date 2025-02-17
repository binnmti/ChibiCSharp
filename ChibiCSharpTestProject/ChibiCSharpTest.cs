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
        Assert.AreEqual("0", Run("return 0;"));
        Assert.AreEqual("42", Run("return 42;"));
        Assert.AreEqual("21", Run("return 5+20-4;"));
        Assert.AreEqual("41", Run("return 12 + 34 - 5 ;"));
        Assert.AreEqual("15", Run("return 5 *(9-6) ;"));
        Assert.AreEqual("4", Run("return (3+5)/2 ;"));

        Assert.AreEqual("10", Run("return - 10+20;"));
        Assert.AreEqual("10", Run("return - -10;"));
        Assert.AreEqual("10", Run("return - -  +10;"));

        Assert.AreEqual("0", Run("return 0==1;"));
        Assert.AreEqual("1", Run("return 1==1;"));
        Assert.AreEqual("1", Run("return 42==42;"));
        Assert.AreEqual("1", Run("return 0!=1;"));
        Assert.AreEqual("0", Run("return 42!=42;"));

        Assert.AreEqual("1", Run("return 0<1;"));
        Assert.AreEqual("0", Run("return 1<1;"));
        Assert.AreEqual("0", Run("return 2<1;"));
        Assert.AreEqual("1", Run("return 0<=1;"));
        Assert.AreEqual("1", Run("return 1<=1;"));
        Assert.AreEqual("0", Run("return 2<=1;"));

        Assert.AreEqual("1", Run("return 1>0;"));
        Assert.AreEqual("0", Run("return 1>1;"));
        Assert.AreEqual("0", Run("return 1>2;"));
        Assert.AreEqual("1", Run("return 1>=0;"));
        Assert.AreEqual("1", Run("return 1>=1;"));
        Assert.AreEqual("0", Run("return 1>=2;"));
        Assert.AreEqual("3", Run("a = 3; return a;"));
        Assert.AreEqual("8", Run("a=3; z=5; return a+z;"));
        Assert.AreEqual("3", Run("foo=3; return foo;"));
        Assert.AreEqual("8", Run("foo123=3; bar=5; return foo123+bar;"));

        //Assert.AreEqual("1", Run("return 1; 2; 3;"));
    }
}
