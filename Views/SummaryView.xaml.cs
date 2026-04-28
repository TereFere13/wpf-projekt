using System.Windows;
using System.Windows.Controls;
using wpf_projekt.ViewModels;

namespace wpf_projekt.Views
{
    public partial class SummaryView : UserControl
    {
        public SummaryView()
        {
            InitializeComponent();
            Loaded += SummaryView_Loaded;
        }

        private void SummaryView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SummaryViewModel vm)
                vm.Load();
        }
    }
}