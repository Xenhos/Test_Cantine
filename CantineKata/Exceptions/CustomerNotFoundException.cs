using System;

public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException() : base("Customer not found.") { }

    public CustomerNotFoundException(string message) : base(message) { }

    public CustomerNotFoundException(string message, Exception innerException) 
        : base(message, innerException) { }
}
