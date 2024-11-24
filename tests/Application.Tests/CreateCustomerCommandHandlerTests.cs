using Bogus;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Application.Tests;

public class CreateCustomerCommandHandlerTests
{

    private static bool Compare<T>(T actual, T expected) where T : IEntity
    {
        actual.Should().BeEquivalentTo(expected, options => options.Excluding(c => c.Id));
        return true;
    }

    private readonly Mock<ICustomerRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_ShouldCreateCustomer_WhenCommandIsValid() // GivenValidCommand_WhenHandleIsCalled_ThenCustomerIsCreated
    {
        // arrange
        const int MaxAge = 70;

        var customer = new Faker<Customer>()
            .CustomInstantiator(f => new Customer(
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Internet.Email(),
                DateOnly.FromDateTime(f.Date.PastOffset(MaxAge, DateTime.Now.AddYears(-Customer.AdultAge)).Date),
                f.PickRandom<Gender>()
            ))
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .Generate();

        var command = new CreateCustomerCommand(customer.FirstName, customer.LastName, customer.Dob, customer.Email, customer.Gender);

        _repositoryMock.Setup(x => x.Add(It.Is<Customer>(c => Compare(c, customer)), CancellationToken.None))
            .ReturnsAsync(true);

        var sut = new CreateCustomerCommandHandler(_repositoryMock.Object);

        // act
        var response = await sut.Handle(command, CancellationToken.None);

        // assert
        _repositoryMock.Verify(x => x.Add(It.Is<Customer>(c => Compare(c, customer)), CancellationToken.None), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();

        var verifySettings = new VerifySettings();
        verifySettings.ScrubMember<CreateCustomerResponse>(f => f.FirstName);
        verifySettings.ScrubMember<CreateCustomerResponse>(f => f.LastName);

        await Verify(response, verifySettings);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCustomerIsYoungerThan18YearsOld()
    {
        // arrange
        var command = new Faker<CreateCustomerCommand>()
            .CustomInstantiator(f => new CreateCustomerCommand(
                f.Name.FirstName(),
                f.Name.LastName(),
                DateOnly.FromDateTime(f.Date.PastOffset(5).Date),
                f.Internet.Email(),
                f.PickRandom<Gender>()
            ))
           .RuleFor(c => c.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
           .Generate();


        var sut = new CreateCustomerCommandHandler(_repositoryMock.Object);

        // act
        Task<CreateCustomerResponse> action() => sut.Handle(command, CancellationToken.None);


        // assert
        await ThrowsTask(action);
        _repositoryMock.VerifyNoOtherCalls();
    }
}
