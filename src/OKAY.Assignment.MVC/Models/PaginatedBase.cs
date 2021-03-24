using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OKAY.Assignment.MVC.Models
{
    public class PaginatedBase<T> : List<T> where T : class
    {
        public PaginatedBase(int PageIndex, int PageSize, int ItemCount, string Order, string Direction, ICollection<T> Rows)
        {
            page = PageIndex;
            pageSize = PageSize;
            pageCount = (int)Math.Ceiling(ItemCount / (double)PageSize);
            order = Order;
            direction = Direction;
            this.AddRange(Rows);
        }

        public int page { get; }
        public int pageSize { get; }
        public int pageCount { get; }
        public string order { get; }
        public string direction { get; }
        // public virtual ICollection<T> rows { get; }

        public bool HasPrevious => page > 1;
        public bool HasNext => page < pageCount;

        public static async Task<PaginatedBase<T>> CreateAsync(IQueryable<T> Source, int PageIndex, int PageSize, string Order, string Direction)
        {
            var count = await Source.CountAsync();
            var items = await Source.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
            return new PaginatedBase<T>(PageIndex, PageSize, count, Order, Direction, items);
        }
    }
}
