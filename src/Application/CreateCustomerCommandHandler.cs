using Domain.Entities;
using Domain.Repositories;

namespace Application;

internal sealed class CreateCustomerCommandHandler(ICustomerRepository repository)
{
    public async Task<CreateCustomerResponse> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {

        var customer = new Customer(command.FirstName, command.LastName, command.Email, command.Dob, command.Gender);

        await repository.Add(customer, cancellationToken);

        return new CreateCustomerResponse(customer.Id, customer.FirstName, customer.LastName, customer.Dob);
    }
}