using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Dtos.Responses
{
    public class PagedUserResponseDto
    {
        public IEnumerable<UserResponseDto> Users { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public PagedResponse() { }

        public PagedResponse(List<T> data, int pageNumber, int pageSize, int totalCount)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize); 
        }

        public static PagedResponse<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            return new PagedResponse<T>(items, pageNumber, pageSize, count);
        }
    }
}
