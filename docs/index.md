# GamaLearn.Maui Documentation

Welcome to the official documentation for **GamaLearn.Maui** - a comprehensive collection of utilities and components for building robust .NET MAUI applications.

## 🚀 Getting Started

GamaLearn.Maui provides essential building blocks for .NET MAUI development, including:

- **Threading Utilities** - Debouncing and throttling for efficient UI updates
- **Collections** - High-performance observable collections with bulk operations
- **Guard Clauses** - Robust argument validation utilities
- **Services** - Cross-platform services like app rating prompts
- **Extensions** - Helpful extension methods for common tasks

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package GamaLearn.Maui.Core
```

Or via Package Manager Console:

```powershell
Install-Package GamaLearn.Maui.Core
```

## 📚 Documentation Sections

### [API Reference](api/README.md)
Comprehensive API documentation for all classes, methods, and properties.

## 🎯 Quick Example

```csharp
using GamaLearn;

// Use Debouncer to delay search execution
var debouncer = new Debouncer();
await debouncer.DebounceAsync(async () =>
{
    await PerformSearchAsync(searchText);
}, TimeSpan.FromMilliseconds(500));

// Use ObservableRangeCollection for efficient bulk updates
var collection = new ObservableRangeCollection<Item>();
collection.AddRange(newItems); // Single notification for multiple items
```

## 🌐 Platform Support

- ✅ Windows (WinUI 3)
- ✅ Android
- ✅ iOS
- ✅ MacCatalyst

**Requirements:**
- .NET 9.0 or later
- .NET MAUI workload installed

## 🔗 Links

- [GitHub Repository](https://github.com/GamaLearn/GamaLearn.Maui)
- [NuGet Package](https://www.nuget.org/packages/GamaLearn.Maui.Core)
- [Report an Issue](https://github.com/GamaLearn/GamaLearn.Maui/issues)

## 📄 License

GamaLearn.Maui is licensed under the MIT License.

---

© GamaLearn - All rights reserved
