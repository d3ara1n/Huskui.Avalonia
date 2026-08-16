# Grammar Assets 维护指南

`Huskui.Avalonia.Code` 不再依赖 `TextMateSharp.Grammars`（6.75 MB 单体资源包），改为在本程序集内嵌入 gzip 压缩的 TextMate 语法快照，运行时经自定义 `IRegistryOptions` 解压喂给 `TextMateSharp` 引擎。语言覆盖与上游完全一致，代价是语法数据成为本仓库的自维护项。

## 当前快照

| 项 | 值 |
| --- | --- |
| 数据来源 | NuGet 包 `TextMateSharp.Grammars` 2.0.4（上游 [danipen/TextMateSharp]，commit `622d1b2`） |
| 提取日期 | 2026-08-16 |
| Grammar 数量 | 75（另 1 个 upstream 悬空引用，见下） |
| 语言数量 | 64 |
| 扩展名映射 | 297 条 |
| gzip 总体积 | ≈ 799 KB（原始 6.17 MB，压缩率 ≈ 7.7:1） |

内嵌位置：`src/Huskui.Avalonia.Code/Assets/Grammars/`（`manifest.json` + `<scope>.json.gz`），以 `EmbeddedResource` 打入程序集（`LogicalName` 前缀 `Huskui.Grammars.`）。`manifest.json` 里的所有映射都是**上游自身解析的结果**（含 first-grammar 化名等怪癖），不是重新推导，因此行为逐位对齐。

已知怪癖：`javascript` 扩展的 `package.json` 引用了 `Regular Expressions (JavaScript).tmLanguage`，但 upstream 的打包通配符只收 `*.json`，该文件从未被嵌入——这个 scope 在 `TextMateSharp.Grammars` 2.0.4 里本来就不可用，快照如实保持两侧一致（verify 会校验）。

## 更新流程

1. 在 `Directory.Packages.props` 中把 `TextMateSharp.Grammars`（工具输入）与 `TextMateSharp`（引擎依赖）bump 到同一版本。
2. 重新生成快照：

   ```bash
   cd tools/Huskui.GrammarExtractor
   dotnet run -- extract ../../src/Huskui.Avalonia.Code/Assets/Grammars
   ```

3. 构建并跑等价性验证（对每个语言/化名/扩展名比对 scope 解析结果，对每个 grammar 比对整段 token 流）：

   ```bash
   dotnet run -- verify ../../src/Huskui.Avalonia.Code/Assets/Grammars
   ```

   预期输出 `PASS`；grammar/语言/扩展映射计数与上表一致时更新上表（数量变化本身不是失败，如实记录即可）。
4. 更新本文件顶部的快照表（版本、日期、计数）。
5. Gallery 肉眼冒烟：`CodeViewer` / `DiffView` 在亮暗两套主题下的高亮。

## 周期性提醒

上游 TextMate 语法随 VS Code 演进（语法修正、新语言）。建议**每季度或上游发布 2.x 新版时**跑一次上面的更新流程；即使决定不更新，也值得跑一次 `verify` 确认无漂移。

> 提醒载体（issue / cron / 其他）尚未确定——确定后在此处补记具体机制。

[danipen/TextMateSharp]: https://github.com/danipen/TextMateSharp
