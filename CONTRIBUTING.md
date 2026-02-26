# Contributing to GamaLearn.Maui

Thank you for your interest in contributing to GamaLearn.Maui! This document provides guidelines and instructions for contributing to the project.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Coding Standards](#coding-standards)
- [Submitting Changes](#submitting-changes)
- [Reporting Issues](#reporting-issues)

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for all contributors.

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/YOUR-USERNAME/GamaLearn.Maui.git`
3. Create a feature branch: `git checkout -b feature/your-feature-name`
4. Make your changes
5. Run tests: `dotnet test`
6. Commit your changes: `git commit -m "Add your feature"`
7. Push to your fork: `git push origin feature/your-feature-name`
8. Open a Pull Request

## Development Setup

### Prerequisites

- .NET 9.0 SDK or later
- .NET MAUI workload: `dotnet workload install maui`
- Visual Studio 2022 or VS Code with C# extension

### Building the Project

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Pack NuGet packages
dotnet pack
```

## Coding Standards

### Code Style

- **Naming Conventions**
  - `PascalCase` for public members, types, and methods
  - `camelCase` for private fields (prefix with `_` for instance fields)
  - `UPPER_CASE` for constants
  - Meaningful, descriptive names (avoid abbreviations)

- **Code Organization**
  - Keep files focused (one primary type per file)
  - Group related functionality together
  - Use regions sparingly (only for large files)

- **Documentation**
  - XML documentation comments for all public APIs
  - Include `<summary>`, `<param>`, `<returns>`, and `<example>` tags
  - Keep comments concise and up-to-date

### Best Practices

- **Null Safety**
  - Use nullable reference types (`string?` for nullable)
  - Guard against null with `Guard.Against.Null()`
  - Prefer `??` and `?.` operators

- **Async/Await**
  - Always use `async`/`await` (never `.Result` or `.Wait()`)
  - Add `Async` suffix to async method names
  - Use `ConfigureAwait(false)` in library code

- **Performance**
  - Prefer `StringBuilder` for string concatenation in loops
  - Use `ObservableRangeCollection` for bulk updates
  - Avoid LINQ in hot paths (use `for` loops instead)

- **Testing**
  - Write unit tests for new features
  - Aim for >80% code coverage
  - Use meaningful test names: `MethodName_Scenario_ExpectedResult`

### Code Quality

- Ensure all tests pass before submitting PR
- No compiler warnings allowed
- Follow existing patterns in the codebase
- Keep pull requests focused and atomic (one feature/fix per PR)

## Submitting Changes

1. **Test your changes** - Ensure all tests pass
2. **Update documentation** - Update relevant markdown files in `docs/`
3. **Follow commit conventions**:
   ```
   feat: Add new feature
   fix: Fix bug in component
   docs: Update documentation
   refactor: Refactor code
   test: Add tests
   ```
4. **Create a Pull Request** with:
   - Clear description of changes
   - Reference to related issues
   - Screenshots (for UI changes)

## Reporting Issues

When reporting issues, please include:

- **Description** - Clear description of the issue
- **Steps to Reproduce** - Detailed steps to reproduce the problem
- **Expected Behavior** - What you expected to happen
- **Actual Behavior** - What actually happened
- **Environment** - OS, .NET version, MAUI version
- **Code Sample** - Minimal reproducible example

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to GamaLearn.Maui! 🚀
