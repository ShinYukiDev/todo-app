# Todo App

## 概要

C# / .NET 10 のコンソールで動作するシンプルなTodoアプリです。  
タスクの追加・変更・削除・一覧表示・完了を行えます。データは Entity Framework Core + SQLite で永続化されます。

## 技術スタック

- .NET 10
- C#
- Entity Framework Core 10（SQLite Provider）
- xUnit

## セットアップ

```bash
dotnet restore
```

## 実行方法

```bash
dotnet run --project src/TodoApp/TodoApp.csproj
```

起動後、メニュー番号を入力して操作します。

- 1: タスク一覧を表示
- 2: タスクを追加
- 3: タスクを変更
- 4: タスクを削除
- 5: タスクを完了
- 0: 終了

## テスト実行

```bash
dotnet test tests/TodoApp.Tests/TodoApp.Tests.csproj
```

## TDD方針

t_wada式の Red → Green → Refactor を前提に、失敗するテストを先に書いてから最小実装を追加する方針です。
