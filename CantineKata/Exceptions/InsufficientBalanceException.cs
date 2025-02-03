public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException() : base("Insufficient balance for this operation.") { }

    public InsufficientBalanceException(string message) : base(message) { }

    public InsufficientBalanceException(string message, Exception innerException) 
        : base(message, innerException) { }
}
