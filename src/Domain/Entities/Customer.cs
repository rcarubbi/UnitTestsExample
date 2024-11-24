using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Customer : IEntity
{

    public Customer(string firstName, string lastName, string email, DateOnly dob, Gender gender)
    {
       
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Dob = dob;
        Gender = gender;
        Validate();
    }

    public const int AdultAge = 18;

    private void Validate()
    {
        if (CalculateAge() < AdultAge)
        {
            throw new InvalidAgeException();
        }
    }

    private int CalculateAge()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - Dob.Year;
        if (Dob > today.AddYears(-age)) age--;
        return age;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }

    public DateOnly Dob { get; private set; }
    public Gender Gender { get; private set; }
}
