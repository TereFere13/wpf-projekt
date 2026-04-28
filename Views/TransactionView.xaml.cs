using System.Windows;
using System.Windows.Controls;
using wpf_projekt.ViewModels;

namespace wpf_projekt.Views
{
    public partial class TransactionsView : UserControl
    {
        public TransactionsView()
        {
            InitializeComponent();
            Loaded += TransactionsView_Loaded;
        }

        private void TransactionsView_Loaded(object sender, RoutedEventArgs e)
        {
            // ViewModel jest wstrzykiwany przez MainWindow przez binding,
            // ale potrzebujemy wywołać Load() po załadowaniu widoku.
            if (DataContext is TransactionsViewModel vm)
                vm.Load();
        }
    }
}