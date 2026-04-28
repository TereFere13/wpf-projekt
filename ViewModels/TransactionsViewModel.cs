using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using wpf_projekt.Models;
using System.Windows.Input;



namespace wpf_projekt.ViewModels
{
    /// <summary>
    /// ViewModel dla zakładki Transakcje.
    /// Pobiera dane z MainViewModel (wspólna kolekcja Transactions).
    /// </summary>
    public partial class TransactionsViewModel : ObservableObject
    {
        private readonly MainViewModel _mainVm;

        // ── Kolekcje filtrów ─────────────────────────────────────────────────────
        public ObservableCollection<string> AvailableYears { get; } = new();
        public ObservableCollection<MonthItem> AvailableMonths { get; } = new();
        public ObservableCollection<string> AvailableCategories { get; } = new();

        // ── Wynik filtrowania ────────────────────────────────────────────────────
        [ObservableProperty] private List<Transaction> _filteredTransactions = new();

        // ── Filtry ───────────────────────────────────────────────────────────────
        [ObservableProperty] private string _selectedYear = "Wszystkie";
        [ObservableProperty] private MonthItem? _selectedMonth;
        [ObservableProperty] private string _selectedCategory = "Wszystkie";
        [ObservableProperty] private string _selectedType = "Wszystkie";   // Wszystkie / Wydatek / Przychód
        [ObservableProperty] private string _selectedSort = "Od najnowszej"; // Od najnowszej / Od najstarszej

        public TransactionsViewModel(MainViewModel mainVm)
        {
            _mainVm = mainVm;
            _mainVm.Transactions.CollectionChanged += (_, _) => Refresh();
        }

        // ── Wywołane przez widok po załadowaniu ──────────────────────────────────
        public void Load()
        {
            BuildFilters();
            Apply();
        }

        private void BuildFilters()
        {
            var transactions = _mainVm.Transactions;

            // LATA
            AvailableYears.Clear();
            AvailableYears.Add("Wszystkie");
            foreach (var year in transactions.Select(t => t.Date.Year).Distinct().OrderByDescending(y => y))
                AvailableYears.Add(year.ToString());
            SelectedYear = "Wszystkie";

            // MIESIĄCE
            AvailableMonths.Clear();
            AvailableMonths.Add(new MonthItem(null, "Wszystkie"));
            for (int i = 1; i <= 12; i++)
            {
                var name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                AvailableMonths.Add(new MonthItem(i, char.ToUpper(name[0]) + name[1..]));
            }
            SelectedMonth = AvailableMonths[0];

            // KATEGORIE
            AvailableCategories.Clear();
            AvailableCategories.Add("Wszystkie");
            foreach (var cat in transactions
                .Select(t => t.TransactionType?.Name)
                .Where(n => n != null)
                .Distinct()
                .OrderBy(c => c)!)
                AvailableCategories.Add(cat!);
            SelectedCategory = "Wszystkie";
        }

        // Wywoływane przez widok przy zmianie filtra (przez binding)
        partial void OnSelectedYearChanged(string value) => Apply();
        partial void OnSelectedMonthChanged(MonthItem? value) => Apply();
        partial void OnSelectedCategoryChanged(string value) => Apply();
        partial void OnSelectedTypeChanged(string value) => Apply();
        partial void OnSelectedSortChanged(string value) => Apply();

        private void Apply()
        {
            var data = _mainVm.Transactions.AsEnumerable();

            if (SelectedYear != "Wszystkie" && !string.IsNullOrEmpty(SelectedYear))
                data = data.Where(t => t.Date.Year.ToString() == SelectedYear);

            if (SelectedMonth?.Number != null)
                data = data.Where(t => t.Date.Month == SelectedMonth.Number);

            if (SelectedCategory != "Wszystkie" && !string.IsNullOrEmpty(SelectedCategory))
                data = data.Where(t => t.TransactionType?.Name == SelectedCategory);

            if (SelectedType == "Wydatek") data = data.Where(t => !t.IsPositive);
            else if (SelectedType == "Przychód") data = data.Where(t => t.IsPositive);

            data = SelectedSort == "Od najstarszej"
                ? data.OrderBy(t => t.Date)
                : data.OrderByDescending(t => t.Date);

            FilteredTransactions = data.ToList();
        }

        private void Refresh()
        {
            BuildFilters();
            Apply();
        }

        // ── Eksport CSV ──────────────────────────────────────────────────────────
        [RelayCommand]
        private void ExportToCsv()
        {
            if (!FilteredTransactions.Any())
            {
                MessageBox.Show("Brak danych do wyeksportowania.", "Informacja",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "Plik CSV (*.csv)|*.csv",
                Title = "Eksportuj transakcje",
                FileName = $"Zestawienie_Transakcji_{DateTime.Now:yyyy_MM_dd}.csv"
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                var csv = new StringBuilder();
                csv.AppendLine("Data;Kategoria;Kwota;Typ;Opis");

                foreach (var t in FilteredTransactions)
                {
                    csv.AppendLine(
                        $"{t.Date:dd.MM.yyyy};" +
                        $"{t.TransactionType?.Name ?? "Brak"};" +
                        $"{t.Amount:F2};" +
                        $"{t.TypeName};" +
                        $"{t.Description?.Replace(";", ",") ?? ""}");
                }

                File.WriteAllText(dlg.FileName, csv.ToString(), Encoding.UTF8);
                MessageBox.Show("Dane zostały pomyślnie zapisane!", "Sukces",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu pliku: {ex.Message}", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public List<Dictionary<string, string>> TemporaryImportedRecords { get; private set; } = new();

        [RelayCommand]
        private void ImportCsv()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Pliki CSV (*.csv)|*.csv",
                Title = "Wybierz plik CSV z transakcjami"
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                // 1. Zabezpieczenie przed wczytywaniem skrótów i złych plików
                if (!dlg.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Wybrano nieprawidłowy format. Proszę wybrać plik z rozszerzeniem .csv!", "Błąd pliku",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string[] lines = File.ReadAllLines(dlg.FileName);

                if (lines.Length < 2)
                {
                    MessageBox.Show("Plik jest pusty lub brakuje w nim wierszy z danymi.", "Błąd formatu",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                char delimiter = lines[0].Contains(';') ? ';' : ',';
                string[] headers = lines[0].Split(delimiter);

                // 2. Walidacja nagłówków: Prawdziwy CSV z banku będzie miał chociaż kilka konkretnych kolumn
                if (headers.Length < 2 || string.IsNullOrWhiteSpace(headers[0]))
                {
                    MessageBox.Show("Plik nie wygląda jak poprawny plik z transakcjami (brak odpowiednich kolumn).", "Błąd struktury",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var parsedRecords = new List<Dictionary<string, string>>();

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    string[] values = lines[i].Split(delimiter);

                    // 3. Zabezpieczenie: Ignorujemy wiersze, które ewidentnie są uszkodzone (mają drastycznie mniej kolumn niż nagłówek)
                    if (values.Length < headers.Length - 1) continue;

                    var record = new Dictionary<string, string>();

                    for (int j = 0; j < headers.Length; j++)
                    {
                        // Używamy Trim() aby usunąć ukryte spacje
                        string key = headers[j].Trim();
                        if (string.IsNullOrWhiteSpace(key)) continue;

                        record[key] = j < values.Length ? values[j].Trim() : "";
                    }

                    if (record.Count > 0)
                        parsedRecords.Add(record);
                }

                if (parsedRecords.Count == 0)
                {
                    MessageBox.Show("Nie znaleziono żadnych poprawnych danych do zaimportowania w tym pliku.", "Błąd odczytu",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                TemporaryImportedRecords = parsedRecords;

                // 4. DOWÓD DZIAŁANIA: Podgląd pierwszego wczytanego wiersza
                var firstRow = parsedRecords.First();
                string preview = string.Join("\n", firstRow.Select(kvp => $"- {kvp.Key}: {kvp.Value}"));

                MessageBox.Show($"Pomyślnie wczytano i sparsowano {TemporaryImportedRecords.Count} transakcji!\n\n" +
                                $"Oto podgląd pierwszego wczytanego wiersza:\n\n{preview}\n\n" +
                                $"Dane są w pamięci, gotowe do zapisania do systemu (Zadanie PRO-23, bo ja za kogos robic nie bede ).",
                                "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas czytania pliku:\n{ex.Message}", "Błąd krytyczny",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
    /// <summary>Pomocniczy rekord reprezentujący miesiąc w filtrze.</summary>
    public record MonthItem(int? Number, string Label)
    {
        public override string ToString() => Label;
    }

}