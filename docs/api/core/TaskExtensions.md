# TaskExtensions

Extension methods for working with `Task` and `Task<T>` in MAUI applications.

## Overview

`TaskExtensions` provides utility methods for managing asynchronous operations, including dispatching to the UI thread and handling task continuations safely.

## Key Features

- **UI Thread Dispatching** - Automatically dispatch task results to the main thread
- **Safe Continuations** - Handle exceptions and cancellations gracefully
- **Fire and Forget** - Execute tasks without awaiting
- **Timeout Handling** - Add timeout capabilities to tasks

## Common Use Cases

### Dispatch to UI Thread

```csharp
using GamaLearn;

// Execute task result on UI thread
await SomeAsyncOperation().DispatchAsync(dispatcher);
```

### Fire and Forget

```csharp
// Execute without blocking
SomeAsyncOperation().FireAndForget();
```

## See Also

- [Debouncer](Debouncer.md) - Delay rapid method calls
- [Throttler](Throttler.md) - Limit execution frequency
- [DispatchHelper](DispatchHelper.md) - UI thread dispatching utilities
