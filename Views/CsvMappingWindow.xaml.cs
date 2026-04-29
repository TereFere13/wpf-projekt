using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using wpf_projekt.ViewModels;

namespace wpf_projekt.Views
{
    public partial class CsvMappingWindow : Window
    {
        public CsvMappingWindow(CsvMappingViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.ImportCompleted += count =>
            {
                DialogResult = true;
                Close();
            };

            vm.Cancelled += () =>
            {
                DialogResult = false;
                Close();
            };
        }
    }
}
