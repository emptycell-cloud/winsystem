using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinSystem.Api.Data;
using WinSystem.Api.Models;
using WinSystem.Api.Services;

namespace WinSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MessagesController(AppDbContext db) => _db = db;

        /// <summary>当前用户是否为管理员（可查看全部消息与管理消息）。</summary>
        private bool IsAdmin => User.IsInRole("super_admin") || User.IsInRole("admin");

        /// <summary>消息列表（全量返回，仅管理员可见；普通用户请用 /mine）。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SysMessage>>> GetAll()
        {
            var list = await _db.Messages
                .OrderByDescending(m => m.CreatedAt)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
            return Ok(list);
        }

        /// <summary>消息详情（管理员返回完整含发布范围目标；普通用户仅可查看对其可见的消息，且不返回目标列表）。</summary>
        [HttpGet("{id:long}")]
        public async Task<ActionResult<object>> Get(long id)
        {
            var msg = await _db.Messages.FindAsync(id);
            if (msg == null) return NotFound();

            var userId = CurrentUserId();
            if (!IsAdmin)
            {
                if (string.IsNullOrEmpty(userId) || (await VisibleMessagesAsync(userId)).All(m => m.Id != id))
                    return NotFound();
                return Ok(new
                {
                    msg.Id, msg.Title, msg.Content, msg.Type, msg.Status, msg.Scope,
                    msg.Sender, msg.Remark, msg.CreatedAt, msg.UpdatedAt,
                    TypeText = msg.TypeText, StatusText = msg.StatusText, ScopeText = msg.ScopeText
                });
            }

            var targets = await LoadTargetsAsync(id);
            return Ok(new
            {
                msg.Id, msg.Title, msg.Content, msg.Type, msg.Status, msg.Scope,
                msg.Sender, msg.Remark, msg.CreatedAt, msg.UpdatedAt,
                TypeText = msg.TypeText, StatusText = msg.StatusText, ScopeText = msg.ScopeText,
                Targets = targets
            });
        }

        public record TargetInput(int? TargetType, string? TargetId);
        public record MessageInput(string? Title, string? Content, int? Type, int? Status, int? Scope,
            string? Remark, List<TargetInput>? Targets);

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<SysMessage>> Create(MessageInput input)
        {
            var (title, content, scope) = (input.Title?.Trim(), input.Content?.Trim(), input.Scope ?? 1);
            if (string.IsNullOrWhiteSpace(title)) return BadRequest("消息标题不能为空。");
            if (string.IsNullOrWhiteSpace(content)) return BadRequest("消息内容不能为空。");
            if (scope is < 1 or > 4) return BadRequest("发布范围不合法。");
            if (scope > 1 && (input.Targets == null || input.Targets.Count == 0))
                return BadRequest("请选择发布范围目标（角色/部门/用户）。");

            var msg = new SysMessage
            {
                Title = title,
                Content = content,
                Type = input.Type ?? 1,
                Status = input.Status ?? 1,
                Scope = scope,
                Sender = await CurrentSenderNameAsync(),
                Remark = input.Remark?.Trim(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Messages.Add(msg);
            var sw = Stopwatch.StartNew();
            await _db.SaveChangesAsync();

            SaveTargets(msg.Id, scope, input.Targets);
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "消息通知", "新增", $"新增消息:{msg.Title}", sw.ElapsedMilliseconds,
                afterJson: OperationLogHelper.Snapshot(msg));
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = msg.Id }, msg);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:long}")]
        public async Task<ActionResult> Update(long id, MessageInput input)
        {
            var msg = await _db.Messages.FindAsync(id);
            if (msg == null) return NotFound();

            var beforeJson = OperationLogHelper.Snapshot(msg);
            var sw = Stopwatch.StartNew();
            if (input.Title != null)
            {
                if (string.IsNullOrWhiteSpace(input.Title)) return BadRequest("消息标题不能为空。");
                msg.Title = input.Title.Trim();
            }
            if (input.Content != null)
            {
                if (string.IsNullOrWhiteSpace(input.Content)) return BadRequest("消息内容不能为空。");
                msg.Content = input.Content.Trim();
            }
            var scope = input.Scope ?? msg.Scope;
            if (scope is < 1 or > 4) return BadRequest("发布范围不合法。");
            msg.Scope = scope;
            if (input.Type.HasValue) msg.Type = input.Type.Value;
            if (input.Status.HasValue) msg.Status = input.Status.Value;
            if (input.Remark != null) msg.Remark = input.Remark.Trim();
            msg.UpdatedAt = DateTime.Now;

            if (input.Targets != null)
            {
                if (scope > 1 && input.Targets.Count == 0)
                    return BadRequest("请选择发布范围目标（角色/部门/用户）。");
                _db.MessageTargets.RemoveRange(_db.MessageTargets.Where(t => t.MessageId == id));
                SaveTargets(id, scope, input.Targets);
            }

            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "消息通知", "编辑", $"编辑消息:{msg.Title}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(msg));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            var msg = await _db.Messages.FindAsync(id);
            if (msg == null) return NotFound();
            var beforeJson = OperationLogHelper.Snapshot(msg);
            var sw = Stopwatch.StartNew();
            _db.Messages.Remove(msg);
            _db.MessageTargets.RemoveRange(_db.MessageTargets.Where(t => t.MessageId == id));
            _db.MessageReads.RemoveRange(_db.MessageReads.Where(r => r.MessageId == id));
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "消息通知", "删除", $"删除消息:{msg.Title}", sw.ElapsedMilliseconds,
                beforeJson);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>发布消息（草稿 → 已发布）。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("{id:long}/publish")]
        public async Task<ActionResult> Publish(long id)
        {
            var msg = await _db.Messages.FindAsync(id);
            if (msg == null) return NotFound();
            var beforeJson = OperationLogHelper.Snapshot(msg);
            var sw = Stopwatch.StartNew();
            msg.Status = 1;
            msg.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "消息通知", "发布", $"发布消息:{msg.Title}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(msg));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>我的消息：当前登录用户可见的已发布消息列表（支持分页/标题关键字/类型筛选，含已读状态）。</summary>
        [HttpGet("mine")]
        public async Task<ActionResult<object>> Mine([FromQuery] int page = 1, [FromQuery] int pageSize = 15,
            [FromQuery] string? keyword = null, [FromQuery] int? type = null)
        {
            var userId = CurrentUserId();
            if (string.IsNullOrEmpty(userId) || await _db.Users.FindAsync(userId) == null) return Unauthorized();

            var visible = await VisibleMessagesAsync(userId);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim();
                visible = visible.Where(m => m.Title.Contains(k) || (m.Content ?? "").Contains(k)).ToList();
            }
            if (type.HasValue && type.Value > 0)
                visible = visible.Where(m => m.Type == type.Value).ToList();

            var total = visible.Count;
            var paged = visible.Skip((Math.Max(page, 1) - 1) * Math.Max(pageSize, 1)).Take(Math.Max(pageSize, 1)).ToList();
            var readIds = (await _db.MessageReads
                    .Where(r => r.AccountId == userId && paged.Select(p => p.Id).Contains(r.MessageId))
                    .Select(r => r.MessageId).ToListAsync()).ToHashSet();

            var items = paged.Select(m => new
            {
                m.Id, m.Title, m.Type, m.Status, m.Scope, m.Sender, m.CreatedAt, m.UpdatedAt,
                TypeText = m.TypeText, ScopeText = m.ScopeText,
                IsRead = readIds.Contains(m.Id)
            });
            return Ok(new { Total = total, Items = items });
        }

        /// <summary>我的未读消息数量。</summary>
        [HttpGet("mine/unread-count")]
        public async Task<ActionResult<object>> UnreadCount()
        {
            var userId = CurrentUserId();
            if (string.IsNullOrEmpty(userId) || await _db.Users.FindAsync(userId) == null) return Unauthorized();

            var visible = await VisibleMessagesAsync(userId);
            var readSet = (await _db.MessageReads.Where(r => r.AccountId == userId)
                .Select(r => r.MessageId).ToListAsync()).ToHashSet();
            var count = visible.Count(m => !readSet.Contains(m.Id));
            return Ok(new { Count = count });
        }

        /// <summary>标记我的消息为已读。</summary>
        [HttpPost("mine/{id:long}/read")]
        public async Task<ActionResult> MarkRead(long id)
        {
            var userId = CurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var exists = await _db.MessageReads.AnyAsync(r => r.MessageId == id && r.AccountId == userId);
            if (!exists)
            {
                _db.MessageReads.Add(new MessageRead { MessageId = id, AccountId = userId, ReadAt = DateTime.Now });
                await _db.SaveChangesAsync();
            }
            return NoContent();
        }

        /// <summary>计算当前用户可见的已发布消息（1全部 2角色 3部门及以下 4指定用户）。</summary>
        private async Task<List<SysMessage>> VisibleMessagesAsync(string userId)
        {
            var me = await _db.Users.FindAsync(userId);
            if (me == null) return new List<SysMessage>();

            var roleSet = (await _db.AccountRoles.Where(a => a.AccountId == userId)
                .Select(a => a.RoleId).ToListAsync()).ToHashSet();

            // 用户所属组织向上到根的组织路径（选部门 = 该部门及其子部门下的人可见）
            var orgPathSet = new HashSet<string>();
            var orgs = await _db.Organizations.ToDictionaryAsync(o => o.Id);
            var orgId = me.OrganizationId;
            while (!string.IsNullOrEmpty(orgId) && orgs.TryGetValue(orgId, out var org))
            {
                orgPathSet.Add(orgId);
                orgId = org.ParentId;
            }

            var targets = await _db.MessageTargets.ToListAsync();
            var targetGroup = targets.GroupBy(t => t.MessageId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var msgs = await _db.Messages.Where(m => m.Status == 1)
                .OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id).ToListAsync();

            return msgs.Where(m =>
            {
                if (m.Scope <= 1) return true;
                if (!targetGroup.TryGetValue(m.Id, out var t) || t.Count == 0) return false;
                return m.Scope switch
                {
                    2 => t.Any(x => x.TargetType == 2 && roleSet.Contains(x.TargetId)),
                    3 => t.Any(x => x.TargetType == 3 && orgPathSet.Contains(x.TargetId)),
                    4 => t.Any(x => x.TargetType == 4 && x.TargetId == userId),
                    _ => false
                };
            }).ToList();
        }

        /// <summary>当前登录用户 ID（JWT sub）。</summary>
        private string? CurrentUserId()
            => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst("sub")?.Value;

        /// <summary>当前登录用户姓名：用 JWT 中用户 ID（sub）查库取姓名，回退到用户名。</summary>
        private async Task<string> CurrentSenderNameAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var u = await _db.Users.FindAsync(userId);
                if (u != null) return u.Name;
            }
            return User.Identity?.Name ?? "";
        }

        /// <summary>保存发布范围目标（先删后插），scope=1 全部用户时清空目标。</summary>
        private void SaveTargets(long messageId, int scope, List<TargetInput>? targets)
        {
            if (scope <= 1 || targets == null) return;
            foreach (var t in targets.Where(x => x.TargetType is >= 2 and <= 4 && !string.IsNullOrWhiteSpace(x.TargetId)).DistinctBy(x => (x.TargetType, x.TargetId)))
            {
                _db.MessageTargets.Add(new MessageTarget
                {
                    MessageId = messageId,
                    TargetType = t.TargetType!.Value,
                    TargetId = t.TargetId!.Trim()
                });
            }
        }

        /// <summary>解析发布范围目标名称：2=角色 3=部门 4=用户。</summary>
        private async Task<List<object>> LoadTargetsAsync(long messageId)
        {
            var targets = await _db.MessageTargets
                .Where(t => t.MessageId == messageId)
                .OrderBy(t => t.TargetType)
                .ThenBy(t => t.TargetId)
                .ToListAsync();

            var result = new List<object>();
            foreach (var t in targets)
            {
                string? name = t.TargetType switch
                {
                    2 => (await _db.Roles.FindAsync(t.TargetId))?.Name,
                    3 => (await _db.Organizations.FindAsync(t.TargetId))?.Name,
                    4 => (await _db.Users.FindAsync(t.TargetId))?.Name,
                    _ => null
                };
                result.Add(new { t.Id, t.MessageId, t.TargetType, t.TargetId, TargetName = name ?? t.TargetId });
            }
            return result;
        }
    }
}
