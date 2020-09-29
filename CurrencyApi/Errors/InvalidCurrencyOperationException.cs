using System;

namespace CurrencyApi.Errors
{
    public class InvalidCurrencyOperationException : Exception
    {
        public ErrorType ErrorType { get; }

        public InvalidCurrencyOperationException(ErrorType errorType)
        {
            this.ErrorType = errorType;
        }

        public InvalidCurrencyOperationException(ErrorType errorType, string message) : base(message)
        {
            this.ErrorType = errorType;
        }
    }
}
