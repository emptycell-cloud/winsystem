using System.Collections.Concurrent;

namespace WinSystem.Api.Services
{
    /// <summary>登录防爆破：按 账号/来源IP 记录连续失败次数，达到阈值后临时锁定。</summary>
    public static class LoginGuard
    {
        public const int MaxFailures = 5;
        public static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

        private sealed record Entry(int Failures, DateTime? LockUntil);

        private static readonly ConcurrentDictionary<string, Entry> _map = new();

        /// <summary>指定键（账号或 IP）是否处于锁定状态。</summary>
        public static bool IsLocked(string key, out DateTime lockUntil)
        {
            if (_map.TryGetValue(key, out var e) && e.LockUntil is { } until && until > DateTime.Now)
            {
                lockUntil = until;
                return true;
            }
            lockUntil = default;
            return false;
        }

        /// <summary>记录一次失败；达到阈值后切换为锁定状态。</summary>
        public static void OnFailure(string key)
        {
            _map.AddOrUpdate(key,
                _ => new Entry(1, null),
                (_, e) =>
                {
                    var failures = e.Failures + 1;
                    return failures >= MaxFailures
                        ? new Entry(0, DateTime.Now.Add(LockDuration))
                        : new Entry(failures, e.LockUntil);
                });
        }

        /// <summary>登录成功，清除失败记录。</summary>
        public static void OnSuccess(string key) => _map.TryRemove(key, out _);
    }
}
