using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using wpf_projekt.models;
using wpf_projekt.Services;
using Microsoft.EntityFrameworkCore;

namespace wpf_projekt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static IServiceProvider ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "finance_manager.db")}"));

            services.AddScoped<IEventLogService, EventLogService>();

            ServiceProvider = services.BuildServiceProvider();
            var context = ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();

            // Wymusza na WPF używanie formatowania zgodnego z systemem (np. przecinek w PL)
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
    }
}
