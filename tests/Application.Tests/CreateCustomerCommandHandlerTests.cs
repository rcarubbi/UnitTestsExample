using Bogus;
using Domain;
using FluentAssertions;
using Moq;

namespace Application.Tests;

public class CreateCustomerCommandHandlerTests
{
 
    private bool Compare<T>(T actual, T expected) where T : IEntity
    {
        actual.Should().BeEquivalentTo(expected, options => options.Excluding(c => c.Id));
        return true;
    }


    [Fact]
    public Task VerifyCheck() => VerifyChecks.Run();
    

    [Fact]
    public async Task Handle_ShouldCreateCustomer_WhenCommandIsValid() // GivenValidCommand_WhenHandleIsCalled_ThenCustomerIsCreated
    {
        // arrange
        var customer = new Faker<Customer>()
            .CustomInstantiator(f => new Customer(
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Internet.Email(),
                f.Date.PastDateOnly(18, DateOnly.FromDateTime(DateTime.Today)),
                f.PickRandom<Gender>()
            ))
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .Generate();


        var command = new CreateCustomerCommand(customer.FirstName, customer.LastName, customer.Dob, customer.Email, customer.Gender);

        var repositoryMock = new Mock<ICustomerRepository>();

        

        repositoryMock.Setup(x => x.Add(It.Is<Customer>(c => Compare(c, customer)), CancellationToken.None))
            .ReturnsAsync(true);


        var commandHandler = new CreateCustomerCommandHandler(repositoryMock.Object);

        // act
        var response = await commandHandler.Handle(command, CancellationToken.None);


        // assert
       
        repositoryMock.Verify(x => x.Add(It.Is<Customer>(c => Compare(c, customer)), CancellationToken.None), Times.Once);
        repositoryMock.VerifyNoOtherCalls();

        await Verify(response);
    }
}
