namespace Domain;

public interface ICustomerRepository
{
    Task<bool> Add(Customer customer, CancellationToken cancellationToken);
}
