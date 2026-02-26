# GamaLearn.Maui.Core API Reference

Comprehensive API documentation for GamaLearn.Maui.Core library - essential utilities and foundational components for .NET MAUI applications.

---

## 📚 API Reference by Category

### 🔄 Threading & Utilities

Utilities for managing asynchronous operations and UI thread synchronization.

- **[Debouncer](core/Debouncer.md)** - Delay method execution until a pause in calls
  - Perfect for search-as-you-type scenarios
  - Prevents excessive API calls or UI updates

- **[Throttler](core/Throttler.md)** - Limit method execution frequency
  - Rate-limit expensive operations
  - Prevent overwhelming APIs or resources

- **[DispatchHelper](core/DispatchHelper.md)** - UI thread dispatching utilities
  - Safely execute code on the main thread
  - Thread-safe UI updates from background threads

- **[TaskExtensions](core/TaskExtensions.md)** - Task utility extension methods
  - Fire-and-forget task execution
  - Timeout handling
  - UI thread continuations

### 📦 Collections

High-performance observable collections with advanced features.

- **[ObservableRangeCollection&lt;T&gt;](core/ObservableRangeCollection.md)** - Observable collection with bulk operations
  - Add/remove multiple items with single notification
  - Replace entire collection efficiently
  - Significantly improves UI performance with large datasets

- **[ReactiveCollection&lt;T&gt;](core/ReactiveCollection.md)** - Advanced reactive collection with filtering and sorting
  - Real-time filtering and sorting
  - Detailed change tracking (add, remove, replace, move)
  - Thread-safe operations
  - Automatic UI updates via binding
  - Perfect for dynamic lists with search/filter functionality

- **[CollectionExtensions](core/CollectionExtensions.md)** - Collection utility extension methods
  - Bulk operations
  - Null-safe operations
  - LINQ integration helpers

### ✅ Validation

Robust argument validation and guard clauses.

- **[Guard](core/Guard.md)** - Argument validation utilities
  - Guard against null, empty strings, out-of-range values
  - Fluent API: `Guard.Against.Null(value, nameof(value))`
  - Clear exception messages
  - Reduces boilerplate validation code

### 🌟 Services

Cross-platform services for common app scenarios.

- **[AppRatingService](core/AppRatingService.md)** - Cross-platform in-app rating prompts
  - Native rating dialogs on iOS, Android, macOS, Windows
  - Configurable trigger logic (session counts, days elapsed)
  - Respectful rating prompts (honors user preferences)
  - Full event tracking for analytics

---

## 🔗 Quick Links

- **[Getting Started Guide](../index.md)** - Installation and quick start
- **[GitHub Repository](https://github.com/GamaLearn/GamaLearn.Maui)** - Source code and issues
- **[NuGet Package](https://www.nuget.org/packages/GamaLearn.Maui.Core)** - Latest releases

---

## 🖥️ Platform Support

All components are designed for cross-platform compatibility:

| Platform | Support | Notes |
|----------|---------|-------|
| **Android** | ✅ Full | API 26+ (Android 8.0+) |
| **iOS** | ✅ Full | iOS 15.0+ |
| **MacCatalyst** | ✅ Full | macOS 11.0+ |
| **Windows** | ✅ Full | Windows 10 (Build 19041+) |

### Requirements

- **.NET 9.0 or later**
- **.NET MAUI workload** installed
- **Target framework**: `net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst`, `net9.0-windows`

---

## 💡 Usage Examples

### Debouncing Search Input

```csharp
var debouncer = new Debouncer();

// Delay search until user stops typing for 300ms
await debouncer.DebounceAsync(async () =>
{
    await PerformSearchAsync(searchText);
}, TimeSpan.FromMilliseconds(300));
```

### Reactive Collection with Filtering

```csharp
var products = new ReactiveCollection<Product>();
products.Load(allProducts);

// Apply filter - UI updates automatically
products.Filter(p => p.IsInStock && p.Price < 100);

// Bind in XAML: ItemsSource="{Binding Products.View}"
```

### Bulk Collection Updates

```csharp
var items = new ObservableRangeCollection<string>();

// Single notification for multiple items
items.AddRange(new[] { "Item1", "Item2", "Item3" });
items.ReplaceRange(newItems);
```

### Argument Validation

```csharp
public void ProcessData(string name, int count)
{
    Guard.Against.NullOrWhiteSpace(name, nameof(name));
    Guard.Against.NegativeOrZero(count, nameof(count));

    // Process safely...
}
```

---

## 📖 Additional Resources

- **[Samples](https://github.com/GamaLearn/GamaLearn.Maui/tree/main/samples)** - Example applications
- **[Contributing Guide](https://github.com/GamaLearn/GamaLearn.Maui/blob/main/CONTRIBUTING.md)** - How to contribute
- **[Release Notes](https://github.com/GamaLearn/GamaLearn.Maui/releases)** - What's new

---

© GamaLearn - All rights reserved
