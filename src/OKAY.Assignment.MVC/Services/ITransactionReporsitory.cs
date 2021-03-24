using OKAY.Assignment.MVC.Models;
using System;
using System.Threading.Tasks;

namespace OKAY.Assignment.MVC.Services
{
    public interface ITransactionReporsitory
    {
        Task<TransactionViewModel> CreateAsync(int propertyId);
        Task<TransactionViewModel> UpdateAsync(int id, int propertyId, Guid userId, DateTime transactionDate);
        Task<PaginatedBase<TransactionViewModel>> FindAsync(string keyword, int pageIndex, int pageSize, string order, string direction);
        Task DeleteAsync(int id);
        Task<TransactionViewModel> FindByIdAsync(int id);
    }
}
