using Microsoft.EntityFrameworkCore;
using WinSystem.Api.Data;

namespace WinSystem.Api.Data
{
    /// <summary>初始化数据库：仅确保库存在。演示表 products / orders 与外部表 sys_* 均不做种子写入。</summary>
    public static class DbSeeder
    {
        public static void Initialize(AppDbContext db)
        {
            db.Database.EnsureCreated();
        }
    }
}