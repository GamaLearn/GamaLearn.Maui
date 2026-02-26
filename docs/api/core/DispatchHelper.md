# DispatchHelper

Helper class for dispatching operations to the UI thread in MAUI applications.

## Overview

`DispatchHelper` provides utilities for safely executing code on the main/UI thread, essential for updating UI elements from background threads.

## Key Features

- **Thread-Safe UI Updates** - Ensure UI operations run on the correct thread
- **Async Support** - Both synchronous and asynchronous dispatch methods
- **Dispatcher Access** - Easy access to the current dispatcher
- **Queue Management** - Efficient queuing of UI operations

## Common Use Cases

### Dispatch to UI Thread

```csharp
using GamaLearn;

// Dispatch synchronous action
DispatchHelper.Dispatch(() =>
{
    Label.Text = "Updated from background thread";
});

// Dispatch asynchronous action
await DispatchHelper.DispatchAsync(async () =>
{
    await UpdateUIAsync();
});
```

### Check Current Thread

```csharp
if (DispatchHelper.IsMainThread)
{
    // Already on UI thread
    UpdateUI();
}
else
{
    // Dispatch to UI thread
    DispatchHelper.Dispatch(() => UpdateUI());
}
```

## See Also

- [TaskExtensions](TaskExtensions.md) - Task utility extensions
- [Debouncer](Debouncer.md) - Delay rapid method calls
- [Throttler](Throttler.md) - Limit execution frequency
