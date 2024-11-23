namespace Domain;

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
    }

    public Guid Id { get; private set; }  
    public string FirstName { get; private set; }  
    public string LastName { get; private set; } 
    public string Email { get; private set; } 

    public DateOnly Dob { get; private set; } 
    public Gender Gender { get; private set; }
}
