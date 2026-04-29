# Financial Management — WPF Application

A modern desktop application for managing personal and shared finances, built with **WPF** (.NET 8). The application enables transaction tracking, account management, data import from CSV files, and financial summary reviews.

## Key Features

### Transaction Management
- Adding new transactions (expenses and income)
- Assigning transactions to categories
- Support for multiple account types (personal and shared)
- Transaction history view with filters
- Data export to CSV format
- **Import transactions from CSV files** with advanced column mapping

### Account Management
- Creating new accounts (personal or shared type)
- Balance tracking for each account
- Transfers between accounts

### Filters and Search
- Filtering by year, month, and category
- Filtering by transaction type (expense/income)
- Chronological sorting
- Dynamic list of available categories

### CSV Import/Export
- **Intelligent column mapping** — automatic header detection
- Saved mapping profiles for different banks
- Data validation before import
- Transaction deduplication handling
- Detailed import error reports
- Export filtered transactions to CSV

### Transaction Categories
- Predefined categories
- Ability to create custom categories
- Automatic "Import" category for imported transactions

### Financial Summary
- Overview of balances across all accounts
- Expense and income statistics
- Analysis by category

## Technology and Architecture

### Technology Stack
- **Language:** C# 12
- **Framework:** .NET 8
- **UI:** WPF (Windows Presentation Foundation)
- **ORM:** Entity Framework Core 8
- **Database:** SQLite
- **Architecture:** MVVM (Model-View-ViewModel)
- **Reactive:** CommunityToolkit.MVVM 8.4.2


## System Requirements

- **Operating System:** Windows 7+
- **.NET Runtime:** .NET 8 Desktop Runtime
- **RAM:** Minimum 256 MB
- **Disk Space:** ~50 MB

## Installation

### Required Tools
- Visual Studio 2022 or newer (with Desktop Development with C# workload)
- .NET 8 SDK

### Installation Steps

1. **Clone the repository**
2. **Restore NuGet packages**
3. **Initialize the database**
4. **Compile the project**
5. **Run the application**

## Usage

### Adding Transactions

1. Go to the **"Add"** tab
2. Enter the transaction amount
3. Select the target account
4. Set the transaction date
5. Add a description (optional)
6. Select a category
7. Specify the type (Expense/Income)
8. Click **"Add transaction"**

### Transfers Between Accounts

1. In the **"Add"** tab, go to the "Transfer between accounts" section
2. Select the source account (from which)
3. Select the target account (to which)
4. Enter the transfer amount
5. Add a transfer description (optional)
6. Click **"Execute transfer"**

### Import Transactions from CSV

1. Go to the **"Transactions"** tab
2. Click the **"Import from CSV"** button
3. Select a CSV file with transactions
4. In the mapping window:
   - Assign columns from the file to application fields
   - Set the date format
   - Select the target account
   - Select a category
   - You can save the mapping profile for reuse
5. Click **"Import transactions →"**

### Filtering Transactions

1. Go to the **"Transactions"** tab
2. Use the available filters:
   - **Year** — filter by year
   - **Month** — filter by month
   - **Category** — filter by transaction category
   - **Type** — display only expenses or income
   - **Date** — sort chronologically

### Export Transactions

1. In the **"Transactions"** tab, configure filters
2. Click **"Export to CSV"**
3. Select the file location and name
4. The file will be saved with the current filters

## CSV Import File Format

The CSV file should contain headers in the first row. Supported columns:

| Column | Alias | Description |
|--------|-------|-------------|
| Date | date, Transaction Date, Operation Date | Transaction date (configurable format) |
| Amount | amount, Value | Transaction amount (decimal number) |
| Description | description, Title | Transaction description |
| Type | type, Transaction Type | Specifies whether it's income or expense |


## CSV Mapping

The application supports:
- **Auto-detection of columns** — automatically recognizes popular names
- **Mapping profiles** — save configuration for a specific bank
- **Advanced validation** — verification of date and amount format
- **Deduplication** — automatic skipping of duplicate transactions
- **Error reporting** — detailed information about rows with errors

## Resources

- [Microsoft Learn — WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Entity Framework Core — Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [MVVM Community Toolkit](https://github.com/CommunityToolkit/dotnet)
- [C# 12 — New Features](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12)