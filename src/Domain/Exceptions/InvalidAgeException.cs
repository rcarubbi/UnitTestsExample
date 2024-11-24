using Domain.Entities;

namespace Domain.Exceptions;

internal class InvalidAgeException : Exception
{
    public InvalidAgeException()
    : base($"Customer must be at least {Customer.AdultAge} years old.")
    {

    }
}
