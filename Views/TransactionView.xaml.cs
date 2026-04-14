using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text;
using Microsoft.Win32;
using wpf_projekt.Models; // Upewnij się, że masz to użycie, aby kod "widział" klasę Transaction

namespace wpf_projekt.Views
{
    public partial class TransactionsView : UserControl
    {
        public TransactionsView()
        {
            InitializeComponent();
            this.Loaded += TransactionsView_Loaded;
        }

        private void TransactionsView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow?.Transactions == null) return;

            var transactions = mainWindow.Transactions;

            // 1. ROK
            YearFilterComboBox.Items.Clear();
            YearFilterComboBox.Items.Add("Wszystkie");
            var years = transactions.Select(t => t.Date.Year).Distinct().OrderByDescending(y => y);
            foreach (var year in years) YearFilterComboBox.Items.Add(year.ToString());
            YearFilterComboBox.SelectedIndex = 0;

            // 2. MIESIĄC
            MonthFilterComboBox.Items.Clear();
            MonthFilterComboBox.Items.Add(new ComboBoxItem { Content = "Wszystkie", Tag = null });
            for (int i = 1; i <= 12; i++)
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                MonthFilterComboBox.Items.Add(new ComboBoxItem
                {
                    Content = char.ToUpper(monthName[0]) + monthName.Substring(1),
                    Tag = i
                });
            }
            MonthFilterComboBox.SelectedIndex = 0;

            // 3. KATEGORIE
            CategoryFilterComboBox.Items.Clear();
            CategoryFilterComboBox.Items.Add("Wszystkie");
            var categories = transactions.Select(t => t.TransactionType?.Name)
                                         .Where(n => n != null)
                                         .Distinct()
                                         .OrderBy(c => c);
            foreach (var c in categories) CategoryFilterComboBox.Items.Add(c);
            CategoryFilterComboBox.SelectedIndex = 0;

            // 4. RESET RESZTY
            DateSortComboBox.SelectedIndex = 0;
            TypeFilterComboBox.SelectedIndex = 0;

            Apply();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            Apply();
        }

        private void Apply()
        {
            // Zabezpieczenie przed wywołaniem Apply przed zainicjowaniem kontrolek
            if (YearFilterComboBox == null || MonthFilterComboBox == null ||
                CategoryFilterComboBox == null || TypeFilterComboBox == null) return;

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow?.Transactions == null) return;

            var data = mainWindow.Transactions.AsEnumerable();

            // FILTR ROKU
            var selectedYear = YearFilterComboBox.SelectedItem?.ToString();
            if (selectedYear != "Wszystkie" && !string.IsNullOrEmpty(selectedYear))
            {
                data = data.Where(t => t.Date.Year.ToString() == selectedYear);
            }

            // FILTR MIESIĄCA
            if (MonthFilterComboBox.SelectedItem is ComboBoxItem monthItem && monthItem.Tag != null)
            {
                int monthNumber = (int)monthItem.Tag;
                data = data.Where(t => t.Date.Month == monthNumber);
            }

            // FILTR KATEGORII
            var selectedCat = CategoryFilterComboBox.SelectedItem?.ToString();
            if (selectedCat != "Wszystkie" && !string.IsNullOrEmpty(selectedCat))
            {
                data = data.Where(t => t.TransactionType?.Name == selectedCat);
            }

            // FILTR RODZAJU
            var selectedType = (TypeFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedType == "Wydatek") data = data.Where(t => !t.IsPositive);
            else if (selectedType == "Przychód") data = data.Where(t => t.IsPositive);

            // SORTOWANIE
            var selectedSort = (DateSortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedSort == "Od najnowszej") data = data.OrderByDescending(t => t.Date);
            else if (selectedSort == "Od najstarszej") data = data.OrderBy(t => t.Date);

            TransactionsGrid.ItemsSource = data.ToList();
        }

        private void ExportToCsvButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Pobieramy aktualnie przefiltrowane dane prosto z tabeli
            var dataToExport = TransactionsGrid.ItemsSource as IEnumerable<Transaction>;

            if (dataToExport == null || !dataToExport.Any())
            {
                MessageBox.Show("Brak danych do wyeksportowania (lista jest pusta).", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 2. Okienko do wyboru miejsca zapisu
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Plik CSV (*.csv)|*.csv",
                Title = "Eksportuj transakcje",
                // Proponowana nazwa pliku z dzisiejszą datą
                FileName = $"Zestawienie_Transakcji_{DateTime.Now:yyyy_MM_dd}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    StringBuilder csv = new StringBuilder();

                    // 3. Nagłówki kolumn (dokładnie takie jak w Twoim DataGrid)
                    csv.AppendLine("Data;Kategoria;Kwota;Typ;Opis");

                    // 4. Przechodzimy przez każdą WIDOCZNĄ transakcję
                    foreach (var t in dataToExport)
                    {
                        // Formatujemy datę identycznie jak masz w UI
                        string date = t.Date.ToString("dd.MM.yyyy");

                        string category = t.TransactionType?.Name ?? "Brak";

                        // Kwota - używamy standardowego ToString, co w polskim Windowsie da przecinek (zrozumiały dla Excela)
                        string amount = t.Amount.ToString("F2");

                        // Typ transakcji - używamy Twojej właściwości TypeName
                        string type = t.TypeName ?? "";

                        // Opis - zamieniamy średniki na przecinki, by nie zepsuć struktury CSV
                        string desc = t.Description?.Replace(";", ",") ?? "";

                        // Sklejamy cały wiersz i dodajemy do pliku
                        csv.AppendLine($"{date};{category};{amount};{type};{desc}");
                    }

                    // 5. Zapis do pliku tekstowego z kodowaniem UTF8 (żeby działały polskie znaki w Excelu)
                    File.WriteAllText(saveFileDialog.FileName, csv.ToString(), Encoding.UTF8);

                    MessageBox.Show("Dane zostały pomyślnie zapisane!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas zapisu pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}