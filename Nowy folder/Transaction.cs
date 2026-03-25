using System;

namespace wpf_projekt.Models
{
    public class Transaction
    {
        // Główne pola z diagramu UML
        public int Id { get; set; }
        public decimal Amount { get; set; } // Kwota (najlepiej używać decimal do finansów)
        public bool IsPositive { get; set; } // true = przychód, false = wydatek

        // Pola wymagane z opisu MVP (pkt 1)
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        // Właściwość pomocnicza, żeby łatwo było wyświetlić typ w tabeli
        public string TypeName => IsPositive ? "Przychód" : "Wydatek";
    }
}