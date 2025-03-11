using ChibiCSharp;

namespace ChibiCSharpTestProject;

[TestClass]
public sealed class ChibiCSharpTest
{
    private static string Run(string code)
        => Executer.Run(Compiler.Compile(code));

    [TestMethod]
    public void TestChibiCSharp()
    {
        Assert.AreEqual("0", Run("int main(){ return 0; }"));
        Assert.AreEqual("42", Run("int main(){ return 42; }"));
        Assert.AreEqual("21", Run("int main(){ return 5+20-4; }"));
        Assert.AreEqual("41", Run("int main(){ return 12 + 34 - 5 ; }"));
        Assert.AreEqual("15", Run("int main(){ return 5 *(9-6) ; }"));
        Assert.AreEqual("4", Run("int main(){ return (3+5)/2 ; }"));

        Assert.AreEqual("10", Run("int main(){ return - 10+20; }"));
        Assert.AreEqual("10", Run("int main(){ return - -10; }"));
        Assert.AreEqual("10", Run("int main(){ return - -  +10; }"));

        Assert.AreEqual("0", Run("int main(){ return 0==1; }"));
        Assert.AreEqual("1", Run("int main(){ return 1==1; }"));
        Assert.AreEqual("1", Run("int main(){ return 42==42; }"));
        Assert.AreEqual("1", Run("int main(){ return 0!=1; }"));
        Assert.AreEqual("0", Run("int main(){ return 42!=42; }"));

        Assert.AreEqual("1", Run("int main(){ return 0<1; }"));
        Assert.AreEqual("0", Run("int main(){ return 1<1; }"));
        Assert.AreEqual("0", Run("int main(){ return 2<1; }"));
        Assert.AreEqual("1", Run("int main(){ return 0<=1; }"));
        Assert.AreEqual("1", Run("int main(){ return 1<=1; }"));
        Assert.AreEqual("0", Run("int main(){ return 2<=1; }"));

        Assert.AreEqual("1", Run("int main(){ return 1>0; }"));
        Assert.AreEqual("0", Run("int main(){ return 1>1; }"));
        Assert.AreEqual("0", Run("int main(){ return 1>2; }"));
        Assert.AreEqual("1", Run("int main(){ return 1>=0; }"));
        Assert.AreEqual("1", Run("int main(){ return 1>=1; }"));
        Assert.AreEqual("0", Run("int main(){ return 1>=2; }"));
        Assert.AreEqual("3", Run("int main(){ int a = 3; return a; }"));
        Assert.AreEqual("8", Run("int main(){ int a; int z;a=3; z=5; return a+z; }"));
        Assert.AreEqual("3", Run("int main(){ int foo=3; return foo; }"));
        Assert.AreEqual("8", Run("int main(){ int foo123=3; int bar=5; return foo123+bar; }"));

        Assert.AreEqual("1", Run("int main(){ return 1; 2; 3; }"));
        Assert.AreEqual("2", Run("int main(){ 1; return 2; 3; }"));
        Assert.AreEqual("3", Run("int main(){ 1; 2; return 3; }"));

        Assert.AreEqual("3", Run("int main(){ if (0) return 2; return 3; }"));
        Assert.AreEqual("3", Run("int main(){ if (1-1) return 2; return 3; }"));
        Assert.AreEqual("2", Run("int main(){ if (1) return 2; return 3; }"));
        Assert.AreEqual("2", Run("int main(){ if (2 - 1) return 2; return 3; }"));

        Assert.AreEqual("10", Run("int main(){ int i=0; while(i<10) i=i+1; return i; }"));

        Assert.AreEqual("55", Run("int main(){ int i=0; int j=0; for (i=0; i<=10; i=i+1) j=i+j; return j; }"));
        Assert.AreEqual("3", Run("int main(){ for (;;) return 3; return 5; }"));

        Assert.AreEqual("55", Run("int main(){ int i=0; int j=0; while(i<=10) {j=i+j; i=i+1;} return j; }"));
        Assert.AreEqual("3", Run("int main(){ {1; {2;} return 3;} }"));

        Assert.AreEqual("32", Run("int main() { return ret32(); } int ret32() { return 32; }"));
        Assert.AreEqual("8", Run("int main() { return Add(3, 5); } int Add(int x, int y) { return x + y; }"));
        Assert.AreEqual("1", Run("int main() { return Sub(4, 3); } int Sub(int x, int y) { return x - y; }"));
        Assert.AreEqual("55", Run("int main() { return Fibonacci(9); } int Fibonacci(int x) { if (x<=1) return 1; return Fibonacci(x-1) + Fibonacci(x-2); }"));
    }
}
