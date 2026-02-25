# Guard

Argument validation utilities with helpful error messages and automatic parameter name capture.

**Namespace:** `GamaLearn.Guards`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

The Guard class provides fluent, expressive methods for validating method arguments. It automatically captures parameter names using `CallerArgumentExpression`, making error messages clear and helpful without manual string literals.

---

## Installation

```bash
dotnet add package GamaLearn.Maui.Core
```

---

## Methods

### IsNotNull&lt;T&gt;(T? value)

Throws `ArgumentNullException` if the value is null. Returns the non-null value.

**Type Parameters:**
- `T` - The type of the value

**Parameters:**
- `value` - The value to check
- `parameterName` - Auto-populated via CallerArgumentExpression

**Returns:** `T` - The non-null value

**Throws:** `ArgumentNullException` - If value is null

```csharp
public class UserService
{
    public void CreateUser(User user)
    {
        Guard.IsNotNull(user); // Throws with "user" as parameter name

        // user is guaranteed non-null here
    }
}
```

### IsNotNullOrEmpty(string? value)

Throws `ArgumentException` if the string is null or empty. Returns the non-null, non-empty string.

**Parameters:**
- `value` - The string to check
- `parameterName` - Auto-populated

**Returns:** `string` - The non-null, non-empty string

**Throws:** `ArgumentException` - If value is null or empty

```csharp
public void SendEmail(string emailAddress)
{
    Guard.IsNotNullOrEmpty(emailAddress);

    // emailAddress has content
}
```

### IsNotNullOrWhiteSpace(string? value)

Throws `ArgumentException` if the string is null, empty, or whitespace. Returns the non-null, non-whitespace string.

**Parameters:**
- `value` - The string to check
- `parameterName` - Auto-populated

**Returns:** `string` - The non-null, non-whitespace string

**Throws:** `ArgumentException` - If value is null, empty, or whitespace

```csharp
public void SetUsername(string username)
{
    Guard.IsNotNullOrWhiteSpace(username);

    // username has meaningful content
}
```

### IsInRange&lt;T&gt;(T value, T min, T max)

Throws `ArgumentOutOfRangeException` if the value is not within the specified range (inclusive).

**Type Parameters:**
- `T` - The type of the value (must implement `IComparable<T>`)

**Parameters:**
- `value` - The value to check
- `min` - Minimum allowed value (inclusive)
- `max` - Maximum allowed value (inclusive)
- `parameterName` - Auto-populated

**Returns:** `T` - The value if within range

**Throws:** `ArgumentOutOfRangeException` - If value is outside the range

```csharp
public void SetAge(int age)
{
    Guard.IsInRange(age, 0, 120);

    // age is between 0 and 120
}

public void SetDiscount(decimal discount)
{
    Guard.IsInRange(discount, 0.0m, 1.0m);

    // discount is between 0% and 100%
}
```

### IsPositive(int value)

Throws `ArgumentOutOfRangeException` if the value is less than or equal to zero.

**Parameters:**
- `value` - The value to check
- `parameterName` - Auto-populated

**Returns:** `int` - The positive value

**Throws:** `ArgumentOutOfRangeException` - If value is not positive (≤ 0)

```csharp
public void SetQuantity(int quantity)
{
    Guard.IsPositive(quantity);

    // quantity is > 0
}
```

### IsNotNegative(int value)

Throws `ArgumentOutOfRangeException` if the value is less than zero.

**Parameters:**
- `value` - The value to check
- `parameterName` - Auto-populated

**Returns:** `int` - The non-negative value

**Throws:** `ArgumentOutOfRangeException` - If value is negative (< 0)

```csharp
public void SetBalance(int balance)
{
    Guard.IsNotNegative(balance);

    // balance is >= 0
}
```

### IsNotNullOrEmpty&lt;T&gt;(IEnumerable&lt;T&gt;? collection)

Throws `ArgumentException` if the collection is null or empty.

**Type Parameters:**
- `T` - The type of elements in the collection

**Parameters:**
- `collection` - The collection to check
- `parameterName` - Auto-populated

**Returns:** `IEnumerable<T>` - The non-null, non-empty collection

**Throws:** `ArgumentException` - If collection is null or empty

```csharp
public void ProcessItems(List<string> items)
{
    Guard.IsNotNullOrEmpty(items);

    // items has at least one element
    foreach (var item in items)
    {
        // ...
    }
}
```

### IsTrue(bool condition, string message)

Throws `ArgumentException` if the condition is false.

**Parameters:**
- `condition` - The condition to check
- `message` - The exception message if condition is false
- `parameterName` - Auto-populated

**Throws:** `ArgumentException` - If condition is false

```csharp
public void SetEmail(string email)
{
    Guard.IsTrue(email.Contains("@"), "Email must contain @");

    // email contains @
}
```

### IsFalse(bool condition, string message)

Throws `ArgumentException` if the condition is true.

**Parameters:**
- `condition` - The condition to check
- `message` - The exception message if condition is true
- `parameterName` - Auto-populated

**Throws:** `ArgumentException` - If condition is true

```csharp
public void SetUsername(string username)
{
    Guard.IsFalse(username.Contains(" "), "Username cannot contain spaces");

    // username has no spaces
}
```

---

## Usage Examples

### Service Class Validation

```csharp
public class OrderService
{
    private readonly IPaymentService paymentService;
    private readonly IInventoryService inventoryService;

    public OrderService(
        IPaymentService paymentService,
        IInventoryService inventoryService)
    {
        this.paymentService = Guard.IsNotNull(paymentService);
        this.inventoryService = Guard.IsNotNull(inventoryService);
    }

    public async Task<Order> CreateOrderAsync(
        string customerId,
        List<OrderItem> items,
        decimal totalAmount)
    {
        Guard.IsNotNullOrWhiteSpace(customerId);
        Guard.IsNotNullOrEmpty(items);
        Guard.IsPositive((int)(totalAmount * 100)); // Convert to cents

        // All parameters are validated
        var order = new Order
        {
            CustomerId = customerId,
            Items = items,
            TotalAmount = totalAmount
        };

        return await SaveOrderAsync(order);
    }
}
```

### ViewModel Validation

```csharp
public class RegistrationViewModel
{
    public ICommand RegisterCommand => new Command(async () =>
    {
        try
        {
            await RegisterAsync(Username, Email, Password);
        }
        catch (ArgumentException ex)
        {
            ErrorMessage = ex.Message;
        }
    });

    private async Task RegisterAsync(string username, string email, string password)
    {
        Guard.IsNotNullOrWhiteSpace(username);
        Guard.IsNotNullOrWhiteSpace(email);
        Guard.IsNotNullOrWhiteSpace(password);

        Guard.IsTrue(email.Contains("@"), "Invalid email format");
        Guard.IsInRange(username.Length, 3, 20);
        Guard.IsInRange(password.Length, 8, 100);
        Guard.IsFalse(username.Contains(" "), "Username cannot contain spaces");

        await authService.RegisterAsync(username, email, password);
    }
}
```

### Model Property Setters

```csharp
public class Product
{
    private string name;
    private decimal price;
    private int stockQuantity;

    public string Name
    {
        get => name;
        set => name = Guard.IsNotNullOrWhiteSpace(value);
    }

    public decimal Price
    {
        get => price;
        set
        {
            Guard.IsInRange(value, 0.01m, decimal.MaxValue);
            price = value;
        }
    }

    public int StockQuantity
    {
        get => stockQuantity;
        set
        {
            Guard.IsNotNegative(value);
            stockQuantity = value;
        }
    }
}
```

### API Client Validation

```csharp
public class ApiClient
{
    private readonly string baseUrl;
    private readonly string apiKey;

    public ApiClient(string baseUrl, string apiKey)
    {
        this.baseUrl = Guard.IsNotNullOrWhiteSpace(baseUrl);
        this.apiKey = Guard.IsNotNullOrWhiteSpace(apiKey);

        Guard.IsTrue(baseUrl.StartsWith("https://"),
            "API base URL must use HTTPS");
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        Guard.IsNotNullOrWhiteSpace(endpoint);

        var url = $"{baseUrl}/{endpoint.TrimStart('/')}";
        // Make request...
    }

    public async Task PostAsync<T>(string endpoint, T data)
    {
        Guard.IsNotNullOrWhiteSpace(endpoint);
        Guard.IsNotNull(data);

        // Make request...
    }
}
```

### Collection Processing

```csharp
public class DataProcessor
{
    public async Task<List<ProcessedData>> ProcessBatchAsync(
        IEnumerable<RawData> rawData,
        int batchSize)
    {
        Guard.IsNotNullOrEmpty(rawData);
        Guard.IsPositive(batchSize);
        Guard.IsInRange(batchSize, 1, 1000);

        var results = new List<ProcessedData>();

        foreach (var batch in rawData.Chunk(batchSize))
        {
            var processed = await ProcessChunkAsync(batch);
            results.AddRange(processed);
        }

        return results;
    }
}
```

### Business Logic Validation

```csharp
public class AccountService
{
    public void Transfer(
        string fromAccountId,
        string toAccountId,
        decimal amount)
    {
        Guard.IsNotNullOrWhiteSpace(fromAccountId);
        Guard.IsNotNullOrWhiteSpace(toAccountId);
        Guard.IsPositive((int)(amount * 100)); // Convert to cents

        Guard.IsFalse(fromAccountId == toAccountId,
            "Cannot transfer to the same account");

        Guard.IsInRange(amount, 0.01m, 1_000_000m);

        // Process transfer...
    }
}
```

### Configuration Validation

```csharp
public class AppSettings
{
    public void Configure(
        string databaseConnection,
        int maxConnections,
        TimeSpan timeout)
    {
        Guard.IsNotNullOrWhiteSpace(databaseConnection);
        Guard.IsInRange(maxConnections, 1, 100);
        Guard.IsInRange(timeout.TotalSeconds, 1, 300);

        DatabaseConnection = databaseConnection;
        MaxConnections = maxConnections;
        Timeout = timeout;
    }

    public string DatabaseConnection { get; private set; }
    public int MaxConnections { get; private set; }
    public TimeSpan Timeout { get; private set; }
}
```

---

## Best Practices

### 1. Validate at Boundaries

Always validate at public API boundaries (constructors, public methods, property setters):

```csharp
✅ Good:
public void ProcessOrder(Order order)
{
    Guard.IsNotNull(order);
    // Private method calls don't need guards
    CalculateTotal(order);
}

private void CalculateTotal(Order order)
{
    // No guard needed - already validated
}

❌ Bad:
private void CalculateTotal(Order order)
{
    Guard.IsNotNull(order); // Unnecessary in private method
}
```

### 2. Use Fluent Assignment

Guards return the validated value, enabling fluent assignment:

```csharp
✅ Good:
public UserService(IUserRepository repository)
{
    this.repository = Guard.IsNotNull(repository);
}

Also Good:
public UserService(IUserRepository repository)
{
    Guard.IsNotNull(repository);
    this.repository = repository;
}
```

### 3. Provide Meaningful Messages

For custom validations, provide clear, actionable messages:

```csharp
✅ Good:
Guard.IsTrue(email.Contains("@"), "Email must contain @ symbol");

❌ Bad:
Guard.IsTrue(email.Contains("@"), "Invalid");
```

### 4. Don't Overuse

Don't guard every variable; focus on inputs and critical paths:

```csharp
✅ Good:
public void ProcessData(string data)
{
    Guard.IsNotNullOrEmpty(data);

    // Local variables don't need guards
    var processed = data.ToUpper();
    var length = processed.Length;
}

❌ Bad:
var processed = Guard.IsNotNull(data.ToUpper());
var length = Guard.IsPositive(processed.Length);
```

### 5. Guard Before Using

Always guard before using the parameter:

```csharp
✅ Good:
public void SetName(string name)
{
    Guard.IsNotNullOrWhiteSpace(name);
    this.name = name;
}

❌ Bad:
public void SetName(string name)
{
    this.name = name;
    Guard.IsNotNullOrWhiteSpace(name); // Too late!
}
```

---

## Error Messages

Guards automatically generate helpful error messages with parameter names:

```csharp
Guard.IsNotNull(user);
// throws: ArgumentNullException: Value cannot be null. (Parameter 'user')

Guard.IsNotNullOrEmpty(email);
// throws: ArgumentException: Value cannot be null or empty. (Parameter 'email')

Guard.IsInRange(age, 18, 120);
// throws: ArgumentOutOfRangeException: Value must be between 18 and 120. (Parameter 'age')

Guard.IsTrue(email.Contains("@"), "Email must contain @");
// throws: ArgumentException: Email must contain @ (Parameter 'email.Contains("@")')
```

---

## Performance

- **Zero allocation**: Guards don't allocate unless they throw
- **Inlined**: Simple checks are inlined by the JIT compiler
- **Fast**: Minimal overhead compared to manual validation
- **No reflection**: Uses `CallerArgumentExpression` (compile-time)

---

## See Also

- [Debouncer](Debouncer.md) - Debounce rapid method calls
- [TaskExtensions](TaskExtensions.md) - Task utilities
- [CollectionExtensions](CollectionExtensions.md) - Collection helpers
