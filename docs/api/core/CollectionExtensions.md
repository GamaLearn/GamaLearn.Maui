# CollectionExtensions

Extension methods for working with collections in MAUI applications.

## Overview

`CollectionExtensions` provides utility methods for common collection operations, making it easier to work with lists, arrays, and other collection types.

## Key Features

- **Bulk Operations** - Add/remove multiple items efficiently
- **Null Safety** - Safe operations with null checking
- **LINQ Integration** - Seamless integration with LINQ queries
- **Performance Optimized** - Efficient implementations for large collections

## Common Use Cases

### Add Multiple Items

```csharp
using GamaLearn;

var list = new List<string>();
list.AddRange(new[] { "item1", "item2", "item3" });
```

### Safe Operations

```csharp
// Safe null checking
if (collection.IsNullOrEmpty())
{
    // Handle empty collection
}
```

### Bulk Modifications

```csharp
// Remove multiple items matching predicate
collection.RemoveWhere(x => x.IsExpired);
```

## See Also

- [ObservableRangeCollection](ObservableRangeCollection.md) - Observable collection with bulk operations
- [ReactiveCollection](ReactiveCollection.md) - Reactive collection with change tracking
- [Guard](Guard.md) - Argument validation utilities
