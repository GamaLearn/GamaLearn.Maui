# Debouncer

Debounces rapid calls to an action, executing only after a period of inactivity. Perfect for search boxes, resize handlers, and other scenarios with rapid input.

**Namespace:** `GamaLearn.Threading`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

The Debouncer ensures that an action is only executed after a specified delay has passed since the last call. If the action is called again before the delay expires, the timer resets. This is particularly useful for scenarios where you want to wait for user input to stabilize before performing an expensive operation.

---

## Installation

```bash
dotnet add package GamaLearn.Maui.Core
```

---

## Constructor

### Debouncer(TimeSpan delay)

Creates a new debouncer with the specified delay.

**Parameters:**
- `delay` - The delay to wait after the last call before executing. Must be greater than zero.

**Throws:**
- `ArgumentOutOfRangeException` - If delay is less than or equal to zero

```csharp
var debouncer = new Debouncer(TimeSpan.FromMilliseconds(300));
```

---

## Methods

### Debounce(Action action)

Debounces a synchronous action. If called multiple times within the delay period, only the last call will execute after the delay.

**Parameters:**
- `action` - The action to execute

**Throws:**
- `ArgumentNullException` - If action is null
- `ObjectDisposedException` - If the debouncer has been disposed

```csharp
debouncer.Debounce(() =>
{
    Console.WriteLine("Debounced action executed");
});
```

### DebounceAsync(Func&lt;Task&gt; action)

Debounces an async action. Returns a task that completes when the debounced action executes or is cancelled.

**Parameters:**
- `action` - The async action to execute

**Returns:** `Task` - Completes when the action executes or is cancelled

**Throws:**
- `ArgumentNullException` - If action is null
- `ObjectDisposedException` - If the debouncer has been disposed

```csharp
await debouncer.DebounceAsync(async () =>
{
    await SearchApiAsync(query);
});
```

### DebounceAsync&lt;T&gt;(Func&lt;Task&lt;T&gt;&gt; func)

Debounces an action with a result. Returns the result of the last call, or default if cancelled.

**Type Parameters:**
- `T` - The result type

**Parameters:**
- `func` - The function to execute

**Returns:** `Task<T?>` - The result, or default(T) if cancelled

**Throws:**
- `ArgumentNullException` - If func is null
- `ObjectDisposedException` - If the debouncer has been disposed

```csharp
var result = await debouncer.DebounceAsync(async () =>
{
    return await FetchDataAsync();
});

if (result != null)
{
    Console.WriteLine($"Got result: {result}");
}
```

### Cancel()

Cancels any pending debounced action.

```csharp
debouncer.Cancel();
```

### Dispose()

Cancels any pending action and releases all resources. Implements `IDisposable`.

```csharp
debouncer.Dispose();
```

---

## Properties

### IsPending

**Type:** `bool` (read-only)

Gets whether there is a pending debounced action waiting to execute.

```csharp
if (debouncer.IsPending)
{
    Console.WriteLine("Action is pending...");
}
```

---

## Usage Examples

### Search Box

```csharp
public class SearchViewModel
{
    private readonly Debouncer searchDebouncer;

    public SearchViewModel()
    {
        searchDebouncer = new Debouncer(TimeSpan.FromMilliseconds(300));
    }

    public string SearchQuery
    {
        get => searchQuery;
        set
        {
            searchQuery = value;
            OnPropertyChanged();

            // Debounce search API calls
            searchDebouncer.Debounce(async () =>
            {
                await PerformSearch(value);
            });
        }
    }

    private async Task PerformSearch(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return;

        var results = await SearchApiAsync(query);

        // Update UI on main thread
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            SearchResults.Clear();
            SearchResults.AddRange(results);
        });
    }
}
```

### Text Entry with Live Validation

```xml
<Entry Text="{Binding Username, Mode=TwoWay}"
       Placeholder="Enter username" />
<Label Text="{Binding ValidationMessage}"
       TextColor="Red" />
```

```csharp
public class RegistrationViewModel
{
    private readonly Debouncer validationDebouncer;
    private string username;
    private string validationMessage;

    public RegistrationViewModel()
    {
        validationDebouncer = new Debouncer(TimeSpan.FromMilliseconds(500));
    }

    public string Username
    {
        get => username;
        set
        {
            username = value;
            OnPropertyChanged();

            // Debounce validation
            validationDebouncer.Debounce(async () =>
            {
                await ValidateUsername(value);
            });
        }
    }

    public string ValidationMessage
    {
        get => validationMessage;
        set
        {
            validationMessage = value;
            OnPropertyChanged();
        }
    }

    private async Task ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            ValidationMessage = "";
            return;
        }

        var isAvailable = await CheckUsernameAvailabilityAsync(username);

        ValidationMessage = isAvailable
            ? "✓ Username available"
            : "✗ Username taken";
    }
}
```

### Window Resize Handler

```csharp
public class ResponsiveView : ContentView
{
    private readonly Debouncer resizeDebouncer;

    public ResponsiveView()
    {
        resizeDebouncer = new Debouncer(TimeSpan.FromMilliseconds(150));

        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        resizeDebouncer.Debounce(() =>
        {
            RecalculateLayout();
        });
    }

    private void RecalculateLayout()
    {
        // Expensive layout recalculation
        Console.WriteLine($"Recalculating layout for size: {Width}x{Height}");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        resizeDebouncer.Dispose();
    }
}
```

### Auto-Save with Debouncing

```csharp
public class DocumentEditorViewModel : IDisposable
{
    private readonly Debouncer autoSaveDebouncer;
    private string content;

    public DocumentEditorViewModel()
    {
        autoSaveDebouncer = new Debouncer(TimeSpan.FromSeconds(2));
    }

    public string Content
    {
        get => content;
        set
        {
            content = value;
            OnPropertyChanged();

            // Auto-save after 2 seconds of inactivity
            autoSaveDebouncer.Debounce(async () =>
            {
                await SaveDocument();
            });
        }
    }

    private async Task SaveDocument()
    {
        Console.WriteLine("Auto-saving document...");
        await SaveToFileAsync(Content);
        Console.WriteLine("Document saved");
    }

    public void Dispose()
    {
        autoSaveDebouncer?.Dispose();
    }
}
```

### Cancelling Pending Actions

```csharp
public class SearchPageViewModel : IDisposable
{
    private readonly Debouncer searchDebouncer;

    public SearchPageViewModel()
    {
        searchDebouncer = new Debouncer(TimeSpan.FromMilliseconds(300));
    }

    public async Task OnSearchTextChanged(string query)
    {
        await searchDebouncer.DebounceAsync(async () =>
        {
            await PerformSearchAsync(query);
        });
    }

    public void OnNavigatingAway()
    {
        // Cancel any pending search when leaving the page
        searchDebouncer.Cancel();
    }

    public void Dispose()
    {
        searchDebouncer?.Dispose();
    }
}
```

---

## Best Practices

1. **Choose appropriate delay**:
   - Search boxes: 200-300ms
   - Auto-save: 1-3 seconds
   - Resize handlers: 100-200ms
   - Validation: 300-500ms

2. **Dispose properly**: Always dispose the debouncer when done to prevent memory leaks

3. **Handle cancellation**: Remember that debounced actions may be cancelled and never execute

4. **Use async variants**: Prefer `DebounceAsync` for async operations to properly handle exceptions

5. **Check IsPending**: Use the `IsPending` property to show loading indicators

6. **Single instance per use case**: Create one debouncer per logical operation, not per call

---

## Performance Considerations

- **Memory**: Minimal overhead - uses a single `CancellationTokenSource` per debouncer
- **Thread-safe**: Safe to call from multiple threads
- **No polling**: Uses efficient timer-based approach
- **Cancellation**: Properly cancels previous operations before starting new ones

---

## Common Pitfalls

❌ **Creating new debouncer on each call**
```csharp
// DON'T DO THIS
void OnTextChanged(string text)
{
    var debouncer = new Debouncer(TimeSpan.FromMilliseconds(300));
    debouncer.Debounce(() => Search(text));
}
```

✅ **Reuse the same debouncer**
```csharp
// DO THIS
private readonly Debouncer debouncer = new(TimeSpan.FromMilliseconds(300));

void OnTextChanged(string text)
{
    debouncer.Debounce(() => Search(text));
}
```

---

## See Also

- [Throttler](Throttler.md) - Limit execution frequency (different from debouncing)
- [DispatchHelper](DispatchHelper.md) - UI thread dispatching
- [TaskExtensions](TaskExtensions.md) - Task utilities
