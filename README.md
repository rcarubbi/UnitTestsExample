# Unit Testing in .NET: xUnit, Moq, Fluent Assertions, Verify, and Bogus

This repository contains an example project demonstrating how to write robust and effective unit tests in .NET using popular testing libraries such as **xUnit**, **Moq**, **Fluent Assertions**, **Verify**, and **Bogus**. The project is designed to follow the principles of **Clean Architecture**.

## Project Structure

The project is divided into multiple layers to follow the Clean Architecture principles:

- **Domain**: Contains the core business logic, including entities, value objects, and exceptions.
- **Application**: Includes the command handler logic.
- **Application.Tests**: Contains the unit tests for the application layer.

## Libraries Used

### [xUnit](https://xunit.net/)
xUnit is the testing framework used in this project. It provides attributes such as `[Fact]` and `[Theory]` to define tests.

### [Moq](https://github.com/moq/moq4)
Moq is used to create mock objects and simulate dependencies. For example, it mocks the `ICustomerRepository` in the `CreateCustomerCommandHandlerTests`.

### [Fluent Assertions](https://fluentassertions.com/)
Fluent Assertions is used for writing expressive and readable assertions. It's particularly helpful in this project for comparing complex objects like `Customer` without relying on reference equality.

### [Verify](https://github.com/VerifyTests/Verify)
Verify is used for snapshot testing. It saves a snapshot of test results and compares them in subsequent runs. If there are differences, it opens a diff tool to update the snapshot.

### [Bogus](https://github.com/bchavez/Bogus)
Bogus is used for generating realistic fake data. This ensures that tests are executed with dynamic, yet consistent, data inputs.

---

## Features

### 1. **Testing Complex Command Handlers**
The `CreateCustomerCommandHandler` is tested to ensure that customers are created correctly or errors are thrown when data is invalid. Examples include:
- Validating the age of customers (minimum age: 18).
- Verifying the `Add` method in the repository is called correctly with the expected customer.

### 2. **Clean Architecture Principles**
The project demonstrates how to structure code to follow Clean Architecture:
- `Command Handlers` are internal to the application layer.
- Communication between layers is handled via a mediator.

To enable testing of internal classes, the following configuration is added in the application project:

```xml
<ItemGroup>
    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
        <_Parameter1>Application.Tests</_Parameter1>
    </AssemblyAttribute>
</ItemGroup>
```

### 3. **Snapshot Testing with Verify**
The Verify library is used to compare test outputs. If discrepancies are detected, a diff tool opens to manually approve or reject changes. Random values like GUIDs are scrubbed for consistent comparisons.

---

## How to Run the Tests

### Prerequisites
- .NET SDK installed (version 7.0 or higher).

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/rcarubbi/UnitTestsExample.git
   cd UnitTestsExample
   ```

2. Run the tests:
   ```bash
   dotnet test
   ```

### Notes on Verify Configuration
- Verify requires the following additions to `.gitignore`:
    ```gitignore
    # Verify
    *.received.*
    *.received/
    ```
- The `.gitattributes` file should include:
    ```gitattributes
    # Verify
    *.verified.txt text eol=lf working-tree-encoding=UTF-8
    ```
- The project includes a helper test to validate these configurations:
    ```csharp
    [Fact]
    public Task VerifyCheck() => VerifyChecks.Run();
    ```

---

## Naming Conventions for Tests

This project showcases two common naming conventions for tests:
1. **Given... When... Then...**
   - Example: `GivenValidCommand_WhenHandleIsCalled_ThenCustomerIsCreated`.
2. **Action... Should... When...**
   - Example: `Handle_ShouldCreateCustomer_WhenCommandIsValid`.

---

## Examples of Tests

### Handling Valid Commands
```csharp
[Fact]
public async Task Handle_ShouldCreateCustomer_WhenCommandIsValid()
{
    // Arrange
    var command = new CreateCustomerCommand(/* valid data */);
    var sut = new CreateCustomerCommandHandler(_repositoryMock.Object);

    // Act
    var response = await sut.Handle(command, CancellationToken.None);

    // Assert
    _repositoryMock.Verify(x => x.Add(It.Is<Customer>(c => Compare(c, expectedCustomer)), CancellationToken.None), Times.Once);
    await Verify(response);
}
```

### Handling Invalid Commands
```csharp
[Fact]
public async Task Handle_ShouldFail_WhenCustomerIsYoungerThan18YearsOld()
{
    // Arrange
    var command = new CreateCustomerCommand(/* invalid age */);
    var sut = new CreateCustomerCommandHandler(_repositoryMock.Object);

    // Act
    Func<Task> action = async () => await sut.Handle(command, CancellationToken.None);

    // Assert
    await action.Should().ThrowAsync<InvalidAgeException>();
}
```

---

## Contributions

Feel free to fork, open issues, or submit pull requests to enhance this example repository.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
