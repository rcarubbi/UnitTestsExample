using Domain.ValueObjects;

namespace Application;

public sealed record CreateCustomerCommand(string FirstName, string LastName, DateOnly Dob, string Email, Gender Gender);
