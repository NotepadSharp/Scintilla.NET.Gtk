# Platform Abstraction Layer

This directory contains the platform abstraction layer for Scintilla.NET.Gtk, enabling cross-platform support for Linux, Windows, and macOS.

## Architecture Overview

```
Platform/
├── IScintillaNative.cs          # Interface for Scintilla native operations
├── ILexillaNative.cs            # Interface for Lexilla native operations
├── PlatformDetector.cs          # Runtime platform detection
├── NativeLibraryLoader.cs       # Factory for native library instances
├── ScintillaNativeLinux.cs      # Linux implementation (libscintilla.so)
├── ScintillaNativeWindows.cs    # Windows implementation (scintilla.dll)
├── ScintillaNativeMac.cs        # macOS implementation (libscintilla.dylib)
├── LexillaNativeLinux.cs        # Linux Lexilla (liblexilla.so)
├── LexillaNativeWindows.cs      # Windows Lexilla (lexilla.dll)
└── LexillaNativeMac.cs          # macOS Lexilla (liblexilla.dylib)
```

## Design Patterns

### Factory Pattern
`NativeLibraryLoader` uses the factory pattern to create platform-specific native library instances based on runtime detection.

### Singleton Pattern
Native library instances are cached as singletons to avoid repeated initialization and improve performance.

### Interface Segregation
Separate interfaces (`IScintillaNative` and `ILexillaNative`) for different native library concerns.

## Platform Detection

The `PlatformDetector` class uses `System.Runtime.InteropServices.RuntimeInformation` to detect:
- Operating System (Linux, Windows, macOS)
- Processor Architecture (x64, x86, ARM64, ARM)
- Runtime Identifier (e.g., "linux-x64", "win-x64", "osx-arm64")

Detection happens once at static initialization and is thread-safe.

## Native Library Naming Conventions

| Platform | Architecture | Scintilla | Lexilla |
|----------|--------------|-----------|---------|
| Linux    | x64 | `libscintilla.so` | `liblexilla.so` |
| Windows  | x64 | `scintilla.dll` | `lexilla.dll` |
| Windows  | x86 | `scintilla.dll` | `lexilla.dll` |
| macOS    | x64 | `libscintilla.dylib` | `liblexilla.dylib` |
| macOS    | ARM64 | `libscintilla.dylib` | `liblexilla.dylib` |

## Usage

The abstraction layer is used internally by the main `Scintilla` and `Lexilla` classes:

```csharp
// Automatic platform detection (recommended)
var scintilla = new Scintilla();

// With explicit native implementation (testing/advanced scenarios)
var native = NativeLibraryLoader.CreateScintillaNative();
var scintilla = new Scintilla(native);
```

## Adding Support for New Platforms

To add support for a new platform:

1. **Create Native Implementation Classes**:
   ```csharp
   public class ScintillaNativeNewPlatform : IScintillaNative
   {
       // Implement interface methods with platform-specific P/Invoke
   }
   
   public class LexillaNativeNewPlatform : ILexillaNative
   {
       // Implement interface methods with platform-specific P/Invoke
   }
   ```

2. **Update PlatformDetector**:
   ```csharp
   public enum PlatformOS
   {
       // ... existing platforms
       NewPlatform
   }
   ```

3. **Update NativeLibraryLoader**:
   ```csharp
   _scintillaInstance = PlatformDetector.CurrentPlatform switch
   {
       // ... existing platforms
       PlatformOS.NewPlatform => new ScintillaNativeNewPlatform(),
       // ...
   };
   ```

4. **Update Build System**:
   - Add native library paths to `.csproj`
   - Add conditional ItemGroups to `.targets` file

## Thread Safety

- `PlatformDetector` is initialized once at static construction (thread-safe)
- `NativeLibraryLoader` uses locks to ensure thread-safe singleton creation
- Native library instances are immutable after creation

## Performance Considerations

- Platform detection: One-time cost at startup (~microseconds)
- Native library creation: One-time singleton initialization per library type
- Message passing: No overhead compared to direct P/Invoke (delegates to native implementation)

## Testing

To test platform-specific implementations:

```csharp
using ScintillaNet.Gtk.Platform;

// Check detected platform
Assert.Equal(PlatformOS.Linux, PlatformDetector.CurrentPlatform);

// Test native library creation
var native = NativeLibraryLoader.CreateScintillaNative();
Assert.NotNull(native);

// Test Scintilla creation
var ptr = native.CreateScintilla();
Assert.NotEqual(IntPtr.Zero, ptr);

// Test message sending
var result = native.SendMessage(ptr, SCI_GETLENGTH, IntPtr.Zero, IntPtr.Zero);
Assert.True(result.ToInt32() >= 0);
```

## Debugging

Enable verbose platform information:

```csharp
using ScintillaNet.Gtk.Platform;
using System.Runtime.InteropServices;

Console.WriteLine($"Platform: {PlatformDetector.CurrentPlatform}");
Console.WriteLine($"Runtime ID: {PlatformDetector.RuntimeIdentifier}");
Console.WriteLine($"Is Linux: {PlatformDetector.IsLinux}");
Console.WriteLine($"Is Windows: {PlatformDetector.IsWindows}");
Console.WriteLine($"Is macOS: {PlatformDetector.IsMacOS}");
Console.WriteLine($"Process Arch: {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"OS Description: {RuntimeInformation.OSDescription}");
```

## Known Issues

### Windows
- Requires GTK+ 3 runtime to be installed
- Native DLLs must match application architecture (x64/x86)
- Both 32-bit (x86) and 64-bit (x64) architectures supported
- Place x86 DLLs in `runtimes/win-x86/native/`
- Place x64 DLLs in `runtimes/win-x64/native/`

### macOS
- Universal binaries (fat binaries) not yet supported - use architecture-specific libraries
- May require code signing for distribution

### Linux
- No known issues - fully tested and supported

## License

Same as parent project (MIT License).
