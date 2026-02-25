# GamaLearn.Maui.Core

Core utilities and foundational components for .NET MAUI applications.

---

## Overview

GamaLearn.Maui.Core provides a comprehensive set of utilities, extension methods, and helper classes designed to simplify common tasks in .NET MAUI development. This library focuses on productivity and code quality with thread-safe operations, reactive collections, validation helpers, and more.

---

## Features

### Extension Methods
- String manipulation and validation
- Collection operations
- Task and async helpers
- Type conversion utilities

### Threading Utilities
- **Debouncer**: Delay method execution until a pause in calls
- **Throttler**: Limit method execution frequency
- Thread-safe operations and synchronization helpers

### Reactive Collections
- **ObservableRangeCollection**: Enhanced observable collection with bulk operations
- Change notification optimization
- Thread-safe collection updates

### Guard Clauses
- Comprehensive argument validation
- Null checking
- Range validation
- String validation (null, empty, whitespace)
- Custom validation extensions

### Base Classes & Abstractions
- MVVM base classes
- Common interfaces and abstractions
- Reusable patterns for MAUI apps

### Platform Services
- **AppRatingService**: Cross-platform in-app rating prompts
  - Supports iOS, Android, macOS, and Windows
  - Native platform implementations
  - Configurable rating triggers

---

## Installation

Add the library to your .NET MAUI project:

```bash
dotnet add package GamaLearn.Maui.Core
```

Or via NuGet Package Manager:

```
Install-Package GamaLearn.Maui.Core
```

---

## Quick Start

### Using Extension Methods

```csharp
using GamaLearn.Extensions;

// String extensions
string text = "  Hello World  ";
var trimmed = text.TrimSafe(); // Safely trims with null check

// Collection extensions
var items = new List<int> { 1, 2, 3, 4, 5 };
var hasAny = items.IsNotEmpty(); // Returns true
```

### Using Guard Clauses

```csharp
using GamaLearn.Guards;

public void ProcessData(string data, int count)
{
    // Throws ArgumentNullException if data is null
    Guard.Against.Null(data, nameof(data));

    // Throws ArgumentException if data is empty or whitespace
    Guard.Against.NullOrWhiteSpace(data, nameof(data));

    // Throws ArgumentOutOfRangeException if count is negative
    Guard.Against.Negative(count, nameof(count));
}
```

### Using Debouncer

```csharp
using GamaLearn.Utilities;

// Create a debouncer with 500ms delay
var debouncer = new Debouncer(TimeSpan.FromMilliseconds(500));

// In a search box TextChanged event
private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
{
    // Only executes search after user stops typing for 500ms
    debouncer.Debounce(() => PerformSearch(e.NewTextValue));
}
```

### Using Throttler

```csharp
using GamaLearn.Utilities;

// Create a throttler that allows one call per second
var throttler = new Throttler(TimeSpan.FromSeconds(1));

// In a button click handler
private void OnButtonClicked(object sender, EventArgs e)
{
    // Only executes once per second, even if clicked multiple times
    throttler.Throttle(() => PerformAction());
}
```

### Using ObservableRangeCollection

```csharp
using GamaLearn.Collections;

// Create an observable collection with bulk operation support
var items = new ObservableRangeCollection<string>();

// Add multiple items with a single notification
items.AddRange(new[] { "Item1", "Item2", "Item3" });

// Replace entire collection efficiently
items.ReplaceRange(newItems);

// Remove multiple items at once
items.RemoveRange(itemsToRemove);
```

### Using AppRatingService

```csharp
using GamaLearn.Services;

// In your MauiProgram.cs
builder.Services.AddSingleton<IAppRatingService, AppRatingService>();

// In your ViewModel or Page
public class MainViewModel
{
    private readonly IAppRatingService _ratingService;

    public MainViewModel(IAppRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    public async Task RequestRating()
    {
        // Shows native rating dialog on supported platforms
        await _ratingService.RequestRatingAsync();
    }
}
```

---

## Documentation

Comprehensive API documentation is available:

- **[API Reference](docs/api/README.md)** - Complete API documentation index

**Core Utilities:**
  - [Debouncer](docs/api/core/Debouncer.md) - Debounce rapid method calls
  - [Throttler](docs/api/core/Throttler.md) - Throttle method execution
  - [Guard](docs/api/core/Guard.md) - Argument validation
  - [ObservableRangeCollection](docs/api/core/ObservableRangeCollection.md) - Efficient bulk operations
  - [AppRatingService](docs/api/core/AppRatingService.md) - Cross-platform app rating

---

## Publishing Versions

This library uses automatic versioning with [MinVer](https://github.com/adamralph/minver) and GitHub Actions.

### How to Release a New Version

**1. Ensure all changes are committed and pushed to the main branch:**
```bash
git add .
git commit -m "Your commit message"
git push origin main
```

**2. Create and push a version tag:**

For a **stable release**:
```bash
git tag 1.0.0
git push origin 1.0.0
```

For a **preview/beta release**:
```bash
git tag 1.0.0-preview
git push origin 1.0.0-preview
```

**3. GitHub Actions will automatically:**
- Build the solution
- Run tests (when available)
- Pack NuGet packages with the version from the tag
- Publish to NuGet.org
- Create a GitHub Release with the packages

### Version Tag Guidelines

- Use semantic versioning: `MAJOR.MINOR.PATCH` (e.g., 1.0.0, 2.1.3)
- Preview versions: `MAJOR.MINOR.PATCH-preview` (e.g., 1.0.0-preview, 2.0.0-beta)
- Tags must be pushed via command line (not via GitHub web UI)
- Always commit and push changes before creating tags
- Tags should point to commits that already exist on the remote repository

### Deleting Tags (if needed)

```bash
# Delete local tag
git tag -d 1.0.0

# Delete remote tag
git push origin --delete 1.0.0
```

---

## Platform Support

- **iOS** 15.0+
- **Android** API 21+ (Android 5.0)
- **macOS** 10.15+
- **Windows** 10.0.19041.0+ (Windows 10, version 2004)

---

## Requirements

- .NET 9.0 or later
- .NET MAUI workload installed

To install the MAUI workload:
```bash
dotnet workload install maui
```

---

## Contributing

We welcome contributions! Please feel free to submit pull requests or open issues for bugs and feature requests.

---

## License

MIT License

Copyright (c) 2025 GamaLearn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
