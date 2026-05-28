using System.Collections.Generic;
using System.Threading.Tasks;
using wpf_projekt.Models;
using wpf_projekt.models;

namespace wpf_projekt.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllWithDetailsAsync();
        Task AddAsync(Transaction transaction);
        Task AddRangeAsync(IEnumerable<Transaction> transactions);
    }
}