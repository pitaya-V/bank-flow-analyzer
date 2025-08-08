# 银行流水分析系统 — Clean MVP（LiveCharts2）

**这是一份“可直接编译运行”的最小可用骨架**：.NET 8 + WPF + MVVM + LiveCharts2。  
为避免之前的冲突，本版：**不包含 WinForms、不包含数据库/导入落库**，只演示 UI 结构与图表，并提供清晰的导入交互占位。

## 运行步骤
1. 使用 Visual Studio 打开 `BankFlowAnalyzer.sln`
2. 还原 NuGet 包（包含 LiveCharts2 / Ookii.Dialogs.Wpf / CommunityToolkit.Mvvm）
3. 设启动项目为 `App`，F5 运行

## 界面说明
- **数据导入页**
  - “选择文件夹”：使用 Ookii 的 VistaFolderBrowserDialog，仅选择路径（不落库）。
  - “导入示例数据”：把两行示例数据加载到下方预览表格（用于联调 UI）。
- **流水列表页**
  - 显示两行占位数据与“搜索”按钮（后续接数据库后替换为真实分页）。
- **数据分析页**
  - LiveCharts2 柱状图（示例月度净流入）。

## 下一步（建议按此顺序推进）
1. 接 SQLite + Dapper（落库 & 分页查询）
2. CSV/Excel 解析 + 去重指纹
3. 模板识别（按银行表头 JSON 配置）
4. 真实统计/图表输出 + 导出报告