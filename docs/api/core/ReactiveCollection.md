# ReactiveCollection

An advanced observable collection with detailed change tracking and reactive programming support.

## Overview

`ReactiveCollection<T>` extends the standard observable collection pattern with fine-grained change notifications, making it ideal for reactive UI scenarios and complex data binding.

## Key Features

- **Detailed Change Tracking** - Track individual item changes, additions, removals, and moves
- **Batch Operations** - Perform multiple changes with a single notification
- **Reactive Extensions** - Integration with reactive programming patterns
- **Performance Optimized** - Efficient for large collections with frequent updates

## Common Use Cases

### Create and Use

```csharp
using GamaLearn;

var collection = new ReactiveCollection<Item>();

// Subscribe to changes
collection.CollectionChanged += (s, e) =>
{
    // Handle collection changes
};

// Add items
collection.Add(new Item());
collection.AddRange(items);
```

### Batch Updates

```csharp
using (collection.SuspendNotifications())
{
    // Multiple operations with single notification
    collection.Clear();
    collection.AddRange(newItems);
}
```

### Change Tracking

```csharp
// Track detailed changes
collection.ItemChanged += (s, e) =>
{
    var changedItem = e.Item;
    var propertyName = e.PropertyName;
    // React to item property changes
};
```

## See Also

- [ObservableRangeCollection](ObservableRangeCollection.md) - Simplified observable collection with bulk operations
- [CollectionExtensions](CollectionExtensions.md) - Collection utility methods
- [Debouncer](Debouncer.md) - Debounce collection updates
