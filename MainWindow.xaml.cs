using System.Windows;
using System.Windows.Input;
using wpf_projekt.models;
using wpf_projekt.Repositories;
using wpf_projekt.ViewModels;

namespace wpf_projekt
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // Skład zależności (można zastąpić DI-containerem)
            var context = new AppDbContext();
            var accountRepo = new AccountRepository(context);
            var transactionRepo = new TransactionRepository(context);
            var categoryRepo = new CategoryRepository(context);

            _viewModel = new MainViewModel(context, accountRepo, transactionRepo, categoryRepo);
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