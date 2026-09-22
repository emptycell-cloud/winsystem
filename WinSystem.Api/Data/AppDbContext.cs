using Microsoft.EntityFrameworkCore;
using WinSystem.Api.Models;

namespace WinSystem.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RoleMenu> RoleMenus => Set<RoleMenu>();
        public DbSet<AccountRole> AccountRoles => Set<AccountRole>();
        public DbSet<SysMessage> Messages => Set<SysMessage>();
        public DbSet<MessageTarget> MessageTargets => Set<MessageTarget>();
        public DbSet<MessageRead> MessageReads => Set<MessageRead>();
        public DbSet<SysDictType> DictTypes => Set<SysDictType>();
        public DbSet<SysDictData> DictDatas => Set<SysDictData>();
        public DbSet<SysConfig> Configs => Set<SysConfig>();
        public DbSet<SysSchedule> Schedules => Set<SysSchedule>();
        public DbSet<SysOperationLog> OperationLogs => Set<SysOperationLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("sys_account");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasMaxLength(200);
                e.Property(x => x.Username).HasMaxLength(64).IsRequired();
                e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
                e.Property(x => x.Name).HasMaxLength(64).IsRequired();
                e.Property(x => x.Avatar).HasMaxLength(512);
                e.Property(x => x.Email).HasMaxLength(128);
                e.Property(x => x.Phone).HasMaxLength(32);
                e.Property(x => x.Position).HasMaxLength(100);
                e.Property(x => x.OrganizationId).HasColumnName("organization_id").HasMaxLength(24);
                e.Property(x => x.LoginType).HasColumnName("login_type");
                e.Property(x => x.Status);
                e.Property(x => x.IsProtected).HasColumnName("is_protected");
                e.Property(x => x.LastLoginAt).HasColumnName("last_login_at");
                e.Property(x => x.LastLoginIp).HasColumnName("last_login_ip").HasMaxLength(64);
                e.Property(x => x.LoginCount).HasColumnName("login_count");
                e.Property(x => x.TokenVersion).HasColumnName("token_version").HasDefaultValue(0);
                e.Property(x => x.PasswordChangedAt).HasColumnName("password_changed_at");
                e.Property(x => x.Remark).HasMaxLength(255);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.Username).IsUnique();
            });

            modelBuilder.Entity<Organization>(e =>
            {
                e.ToTable("sys_organization");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasMaxLength(24);
                e.Property(x => x.Code).HasMaxLength(64).IsRequired();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.ParentId).HasColumnName("parent_id").HasMaxLength(24);
                e.Property(x => x.Type);
                e.Property(x => x.Manager).HasMaxLength(100);
                e.Property(x => x.Phone).HasMaxLength(32);
                e.Property(x => x.Email).HasMaxLength(128);
                e.Property(x => x.SortOrder).HasColumnName("sort_order");
                e.Property(x => x.Status);
                e.Property(x => x.Description).HasMaxLength(255);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.Code).IsUnique();
                e.HasIndex(x => x.ParentId);
            });

            modelBuilder.Entity<Menu>(e =>
            {
                e.ToTable("sys_menu");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasMaxLength(24);
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                e.Property(x => x.ParentId).HasColumnName("parent_id").HasMaxLength(24);
                e.Property(x => x.Type).HasColumnName("type");
                e.Property(x => x.Path).HasMaxLength(255);
                e.Property(x => x.Component).HasMaxLength(255);
                e.Property(x => x.Icon).HasMaxLength(64);
                e.Property(x => x.Permission).HasMaxLength(128);
                e.Property(x => x.SortOrder).HasColumnName("sort_order");
                e.Property(x => x.Status);
                e.Property(x => x.IsVisible).HasColumnName("is_visible");
                e.Property(x => x.Remark).HasMaxLength(255);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.ParentId);
                e.HasIndex(x => x.Type);
            });

            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("sys_role");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasMaxLength(24);
                e.Property(x => x.Name).HasMaxLength(64).IsRequired();
                e.Property(x => x.Code).HasMaxLength(64).IsRequired();
                e.Property(x => x.Description).HasMaxLength(255);
                e.Property(x => x.Color).HasMaxLength(128);
                e.Property(x => x.DataScope).HasColumnName("data_scope");
                e.Property(x => x.IsBuiltIn).HasColumnName("is_built_in");
                e.Property(x => x.Status);
                e.Property(x => x.SortOrder).HasColumnName("sort_order");
                e.Property(x => x.Remark).HasMaxLength(255);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<RoleMenu>(e =>
            {
                e.ToTable("sys_role_menu");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.RoleId).HasColumnName("role_id").HasMaxLength(24).IsRequired();
                e.Property(x => x.MenuKey).HasColumnName("menu_key").HasMaxLength(128).IsRequired();
                e.Property(x => x.PermissionState).HasColumnName("permission_state");
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.HasIndex(x => new { x.RoleId, x.MenuKey }).IsUnique();
                e.HasIndex(x => x.MenuKey);
            });

            modelBuilder.Entity<AccountRole>(e =>
            {
                e.ToTable("sys_account_role");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.AccountId).HasColumnName("account_id").HasMaxLength(200).IsRequired();
                e.Property(x => x.RoleId).HasColumnName("role_id").HasMaxLength(24).IsRequired();
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.HasIndex(x => new { x.AccountId, x.RoleId }).IsUnique();
                e.HasIndex(x => x.RoleId);
            });

            modelBuilder.Entity<SysMessage>(e =>
            {
                e.ToTable("sys_message");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Content).IsRequired();
                e.Property(x => x.Type);
                e.Property(x => x.Status);
                e.Property(x => x.Scope);
                e.Property(x => x.Sender).HasMaxLength(100);
                e.Property(x => x.Remark).HasMaxLength(500);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.Status);
            });

            modelBuilder.Entity<MessageTarget>(e =>
            {
                e.ToTable("sys_message_target");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.MessageId).HasColumnName("message_id").IsRequired();
                e.Property(x => x.TargetType).HasColumnName("target_type");
                e.Property(x => x.TargetId).HasColumnName("target_id").HasMaxLength(200).IsRequired();
                e.HasIndex(x => x.MessageId);
                e.HasIndex(x => new { x.TargetType, x.TargetId });
            });

            modelBuilder.Entity<MessageRead>(e =>
            {
                e.ToTable("sys_message_read");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.MessageId).HasColumnName("message_id").IsRequired();
                e.Property(x => x.AccountId).HasColumnName("account_id").HasMaxLength(200).IsRequired();
                e.Property(x => x.ReadAt).HasColumnName("read_at");
                e.HasIndex(x => new { x.MessageId, x.AccountId }).IsUnique();
                e.HasIndex(x => x.AccountId);
            });

            modelBuilder.Entity<SysDictType>(e =>
            {
                e.ToTable("sys_dict_type");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Code).HasMaxLength(100).IsRequired();
                e.Property(x => x.Status);
                e.Property(x => x.SortOrder).HasColumnName("sort_order");
                e.Property(x => x.Remark).HasMaxLength(500);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<SysDictData>(e =>
            {
                e.ToTable("sys_dict_data");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.DictCode).HasColumnName("dict_code").HasMaxLength(100).IsRequired();
                e.Property(x => x.Label).HasMaxLength(100).IsRequired();
                e.Property(x => x.Value).HasMaxLength(100).IsRequired();
                e.Property(x => x.Color).HasMaxLength(100);
                e.Property(x => x.Status);
                e.Property(x => x.SortOrder).HasColumnName("sort_order");
                e.Property(x => x.Remark).HasMaxLength(500);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.DictCode);
            });

            modelBuilder.Entity<SysConfig>(e =>
            {
                e.ToTable("sys_config");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.ConfigKey).HasColumnName("config_key").HasMaxLength(128).IsRequired();
                e.Property(x => x.ConfigValue).HasColumnName("config_value").HasColumnType("text").IsRequired();
                e.Property(x => x.ConfigName).HasColumnName("config_name").HasMaxLength(128).IsRequired();
                e.Property(x => x.ConfigType).HasColumnName("config_type");
                e.Property(x => x.IsSystem).HasColumnName("is_system");
                e.Property(x => x.Remark).HasMaxLength(500);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.ConfigKey).IsUnique();
            });

            modelBuilder.Entity<SysSchedule>(e =>
            {
                e.ToTable("sys_schedule");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Description).HasColumnType("text");
                e.Property(x => x.ScheduleDate).HasColumnName("schedule_date");
                e.Property(x => x.StartTime).HasColumnName("start_time").HasMaxLength(10);
                e.Property(x => x.EndTime).HasColumnName("end_time").HasMaxLength(10);
                e.Property(x => x.Color).HasMaxLength(20);
                e.Property(x => x.Location).HasMaxLength(200);
                e.Property(x => x.IsCompleted).HasColumnName("is_completed");
                e.Property(x => x.CreatorId).HasColumnName("creator_id").HasMaxLength(255);
                e.Property(x => x.CreatorName).HasColumnName("creator_name").HasMaxLength(64);
                e.Property(x => x.Status);
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.HasIndex(x => x.ScheduleDate);
                e.HasIndex(x => x.CreatorId);
            });

            modelBuilder.Entity<SysOperationLog>(e =>
            {
                e.ToTable("sys_operation_log");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.AccountId).HasColumnName("account_id").HasMaxLength(200);
                e.Property(x => x.Username).HasMaxLength(64);
                e.Property(x => x.Name).HasMaxLength(64);
                e.Property(x => x.Module).HasMaxLength(64).IsRequired();
                e.Property(x => x.Action).HasMaxLength(32).IsRequired();
                e.Property(x => x.Method).HasMaxLength(16);
                e.Property(x => x.Path).HasMaxLength(255);
                e.Property(x => x.Detail).HasMaxLength(1000);
                e.Property(x => x.Status);
                e.Property(x => x.Ip).HasMaxLength(64);
                e.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(512);
                e.Property(x => x.DurationMs).HasColumnName("duration_ms");
                e.Property(x => x.BeforeData).HasColumnName("before_data").HasColumnType("text");
                e.Property(x => x.AfterData).HasColumnName("after_data").HasColumnType("text");
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.HasIndex(x => x.AccountId);
                e.HasIndex(x => x.Module);
                e.HasIndex(x => x.CreatedAt);
            });
        }
    }
}