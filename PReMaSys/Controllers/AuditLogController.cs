using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PReMaSys.Data;
using PReMaSys.Models;

namespace PReMaSys.Controllers
{
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuditLogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void LogAudit(string userId, string action, string details)
        {
            var auditLogEntry = new AuditLogEntry
            {
                UserId = userId,
                Timestamp = DateTime.Now,
                Action = action,
                Details = details
            };

            _context.AuditLogs.Add(auditLogEntry);
            _context.SaveChanges();
        }

        public void LogLoginEvent(string userId, string eventType)
        {
            var auditLogEntry = new AuditLogEntry
            {
                UserId = userId,
                Timestamp = DateTime.Now,
                Action = "Login",
                Details = "User login event",
                EventType = eventType
            };

            _context.AuditLogs.Add(auditLogEntry);
            _context.SaveChanges();
        }
    }

}
