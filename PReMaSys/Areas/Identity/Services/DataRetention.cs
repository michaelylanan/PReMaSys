using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PReMaSys.Data;

namespace PReMaSys.Areas.Identity.Services
{
    public class DataRetention:BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly int _rententionTime;

        public DataRetention(IServiceProvider serviceProvider, int rententionTime)
        {
            _serviceProvider = serviceProvider;
            _rententionTime = rententionTime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime retentionDate = DateTime.Now.AddYears(-_rententionTime);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
               

                    var archiveAdmins = await userManager.Users
                        .Where(u => u.IsArchived != null && u.IsArchived < retentionDate)
                        .ToListAsync();

                    foreach (var admin in archiveAdmins)
                    {
                        await userManager.DeleteAsync(admin);
                    }

                    await Task.Delay(TimeSpan.FromDays(365 * _rententionTime), stoppingToken);


                }
               
            }
        }

       
    }
}
