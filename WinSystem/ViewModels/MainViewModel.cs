using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WinSystem.Commands;
using WinSystem.Models;
using WinSystem.Services;
using WinSystem.ViewModels;
using MenuItemModel = WinSystem.Models.Menu;

namespace WinSystem.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private WorkTab? _activeTab;
        private readonly DispatcherTimer _timer = new();
        private readonly Dictionary<string, string> _menuNames = new();
        private readonly Dictionary<string, string> _menuTitlesByPath = new();

        /// <summary>当前主窗口 ViewModel 实例（供“我的消息”页刷新侧边栏未读气泡）。</summary>
        public static MainViewModel? Current { get; private set; }

        public MainViewModel()
        {
            Current = this;
            Dashboard = new DashboardViewModel();
            Dashboard.OpenSchedule = OpenScheduleModule;
            Users = new UserViewModel();
            Products = new ProductViewModel();
            Orders = new OrderViewModel();
            Settings = new SettingsViewModel();
            MenuConfig = new MenuViewModel();

            NavigateCommand = new RelayCommand(Navigate, CanNavigate);
            MenuCommand = new RelayCommand(MenuClick);
            CloseTabCommand = new RelayCommand(p => CloseTab(p as WorkTab));
            LogoutCommand = new RelayCommand(Logout);
            OpenMyMessagesCommand = new RelayCommand(_ => OpenTab("/Views/MyMessageView.xaml"));

            BuildMenu();
            OpenTab("Dashboard");
            LoadMyMessageUnreadBadgeAsync();   // 登录后加载“我的消息”未读数气泡

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (_, _) => Clock = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _timer.Start();
        }

        public DashboardViewModel Dashboard { get; }
        public UserViewModel Users { get; }
        public ProductViewModel Products { get; }
        public OrderViewModel Orders { get; }
        public SettingsViewModel Settings { get; }
        public MenuViewModel MenuConfig { get; }

        public ObservableCollection<NavItem> MenuItems { get; } = new();
        public ObservableCollection<WorkTab> Tabs { get; } = new();

        private string _clock = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string Clock { get => _clock; set => SetProperty(ref _clock, value); }

        private int _myMessageUnread;
        /// <summary>我的消息未读数量（顶栏铃铛气泡）。</summary>
        public int MyMessageUnread
        {
            get => _myMessageUnread;
            set
            {
                if (SetProperty(ref _myMessageUnread, value))
                {
                    OnPropertyChanged(nameof(MyMessageUnreadVisible));
                    OnPropertyChanged(nameof(MyMessageUnreadText));
                }
            }
        }

        /// <summary>顶栏未读气泡是否显示（大于 0 时显示）。</summary>
        public bool MyMessageUnreadVisible => _myMessageUnread > 0;

        /// <summary>顶栏未读气泡文本（超过 99 显示 99+）。</summary>
        public string MyMessageUnreadText => _myMessageUnread > 99 ? "99+" : _myMessageUnread.ToString();

        public string CurrentUserName => Session.CurrentUser?.Name ?? "";
        public string CurrentUserRole => Session.CurrentUser?.Position ?? "";

        public WorkTab? ActiveTab
        {
            get => _activeTab;
            set => SetProperty(ref _activeTab, value);
        }

        public ICommand NavigateCommand { get; }
        public ICommand MenuCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand LogoutCommand { get; }

        /// <summary>点击顶栏铃铛：打开“我的消息”页。</summary>
        public ICommand OpenMyMessagesCommand { get; }

        /// <summary>从 sys_menu（type=1/2）构建侧边栏菜单，并按当前用户角色权限过滤；数据库无数据时回退到内置菜单。</summary>
        private void BuildMenu()
        {
            MenuItems.Clear();
            _menuNames.Clear();
            _menuTitlesByPath.Clear();
            var menus = Session.Data.Menus;
            if (menus.Count == 0)
            {
                BuildFallbackMenu();
                return;
            }

            var allowed = Session.MenuPermissions;

            foreach (var root in menus)
            {
                // 仅保留有权限的节点：节点自身 permission 被授权，或其子孙有被授权项
                if (root.Children.Count == 0)
                {
                    if (allowed.Count > 0 && !allowed.Contains(root.Permission ?? "")) continue;
                }
                else
                {
                    if (!SubtreeAllowed(root, allowed)) continue;
                }

                _menuNames[root.Id] = root.Name;
                if (!string.IsNullOrWhiteSpace(root.Path)) _menuTitlesByPath[root.Path.Trim()] = root.Name;
                var item = new NavItem
                {
                    Title = root.Name,
                    IconData = IconForMenu(root.Icon),
                    PageKey = PageKeyForMenu(root)
                };
                foreach (var child in root.Children)
                {
                    if (!Visible(child, allowed)) continue;
                    _menuNames[child.Id] = child.Name;
                    if (!string.IsNullOrWhiteSpace(child.Path)) _menuTitlesByPath[child.Path.Trim()] = child.Name;
                    item.Children.Add(new NavItem
                    {
                        Title = child.Name,
                        IconData = IconForMenu(child.Icon),
                        PageKey = PageKeyForMenu(child)
                    });
                }
                if (item.Children.Count > 0 || root.Children.Count == 0)
                    MenuItems.Add(item);
            }
        }

        /// <summary>节点或其子树内是否存在被授权的节点（用于保留有授权子项的目录）。</summary>
        private static bool SubtreeAllowed(MenuItemModel node, HashSet<string> allowed)
        {
            if (allowed.Contains(node.Permission ?? "")) return true;
            return node.Children.Any(c => SubtreeAllowed(c, allowed));
        }

        /// <summary>节点是否可见：有权限或子树内有被授权节点。</summary>
        private static bool Visible(MenuItemModel node, HashSet<string> allowed)
        {
            if (allowed.Count == 0) return true;
            if (allowed.Contains(node.Permission ?? "")) return true;
            return node.Children.Any(c => Visible(c, allowed));
        }

        /// <summary>将 Lucide 图标名映射为 WPF Path 几何数据；未知图标使用默认图标。</summary>
        private static string IconForMenu(string? lucide)
        {
            switch (lucide?.ToLowerInvariant())
            {
                case "layoutdashboard":
                case "dashboard": return DashboardViewModel.Icons.Dashboard;
                case "users": return DashboardViewModel.Icons.Users;
                case "user":
                case "usercog": return DashboardViewModel.Icons.User;
                case "shoppingcart":
                case "cart": return DashboardViewModel.Icons.Cart;
                case "package":
                case "box": return DashboardViewModel.Icons.Box;
                case "settings":
                case "settings2": return DashboardViewModel.Icons.Settings;
                case "bell": return DashboardViewModel.Icons.Bell;
                default: return DashboardViewModel.Icons.Box;
            }
        }

        /// <summary>由菜单路由推导页面 key：.xaml 视图路径直接返回原路径（动态加载），其余映射到现有页面或用占位 key。</summary>
        private static string PageKeyForMenu(MenuItemModel m)
        {
            var path = m.Path?.Trim() ?? "";
            if (path.Length == 0) return ""; // 目录无路由，作为分组处理
            if (path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)) return path; // 形如 /Views/MenuView.xaml
            if (string.Equals(path, "/dashboard", StringComparison.OrdinalIgnoreCase)) return "Dashboard";
            if (path.Contains("users/list", StringComparison.OrdinalIgnoreCase)) return "Users";
            if (path.Contains("products/list", StringComparison.OrdinalIgnoreCase)) return "Products";
            if (path.Contains("orders/list", StringComparison.OrdinalIgnoreCase)) return "Orders";
            if (path.Contains("system/config", StringComparison.OrdinalIgnoreCase)) return "Settings";
            if (path.Contains("system/menus", StringComparison.OrdinalIgnoreCase)) return "Menus";
            return "Placeholder:" + m.Id;
        }

        /// <summary>登录后异步加载“我的消息”未读数量：更新顶栏铃铛气泡与侧边栏菜单项气泡。</summary>
        private async void LoadMyMessageUnreadBadgeAsync()
        {
            try
            {
                var count = await Session.Data.GetMyUnreadCountAsync();
                ApplyMyMessageUnread(count);
            }
            catch { /* 未读数加载失败不影响主界面 */ }
        }

        /// <summary>刷新未读气泡（供消息页标记已读后调用）。</summary>
        public async void RefreshMyMessageUnreadBadge()
        {
            try
            {
                var count = await Session.Data.GetMyUnreadCountAsync();
                ApplyMyMessageUnread(count);
            }
            catch { /* 忽略加载失败 */ }
        }

        /// <summary>把未读数量同步到顶栏铃铛气泡与侧边栏“我的消息”菜单项。</summary>
        private void ApplyMyMessageUnread(int count)
        {
            MyMessageUnread = count;
            var item = FindMyMessageItem();
            if (item != null) item.UnreadCount = count;
        }

        /// <summary>在侧边栏菜单中定位“我的消息”菜单项。</summary>
        private NavItem? FindMyMessageItem()
            => MenuItems.SelectMany(m => m.Children).FirstOrDefault(c => c.Title == "我的消息");

        private void BuildFallbackMenu()
        {
            var biz = new NavItem
            {
                Title = "业务管理",
                IconData = DashboardViewModel.Icons.Money,
                IsGroupHeader = true
            };
            biz.Children.Add(new NavItem { Title = "用户管理", IconData = DashboardViewModel.Icons.Users, PageKey = "Users" });
            biz.Children.Add(new NavItem { Title = "商品管理", IconData = DashboardViewModel.Icons.Box, PageKey = "Products" });
            biz.Children.Add(new NavItem { Title = "订单管理", IconData = DashboardViewModel.Icons.Order, PageKey = "Orders" });

            MenuItems.Add(new NavItem { Title = "工作台", IconData = DashboardViewModel.Icons.Dashboard, PageKey = "Dashboard" });
            MenuItems.Add(biz);
            MenuItems.Add(new NavItem { Title = "系统设置", IconData = DashboardViewModel.Icons.Settings, PageKey = "Settings" });
        }

        /// <summary>点击一级菜单：有子菜单则展开/收起，叶子菜单则打开选项卡。</summary>
        private void MenuClick(object? param)
        {
            if (param is not NavItem item) return;
            if (item.HasChildren)
            {
                item.IsExpanded = !item.IsExpanded;
                return;
            }
            OpenTab(item.PageKey);
        }

        private bool CanNavigate(object? param) => param is string;

        private void Navigate(object? param) => OpenTab(param as string ?? "");

        /// <summary>打开功能选项卡：已存在则激活，否则新建。</summary>
        private void OpenTab(string key)
        {
            var existing = Tabs.FirstOrDefault(t => t.PageKey == key);
            if (existing != null) { Activate(existing); return; }

            WorkTab? tab;
            if (key.StartsWith("/Views/", StringComparison.OrdinalIgnoreCase) &&
                key.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
            {
                // 动态加载：path 形如 /Views/MenuView.xaml
                tab = CreateViewTab(key);
            }
            else
            {
                tab = key switch
                {
                    "Dashboard" => new WorkTab("Dashboard", "工作台", Dashboard, false),
                    "Users" => new WorkTab("Users", "用户管理", Users, true, t => CloseTab(t)),
                    "Products" => new WorkTab("Products", "商品管理", Products, true, t => CloseTab(t)),
                    "Orders" => new WorkTab("Orders", "订单管理", Orders, true, t => CloseTab(t)),
                    "Settings" => new WorkTab("Settings", "系统设置", Settings, true, t => CloseTab(t)),
                    "Menus" => new WorkTab("Menus", "菜单配置", MenuConfig, true, t => CloseTab(t)),
                    _ when key.StartsWith("Placeholder:") => CreatePlaceholderTab(key),
                    _ => null
                };
            }
            if (tab == null) return;

            Tabs.Add(tab);
            Activate(tab);
        }

        /// <summary>工作台点击某条日程：打开日程管理模块并定位到该日程的日期。</summary>
        private void OpenScheduleModule(SysSchedule? schedule)
        {
            OpenTab("/Views/ScheduleView.xaml");
            if (schedule == null) return;

            if (Tabs.FirstOrDefault(t => t.PageKey == "/Views/ScheduleView.xaml")?.Content is FrameworkElement fe &&
                fe.DataContext is ScheduleViewModel svm)
                svm.NavigateTo(schedule.ScheduleDate);
        }

        /// <summary>根据 sys_menu.path（形如 /Views/MenuView.xaml）动态创建选项卡：
        /// 反射加载对应 View，按约定（MenuView→MenuViewModel）创建 ViewModel 并绑定，找不到视图则回退占位页。</summary>
        private WorkTab? CreateViewTab(string path)
        {
            var viewName = System.IO.Path.GetFileNameWithoutExtension(path); // MenuView

            var viewType = FindType("WinSystem.Views." + viewName);
            var fallbackTitle = _menuTitlesByPath.TryGetValue(path, out var n) && !string.IsNullOrEmpty(n) ? n : viewName;

            if (viewType == null)
                return new WorkTab(path, fallbackTitle, BuildPlaceholder(fallbackTitle), true, t => CloseTab(t));

            // 约定式 ViewModel：MenuView → MenuViewModel
            var vmTypeName = viewName.EndsWith("View", StringComparison.Ordinal)
                ? "WinSystem.ViewModels." + viewName[..^4] + "ViewModel"
                : "WinSystem.ViewModels." + viewName + "ViewModel";
            var vmType = FindType(vmTypeName);
            var vm = vmType != null && vmType.GetConstructor(Type.EmptyTypes) != null
                ? Activator.CreateInstance(vmType)
                : null;

            var view = (FrameworkElement)Activator.CreateInstance(viewType)!;
            if (vm != null) view.DataContext = vm;

            // 初始化页面数据（VM 提供 ReloadAsync 时加载一次）
            if (vm != null)
            {
                var reload = vm.GetType().GetMethod("ReloadAsync", Type.EmptyTypes);
                if (reload != null && typeof(Task).IsAssignableFrom(reload.ReturnType))
                    _ = (Task)reload.Invoke(vm, null)!;
            }

            return new WorkTab(path, fallbackTitle, view, true, t => CloseTab(t));
        }

        /// <summary>按完整类型名查找类型：先查调用程序集，再查已加载的所有程序集。</summary>
        private static Type? FindType(string fullName)
        {
            var t = Type.GetType(fullName);
            if (t != null) return t;
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(fullName))
                .FirstOrDefault(x => x != null);
        }

        /// <summary>为尚未实现对应页面的菜单创建“功能开发中”占位选项卡。</summary>
        private WorkTab CreatePlaceholderTab(string key)
        {
            var id = key["Placeholder:".Length..];
            var title = _menuNames.TryGetValue(id, out var n) && !string.IsNullOrEmpty(n) ? n : "功能页面";
            return new WorkTab(key, title, BuildPlaceholder(title), true, t => CloseTab(t));
        }

        private static FrameworkElement BuildPlaceholder(string title)
        {
            var stack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stack.Children.Add(new TextBlock
            {
                Text = "功能开发中",
                FontSize = 22,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)Application.Current.FindResource("TextPrimaryBrush"),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stack.Children.Add(new TextBlock
            {
                Text = $"「{title}」模块尚未实现，敬请期待。",
                FontSize = 13,
                Margin = new Thickness(0, 12, 0, 0),
                Foreground = (Brush)Application.Current.FindResource("TextSecondaryBrush"),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            return stack;
        }

        private void Activate(WorkTab tab)
        {
            foreach (var t in Tabs) t.IsActive = t == tab;
            ActiveTab = tab;
            UpdateMenuActive(tab.PageKey);

            // 动态加载的 XAML 页面：其 DataContext 若提供 ReloadAsync，则激活时刷新数据
            if (tab.Content is FrameworkElement fe && fe.DataContext != null)
            {
                var reload = fe.DataContext.GetType().GetMethod("ReloadAsync", Type.EmptyTypes);
                if (reload != null && typeof(Task).IsAssignableFrom(reload.ReturnType))
                    _ = (Task)reload.Invoke(fe.DataContext, null)!;
            }

            // 激活用户管理页时实时从 API 刷新数据
            if (tab.PageKey == "Users")
            {
                _ = RefreshUsersAsync();
            }
            // 激活菜单配置页时实时从 API 刷新数据
            if (tab.PageKey == "Menus")
            {
                _ = RefreshMenusAsync();
            }
            // 激活工作台时重新拉取日程统计（工作台 Content 是 ViewModel，走显式分支）
            if (tab.PageKey == "Dashboard")
            {
                _ = Dashboard.ReloadAsync();
            }
        }

        private async Task RefreshUsersAsync()
        {
            try { await Users.ReloadAsync(); }
            catch { /* 忽略刷新异常，保持现有数据 */ }
        }

        private async Task RefreshMenusAsync()
        {
            try { await MenuConfig.ReloadAsync(); }
            catch { /* 忽略刷新异常，保持现有数据 */ }
        }

        private void CloseTab(WorkTab? tab)
        {
            if (tab == null || !tab.CanClose) return;
            var index = Tabs.IndexOf(tab);
            Tabs.Remove(tab);

            // 若关闭的是激活页，则激活相邻页
            if (ActiveTab == tab)
            {
                var next = Tabs.Count > 0 ? Tabs[Math.Clamp(index, 0, Tabs.Count - 1)] : null;
                if (next != null) Activate(next);
                else if (Tabs.Count == 0) OpenTab("Dashboard");
            }
            else
            {
                UpdateMenuActive(ActiveTab?.PageKey ?? "");
            }
        }

        private void UpdateMenuActive(string key)
        {
            foreach (var item in MenuItems)
            {
                item.IsActive = item.PageKey == key;
                foreach (var child in item.Children)
                {
                    child.IsActive = child.PageKey == key;
                    if (child.IsActive) item.IsExpanded = true;
                }
            }
        }

        private void Logout()
        {
            var app = System.Windows.Application.Current;
            app.Shutdown();
        }
    }
}