using BLL.BusinessInterfaces;
using BLL.BusinessService;
using BLL.Repositories;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Contexts;
using System.Windows;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                               //(used if a resource is not found in the page,
                                               // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                               //(used if a resource is not found in the page,
                                               // app, or any theme specific resource dictionaries)
)]

namespace WPF
{
    public partial class App : Application
    {
        public IConfiguration Configuration { get; }
        public IServiceProvider ServiceProvider { get; set; }

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();

            var services = new ServiceCollection();
            ConfigServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigServices(IServiceCollection services)
        {
            // Register Services
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IEventService, EventService>();
            services.AddSingleton<IEventParticipantService, EventParticipantService>();
            services.AddSingleton<IClubService, ClubService>();
            services.AddSingleton<ITeamService, TeamService>();

            // Register Repositories
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IEventRepository, EventRepository>();
            services.AddSingleton<IEventParticipantRepository, EventParticipantRepository>();
            services.AddSingleton<IClubRepository, ClubRepository>();
            services.AddSingleton<ITeamRepository, TeamRepository>();

            // Register DbContext
            services.AddDbContext<ClubManagementContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")));
        }
    }
}
