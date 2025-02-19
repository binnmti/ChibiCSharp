# ChibiCSharp(ChibiC#)
ChibiCSharpは、Rui Ueyama氏による[chibicc](https://github.com/rui314/chibicc)をベースにしたC#コンパイラのリポジトリです。

# きっかけ
- 元々[低レイヤを知りたい人のためのCコンパイラ作成入門](https://www.sigbus.info/compilerbook)を読んで勉強していた
- 机上の空論では限界が来たので、より深く把握するために自分でもプログラムを書き始めた
- 文章の内容はCでCコンパイラを作る話だったが、いかんせんLinuxでの開発に全く魅力を感じなかった
- C#でC#コンパイラを作るように方向転換したら自分のモチベーションは爆上がりした

# CSプロジェクト内容
- ChibiCSharp
  - C#コンパイラのコア部分
- ChibiCSharpTestProject
  - C#コンパイラのコア部分のテストプロジェクト
- CommandLineChibiCSharp
  - コマンドラインのC#コンパイラ
- BlazorChibiCSharp
  - BlazorのC#コンパイラ
