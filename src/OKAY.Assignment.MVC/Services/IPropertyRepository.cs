using OKAY.Assignment.MVC.Models;
using System;
using System.Threading.Tasks;

namespace OKAY.Assignment.MVC.Services
{
    public interface IPropertyRepository
    {
        Task<PropertyViewModel> CreateAsync(string name, int bedroom, bool isAvailiable, decimal leasePrice);
        Task<PaginatedBase<PropertyViewModel>> FindAsync(string keyword, int pageIndex, int pageSize, string order, string direction);
        Task DeleteAsync(int id);
        Task<PropertyViewModel> UpdateAsync(int id, string name, int bedroom, bool isAvailiable, decimal leasePrice);
        Task<PropertyViewModel> FindByIdAsync(int id);
    }
}
