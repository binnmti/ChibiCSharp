# ChibiCSharp(ChibiC#)
ChibiCSharpは、[chibicc](https://github.com/rui314/chibicc)をベースにしたC#コンパイラのリポジトリです。

# csproj
| プロジェクト名|内容|
|:--|:--|
|ChibiCSharp|C#コンパイラのコア部分|
|ChibiCSharpTestProject|C#コンパイラのコア部分のテストプロジェクト|
|CommandLineChibiCSharp|コマンドライン版C#コンパイラ|
|BlazorChibiCSharp|[Blazor版C#コンパイラ](blazorchibicsharp.azurewebsites.net/)|

# きっかけ
- [低レイヤを知りたい人のためのCコンパイラ作成入門](https://www.sigbus.info/compilerbook)を読んで勉強していた
- 机上の空論では限界が来たので、より深く把握するために自分でもプログラムを書き始めた
- CでCコンパイラを作る話だったが、いかんせんLinuxでの開発に全く魅力を感じなかった
- C#でC#コンパイラを作るように方向転換したら自分のモチベーションは爆上がりした
- [オレオレ関数型言語を実行するオレオレ抽象機械](https://katsujukou.github.io/dam4g/)を見て自分もこういうのが作りたくなってBlazor版を作った

