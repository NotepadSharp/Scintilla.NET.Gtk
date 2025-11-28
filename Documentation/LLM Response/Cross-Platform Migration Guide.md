# Cross-Platform Migration Guide

## Overview

Scintilla.NET.Gtk has been successfully refactored to support **Windows, Linux, and macOS** platforms while maintaining backward compatibility. The library now automatically detects the runtime platform and loads the appropriate native libraries.

## What's Changed

### Architecture

The library now uses a **platform abstraction layer** that:
- Automatically detects the operating system at runtime
- Loads platform-specific native libraries (`.so` for Linux, `.dll` for Windows, `.dylib` for macOS)
- Maintains 100% API compatibility with previous versions

### Key Components

1. **Platform Detection** (`Platform/PlatformDetector.cs`)
   - Detects Linux, Windows, macOS, and architecture (x64, ARM64)
   - Provides `PlatformDetector.CurrentPlatform` and `PlatformDetector.RuntimeIdentifier`

2. **Native Interfaces** (`Platform/IScintillaNative.cs`, `Platform/ILexillaNative.cs`)
   - Abstract interfaces for native library operations
   - Platform-specific implementations for each OS

3. **Native Library Loader** (`Platform/NativeLibraryLoader.cs`)
   - Factory pattern for creating platform-specific instances
   - Singleton pattern for efficiency
   - Thread-safe implementation

## Migration Steps

### For Existing Users (Linux Only)

**Good news: No code changes required!** Your existing code will continue to work exactly as before.

```csharp
// This code works unchanged
var scintilla = new Scintilla();
scintilla.Text = "Hello, World!";
```

The library automatically detects it's running on Linux and loads the appropriate `.so` files.

### For New Cross-Platform Projects

1. **Add the NuGet Package** (when published):
   ```bash
   dotnet add package Scintilla.NET.Gtk --version 2.0.0
   ```

2. **Use the Control** (same API across all platforms):
   ```csharp
   using ScintillaNet.Gtk;
   using Gtk;
   
   Application.Init();
   
   var window = new Window("Scintilla Editor");
   var scintilla = new Scintilla();
   
   scintilla.Text = "// Cross-platform code editor\n";
   scintilla.StyleSetFont(Style.Default, "Consolas");
   
   window.Add(scintilla);
   window.ShowAll();
   
   Application.Run();
   ```

3. **Check Platform at Runtime** (optional):
   ```csharp
   using ScintillaNet.Gtk.Platform;
   
   if (PlatformDetector.IsWindows)
   {
       Console.WriteLine("Running on Windows");
   }
   else if (PlatformDetector.IsLinux)
   {
       Console.WriteLine("Running on Linux");
   }
   else if (PlatformDetector.IsMacOS)
   {
       Console.WriteLine("Running on macOS");
   }
   
   Console.WriteLine($"Runtime: {PlatformDetector.RuntimeIdentifier}");
   ```

## Native Library Requirements

### Linux (Fully Supported)
- **Files**: `libscintilla.so`, `liblexilla.so`
- **Location**: Automatically copied to output directory
- **Dependencies**: GTK+ 3, GLib 2.0

### Windows (Platform Ready - Native Binaries Needed)
- **Files**: `scintilla.dll`, `lexilla.dll` (to be provided)
- **Architectures**: Both x64 (64-bit) and x86 (32-bit) supported
- **Location**: Automatically copied to output directory
- **Dependencies**: GTK+ 3 for Windows, GLib runtime

### macOS (Platform Ready - Native Binaries Needed)
- **Files**: `libscintilla.dylib`, `liblexilla.dylib` (to be provided)
- **Architectures**: Both x64 (Intel) and ARM64 (Apple Silicon) supported
- **Location**: Automatically copied to output directory
- **Dependencies**: GTK+ 3 via Homebrew or MacPorts

## Building Native Libraries

### For Windows

1. **Prerequisites**:
   - Visual Studio 2022 with C++ tools
   - GTK+ 3 for Windows ([gtk.org](https://www.gtk.org/docs/installations/windows))
   - Scintilla and Lexilla source code

2. **Build Steps**:
   ```bash
   # Clone Scintilla
   git clone https://github.com/ScintillaOrg/scintilla.git
   git clone https://github.com/ScintillaOrg/lexilla.git
   
   # Build with GTK support
   cd scintilla/gtk
   # Follow Scintilla build instructions for Windows + GTK
   ```

3. **Output**: 
   - Place 64-bit DLLs in `runtimes/win-x64/native/`
   - Place 32-bit DLLs in `runtimes/win-x86/native/`

### For macOS

1. **Prerequisites**:
   ```bash
   brew install gtk+3 pkg-config
   ```

2. **Build Steps**:
   ```bash
   # Clone Scintilla
   git clone https://github.com/ScintillaOrg/scintilla.git
   git clone https://github.com/ScintillaOrg/lexilla.git
   
   # Build for Intel (x64)
   cd scintilla/gtk
   make
   
   # Build for Apple Silicon (ARM64)
   make clean
   arch -arm64 make
   ```

3. **Output**: Place dylibs in appropriate `runtimes/osx-*/native/` folders

## Testing

### Running Tests

```bash
# Build all
dotnet build

# Run on current platform
dotnet run --project TestApp/TestApp.csproj

# Run with specific runtime
dotnet run --project TestApp/TestApp.csproj --runtime linux-x64
dotnet run --project TestApp/TestApp.csproj --runtime win-x64
dotnet run --project TestApp/TestApp.csproj --runtime osx-x64
```

### Platform-Specific Testing

Create a simple test to verify the platform abstraction:

```csharp
using System;
using ScintillaNet.Gtk;
using ScintillaNet.Gtk.Platform;
using Gtk;

class Program
{
    static void Main()
    {
        Console.WriteLine($"Platform: {PlatformDetector.CurrentPlatform}");
        Console.WriteLine($"Runtime ID: {PlatformDetector.RuntimeIdentifier}");
        
        Application.Init();
        
        var window = new Window($"Scintilla on {PlatformDetector.CurrentPlatform}");
        var scintilla = new Scintilla();
        
        scintilla.Text = $"// Running on {PlatformDetector.CurrentPlatform}\n";
        scintilla.Text += $"// Runtime: {PlatformDetector.RuntimeIdentifier}\n";
        scintilla.Text += "Console.WriteLine(\"Hello, cross-platform world!\");\n";
        
        window.Add(scintilla);
        window.SetDefaultSize(800, 600);
        window.DeleteEvent += (o, args) => Application.Quit();
        window.ShowAll();
        
        Application.Run();
    }
}
```

## Troubleshooting

### "Native library not found" Errors

**Symptom**: `DllNotFoundException` or similar errors when running the application.

**Solutions**:
1. **Linux**: Ensure `libscintilla.so` and `liblexilla.so` are in the output directory or system library path
2. **Windows**: Ensure GTK+ 3 runtime is installed and `scintilla.dll`/`lexilla.dll` are present
3. **macOS**: Ensure GTK+ 3 is installed via Homebrew (`brew install gtk+3`)

### Platform Detection Issues

**Symptom**: Wrong platform detected or "Platform not supported" error.

**Check**:
```csharp
using ScintillaNet.Gtk.Platform;
using System.Runtime.InteropServices;

Console.WriteLine($"Current Platform: {PlatformDetector.CurrentPlatform}");
Console.WriteLine($"Runtime ID: {PlatformDetector.RuntimeIdentifier}");
Console.WriteLine($"Process Architecture: {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"OS Description: {RuntimeInformation.OSDescription}");
```

### Native Library Architecture Mismatch

**Symptom**: `BadImageFormatException` or similar errors.

**Solution**: Ensure the native library architecture (x64, x86, ARM64) matches your application's target architecture.

## API Compatibility

### Breaking Changes
**None!** The public API is 100% backward compatible.

### New APIs

```csharp
// Platform detection
PlatformDetector.CurrentPlatform // PlatformOS enum
PlatformDetector.RuntimeIdentifier // "linux-x64", "win-x64", etc.
PlatformDetector.IsLinux // bool
PlatformDetector.IsWindows // bool
PlatformDetector.IsMacOS // bool
```

### Internal Changes

The following changes are internal and do not affect public API:
- `Scintilla` constructor now accepts optional `IScintillaNative` parameter (internal use only)
- `Lexilla` constructor now accepts optional `ILexillaNative` parameter (internal use only)
- Direct P/Invoke calls replaced with abstraction layer

## Performance

The platform abstraction layer has **negligible performance impact**:
- Platform detection happens once at startup
- Native library instances are singletons (no repeated initialization)
- Message passing to native libraries is unchanged (same overhead as before)

## Contributing

### Adding Support for New Platforms

1. Implement `IScintillaNative` for your platform
2. Implement `ILexillaNative` for your platform
3. Update `NativeLibraryLoader` to include your platform
4. Add the platform to `PlatformOS` enum in `PlatformDetector`
5. Update `.targets` file with native library paths
6. Add native libraries to `runtimes/{rid}/native/` structure

## Version History

### Version 2.0.0 (Cross-Platform Release)
- ✅ **Phase 1**: Abstraction layer implementation (Linux support maintained)
- ✅ **Phase 2**: Windows platform support added
- ✅ **Phase 3**: macOS platform support added (x64 and ARM64)
- ✅ **Phase 4**: Enhanced build system with runtime-specific packaging
- ✅ **Phase 5**: Documentation and testing infrastructure

### Version 1.0.11.5320 (Previous)
- Linux-only support
- Direct P/Invoke to libscintilla.so

## Additional Resources

- [Scintilla Documentation](https://www.scintilla.org/ScintillaDoc.html)
- [GTK# Documentation](https://www.mono-project.com/docs/gui/gtksharp/)
- [GitHub Repository](https://github.com/pdavis68/Scintilla.NET.Gtk)
- [Issue Tracker](https://github.com/pdavis68/Scintilla.NET.Gtk/issues)

## Support

For questions, issues, or contributions:
- **Issues**: [GitHub Issues](https://github.com/pdavis68/Scintilla.NET.Gtk/issues)
- **Discussions**: [GitHub Discussions](https://github.com/pdavis68/Scintilla.NET.Gtk/discussions)

---

*Last Updated: December 2025*
