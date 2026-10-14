# SolidWorks 转 URDF 导出插件

作者与维护：[Stephen Brawner](brawner@gmail.com)。曾获得 PickNik Consulting、Verb Surgical、Open Robotics、Willow Garage 等机构的支持。

[English](./README.md) | 简体中文

## 相对于上游版本的修改（1.6.1）

本分支修复了导出 URDF 与 SolidWorks 质量属性不一致的问题：

1. **惯量张量与 SolidWorks 质量属性一致**
   原实现 `ComputeInertialProperties` 通过 `AddBodies` 聚合实体，对装配体计算出的 link 惯量不正确。现改为直接从每个零件文档读取质量属性，再用**平行轴定理**合成 link 惯量，使导出的 `inertial` 块与 SolidWorks「质量属性」对话框显示的数值一致。

2. **惯性积符号修正**
   `Inertia.SetMomentMatrix` 对非对角元素（Ixy / Ixz / Iyz）存在双重取负，已去掉多余的负号。

3. **关节轴方向规整**
   `GetRefAxis` 和 `LocalizeJoint` 现在会把轴向量归一化，使第一个非零分量为正。原先轴可能出现 `[-1,0,0]`、`[0,-1,0]` 或 `[0,0,-1]`，会翻转关节正方向、导致关节限位错误。

## 最新版本

https://github.com/ros/solidworks_urdf_exporter/releases/tag/1.6.0

## SolidWorks 版本要求

1. 本插件最低要求 SolidWorks 2018 SP5，SolidWorks 2017 及更早版本可能可用，详见 [此 issue](https://github.com/ros/solidworks_urdf_exporter/issues/73)。

## 使用方法

参见 [ROS Wiki](http://wiki.ros.org/sw_urdf_exporter) 及相关[教程](http://wiki.ros.org/sw_urdf_exporter/Tutorials)。

## 安装

从 Releases 页面下载 `SW2URDF_Setup.exe`，**右键 → 以管理员身份运行**，安装完成后重启 SolidWorks，在「工具 → 插件」中勾选 SW2URDF。

## 开发

1. 安装 Visual Studio 2017 或更高版本
1. 安装 .NET 桌面开发工作负载
   1. 在 Visual Studio 中：`工具 → 获取工具和功能...`
   1. 勾选 `.NET 桌面开发`
   1. 点击「修改」
1. 以管理员身份启动 Visual Studio（右键 → 以管理员身份运行）
1. 打开 `sw2urdf/SW2URDF.sln`
1. 配置调试
   1. 在解决方案资源管理器中右键 `SW2URDF`
   1. 点击「调试」选项卡
   1. 确保 `Configuration:` 设为 `Debug`
   1. 确保 `Start external program:` 指向 SolidWorks 可执行文件，例如 `C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe`

### 常见问题

1. `AxImp.exe` 错误 — 检查 .NET 工具安装，如无错误请安装 Windows 10 SDK。
1. `Resource.resx` 错误 — 检查 `sw2urdf/SW2URDF/Resources.resx` 是否存在且为空。若为空，删除该文件，然后在解决方案资源管理器中右键 `SW2URDF` →「属性」→ 导航到「资源」选项卡并点击按钮重新创建。
