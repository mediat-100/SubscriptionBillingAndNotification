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
}
