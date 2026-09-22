# WinSystem · 企业级一体化管理系统

WinSystem 是一款基于 **WPF (MVVM) + ASP.NET Core WebAPI + MySQL** 的企业级一体化管理系统。
它覆盖组织架构、用户角色、权限菜单、消息通知、日程管理与操作审计等核心管理场景，
以现代化界面与纵深防御设计，为企业提供统一、可信的管理中枢。

## ✨ 功能亮点

<div align="center">

| 模块 | 说明 |
| --- | --- |
| 📊 数据看板 | 全局核心指标总览 |
| 👥 用户管理 | 账号 CRUD、密码重置、启停、角色分配、分页检索 |
| 🏢 组织架构 | 树形组织维护、上下级层级管理 |
| 🔐 角色与权限 | 基于角色的授权（RBAC）、菜单权限标识过滤 |
| 🧭 菜单管理 | 二级侧边栏目录 / 菜单动态配置 |
| 📖 数据字典 | 字典类型与条目维护 |
| ⚙️ 系统参数 | 全局配置项维护 |
| 💬 消息通知 | 站内消息发布与查看（按角色可见性控制） |
| 📅 日程管理 | 日程新增 / 编辑 / 删除，含归属校验 |
| 🗂️ 操作日志 | 全写操作自动审计，含修改前后快照，支持查询 / 删除 / 清空 |

</div>

## 🛡️ 安全体系

WinSystem 已完成四轮安全审计与加固，覆盖如下能力：

- **服务端 RBAC 鉴权**：`AdminOnly` 策略限制 `super_admin` / `admin` 角色访问 24 个管理接口
- **JWT 认证 + Token 吊销**：令牌携带 `tv` (token_version) Claim，与数据库比对；
  密码变更、重置、账号禁用 / 锁定后旧令牌立即失效
- **登录防爆破**：账号级 5 次失败锁定 15 分钟 + IP 级限流（每分钟 10 次）
- **密码策略**：8-64 位，须同时含字母与数字；密码使用 BCrypt 加盐哈希存储
- **密钥外部化**：JWT 签名密钥从环境变量 `JWT_KEY` 读取（缺失则拒绝启动）
- **受保护账号**：`IsProtected` 账号禁止删除 / 重置密码 / 禁用 / 改登录名 / 改角色，防止系统失管
- **越权防护**：改密身份取自 JWT（忽略客户端传入 ID）；非管理员仅可操作本人日程与查看本人资料
- **数据过度暴露控制**：用户列表 / 搜索 / 消息列表仅管理员可见；消息详情按角色可见性过滤
- **操作审计**：创建 / 修改 / 删除及登录动作自动写入 `sys_operation_log`，保存修改前后快照（中文可读）

> 详细审计记录见 [SECURITY_AUDIT_REPORT.md](SECURITY_AUDIT_REPORT.md)。

## 🧱 技术架构

```
┌─────────────────────────────────────────────────────────┐
│                      WinSystem (WPF 客户端)              │
│        MVVM · 无边框窗体 · 多标签页 · 卡片式 UI           │
└──────────────────────────┬──────────────────────────────┘
                           │  HTTPS/HTTP · JSON · JWT
┌──────────────────────────▼──────────────────────────────┐
│                   WinSystem.Api (WebAPI)                 │
│    ASP.NET Core · EF Core · JWT 认证 · RBAC · 审计中间件   │
└──────────────────────────┬──────────────────────────────┘
                           │  EF Core
┌──────────────────────────▼──────────────────────────────┐
│                        MySQL 8.4                          │
└─────────────────────────────────────────────────────────┘
```

| 层 | 技术 |
| --- | --- |
| 客户端 | WPF · MVVM（View/ViewModel/Model）· RelayCommand · 自定义样式控件 |
| 服务端 | ASP.NET Core WebAPI · EF Core · JWT Bearer · 固定窗口限流 |
| 数据层 | MySQL 8.4 · 统一 `sys_` 前缀业务表 |
| 安全 | BCrypt · RBAC 策略 · Token 吊销 · 操作审计中间件 |

## 📁 项目结构

```
winsystem/
├── WinSystem/                  # WPF 客户端
│   ├── Views/                  # 视图（多页面 + 二级菜单）
│   ├── ViewModels/             # 视图模型（MVVM）
│   ├── Models/                 # 客户端数据模型
│   ├── Services/               # DataService（HTTP 客户端）、Session 等
│   └── appsettings.json        # API 地址配置（apiService.baseUrl）
├── WinSystem.Api/              # ASP.NET Core WebAPI 服务端
│   ├── Controllers/            # 业务控制器
│   ├── Data/                   # EF Core DbContext
│   ├── Models/                 # 实体模型
│   ├── Services/               # 登录守卫、密码策略、操作日志助手等
│   └── appsettings.json        # 连接串、JWT 配置（不含密钥）
├── website/                    # 项目介绍静态页面
├── backups/                    # 数据库备份（按时间戳命名）
└── SECURITY_AUDIT_REPORT.md    # 安全审计修复报告
```

## 🚀 快速开始

### 环境要求

- .NET (支持 WPF 与 ASP.NET Core，建议 .NET 8)
- MySQL 8.x（本地服务示例：`MySQL84`）
- Windows（WPF 客户端）· Visual Studio / VS Code

### 1. 初始化数据库

1. 安装并启动 MySQL 8.x，创建数据库：

```sql
CREATE DATABASE `winsystem` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
```

2. 导入数据库结构 / 初始数据（请选用最新备份）：

```
mysql -u root -p winsystem < backups/winsystem.sql
```

### 2. 配置服务端 `WinSystem.Api`

编辑 `WinSystem.Api/appsettings.json`，确认数据库连接串：

```json
{
  "ConnectionStrings": {
    "MySql": "Server=localhost;Port=3306;Database=winsystem;User=root;Password=root;"
  },
  "Jwt": {
    "Issuer": "WinSystem",
    "Audience": "WinSystem.Client",
    "ExpireHours": 24
  }
}
```

> 注意：JWT 签名密钥**不**写在配置文件里，必须通过环境变量提供。缺失密钥时服务将拒绝启动。

```powershell
# Windows PowerShell
$env:JWT_KEY = "请替换为足够长的随机密钥串"
```

启动 API（监听 `http://localhost:5210`）：

```powershell
cd WinSystem.Api
dotnet run
```

### 3. 配置与启动客户端 `WinSystem`

编辑 `WinSystem/appsettings.json`，将 API 地址指向服务端：

```json
{
  "apiService": {
    "baseUrl": "http://localhost:5210"
  }
}
```

启动客户端：

```powershell
cd WinSystem
dotnet run
```

> 启动顺序：MySQL → API → 客户端。

### 默认账号

| 账号 | 密码 | 说明 |
| --- | --- | --- |
| `admin` | `admin123` | 超级管理员（受保护账号，初始密码请及时修改） |

> 管理员重置用户密码的默认值为 `Aa123456`；调用后请提醒用户首次登录修改。

## 🔧 进阶配置

- **切换 API 地址**：修改客户端 `appsettings.json` 的 `apiService.baseUrl`，无需改代码即可适配部署环境
- **JWT 密钥**：通过环境变量 `JWT_KEY` 注入（生产环境切勿硬编码）
- **数据库备份**：历史备份位于 `backups/`，可按时间戳恢复

## 📄 License

本项目以 [MIT](LICENSE) 许可证开源。

---
