using Domain.Entities;

namespace Domain.Repositories;

public interface ICustomerRepository
{
    Task<bool> Add(Customer customer, CancellationToken cancellationToken);
}
