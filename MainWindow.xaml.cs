using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using wpf_projekt.models;
using wpf_projekt.Models;

namespace wpf_projekt
{
    public partial class MainWindow : Window
    {
        private AppDbContext _context = new AppDbContext();
        // Listy udostępnione dla reszty zespołu
        public static List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public static List<PersonalAccount> PersonalAccounts { get; set; } = new List<PersonalAccount>();
        public static List<SharedAccount> SharedAccounts { get; set; } = new List<SharedAccount>();

        public MainWindow()
        {
            InitializeComponent();

            // Symulacja danych początkowych (normalnie byłyby z bazy/pliku)
            SeedInitialData();

            LoadDefaultCategories();
            LoadAccountsToUI();

            TransactionDatePicker.SelectedDate = DateTime.Now;
        }

        private void SeedInitialData()
        {
            // Przykładowe konto, żeby było do czego przypisać transakcję
            PersonalAccounts.Add(new PersonalAccount { Id = 1, Balance = 1000, UserId = 1 });
        }

        private void LoadAccountsToUI()
        {
            foreach (var acc in PersonalAccounts)
                AccountComboBox.Items.Add($"Osobiste #{acc.Id}");
        }

        private void LoadDefaultCategories()
        {
            // Teraz to są obiekty TransactionType zgodnie z diagramem
            CategoryComboBox.Items.Add(new TransactionType { Id = 1, Name = "Jedzenie" });
            CategoryComboBox.Items.Add(new TransactionType { Id = 2, Name = "Transport" });
            CategoryComboBox.Items.Add(new TransactionType { Id = 3, Name = "Rachunki" });

            CategoryComboBox.DisplayMemberPath = "Name"; // Wyświetlaj tylko nazwę w UI
            CategoryComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Walidacja kwoty
            if (!decimal.TryParse(AmountTextBox.Text, out decimal parsedAmount))
            {
                MessageBox.Show("Wprowadź poprawną kwotę.", "Błąd");
                return;
            }

            // 2. Pobranie wybranego typu i konta
            var selectedType = CategoryComboBox.SelectedItem as TransactionType;
            bool isIncome = IncomeRadio.IsChecked == true;

            // 3. Stworzenie transakcji z relacjami
            Transaction newTransaction = new Transaction
            {
                Id = Transactions.Count + 1,
                Amount = parsedAmount,
                IsPositive = isIncome,
                Date = TransactionDatePicker.SelectedDate ?? DateTime.Now,
                Description = DescriptionTextBox.Text,

                // Przypisanie kluczy obcych i obiektów nawigacyjnych
                TransactionTypeId = selectedType?.Id ?? 1,
                TransactionType = selectedType,

                // Przypisujemy do konta osobistego (uproszczenie dla przykładu)
                PersonalAccountId = PersonalAccounts.FirstOrDefault()?.Id,
                PersonalAccount = PersonalAccounts.FirstOrDefault()
            };
            _context.Transactions.Add(newTransaction);
            _context.SaveChanges();
            MessageBox.Show("Zapisano w bazie SQLite!");

            // 4. Logika biznesowa: Aktualizacja salda konta!
            if (newTransaction.PersonalAccount != null)
            {
                if (isIncome)
                    newTransaction.PersonalAccount.Balance += (int)parsedAmount; // rzutowanie na int wg diagramu
                else
                    newTransaction.PersonalAccount.Balance -= (int)parsedAmount;
            }

            // 5. Zapis do listy
            Transactions.Add(newTransaction);

            MessageBox.Show($"Zapisano! Nowe saldo konta: {newTransaction.PersonalAccount?.Balance}");

            // Czyszczenie
            AmountTextBox.Clear();
            DescriptionTextBox.Clear();
        }
    }
}