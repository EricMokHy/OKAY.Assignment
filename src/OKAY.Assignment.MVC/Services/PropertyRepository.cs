using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OKAY.Assignment.MVC.Data;
using OKAY.Assignment.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OKAY.Assignment.MVC.Services
{
    public class PropertyRepository : RepositoryBase, IPropertyRepository
    {
        public PropertyRepository(ApplicationDbContext context, IHttpContextAccessor accessor) : base(context, accessor) { }

        internal Expression<Func<Property, PropertyViewModel>> viewModelMapper = (x) => new PropertyViewModel()
        {
            id = x.id,
            bedroom = x.bedroom,
            createdDate = x.createdDate,
            isAvailable = x.isAvailable,
            leasePrice = x.leasePrice,
            name = x.name,
            owner = x.User.UserName,
            updatedDate = x.updatedDate
        };

        private IQueryable<Property> basicQuery
        {
            get
            {
                return _context.Properties.Include(x => x.User).Where(x => x.userId == userId || isAdminUser);
            }
        }

        public async Task<PropertyViewModel> CreateAsync(string name, int bedroom, bool isAvailiable, decimal leasePrice)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Property Name is required");
            if (name.Length > 200) throw new ArgumentException("The length of Propoerty Name is exceed");

            Property item = new Property()
            {
                name = name,
                bedroom = bedroom,
                isAvailable = isAvailiable,
                leasePrice = leasePrice,
                userId = userId,
                createdDate = DateTime.Now,
                updatedDate = DateTime.Now
            };
            _context.Properties.Add(item);
            await _context.SaveChangesAsync();
            return await FindByIdAsync(item.id);
        }

        public async Task DeleteAsync(int id)
        {
            var p = basicQuery.Where(x => x.id == id).FirstOrDefault();
            if (p == null) throw new KeyNotFoundException("Property not found or you don't have permission");

            _context.Properties.Remove(p);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedBase<PropertyViewModel>> FindAsync(string keyword, int pageIndex, int pageSize, string order, string direction)
        {
            string[] supportedOrder = new string[] { "name", "bedroom", "leaseprice", "createddate", "updateddate", "owner" };
            var o = string.IsNullOrEmpty(order) ? "" : order.ToLower();
            if (!supportedOrder.Contains(o)) o = supportedOrder[0];

            var d= direction.ToLower();
            bool isAsc = d != "desc";

            var q = string.IsNullOrEmpty(keyword) ? basicQuery : basicQuery.Where(x => x.name.ToLower().IndexOf(keyword.ToLower()) >= 0 || x.User.UserName.ToLower().IndexOf(keyword.ToLower()) >= 0);

            switch (o)
            {
                case "bedroom":
                    q = isAsc ? q.OrderBy(x => x.bedroom) : q.OrderByDescending(x => x.bedroom);
                    break;
                case "leaseprice":
                    q = isAsc ? q.OrderBy(x => x.leasePrice) : q.OrderByDescending(x => x.leasePrice);
                    break;
                case "createddate":
                    q = isAsc ? q.OrderBy(x => x.createdDate) : q.OrderByDescending(x => x.createdDate);
                    break;
                case "updateddate":
                    q = isAsc ? q.OrderBy(x => x.updatedDate) : q.OrderByDescending(x => x.updatedDate);
                    break;
                case "owner":
                    q = isAsc ? q.OrderBy(x => x.User.UserName) : q.OrderByDescending(x => x.User.UserName);
                    break;
                default:
                    q = isAsc ? q.OrderBy(x => x.name) : q.OrderByDescending(x => x.name);
                    break;
            }

            var mapped = q.Select(viewModelMapper);
            return await PaginatedBase<PropertyViewModel>.CreateAsync(mapped, pageIndex, pageSize, order, direction);
        }

        public async Task<PropertyViewModel> FindByIdAsync(int id)
        {
            return await basicQuery.Where(x => x.id == id).Select(viewModelMapper).FirstOrDefaultAsync();
        }

        public async Task<PropertyViewModel> UpdateAsync(int id, string name, int bedroom, bool isAvailiable, decimal leasePrice)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Property Name is required");
            if (name.Length > 200) throw new ArgumentException("The length of Propoerty Name is exceed");

            var p = basicQuery.Where(x => x.id == id).FirstOrDefault();
            if (p == null) throw new KeyNotFoundException("Property not found or you don't have permission");

            p.name = name;
            p.bedroom = bedroom;
            p.isAvailable = isAvailiable;
            p.leasePrice = leasePrice;
            p.updatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return await FindByIdAsync(p.id);
        }
    }
}
