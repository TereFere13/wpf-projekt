using System.Windows;
using wpf_projekt.Services;
using wpf_projekt.Views;

namespace wpf_projekt
{
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // Wymusza na WPF używanie formatowania zgodnego z systemem (np. przecinek w PL)
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));

            await DbInitializer.InitializeAsync();

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
