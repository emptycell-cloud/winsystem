using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinSystem.Api.Data;

namespace WinSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<object>> Get()
        {
            var today = DateTime.Today;

            var users = await _db.Users.ToListAsync();
            var roles = await _db.Roles.ToListAsync();
            var organizations = await _db.Organizations.ToListAsync();

            return Ok(new
            {
                stats = new
                {
                    userCount = users.Count,
                    roleCount = roles.Count,
                    orgCount = organizations.Count,
                    newUsers30d = users.Count(u => u.CreatedAt >= today.AddDays(-30))
                }
            });
        }
    }
}