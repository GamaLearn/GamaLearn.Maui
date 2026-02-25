# Throttler

Throttles method execution to a maximum frequency. Ensures an action is executed at most once per time period, regardless of how many times it's called.

**Namespace:** `GamaLearn.Threading`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

The Throttler limits the execution frequency of an action. When throttled, the first call executes immediately, and subsequent calls within the throttle period are ignored. This differs from Debouncer, which waits for inactivity before executing.

**Key Difference:**
- **Throttler**: Executes immediately, then ignores calls for a period
- **Debouncer**: Waits for inactivity, then executes once

---

## Installation

```bash
dotnet add package GamaLearn.Maui.Core
```

---

## Constructor

### Throttler(TimeSpan interval)

Creates a new throttler with the specified interval.

**Parameters:**
- `interval` - The minimum time between executions. Must be greater than zero.

**Throws:**
- `ArgumentOutOfRangeException` - If interval is less than or equal to zero

```csharp
var throttler = new Throttler(TimeSpan.FromSeconds(1));
```

---

## Methods

### Throttle(Action action)

Throttles a synchronous action. First call executes immediately; subsequent calls within the interval are ignored.

**Parameters:**
- `action` - The action to execute

**Throws:**
- `ArgumentNullException` - If action is null
- `ObjectDisposedException` - If the throttler has been disposed

```csharp
throttler.Throttle(() =>
{
    Console.WriteLine("Throttled action executed");
});
```

### ThrottleAsync(Func&lt;Task&gt; action)

Throttles an async action. Returns a task that completes when the action executes or is skipped.

**Parameters:**
- `action` - The async action to execute

**Returns:** `Task` - Completes when the action executes or is skipped

**Throws:**
- `ArgumentNullException` - If action is null
- `ObjectDisposedException` - If the throttler has been disposed

```csharp
await throttler.ThrottleAsync(async () =>
{
    await SaveDataAsync();
});
```

### Reset()

Resets the throttle state, allowing the next call to execute immediately.

```csharp
throttler.Reset();
```

### Dispose()

Releases all resources. Implements `IDisposable`.

```csharp
throttler.Dispose();
```

---

## Properties

### CanExecute

**Type:** `bool` (read-only)

Gets whether the throttler can currently execute an action (throttle period has passed).

```csharp
if (throttler.CanExecute)
{
    Console.WriteLine("Ready to execute");
}
```

---

## Usage Examples

### Button Click Throttling

```csharp
public class MyViewModel
{
    private readonly Throttler saveThrottler;

    public MyViewModel()
    {
        // Prevent rapid saves - max once per second
        saveThrottler = new Throttler(TimeSpan.FromSeconds(1));
    }

    public ICommand SaveCommand => new Command(() =>
    {
        saveThrottler.Throttle(async () =>
        {
            await SaveDataAsync();
        });
    });
}
```

### Scroll Position Updates

```csharp
public class InfiniteScrollView : ScrollView
{
    private readonly Throttler scrollThrottler;

    public InfiniteScrollView()
    {
        scrollThrottler = new Throttler(TimeSpan.FromMilliseconds(200));

        Scrolled += OnScrolled;
    }

    private void OnScrolled(object sender, ScrolledEventArgs e)
    {
        // Throttle scroll position updates
        scrollThrottler.Throttle(() =>
        {
            UpdateScrollPosition(e.ScrollY);
        });
    }

    private void UpdateScrollPosition(double scrollY)
    {
        Console.WriteLine($"Scroll position: {scrollY}");

        // Check if near bottom for infinite scroll
        if (scrollY >= ContentSize.Height - Height - 100)
        {
            LoadMoreItems();
        }
    }
}
```

### API Rate Limiting

```csharp
public class ApiClient
{
    private readonly Throttler apiThrottler;

    public ApiClient()
    {
        // Rate limit: Max 1 request per 500ms
        apiThrottler = new Throttler(TimeSpan.FromMilliseconds(500));
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        await apiThrottler.ThrottleAsync(async () =>
        {
            // Make API call
            var response = await httpClient.GetAsync(endpoint);
            return await response.Content.ReadFromJsonAsync<T>();
        });

        return default;
    }
}
```

### Real-time Data Saving

```csharp
public class RealtimeEditor : ContentView
{
    private readonly Throttler autoSaveThrottler;
    private Editor editor;

    public RealtimeEditor()
    {
        // Auto-save max once every 2 seconds
        autoSaveThrottler = new Throttler(TimeSpan.FromSeconds(2));

        editor = new Editor();
        editor.TextChanged += OnTextChanged;

        Content = editor;
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        autoSaveThrottler.Throttle(async () =>
        {
            await SaveContentAsync(e.NewTextValue);
            Console.WriteLine("Content auto-saved");
        });
    }

    private async Task SaveContentAsync(string content)
    {
        // Save to database or API
        await Task.Delay(100); // Simulate save
    }
}
```

### Location Updates

```csharp
public class LocationTracker : IDisposable
{
    private readonly Throttler locationThrottler;

    public LocationTracker()
    {
        // Update location max once per 5 seconds
        locationThrottler = new Throttler(TimeSpan.FromSeconds(5));
    }

    public async Task OnLocationChanged(Location location)
    {
        await locationThrottler.ThrottleAsync(async () =>
        {
            Console.WriteLine($"Updating location: {location.Latitude}, {location.Longitude}");
            await SaveLocationToServerAsync(location);
        });
    }

    private async Task SaveLocationToServerAsync(Location location)
    {
        // Send to server
        await Task.Delay(200); // Simulate network call
    }

    public void Dispose()
    {
        locationThrottler?.Dispose();
    }
}
```

### Analytics Event Tracking

```csharp
public class AnalyticsService
{
    private readonly Dictionary<string, Throttler> eventThrottlers = new();

    public void TrackEvent(string eventName, Dictionary<string, object> properties = null)
    {
        // Get or create throttler for this event type
        if (!eventThrottlers.ContainsKey(eventName))
        {
            eventThrottlers[eventName] = new Throttler(TimeSpan.FromSeconds(10));
        }

        var throttler = eventThrottlers[eventName];

        throttler.Throttle(() =>
        {
            // Send to analytics service
            Console.WriteLine($"Tracked event: {eventName}");
            SendToAnalyticsAsync(eventName, properties);
        });
    }

    private async Task SendToAnalyticsAsync(string eventName, Dictionary<string, object> properties)
    {
        // Send to analytics platform
        await Task.Delay(100);
    }
}
```

### Manual Reset Example

```csharp
public class GameViewModel
{
    private readonly Throttler fireThrottler;

    public GameViewModel()
    {
        // Player can fire once per second
        fireThrottler = new Throttler(TimeSpan.FromSeconds(1));
    }

    public void OnFireButtonPressed()
    {
        fireThrottler.Throttle(() =>
        {
            FireWeapon();
        });
    }

    public void OnPowerUpCollected()
    {
        // Power-up resets fire cooldown
        fireThrottler.Reset();
        Console.WriteLine("Fire cooldown reset!");
    }

    private void FireWeapon()
    {
        Console.WriteLine("Weapon fired!");
    }
}
```

---

## Throttler vs Debouncer

### When to Use Throttler

✅ Use Throttler when you want to:
- **Rate limit** API calls
- **Throttle rapid events** but ensure they execute periodically
- **Limit execution frequency** for performance
- **Control resource usage** (network, CPU, etc.)

Examples:
- Scroll position updates
- Mouse move tracking
- Real-time saving
- Button click rate limiting

### When to Use Debouncer

✅ Use Debouncer when you want to:
- **Wait for user to finish typing** before searching
- **Wait for inactivity** before executing
- **Batch rapid changes** into a single action

Examples:
- Search boxes
- Auto-complete
- Form validation
- Window resize

### Visual Comparison

```
Input:     ||||||||  pause  |||  pause  ||||
           ↓
Throttler: |        |        |          |      (executes at intervals)
Debouncer:                   |          |      (executes after pauses)
```

---

## Best Practices

1. **Choose appropriate interval**:
   - API rate limiting: 500ms - 2 seconds
   - Scroll updates: 100-200ms
   - Auto-save: 2-5 seconds
   - Button clicks: 500ms - 1 second

2. **Dispose properly**: Always dispose throttlers to prevent memory leaks

3. **Use per-operation**: Create separate throttlers for different operations

4. **Consider user experience**: Too aggressive throttling feels unresponsive

5. **Check CanExecute**: Use for UI feedback (e.g., disable button during throttle period)

6. **Reset when needed**: Use `Reset()` for game mechanics or special scenarios

---

## Performance Considerations

- **Memory**: Minimal overhead - tracks only last execution time
- **Thread-safe**: Safe to call from multiple threads
- **No polling**: Uses efficient timestamp comparison
- **Fast execution**: First call executes immediately with no delay

---

## Common Use Cases

### 1. Prevent Accidental Double-Clicks

```csharp
private readonly Throttler clickThrottler = new(TimeSpan.FromMilliseconds(500));

void OnButtonClicked()
{
    clickThrottler.Throttle(async () =>
    {
        await ProcessPaymentAsync();
    });
}
```

### 2. Limit Network Requests

```csharp
private readonly Throttler networkThrottler = new(TimeSpan.FromSeconds(1));

void OnDataNeeded()
{
    networkThrottler.Throttle(async () =>
    {
        await FetchDataFromApiAsync();
    });
}
```

### 3. Performance Optimization

```csharp
private readonly Throttler renderThrottler = new(TimeSpan.FromMilliseconds(16)); // ~60 FPS

void OnDataChanged()
{
    renderThrottler.Throttle(() =>
    {
        RefreshUI();
    });
}
```

---

## See Also

- [Debouncer](Debouncer.md) - Delay execution until inactivity
- [DispatchHelper](DispatchHelper.md) - UI thread dispatching
- [TaskExtensions](TaskExtensions.md) - Task utilities
