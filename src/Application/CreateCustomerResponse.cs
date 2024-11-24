namespace Application;

public sealed record CreateCustomerResponse(Guid Id, string FirstName, string LastName, DateOnly Dob);
