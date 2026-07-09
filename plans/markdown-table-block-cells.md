# 支持表格单元格的块级内容

> 状态：待办 · 关联：`Huskui.Avalonia.Markdown`

## 背景

Avalonia 12.1 的 `TableView` 已取代自制的 `MarkdownTable`（见 `refactor(Markdown): render tables via TableView`）。表格渲染现在通过一个薄行模型 `MarkdownTableRow` 桥接：每列 `Binding="[i]"` 反射到行模型的索引器，取回预渲染的单元格控件。

## 问题

Markdig 同时开了 `UsePipeTables()` 和 `UseGridTables()`。两者对**单元格内容能力**不同：

| | 单元格内容 |
|---|---|
| Pipe table | 只能单行内联（文本/链接/强调） |
| Grid table | 可含**块级内容**：多段落、列表、代码块，甚至嵌套表格 |

但 `MarkdownViewer.SpawnCell` 只处理 `ParagraphBlock`：

```csharp
foreach (var subBlock in cell)
{
    if (subBlock is ParagraphBlock para && para.Inline is not null)
        foreach (var inline in RenderInlines(para.Inline))
            inlines.Add(inline);
}
text.Inlines = inlines;
```

后果：grid table 单元格里的**列表、代码块、多段落**被丢弃或压扁进一个 `TextBlock`。grid table 理论上比 pipe table 强，但目前没体现出来。

## 目标

让 grid table 的富单元格正确渲染，pipe table 行为不变。

## 思路

`SpawnCell` 当前硬编码返回单个 `TextBlock`。改为：单元格内有多个块时，用一个纵向容器（`StackPanel`）逐块渲染，复用现有的块渲染逻辑（段落→`SpawnText` + inlines、列表、代码块→`CodeViewer` 等）。

要点：
- 单元格只有单段落时，保持现状（直接返回 `TextBlock`，避免无谓嵌套）。
- 多块时返回 `StackPanel`，每块复用文档主体的 `SpawnXxx` 分发逻辑。
- 评估是否抽取一个公共的「渲染单个 Block → Control」方法，供文档主体和单元格共用（当前块分发逻辑内联在 `RenderDocument` 的大 switch 里）。

## 不在范围内

- 不改 pipe table 行为（其单元格天然只有单段落）。
- 不引入嵌套表格（grid table 嵌套表格极少见，成本高，暂不做）。

## 验证

在 Gallery 的 Grid Table 示例里加一个含列表/代码块的单元格，确认各块正确分行渲染、列宽与对齐仍正确。
