using BLL.BusinessInterfaces;
using BLL.BusinessService;
using BLL.Interfaces;
using BLL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Contexts;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using WPF;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
            //Register Services here
            services.AddSingleton<IAccountService, AccountService>();
           


            //Register Repository here
            services.AddSingleton<IAccountRepository, AccountRepository>();
        

            //Register Dbcontext
            services.AddDbContext<ClubManagementContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")));



        }
    }

}
