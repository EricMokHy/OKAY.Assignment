using OKAY.Assignment.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OKAY.Assignment.MVC.Data;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using OKAY.Assignment.MVC.Entities;
using Microsoft.EntityFrameworkCore;

namespace OKAY.Assignment.MVC.Services
{
    public class TransactionReporsitory : RepositoryBase, ITransactionReporsitory
    {
        private readonly IPropertyRepository property;

        public TransactionReporsitory(ApplicationDbContext context, IHttpContextAccessor accessor, IPropertyRepository propertyRepository) : base(context, accessor) 
        {
            property = propertyRepository;
        }

        internal Expression<Func<Transaction, TransactionViewModel>> viewModelMapper = x => new TransactionViewModel
        {
            id = x.id,
            propertyId = x.propertyId,
            propertyName = x.Property.name,
            TransactionDate = x.TransactionDate,
            userId = x.userId,
            userName = x.User.UserName
        };

        private IQueryable<Transaction> basicQuery
        {
            get
            {
                return _context.Transactions.Include(x => x.User).Include(x => x.Property).Where(x => x.userId == userId || isAdminUser);
            }
        }

        public async Task<TransactionViewModel> CreateAsync(int propertyId)
        {
            var p = await property.FindByIdAsync(propertyId);
            if (p == null) throw new KeyNotFoundException("Property not found or you don't have permission");

            Transaction item = new Transaction
            {
                userId = userId,
                propertyId = propertyId,
                TransactionDate = DateTime.Now
            };

            _context.Transactions.Add(item);
            await _context.SaveChangesAsync();
            return await FindByIdAsync(propertyId);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await basicQuery.Where(x => x.id == id).FirstOrDefaultAsync();
            if (item == null) throw new KeyNotFoundException("Transaction not found or you don't have permission");

            _context.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedBase<TransactionViewModel>> FindAsync(string keyword, int pageIndex, int pageSize, string order, string direction)
        {
            string[] supportedOrder = new string[] { "propertyname", "username", "transactiondate" };
            var o = order.ToLower();
            if (!supportedOrder.Contains(o)) o = supportedOrder[0];

            var d = direction.ToLower();
            bool isAsc = d != "desc";

            var q = string.IsNullOrEmpty(keyword) ? basicQuery : basicQuery.Where(x => x.Property.name.ToLower().IndexOf(keyword.ToLower()) >= 0 ||
                x.User.UserName.ToLower().IndexOf(keyword.ToLower()) >= 0);

            switch (o)
            {
                case "username":
                    q = isAsc ? q.OrderBy(x => x.User.UserName) : q.OrderByDescending(x => x.User.UserName);
                    break;
                case "transactiondate":
                    q = isAsc ? q.OrderBy(x => x.TransactionDate) : q.OrderByDescending(x => x.TransactionDate);
                    break;
                default:
                    q = isAsc ? q.OrderBy(x => x.Property.name) : q.OrderByDescending(x => x.Property.name);
                    break;
            }

            var mapped = q.Select(viewModelMapper);
            return await PaginatedBase<TransactionViewModel>.CreateAsync(mapped, pageIndex, pageSize, order, direction);
        }

        public async Task<TransactionViewModel> FindByIdAsync(int id)
        {
            return await basicQuery.Where(x => x.id == id).Select(viewModelMapper).FirstOrDefaultAsync();
        }

        public async Task<TransactionViewModel> UpdateAsync(int id, int propertyId, Guid userId, DateTime transactionDate)
        {
            var t = await FindByIdAsync(id);
            if (t == null) throw new KeyNotFoundException("Transaction not found or you don't have permission");

            var p = await property.FindByIdAsync(propertyId);
            if (p == null) throw new KeyNotFoundException("Property not found or you don't have permission");

            var u = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (u == null) throw new KeyNotFoundException("User not found");

            if (!isAdminUser && (u.Id != base.userId || userId != base.userId)) throw new ArgumentException("Only administrator is allowed for this operation");

            t.propertyId = propertyId;
            t.userId = userId;
            t.TransactionDate = transactionDate;

            await _context.SaveChangesAsync();
            return await FindByIdAsync(id);
        }
    }
}
