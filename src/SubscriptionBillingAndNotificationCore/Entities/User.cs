using SubscriptionBillingAndNotificationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Password { get; set; }
        public  UserStatus Status { get; set; }
        public UserType UserType { get; set; }
        public DateTime RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
