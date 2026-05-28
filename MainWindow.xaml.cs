using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using wpf_projekt.models;
using wpf_projekt.Repositories;
using wpf_projekt.Services;
using wpf_projekt.ViewModels;


namespace wpf_projekt
{  
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {

            InitializeComponent();

            // Pobierz gotowy kontekst z serwisu (zamiast tworzyć new AppDbContext())
            var context = App.ServiceProvider.GetRequiredService<AppDbContext>();

            // Teraz inicjalizuj repozytoria
            var eventLogService = new EventLogService(context);
            var accountRepo = new AccountRepository(context);
            var transactionRepo = new TransactionRepository(context);
            var categoryRepo = new CategoryRepository(context);

            _viewModel = new MainViewModel(
                context, accountRepo, transactionRepo, categoryRepo, eventLogService);

            DataContext = _viewModel;
            Loaded += async (_, _) => await _viewModel.InitializeAsync();
        }

        // Jedyna pozostała metoda w code-behind – walidacja inputu klawiszowego
        // Nie zawiera logiki biznesowej, więc może zostać tutaj.
        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0) && e.Text != "," && e.Text != ".";
        }
    }

}