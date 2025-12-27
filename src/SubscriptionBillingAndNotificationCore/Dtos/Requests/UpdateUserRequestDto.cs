using SubscriptionBillingAndNotificationCore.Enums;

namespace SubscriptionBillingAndNotificationCore.Dtos.Requests
{
    public class UpdateUserRequestDto
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
