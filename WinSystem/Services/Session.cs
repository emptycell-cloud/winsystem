using System.Text.Json.Serialization;
using WinSystem.Models;

namespace WinSystem.Services
{
    /// <summary>应用级会话：当前登录用户、用户菜单权限与全局数据服务。</summary>
    public static class Session
    {
        public static DataService Data { get; } = new();
        public static User? CurrentUser { get; set; }
        public static string? AuthToken { get; set; }

        /// <summary>当前用户是否为管理员（super_admin / admin），用于客户端控制数据加载范围。</summary>
        public static bool IsAdmin { get; set; }

        /// <summary>当前用户拥有权限的菜单权限标识集合（来自 sys_role_menu），用于过滤侧边栏菜单。</summary>
        public static HashSet<string> MenuPermissions { get; } = new();
    }
}