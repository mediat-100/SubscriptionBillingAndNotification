using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Enums;

namespace SubscriptionBillingAndNotificationCore.Dtos.Responses
{
    public class UserResponseDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public UserStatus Status { get; set; }
        public UserType UserType { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
