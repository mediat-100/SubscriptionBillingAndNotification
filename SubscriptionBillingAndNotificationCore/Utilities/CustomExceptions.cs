using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Utilities
{
    public class CustomExceptions
    {
        public class ValidationException : Exception
        {
            public IEnumerable<string>? Errors { get; }

            public ValidationException(string message) : base(message) { }

            public ValidationException(string message, IEnumerable<string> errors) : base(message)
            {
                Errors = errors;
            }
        }

        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }

        public class UnauthorizedException : Exception
        {
            public UnauthorizedException(string message) : base(message) { }
        }

        public class ForbiddenException : Exception
        {
            public ForbiddenException(string message) : base(message) { }
        }
    }
}
