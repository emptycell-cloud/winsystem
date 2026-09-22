using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.IO;
using System.Text.Json;
using WinSystem.Models;

namespace WinSystem.Services
{
    /// <summary>集中管理业务数据：通过 WebAPI 对接 MySQL 数据库，内存中保留集合供 UI 绑定。</summary>
    public class DataService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;
        private bool _orgTreeLoaded;
        private bool _orgsFlatLoaded;
        private bool _menusAllLoaded;

        public DataService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            // 发送默认 User-Agent，便于操作日志记录来源
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("WinSystemClient/1.0");
        }

        /// <summary>设置 JWT Token，后续所有请求自动携带 Authorization 头。</summary>
        public void SetAuthToken(string? token)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<Order> Orders { get; } = new();
        public ObservableCollection<Organization> Organizations { get; } = new();
        public ObservableCollection<Menu> Menus { get; } = new();
        public ObservableCollection<Menu> AllMenus { get; } = new();
        public ObservableCollection<Organization> AllOrganizations { get; } = new();
        public ObservableCollection<Role> Roles { get; } = new();
        public ObservableCollection<SysMessage> Messages { get; } = new();
        public ObservableCollection<SysDictType> DictTypes { get; } = new();
        public ObservableCollection<SysConfig> Configs { get; } = new();
        public ObservableCollection<SysSchedule> Schedules { get; } = new();
        public ObservableCollection<SysOperationLog> OperationLogs { get; } = new();

        // ================= 启动加载 =================

        /// <summary>从 API 拉取全部业务数据到内存集合。</summary>
        public async Task LoadAsync()
        {
            Users.Clear();

            // 用户目录仅管理员可访问（服务端已限 AdminOnly），非管理员不加载
            if (Session.IsAdmin)
            {
                var users = await _http.GetFromJsonAsync<List<User>>("/api/users", JsonOptions) ?? new();
                foreach (var u in users) Users.Add(u);
            }

            await LoadAllOrganizationsAsync(); // 预载组织扁平列表，供上级名称即时解析
            await LoadAllMenusAsync();         // 预载全部菜单，供菜单配置页即时组装树
            await LoadRolesAsync();            // 预载角色，供用户编辑下拉即时使用
            await LoadUserMenuPermissionsAsync(); // 按当前用户角色加载菜单权限，用于侧边栏过滤
            await LoadOrganizationsAsync();
            await LoadMenusAsync();
        }

        /// <summary>加载当前登录用户所拥有的菜单权限标识，存到 Session，供侧边栏过滤。</summary>
        public async Task LoadUserMenuPermissionsAsync()
        {
            Session.MenuPermissions.Clear();
            var userId = Session.CurrentUser?.Id;
            if (string.IsNullOrEmpty(userId)) return;
            var keys = await _http.GetFromJsonAsync<List<string>>($"/api/users/{userId}/menu-permissions", JsonOptions) ?? new();
            foreach (var k in keys) Session.MenuPermissions.Add(k);
        }

        /// <summary>加载侧边菜单树（type=1/2，由 API 组装层级）。</summary>
        public async Task LoadMenusAsync()
        {
            Menus.Clear();
            var menus = await _http.GetFromJsonAsync<List<Menu>>("/api/menus", JsonOptions) ?? new();
            foreach (var m in menus) Menus.Add(m);
        }

        // ================= 菜单配置 =================

        /// <summary>加载全部菜单扁平列表（含 type=3，全局缓存），供菜单配置页使用。</summary>
        public async Task LoadAllMenusAsync()
            => await LoadAllMenusAsync(force: false);

        /// <summary>加载全部菜单扁平列表；force=true 忽略缓存强制重新拉取（增删改后用）。</summary>
        public async Task LoadAllMenusAsync(bool force)
        {
            if (!force && _menusAllLoaded) return;
            AllMenus.Clear();
            var menus = await _http.GetFromJsonAsync<List<Menu>>("/api/menus/all", JsonOptions) ?? new();
            foreach (var m in menus) AllMenus.Add(m);
            _menusAllLoaded = true;
        }

        public async Task AddMenuAsync(Menu menu)
        {
            var payload = new
            {
                menu.Name, menu.ParentId, menu.Type, menu.Path, menu.Component,
                menu.Icon, menu.Permission, menu.SortOrder, menu.Status, menu.IsVisible, menu.Remark
            };
            var resp = await _http.PostAsJsonAsync("/api/menus", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<Menu>(JsonOptions);
            if (created != null)
            {
                menu.Id = created.Id;
                AllMenus.Add(menu);
            }
        }

        public async Task UpdateMenuAsync(Menu menu)
        {
            var payload = new
            {
                menu.Name, menu.ParentId, menu.Type, menu.Path, menu.Component,
                menu.Icon, menu.Permission, menu.SortOrder, menu.Status, menu.IsVisible, menu.Remark
            };
            var resp = await _http.PutAsJsonAsync($"/api/menus/{menu.Id}", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteMenuAsync(Menu menu)
        {
            var resp = await _http.DeleteAsync($"/api/menus/{menu.Id}");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "删除菜单失败。" : msg);
            }
            AllMenus.Remove(menu);
        }

        // ================= 角色权限 =================

        /// <summary>加载角色列表。</summary>
        public async Task LoadRolesAsync()
        {
            var roles = await _http.GetFromJsonAsync<List<Role>>("/api/roles", JsonOptions) ?? new();
            Roles.Clear();
            foreach (var r in roles) Roles.Add(r);
        }

        public async Task AddRoleAsync(Role role)
        {
            var payload = new
            {
                role.Name, role.Code, role.Description, role.Color,
                role.DataScope, role.Status, role.SortOrder, role.Remark
            };
            var resp = await _http.PostAsJsonAsync("/api/roles", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<Role>(JsonOptions);
            if (created != null)
            {
                role.Id = created.Id;
                Roles.Add(role);
            }
        }

        public async Task UpdateRoleAsync(Role role)
        {
            var payload = new
            {
                role.Name, role.Code, role.Description, role.Color,
                role.DataScope, role.Status, role.SortOrder, role.Remark
            };
            var resp = await _http.PutAsJsonAsync($"/api/roles/{role.Id}", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteRoleAsync(Role role)
        {
            var resp = await _http.DeleteAsync($"/api/roles/{role.Id}");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "删除角色失败。" : msg);
            }
            Roles.Remove(role);
        }

        /// <summary>获取角色已分配的权限标识集合。</summary>
        public async Task<List<string>> LoadRolePermissionsAsync(string roleId)
        {
            var keys = await _http.GetFromJsonAsync<List<string>>($"/api/roles/{roleId}/permissions", JsonOptions) ?? new();
            return keys;
        }

        /// <summary>保存角色权限（全量替换）。</summary>
        public async Task SaveRolePermissionsAsync(string roleId, IEnumerable<string> permissions)
        {
            var resp = await _http.PutAsJsonAsync($"/api/roles/{roleId}/permissions", new { Permissions = permissions }, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        /// <summary>加载组织机构树（全局缓存：已加载则不再重复拉取，避免登录与页面进入时加载两遍）。</summary>
        public async Task LoadOrganizationsAsync()
            => await LoadOrganizationsAsync(force: false);

        /// <summary>加载组织机构树；force=true 忽略缓存强制重新拉取（增删改后用）。</summary>
        public async Task LoadOrganizationsAsync(bool force)
        {
            if (!force && _orgTreeLoaded) return;
            Organizations.Clear();
            var roots = await _http.GetFromJsonAsync<List<Organization>>("/api/organizations", JsonOptions) ?? new();
            foreach (var root in roots) Organizations.Add(root);
            _orgTreeLoaded = true;
        }

        // ================= 数据字典 =================

        public async Task LoadDictTypesAsync()
        {
            var types = await _http.GetFromJsonAsync<List<SysDictType>>("/api/dict-types", JsonOptions) ?? new();
            DictTypes.Clear();
            foreach (var t in types) DictTypes.Add(t);
        }

        public async Task AddDictTypeAsync(SysDictType type)
        {
            var payload = new { type.Name, type.Code, type.Status, type.SortOrder, type.Remark };
            var resp = await _http.PostAsJsonAsync("/api/dict-types", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<SysDictType>(JsonOptions);
            if (created != null)
            {
                type.Id = created.Id;
                DictTypes.Add(type);
            }
        }

        public async Task UpdateDictTypeAsync(SysDictType type)
        {
            var payload = new { type.Name, type.Code, type.Status, type.SortOrder, type.Remark };
            var resp = await _http.PutAsJsonAsync($"/api/dict-types/{type.Id}", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteDictTypeAsync(SysDictType type)
        {
            var resp = await _http.DeleteAsync($"/api/dict-types/{type.Id}");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "删除字典类型失败。" : msg);
            }
            DictTypes.Remove(type);
        }

        public async Task<List<SysDictData>> LoadDictDataAsync(string code)
        {
            var datas = await _http.GetFromJsonAsync<List<SysDictData>>($"/api/dict-types/{Uri.EscapeDataString(code)}/data", JsonOptions) ?? new();
            return datas;
        }

        public async Task<SysDictData> AddDictDataAsync(SysDictData data)
        {
            var payload = new { data.DictCode, data.Label, data.Value, data.Status, data.SortOrder, data.Remark };
            var resp = await _http.PostAsJsonAsync("/api/dict-types/data", payload, JsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "新增字典数据失败。" : msg);
            }
            return await resp.Content.ReadFromJsonAsync<SysDictData>(JsonOptions) ?? data;
        }

        public async Task UpdateDictDataAsync(SysDictData data)
        {
            var payload = new { data.DictCode, data.Label, data.Value, data.Status, data.SortOrder, data.Remark };
            var resp = await _http.PutAsJsonAsync($"/api/dict-types/data/{data.Id}", payload, JsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "修改字典数据失败。" : msg);
            }
        }

        public async Task DeleteDictDataAsync(SysDictData data)
        {
            var resp = await _http.DeleteAsync($"/api/dict-types/data/{data.Id}");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "删除字典数据失败。" : msg);
            }
        }

        // ================= 系统参数 =================

        public async Task LoadConfigsAsync()
        {
            var list = await _http.GetFromJsonAsync<List<SysConfig>>("/api/configs", JsonOptions) ?? new();
            Configs.Clear();
            foreach (var c in list) Configs.Add(c);
        }

        public async Task AddConfigAsync(SysConfig cfg)
        {
            var payload = new { cfg.ConfigName, cfg.ConfigKey, cfg.ConfigValue, cfg.ConfigType, cfg.IsSystem, cfg.Remark };
            var resp = await _http.PostAsJsonAsync("/api/configs", payload, JsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "新增系统参数失败。" : msg);
            }
            var created = await resp.Content.ReadFromJsonAsync<SysConfig>(JsonOptions);
            if (created != null)
            {
                cfg.Id = created.Id;
                Configs.Add(cfg);
            }
        }

        public async Task UpdateConfigAsync(SysConfig cfg)
        {
            var payload = new { cfg.ConfigName, cfg.ConfigKey, cfg.ConfigValue, cfg.ConfigType, cfg.Remark };
            var resp = await _http.PutAsJsonAsync($"/api/configs/{cfg.Id}", payload, JsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "修改系统参数失败。" : msg);
            }
        }

        public async Task DeleteConfigAsync(SysConfig cfg)
        {
            var resp = await _http.DeleteAsync($"/api/configs/{cfg.Id}");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "删除系统参数失败。" : msg);
            }
            Configs.Remove(cfg);
        }

        // ================= 日程管理 =================

        public async Task LoadSchedulesAsync()
        {
            var list = await _http.GetFromJsonAsync<List<SysSchedule>>("/api/schedules", JsonOptions) ?? new();
            Schedules.Clear();
            foreach (var s in list) Schedules.Add(s);
        }

        public async Task AddScheduleAsync(SysSchedule schedule)
        {
            var payload = new
            {
                schedule.Title,
                schedule.Description,
                schedule.ScheduleDate,
                schedule.StartTime,
                schedule.EndTime,
                schedule.Color,
                schedule.Location,
                schedule.IsCompleted
            };
            var resp = await _http.PostAsJsonAsync("/api/schedules", payload, JsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "新增日程失败。" : msg);
            }
            var created = await resp.Content.ReadFromJsonAsync<SysSchedule>(JsonOptions);
            if (created != null)
            {
                schedule.Id = created.Id;
                schedule.CreatorName = created.CreatorName;
                Schedules.Add(schedule);
            }
        }

        public async Task UpdateScheduleAsync(SysSchedule schedule)
        {
            var payload = new
            {
                schedule.Title,
                schedule.Description,
                schedule.ScheduleDate,
                schedule.StartTime,
                schedule.EndTime,
                schedule.Color,
                schedule.Location,
                schedule.IsCompleted
            };
            var resp = await _http.PutAsJsonAsync($"/api/schedules/{schedule.Id}", payload, JsonOptions);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "修改日程失败。" : msg);
            }
        }

        public async Task DeleteScheduleAsync(SysSchedule schedule)
        {
            var resp = await _http.DeleteAsync($"/api/schedules/{schedule.Id}");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "删除日程失败。" : msg);
            }
            Schedules.Remove(schedule);
        }

        // ================= 操作日志 =================

        public async Task LoadOperationLogsAsync()
        {
            var list = await _http.GetFromJsonAsync<List<SysOperationLog>>("/api/operation-logs", JsonOptions) ?? new();
            OperationLogs.Clear();
            foreach (var l in list) OperationLogs.Add(l);
        }

        public async Task DeleteOperationLogAsync(SysOperationLog log)
        {
            var resp = await _http.DeleteAsync($"/api/operation-logs/{log.Id}");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "删除日志失败。" : msg);
            }
            OperationLogs.Remove(log);
        }

        public async Task ClearOperationLogsAsync()
        {
            var resp = await _http.DeleteAsync("/api/operation-logs");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "清空日志失败。" : msg);
            }
            OperationLogs.Clear();
        }

        // ================= 组织机构配置 =================

        /// <summary>加载组织扁平列表（全局缓存：已加载则不再重复拉取），登录时预载以便上级名称即时解析。</summary>
        public async Task LoadAllOrganizationsAsync()
            => await LoadAllOrganizationsAsync(force: false);

        /// <summary>加载组织扁平列表；force=true 忽略缓存强制重新拉取（增删改后用）。</summary>
        public async Task LoadAllOrganizationsAsync(bool force)
        {
            if (!force && _orgsFlatLoaded) return;
            AllOrganizations.Clear();
            var orgs = await _http.GetFromJsonAsync<List<Organization>>("/api/organizations/all", JsonOptions) ?? new();
            foreach (var o in orgs) AllOrganizations.Add(o);
            _orgsFlatLoaded = true;
        }

        public async Task AddOrganizationAsync(Organization org)
        {
            var payload = new
            {
                org.Name, org.Code, org.ParentId, org.Type, org.Manager,
                org.Phone, org.Email, org.SortOrder, org.Status, org.Description
            };
            var resp = await _http.PostAsJsonAsync("/api/organizations", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<Organization>(JsonOptions);
            if (created != null)
            {
                org.Id = created.Id;
                AllOrganizations.Add(org);
            }
        }

        public async Task UpdateOrganizationAsync(Organization org)
        {
            var payload = new
            {
                org.Name, org.Code, org.ParentId, org.Type, org.Manager,
                org.Phone, org.Email, org.SortOrder, org.Status, org.Description
            };
            var resp = await _http.PutAsJsonAsync($"/api/organizations/{org.Id}", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteOrganizationAsync(Organization org)
        {
            var resp = await _http.DeleteAsync($"/api/organizations/{org.Id}");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "删除组织失败。" : msg);
            }
            AllOrganizations.Remove(org);
        }

        /// <summary>从 API 实时重新加载用户列表。</summary>
        public async Task LoadUsersAsync()
        {
            var users = await _http.GetFromJsonAsync<List<User>>("/api/users", JsonOptions) ?? new();
            Users.Clear();
            foreach (var u in users) Users.Add(u);
        }

        // ================= 登录 =================

        public async Task<User?> LoginAsync(string username, string password)
        {
            var resp = await _http.PostAsJsonAsync("/api/auth/login", new { Username = username, Password = password }, JsonOptions);
            resp.EnsureSuccessStatusCode();

            var result = await resp.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
            if (result is not { Success: true } || result.User == null) return null;

            // 保存 JWT Token，后续所有 API 请求自动携带
            if (!string.IsNullOrEmpty(result.Token))
            {
                Session.AuthToken = result.Token;
                SetAuthToken(result.Token);
            }

            // 记录是否为管理员，供启动加载等场景按角色控制数据范围
            Session.IsAdmin = result.Roles != null &&
                (result.Roles.Contains("super_admin") || result.Roles.Contains("admin"));

            return new User
            {
                Id = result.User.Id,
                Username = result.User.Username,
                Name = result.User.Name,
                Position = result.User.Position,
                Email = result.User.Email,
                Phone = result.User.Phone,
                Status = result.User.Status,
                OrganizationId = result.User.OrganizationId,
                LoginCount = result.User.LoginCount,
                LastLoginAt = result.User.LastLoginAt,
                LastLoginIp = result.User.LastLoginIp
            };
        }

        private class LoginResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = "";
            public LoginUser? User { get; set; }
            public string? Token { get; set; }
            public List<string>? Roles { get; set; }
        }

        private class LoginUser
        {
            public string Id { get; set; } = "";
            public string Username { get; set; } = "";
            public string Name { get; set; } = "";
            public string? Position { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public int Status { get; set; }
            public string? OrganizationId { get; set; }
            public int LoginCount { get; set; }
            public DateTime? LastLoginAt { get; set; }
            public string? LastLoginIp { get; set; }
        }

        // ================= 修改密码 =================

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string userId, string oldPwd, string newPwd)
        {
            var resp = await _http.PostAsJsonAsync("/api/auth/change-password",
                new { Id = userId, OldPassword = oldPwd, NewPassword = newPwd }, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<ChangePasswordResult>(JsonOptions);
            return result == null ? (false, "无法连接服务器。") : (result.Success, result.Message);
        }

        private class ChangePasswordResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = "";
        }

        // ================= 用户 =================

        public async Task AddUserAsync(User user)
        {
            var payload = new
            {
                user.Username,
                user.Name,
                Password = user.PasswordHash,
                user.Position,
                user.Email,
                user.Phone,
                user.OrganizationId,
                user.Status,
                user.LoginType,
                user.Remark,
                RoleIds = user.RoleIds
            };
            var resp = await _http.PostAsJsonAsync("/api/users", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<User>(JsonOptions);
            if (created != null)
            {
                user.Id = created.Id;
                user.CreatedAt = created.CreatedAt;
                Users.Add(user);
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            var payload = new
            {
                user.Username,
                user.Name,
                Password = user.PasswordHash,
                user.Position,
                user.Email,
                user.Phone,
                user.OrganizationId,
                user.Status,
                user.LoginType,
                user.Remark,
                RoleIds = user.RoleIds
            };
            var resp = await _http.PutAsJsonAsync($"/api/users/{user.Id}", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        /// <summary>获取用户已分配的角色 ID 列表。</summary>
        public async Task<List<string>> LoadUserRoleIdsAsync(string userId)
        {
            var ids = await _http.GetFromJsonAsync<List<string>>($"/api/users/{userId}/roles", JsonOptions) ?? new();
            return ids;
        }

        public async Task DeleteUserAsync(User user)
        {
            var resp = await _http.DeleteAsync($"/api/users/{user.Id}");
            resp.EnsureSuccessStatusCode();
            Users.Remove(user);
        }

        public async Task ResetPasswordAsync(User user)
        {
            var resp = await _http.PostAsync($"/api/users/{user.Id}/reset-password", null);
            resp.EnsureSuccessStatusCode();
        }

        // ================= 商品 =================

        public async Task AddProductAsync(Product product)
        {
            var resp = await _http.PostAsJsonAsync("/api/products", product, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<Product>(JsonOptions);
            if (created != null)
            {
                product.Id = created.Id;
                Products.Add(product);
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            var resp = await _http.PutAsJsonAsync($"/api/products/{product.Id}", product, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductAsync(Product product)
        {
            var resp = await _http.DeleteAsync($"/api/products/{product.Id}");
            resp.EnsureSuccessStatusCode();
            Products.Remove(product);
        }

        // ================= 订单 =================

        public async Task AddOrderAsync(Order order)
        {
            var resp = await _http.PostAsJsonAsync("/api/orders", order, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<Order>(JsonOptions);
            if (created != null)
            {
                order.Id = created.Id;
                Orders.Add(order);
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var resp = await _http.PutAsJsonAsync($"/api/orders/{order.Id}", order, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteOrderAsync(Order order)
        {
            var resp = await _http.DeleteAsync($"/api/orders/{order.Id}");
            resp.EnsureSuccessStatusCode();
            Orders.Remove(order);
        }

        /// <summary>按关键字（姓名/账号）搜索用户，供消息通知选择接收人（分页加载，避免全量）。</summary>
        public async Task<List<UserSearchItem>> SearchUsersAsync(string keyword, int limit = 50)
        {
            var url = $"/api/users/search?keyword={Uri.EscapeDataString(keyword)}&limit={limit}";
            return await _http.GetFromJsonAsync<List<UserSearchItem>>(url, JsonOptions) ?? new();
        }

        // ================= 消息通知 =================

        /// <summary>加载全部消息（全量，客户端本地分页/筛选）。</summary>
        public async Task LoadMessagesAsync()
        {
            var messages = await _http.GetFromJsonAsync<List<SysMessage>>("/api/messages", JsonOptions) ?? new();
            Messages.Clear();
            foreach (var m in messages) Messages.Add(m);
        }

        /// <summary>获取消息详情（含发布范围目标及目标名称）。</summary>
        public async Task<SysMessage?> LoadMessageAsync(long id)
        {
            return await _http.GetFromJsonAsync<SysMessage>($"/api/messages/{id}", JsonOptions);
        }

        public async Task AddMessageAsync(SysMessage message)
        {
            var payload = new
            {
                message.Title, message.Content, message.Type, message.Status, message.Scope,
                message.Remark,
                Targets = message.Targets.Select(t => new { t.TargetType, t.TargetId })
            };
            var resp = await _http.PostAsJsonAsync("/api/messages", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<SysMessage>(JsonOptions);
            if (created != null)
            {
                message.Id = created.Id;
                message.CreatedAt = created.CreatedAt;
                Messages.Add(message);
            }
        }

        public async Task UpdateMessageAsync(SysMessage message)
        {
            var payload = new
            {
                message.Title, message.Content, message.Type, message.Status, message.Scope,
                message.Remark,
                Targets = message.Targets.Select(t => new { t.TargetType, t.TargetId })
            };
            var resp = await _http.PutAsJsonAsync($"/api/messages/{message.Id}", payload, JsonOptions);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteMessageAsync(SysMessage message)
        {
            var resp = await _http.DeleteAsync($"/api/messages/{message.Id}");
            resp.EnsureSuccessStatusCode();
            Messages.Remove(message);
        }

        /// <summary>发布消息（草稿 → 已发布）。</summary>
        public async Task PublishMessageAsync(SysMessage message)
        {
            var resp = await _http.PostAsync($"/api/messages/{message.Id}/publish", null);
            resp.EnsureSuccessStatusCode();
            message.Status = 1;
        }

        // ================= 我的消息（接收端） =================

        /// <summary>加载当前登录用户可见的消息（服务端分页 + 已读状态）。</summary>
        public async Task<MyMessagePage> LoadMyMessagesAsync(int page, int pageSize, string keyword, int type)
        {
            var url = $"/api/messages/mine?page={page}&pageSize={pageSize}" +
                      $"&keyword={Uri.EscapeDataString(keyword)}&type={type}";
            return await _http.GetFromJsonAsync<MyMessagePage>(url, JsonOptions) ?? new();
        }

        /// <summary>我的未读消息数量。</summary>
        public async Task<int> GetMyUnreadCountAsync()
        {
            var r = await _http.GetFromJsonAsync<MyMessageCount>("/api/messages/mine/unread-count", JsonOptions);
            return r?.Count ?? 0;
        }

        /// <summary>标记我的消息为已读。</summary>
        public async Task MarkMyMessageReadAsync(long id)
        {
            var resp = await _http.PostAsync($"/api/messages/mine/{id}/read", null);
            resp.EnsureSuccessStatusCode();
        }

        // ================= ID 生成 =================

        /// <summary>账号 ID 由服务端生成（A10001…），此处不再预判。</summary>
        public string NextUserId() => "";
        public int NextProductId() => Products.Count == 0 ? 1 : Products.Max(p => p.Id) + 1;
        public int NextOrderId() => Orders.Count == 0 ? 1 : Orders.Max(o => o.Id) + 1;
    }

    /// <summary>WebAPI 连接配置。</summary>
    public static class ApiConfig
    {
        /// <summary>默认后端地址（当配置文件缺失或解析失败时使用）。</summary>
        private const string DefaultBaseUrl = "http://localhost:5210";

        private static string? _baseUrl;

        /// <summary>后端 API 基地址，优先从程序目录下的 appsettings.json 的 apiService.baseUrl 读取。</summary>
        public static string BaseUrl
        {
            get
            {
                if (_baseUrl != null) return _baseUrl;
                _baseUrl = LoadFromConfig() ?? DefaultBaseUrl;
                return _baseUrl;
            }
        }

        private static string? LoadFromConfig()
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                if (!File.Exists(path)) return null;

                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                if (doc.RootElement.TryGetProperty("apiService", out var api)
                    && api.TryGetProperty("baseUrl", out var url))
                {
                    var value = url.GetString();
                    return string.IsNullOrWhiteSpace(value) ? null : value;
                }
            }
            catch
            {
                // 配置读取失败时回退到默认地址
            }
            return null;
        }
    }
}