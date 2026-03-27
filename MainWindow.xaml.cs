using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using wpf_projekt.models;
using wpf_projekt.Models;

namespace wpf_projekt
{
    public partial class MainWindow : Window
    {
        // Publiczna lista - na razie przechowuje dane w pamięci. 
        // Inna osoba z zespołu użyje tej listy, by ją zapisać do JSON (MVP pkt 7)!
        public static List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Pozwala na Binding w XAML

            // Ustawienia początkowe formularza po uruchomieniu
            LoadDefaultCategories();
            TransactionDatePicker.SelectedDate = DateTime.Now; // Domyślnie dzisiejsza data
        }

        private void LoadDefaultCategories()
        {
            // Ładujemy domyślne kategorie z MVP pkt 3
            CategoryComboBox.Items.Add("Jedzenie");
            CategoryComboBox.Items.Add("Transport");
            CategoryComboBox.Items.Add("Rachunki");
            CategoryComboBox.Items.Add("Rozrywka");
            CategoryComboBox.Items.Add("Inne");

            CategoryComboBox.SelectedIndex = 0; // Wybiera pierwszą na liście z góry
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Sprawdzenie, czy wpisana kwota jest poprawną liczbą
            if (!decimal.TryParse(AmountTextBox.Text, out decimal parsedAmount))
            {
                MessageBox.Show("Wprowadź poprawną kwotę (np. 150,50).", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isIncome = IncomeRadio.IsChecked == true;

            // 3. Stworzenie nowego obiektu transakcji
            Transaction newTransaction = new Transaction
            {
                Id = Transactions.Count + 1, // Proste generowanie ID
                Amount = parsedAmount,
                Date = TransactionDatePicker.SelectedDate ?? DateTime.Now,
                Description = DescriptionTextBox.Text,
                TransactionTypeId = selectedType.Id,
                PersonalAccountId = selectedAccount.Id
            };

            // 4. Dodanie do wspólnej listy
            Transactions.Add(newTransaction);

            // 5. Powiadomienie użytkownika o sukcesie
            MessageBox.Show($"Pomyślnie dodano transakcję!\n\nTyp: {newTransaction.TypeName}\nKwota: {newTransaction.Amount} zł\nKategoria: {newTransaction.Category}\nOpis: {newTransaction.Description}",
                            "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

            // 6. Wyczyszczenie formularza dla kolejnej transakcji
            AmountTextBox.Clear();
            DescriptionTextBox.Clear();
            TransactionDatePicker.SelectedDate = DateTime.Now;
        }
    }
}