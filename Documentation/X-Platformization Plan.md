# Cross-Platformization Plan for Scintilla.NET.Gtk

## Executive Summary

This document outlines a comprehensive strategy to transform **Scintilla.NET.Gtk** from a Linux-only GTK# library into a true cross-platform solution supporting **Linux**, **Windows**, and **macOS**, all while maintaining GTK# as the consistent UI framework across all platforms.

**Current State**: Linux-only with hardcoded `libscintilla.so` and `liblexilla.so` bindings
**Target State**: Cross-platform GTK# wrapper with platform-specific native library loading

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Current Architecture Analysis](#current-architecture-analysis)
3. [Proposed Architecture](#proposed-architecture)
4. [Implementation Phases](#implementation-phases)
5. [Technical Design](#technical-design)
6. [Platform-Specific Considerations](#platform-specific-considerations)
7. [Build System Changes](#build-system-changes)
8. [Testing Strategy](#testing-strategy)
9. [Migration Path](#migration-path)
10. [Risks and Mitigations](#risks-and-mitigations)

---

## Architecture Overview

### Design Goals

1. **Single Codebase**: One C# codebase that compiles and runs on all platforms
2. **GTK# Consistency**: Use GTK# as the UI framework on all platforms (Windows, Linux, macOS)
3. **Native Performance**: Direct P/Invoke to native Scintilla libraries on each platform
4. **Clean Abstraction**: Platform-specific code isolated behind well-defined interfaces
5. **Maintainability**: Minimal code duplication, clear separation of concerns
6. **Backward Compatibility**: Existing Linux code continues to work

---

## Current Architecture Analysis

### Current Components

```
Scintilla.NET.Gtk/
├── Scintilla.cs                    # Main control (hardcoded Linux P/Invoke)
├── Lexilla.cs                      # Lexer support (hardcoded Linux P/Invoke)
├── IScintillaLinux.cs             # Linux-specific interface
├── NativeEventHandling.cs          # Event processing
├── Helpers.cs                      # Key translation helpers
├── Collections/                    # Collection wrappers
│   ├── Style.cs                   # Uses Gdk.Color
│   ├── Margin.cs                  # Uses Gdk.Color
│   ├── Marker.cs                  # Uses Gdk.Color, Gtk.Image
│   ├── Indicator.cs               # Uses Gdk.Color
│   └── ...
├── GdkUtils/
│   └── ColorTranslator.cs         # Gdk.Color <-> int conversion
└── Native Libraries:
    ├── libscintilla.so            # Linux native library
    └── liblexilla.so              # Linux lexer library
```

### Key Dependencies

- **GtkSharp 3.24.24.38**: Cross-platform GTK# bindings (already multi-platform!)
- **Scintilla.NET.Abstractions 1.0.11**: Platform-agnostic abstractions
- **Native Libraries**: Platform-specific Scintilla builds

### Current Limitations

1. **Hardcoded DllImport**: All P/Invoke calls specify `"libscintilla"` directly
2. **Linux-Specific Interface**: `IScintillaLinux.cs` name implies platform limitation
3. **Single Native Binary**: Only `.so` files included
4. **No Platform Detection**: No runtime platform detection or library resolution
5. **Build Targets**: MSBuild targets only copy `.so` files

---

## Proposed Architecture

### High-Level Structure

```
Scintilla.NET.Gtk/
├── Scintilla.cs                    # Main control (platform-agnostic)
├── Collections/                    # Unchanged (already use Gdk.Color)
├── GdkUtils/                       # Unchanged
├── EventArguments/                 # Unchanged
├── Abstractions/
│   ├── IScintillaNative.cs        # Platform-agnostic native interface
│   └── ILexillaNative.cs          # Platform-agnostic lexer interface
├── Platform/
│   ├── PlatformDetector.cs        # Runtime platform detection
│   ├── INativeLibraryLoader.cs    # Library loading abstraction
│   ├── NativeLibraryLoader.cs     # Default loader implementation
│   ├── ScintillaNativeLinux.cs    # Linux P/Invoke wrapper
│   ├── ScintillaNativeWindows.cs  # Windows P/Invoke wrapper
│   ├── ScintillaNativeMac.cs      # macOS P/Invoke wrapper
│   ├── LexillaNativeLinux.cs      # Linux Lexilla wrapper
│   ├── LexillaNativeWindows.cs    # Windows Lexilla wrapper
│   └── LexillaNativeMac.cs        # macOS Lexilla wrapper
├── runtimes/
│   ├── linux-x64/
│   │   └── native/
│   │       ├── libscintilla.so
│   │       └── liblexilla.so
│   ├── win-x64/
│   │   └── native/
│   │       ├── scintilla.dll
│   │       └── lexilla.dll
│   ├── osx-x64/
│   │   └── native/
│   │       ├── libscintilla.dylib
│   │       └── liblexilla.dylib
│   └── osx-arm64/
│       └── native/
│           ├── libscintilla.dylib
│           └── liblexilla.dylib
└── build/
    └── Scintilla.NET.Gtk.targets  # Enhanced MSBuild targets
```

### Core Interfaces

#### IScintillaNative
```csharp
public interface IScintillaNative
{
    IntPtr CreateScintilla();
    IntPtr SendMessage(IntPtr ptr, int message, IntPtr wParam, IntPtr lParam);
    void Dispose();
}
```

#### ILexillaNative
```csharp
public interface ILexillaNative
{
    int GetLexerCount();
    string GetLexerName(uint index);
    IntPtr CreateLexer(string name);
    void Dispose();
}
```

### Platform Detector

```csharp
public enum PlatformOS
{
    Windows,
    Linux,
    MacOSX,
    Unknown
}

public static class PlatformDetector
{
    public static PlatformOS CurrentPlatform { get; }
    public static string RuntimeIdentifier { get; }
    
    static PlatformDetector()
    {
        // Detect platform using RuntimeInformation
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            CurrentPlatform = PlatformOS.Windows;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            CurrentPlatform = PlatformOS.Linux;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            CurrentPlatform = PlatformOS.MacOSX;
        else
            CurrentPlatform = PlatformOS.Unknown;
            
        RuntimeIdentifier = GetRuntimeIdentifier();
    }
    
    private static string GetRuntimeIdentifier()
    {
        // linux-x64, win-x64, osx-x64, osx-arm64
    }
}
```

---

## Implementation Phases

### Phase 1: Abstraction Layer (Week 1-2)

**Goal**: Introduce platform abstraction without breaking existing functionality

**Tasks**:
1. Create `IScintillaNative` and `ILexillaNative` interfaces
2. Create `PlatformDetector` static class
3. Wrap existing Linux P/Invoke in `ScintillaNativeLinux` class
4. Modify `Scintilla.cs` to use `IScintillaNative` instead of direct P/Invoke
5. Modify `Lexilla.cs` to use `ILexillaNative`
6. Add `NativeLibraryLoader` factory class
7. Unit test on Linux to ensure no regression

**Files to Modify**:
- `Scintilla.cs` (remove DllImport, add dependency injection)
- `Lexilla.cs` (remove DllImport, add dependency injection)

**Files to Create**:
- `Platform/IScintillaNative.cs`
- `Platform/ILexillaNative.cs`
- `Platform/PlatformDetector.cs`
- `Platform/NativeLibraryLoader.cs`
- `Platform/ScintillaNativeLinux.cs`
- `Platform/LexillaNativeLinux.cs`

**Success Criteria**: Linux builds and runs identically to current implementation

### Phase 2: Windows Support (Week 3-4)

**Goal**: Add Windows platform support

**Tasks**:
1. Build or obtain Scintilla/Lexilla Windows DLLs for GTK
2. Create `ScintillaNativeWindows` class
3. Create `LexillaNativeWindows` class
4. Test key translation for Windows (may differ from Linux)
5. Update MSBuild targets to include Windows binaries
6. Test on Windows with GTK# runtime

**Files to Create**:
- `Platform/ScintillaNativeWindows.cs`
- `Platform/LexillaNativeWindows.cs`
- `runtimes/win-x64/native/scintilla.dll`
- `runtimes/win-x64/native/lexilla.dll`

**Platform-Specific Considerations**:
- Windows DLL naming conventions (no `lib` prefix)
- Path separators and line endings
- GTK# runtime installation on Windows

**Success Criteria**: 
- Builds on Windows
- Basic text editing works
- Syntax highlighting works
- No crashes or memory leaks

### Phase 3: macOS Support (Week 5-6)

**Goal**: Add macOS platform support

**Tasks**:
1. Build or obtain Scintilla/Lexilla macOS dylibs
2. Create `ScintillaNativeMac` class
3. Create `LexillaNativeMac` class
4. Handle both Intel (x64) and Apple Silicon (ARM64)
5. Test GTK# integration on macOS
6. Update MSBuild targets for macOS binaries

**Files to Create**:
- `Platform/ScintillaNativeMac.cs`
- `Platform/LexillaNativeMac.cs`
- `runtimes/osx-x64/native/libscintilla.dylib`
- `runtimes/osx-x64/native/liblexilla.dylib`
- `runtimes/osx-arm64/native/libscintilla.dylib`
- `runtimes/osx-arm64/native/liblexilla.dylib`

**Platform-Specific Considerations**:
- macOS Gatekeeper and code signing
- Universal binaries vs separate architectures
- GTK# via Homebrew or system installation
- macOS keyboard shortcuts (Cmd vs Ctrl)

**Success Criteria**: 
- Builds on both Intel and ARM Macs
- GTK# UI renders correctly
- All core features work
- Performance is acceptable

### Phase 4: Enhanced Build System (Week 7)

**Goal**: Robust, maintainable build and packaging system

**Tasks**:
1. Update `.csproj` with runtime-specific includes
2. Create comprehensive MSBuild targets
3. Add NuGet package support for all platforms
4. Create build scripts for native libraries
5. Add developer documentation

**Files to Modify**:
- `Scintilla.NET.Gtk.csproj`
- `Scintilla.NET.Gtk.targets`

**Files to Create**:
- `build/BuildNativeLibraries.md`
- `build/build-linux.sh`
- `build/build-windows.ps1`
- `build/build-macos.sh`

### Phase 5: Testing and Documentation (Week 8)

**Goal**: Comprehensive testing and documentation

**Tasks**:
1. Create platform-specific test applications
2. Performance benchmarking across platforms
3. Update README with platform requirements
4. Update developer guide with platform differences
5. Create migration guide for existing users
6. CI/CD pipeline for all platforms

**Files to Create**:
- `TestApp/TestApp.Linux/`
- `TestApp/TestApp.Windows/`
- `TestApp/TestApp.Mac/`
- `.github/workflows/build-linux.yml`
- `.github/workflows/build-windows.yml`
- `.github/workflows/build-macos.yml`

---

## Technical Design

### Platform Abstraction Implementation

#### Before (Current - Linux Only)

```csharp
public class Scintilla : Widget
{
    [DllImport("libscintilla", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr scintilla_new();
    
    [DllImport("libscintilla", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr scintilla_send_message(IntPtr ptr, int msg, IntPtr wParam, IntPtr lParam);
    
    public Scintilla() : base(scintilla_new())
    {
        // ...
    }
}
```

#### After (Cross-Platform)

```csharp
public class Scintilla : Widget
{
    private readonly IScintillaNative _native;
    
    public Scintilla() : this(NativeLibraryLoader.CreateScintillaNative())
    {
    }
    
    internal Scintilla(IScintillaNative native) : base(native.CreateScintilla())
    {
        _native = native;
        // ...
    }
    
    public IntPtr DirectMessage(int msg, IntPtr wParam, IntPtr lParam)
    {
        return _native.SendMessage(editor, msg, wParam, lParam);
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _native?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### Native Library Loader

```csharp
public static class NativeLibraryLoader
{
    private static IScintillaNative? _scintillaInstance;
    private static ILexillaNative? _lexillaInstance;
    private static readonly object _lock = new object();
    
    public static IScintillaNative CreateScintillaNative()
    {
        lock (_lock)
        {
            if (_scintillaInstance == null)
            {
                _scintillaInstance = PlatformDetector.CurrentPlatform switch
                {
                    PlatformOS.Linux => new ScintillaNativeLinux(),
                    PlatformOS.Windows => new ScintillaNativeWindows(),
                    PlatformOS.MacOSX => new ScintillaNativeMac(),
                    _ => throw new PlatformNotSupportedException(
                        $"Platform {PlatformDetector.CurrentPlatform} is not supported")
                };
            }
            return _scintillaInstance;
        }
    }
    
    public static ILexillaNative CreateLexillaNative()
    {
        lock (_lock)
        {
            if (_lexillaInstance == null)
            {
                _lexillaInstance = PlatformDetector.CurrentPlatform switch
                {
                    PlatformOS.Linux => new LexillaNativeLinux(),
                    PlatformOS.Windows => new LexillaNativeWindows(),
                    PlatformOS.MacOSX => new LexillaNativeMac(),
                    _ => throw new PlatformNotSupportedException(
                        $"Platform {PlatformDetector.CurrentPlatform} is not supported")
                };
            }
            return _lexillaInstance;
        }
    }
}
```

### Platform-Specific Implementations

#### Linux (ScintillaNativeLinux.cs)

```csharp
public class ScintillaNativeLinux : IScintillaNative
{
    private const string LibScintilla = "libscintilla.so";
    
    [DllImport(LibScintilla, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr scintilla_new();
    
    [DllImport(LibScintilla, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr scintilla_send_message(IntPtr ptr, int msg, IntPtr wParam, IntPtr lParam);
    
    public IntPtr CreateScintilla() => scintilla_new();
    
    public IntPtr SendMessage(IntPtr ptr, int msg, IntPtr wParam, IntPtr lParam)
        => scintilla_send_message(ptr, msg, wParam, lParam);
    
    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

#### Windows (ScintillaNativeWindows.cs)

```csharp
public class ScintillaNativeWindows : IScintillaNative
{
    private const string LibScintilla = "scintilla.dll";
    
    [DllImport(LibScintilla, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr scintilla_new();
    
    [DllImport(LibScintilla, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr scintilla_send_message(IntPtr ptr, int msg, IntPtr wParam, IntPtr lParam);
    
    public IntPtr CreateScintilla() => scintilla_new();
    
    public IntPtr SendMessage(IntPtr ptr, int msg, IntPtr wParam, IntPtr lParam)
        => scintilla_send_message(ptr, msg, wParam, lParam);
    
    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

#### macOS (ScintillaNativeMac.cs)

```csharp
public class ScintillaNativeMac : IScintillaNative
{
    private const string LibScintilla = "libscintilla.dylib";
    
    [DllImport(LibScintilla, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr scintilla_new();
    
    [DllImport(LibScintilla, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr scintilla_send_message(IntPtr ptr, int msg, IntPtr wParam, IntPtr lParam);
    
    public IntPtr CreateScintilla() => scintilla_new();
    
    public IntPtr SendMessage(IntPtr ptr, int msg, IntPtr wParam, IntPtr lParam)
        => scintilla_send_message(ptr, msg, wParam, lParam);
    
    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

### Interface Naming

**Rename**: `IScintillaLinux.cs` → `IScintillaGtk.cs`

The interface is not truly Linux-specific; it's GTK-specific. It should work on any platform that supports GTK#.

---

## Platform-Specific Considerations

### Linux

#### Challenges
- Multiple distributions with different GTK versions
- Library path variations (`/usr/lib`, `/usr/local/lib`, etc.)
- Different package managers

#### Solutions
- Use standard library search paths
- Document supported distributions
- Provide build instructions for custom builds

#### Native Library Building
```bash
# Build Scintilla for Linux/GTK
cd scintilla/gtk
make
# Output: libscintilla.so

# Build Lexilla
cd lexilla/src
make
# Output: liblexilla.so
```

### Windows

#### Challenges
- GTK# runtime must be installed separately
- DLL search path issues
- Windows-specific keyboard shortcuts

#### Solutions
- Bundle GTK# runtime or document installation
- Use proper DLL deployment (next to executable)
- Handle Ctrl vs Cmd key differences in `Helpers.cs`

#### Native Library Building
```powershell
# Build Scintilla for Windows/GTK
cd scintilla\win32
# Use Visual Studio or MinGW
nmake -f scintilla.mak
# Output: scintilla.dll

# Build Lexilla
cd lexilla\src
nmake -f lexilla.mak
# Output: lexilla.dll
```

#### GTK# on Windows
- Install via MSYS2: `pacman -S mingw-w64-x86_64-gtk3`
- Or use GTK# installer from gtk-sharp.github.io
- Set PATH to include GTK# binaries

### macOS

#### Challenges
- Code signing requirements (Gatekeeper)
- Both Intel and Apple Silicon support
- GTK# installation complexity
- macOS keyboard shortcuts (Cmd vs Ctrl)

#### Solutions
- Provide signed binaries or signing instructions
- Build universal binaries or separate architectures
- Document Homebrew installation: `brew install gtk+3`
- Platform-specific key mappings

#### Native Library Building
```bash
# Install dependencies via Homebrew
brew install gtk+3 pkg-config

# Build for Intel
cd scintilla/gtk
make
# Output: libscintilla.dylib

# Build for Apple Silicon
arch -arm64 make
# Output: libscintilla.dylib (ARM64)

# Create universal binary
lipo -create libscintilla-x64.dylib libscintilla-arm64.dylib \
     -output libscintilla.dylib
```

#### Code Signing (if distributing)
```bash
codesign --force --sign "Developer ID Application: YourName" libscintilla.dylib
```

---

## Build System Changes

### Project File Updates

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <RootNamespace>ScintillaNet.Gtk</RootNamespace>
    
    <!-- Multi-platform support -->
    <RuntimeIdentifiers>linux-x64;win-x64;osx-x64;osx-arm64</RuntimeIdentifiers>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="GtkSharp" Version="3.24.24.38" />
    <PackageReference Include="Scintilla.NET.Abstractions" Version="1.0.11" />
    <PackageReference Include="System.Runtime.InteropServices.RuntimeInformation" Version="4.3.0" />
  </ItemGroup>

  <!-- Linux native libraries -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Linux'))">
    <None Include="runtimes/linux-x64/native/libscintilla.so">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <PackagePath>runtimes/linux-x64/native/</PackagePath>
      <Pack>true</Pack>
    </None>
    <None Include="runtimes/linux-x64/native/liblexilla.so">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <PackagePath>runtimes/linux-x64/native/</PackagePath>
      <Pack>true</Pack>
    </None>
  </ItemGroup>

  <!-- Windows native libraries -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Windows'))">
    <None Include="runtimes/win-x64/native/scintilla.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <PackagePath>runtimes/win-x64/native/</PackagePath>
      <Pack>true</Pack>
    </None>
    <None Include="runtimes/win-x64/native/lexilla.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <PackagePath>runtimes/win-x64/native/</PackagePath>
      <Pack>true</Pack>
    </None>
  </ItemGroup>

  <!-- macOS native libraries -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('OSX'))">
    <!-- Intel -->
    <None Include="runtimes/osx-x64/native/libscintilla.dylib">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <PackagePath>runtimes/osx-x64/native/</PackagePath>
      <Pack>true</Pack>
    </None>
    <None Include="runtimes/osx-x64/native/liblexilla.dylib">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <PackagePath>runtimes/osx-x64/native/</PackagePath>
      <Pack>true</Pack>
    </None>
    
    <!-- Apple Silicon -->
    <None Include="runtimes/osx-arm64/native/libscintilla.dylib">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <PackagePath>runtimes/osx-arm64/native/</PackagePath>
      <Pack>true</Pack>
    </None>
    <None Include="runtimes/osx-arm64/native/liblexilla.dylib">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <PackagePath>runtimes/osx-arm64/native/</PackagePath>
      <Pack>true</Pack>
    </None>
  </ItemGroup>

  <!-- Build targets for consuming projects -->
  <ItemGroup>
    <None Include="build\Scintilla.NET.Gtk.targets">
      <PackagePath>build\</PackagePath>
      <Pack>true</Pack>
    </None>
  </ItemGroup>
</Project>
```

### Enhanced MSBuild Targets

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  
  <!-- Linux -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Linux'))">
    <None Include="$(MSBuildThisFileDirectory)../runtimes/linux-x64/native/libscintilla.so">
      <Link>libscintilla.so</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(MSBuildThisFileDirectory)../runtimes/linux-x64/native/liblexilla.so">
      <Link>liblexilla.so</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

  <!-- Windows -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Windows'))">
    <None Include="$(MSBuildThisFileDirectory)../runtimes/win-x64/native/scintilla.dll">
      <Link>scintilla.dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(MSBuildThisFileDirectory)../runtimes/win-x64/native/lexilla.dll">
      <Link>lexilla.dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

  <!-- macOS Intel -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('OSX')) And '$(PlatformTarget)' != 'ARM64'">
    <None Include="$(MSBuildThisFileDirectory)../runtimes/osx-x64/native/libscintilla.dylib">
      <Link>libscintilla.dylib</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(MSBuildThisFileDirectory)../runtimes/osx-x64/native/liblexilla.dylib">
      <Link>liblexilla.dylib</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

  <!-- macOS Apple Silicon -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('OSX')) And '$(PlatformTarget)' == 'ARM64'">
    <None Include="$(MSBuildThisFileDirectory)../runtimes/osx-arm64/native/libscintilla.dylib">
      <Link>libscintilla.dylib</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(MSBuildThisFileDirectory)../runtimes/osx-arm64/native/liblexilla.dylib">
      <Link>liblexilla.dylib</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>
```

---

## Testing Strategy

### Unit Tests

Create platform-agnostic unit tests:

```csharp
[TestFixture]
public class ScintillaBasicTests
{
    private Scintilla _scintilla;
    
    [SetUp]
    public void Setup()
    {
        Application.Init();
        _scintilla = new Scintilla();
    }
    
    [Test]
    public void TestTextProperty()
    {
        _scintilla.Text = "Hello, World!";
        Assert.AreEqual("Hello, World!", _scintilla.Text);
    }
    
    [Test]
    public void TestLexerSetting()
    {
        _scintilla.LexerName = "cpp";
        Assert.AreEqual("cpp", _scintilla.LexerLanguage);
    }
    
    // More tests...
}
```

### Integration Tests

Platform-specific test applications:

```
TestApp/
├── TestApp.Linux/      # Linux-specific tests
├── TestApp.Windows/    # Windows-specific tests
└── TestApp.Mac/        # macOS-specific tests
```

### Manual Testing Checklist

For each platform:
- [ ] Application launches without errors
- [ ] Text can be entered and edited
- [ ] Syntax highlighting works
- [ ] Code folding works
- [ ] Margins and line numbers display correctly
- [ ] Markers appear correctly
- [ ] Indicators render properly
- [ ] Auto-completion functions
- [ ] Call tips display
- [ ] Search and replace works
- [ ] Undo/redo operates correctly
- [ ] Copy/paste works with system clipboard
- [ ] Multiple selections work
- [ ] Keyboard shortcuts function (platform-appropriate)
- [ ] No memory leaks on long-running sessions
- [ ] Performance is acceptable

### CI/CD Pipeline

#### GitHub Actions Workflow

```yaml
name: Multi-Platform Build

on: [push, pull_request]

jobs:
  build-linux:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 7.0.x
      - name: Install GTK3
        run: sudo apt-get install -y libgtk-3-dev
      - name: Build
        run: dotnet build --configuration Release
      - name: Test
        run: dotnet test --configuration Release

  build-windows:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 7.0.x
      - name: Install GTK3
        run: |
          choco install gtk-runtime
      - name: Build
        run: dotnet build --configuration Release
      - name: Test
        run: dotnet test --configuration Release

  build-macos:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 7.0.x
      - name: Install GTK3
        run: brew install gtk+3
      - name: Build
        run: dotnet build --configuration Release
      - name: Test
        run: dotnet test --configuration Release
```

---

## Migration Path

### For Existing Users

#### No Code Changes Required (Best Case)

For most users, the library will work transparently:

```csharp
// Existing code continues to work
var scintilla = new Scintilla();
scintilla.Text = "Hello, World!";
```

The platform detection and native library loading happens automatically.

#### Optional: Explicit Platform Control

For advanced users who need control:

```csharp
// Explicit platform selection (advanced usage)
var native = new ScintillaNativeLinux(); // Or Windows, Mac
var scintilla = new Scintilla(native);
```

### Versioning Strategy

**Semantic Versioning**:
- Current: `1.0.11.5320` (last digit = Scintilla version)
- After refactor: `2.0.0.5320` (major version bump for architecture change)

**NuGet Package**:
- New package name: `Scintilla.NET.Gtk` (keep existing name)
- Add `linux`, `windows`, `osx` tags
- Update description to mention cross-platform support

### Documentation Updates

1. **README.md**: Add platform support section
2. **Developer Guide**: Add platform-specific sections
3. **Migration Guide**: Document changes from 1.x to 2.x
4. **Installation Guide**: Platform-specific setup instructions

---

## Risks and Mitigations

### Risk 1: Native Library Compatibility

**Risk**: Native Scintilla libraries may have platform-specific behavior differences

**Mitigation**:
- Thorough testing on all platforms
- Maintain test matrix covering all platforms
- Document known platform differences
- Version-lock native library versions

### Risk 2: GTK# Platform Differences

**Risk**: GTK# may behave differently on different platforms

**Mitigation**:
- Use well-tested GTK# 3.24 release
- Test on multiple GTK# runtime versions
- Document supported GTK# versions per platform
- Provide fallback behavior for platform quirks

### Risk 3: Breaking Changes

**Risk**: Refactoring may break existing code

**Mitigation**:
- Maintain backward compatibility where possible
- Use semantic versioning (2.0.0 indicates breaking changes)
- Provide migration guide
- Keep 1.x branch for critical bug fixes
- Beta releases for early adopters

### Risk 4: Build Complexity

**Risk**: Multi-platform builds are more complex

**Mitigation**:
- Comprehensive build documentation
- Automated CI/CD for all platforms
- Build scripts for each platform
- Docker containers for reproducible builds

### Risk 5: Native Binary Acquisition

**Risk**: Getting pre-built native libraries for all platforms

**Mitigation**:
- Build from Scintilla source (well documented)
- Provide build scripts for each platform
- Host pre-built binaries in releases
- Document build process clearly

### Risk 6: Performance Variations

**Risk**: Performance may vary across platforms

**Mitigation**:
- Benchmark on all platforms
- Profile and optimize platform-specific bottlenecks
- Document expected performance characteristics
- Provide performance tuning guidelines

---

## Native Library Building Guide

### Building Scintilla from Source

#### Prerequisites
- CMake 3.15+
- C++ compiler (GCC, Clang, MSVC)
- GTK 3.x development headers

#### Common Steps

```bash
# Clone Scintilla
git clone https://github.com/ScintillaOrg/scintilla.git
cd scintilla

# Clone Lexilla
git clone https://github.com/ScintillaOrg/lexilla.git
```

#### Linux Build

```bash
# Build Scintilla
cd scintilla/gtk
make

# Build Lexilla
cd ../../lexilla/src
make

# Output files:
# scintilla/bin/libscintilla.so
# lexilla/bin/liblexilla.so
```

#### Windows Build (MSYS2/MinGW)

```bash
# Install MSYS2, then install dependencies
pacman -S mingw-w64-x86_64-gcc mingw-w64-x86_64-gtk3

# Build Scintilla
cd scintilla/win32
make -f scintilla.mak

# Build Lexilla
cd ../../lexilla/src
make -f lexilla.mak

# Output files:
# scintilla/bin/scintilla.dll
# lexilla/bin/lexilla.dll
```

#### macOS Build

```bash
# Install dependencies
brew install gtk+3 pkg-config

# Build Scintilla
cd scintilla/gtk
make

# Build Lexilla
cd ../../lexilla/src
make

# Output files:
# scintilla/bin/libscintilla.dylib
# lexilla/bin/liblexilla.dylib
```

---

## Timeline Summary

| Phase | Duration | Deliverable |
|-------|----------|-------------|
| Phase 1: Abstraction Layer | 2 weeks | Platform abstraction, no regression |
| Phase 2: Windows Support | 2 weeks | Windows builds and runs |
| Phase 3: macOS Support | 2 weeks | macOS (Intel & ARM) support |
| Phase 4: Build System | 1 week | Multi-platform packaging |
| Phase 5: Testing & Docs | 1 week | Complete test coverage, docs |
| **Total** | **8 weeks** | **Cross-platform release** |

---

## Success Metrics

### Technical Metrics
- [ ] Builds successfully on Linux, Windows, and macOS
- [ ] All existing unit tests pass on all platforms
- [ ] No performance regression on Linux (baseline platform)
- [ ] Memory usage comparable across platforms
- [ ] Zero P/Invoke-related crashes

### Quality Metrics
- [ ] Code coverage > 80% on shared code
- [ ] Documentation complete for all platforms
- [ ] CI/CD passing on all platforms
- [ ] Zero critical bugs in release candidate

### Adoption Metrics
- [ ] Successful migration of test application
- [ ] At least 3 beta testers per platform
- [ ] Positive community feedback
- [ ] NuGet package downloads across platforms

---

## Appendix A: File Structure After Refactoring

```
Scintilla.NET.Gtk/
├── Scintilla.NET.Gtk.sln
├── README.md
├── LICENSE
├── Documentation/
│   ├── Scintilla.NET Developer Guide.md
│   ├── X-Platformization Plan.md (this document)
│   ├── Migration Guide 1.x to 2.x.md
│   └── Platform-Specific Notes.md
├── Scintilla.NET.Gtk/
│   ├── Scintilla.NET.Gtk.csproj
│   ├── Scintilla.cs                      # Main control (platform-agnostic)
│   ├── IScintillaGtk.cs                  # Renamed from IScintillaLinux
│   ├── Helpers.cs                        # Platform-aware key translation
│   ├── NativeEventHandling.cs
│   ├── NativeImageRgbaConverter.cs
│   ├── Platform/
│   │   ├── PlatformDetector.cs
│   │   ├── NativeLibraryLoader.cs
│   │   ├── IScintillaNative.cs
│   │   ├── ILexillaNative.cs
│   │   ├── ScintillaNativeLinux.cs
│   │   ├── ScintillaNativeWindows.cs
│   │   ├── ScintillaNativeMac.cs
│   │   ├── LexillaNativeLinux.cs
│   │   ├── LexillaNativeWindows.cs
│   │   └── LexillaNativeMac.cs
│   ├── Collections/
│   │   ├── Indicator.cs
│   │   ├── IndicatorCollection.cs
│   │   ├── Line.cs
│   │   ├── LineCollection.cs
│   │   ├── Margin.cs
│   │   ├── MarginCollection.cs
│   │   ├── Marker.cs
│   │   ├── MarkerCollection.cs
│   │   ├── Selection.cs
│   │   ├── SelectionCollection.cs
│   │   ├── Style.cs
│   │   └── StyleCollection.cs
│   ├── EventArguments/
│   │   └── [various event args...]
│   ├── GdkUtils/
│   │   └── ColorTranslator.cs
│   ├── build/
│   │   └── Scintilla.NET.Gtk.targets
│   └── runtimes/
│       ├── linux-x64/native/
│       │   ├── libscintilla.so
│       │   └── liblexilla.so
│       ├── win-x64/native/
│       │   ├── scintilla.dll
│       │   └── lexilla.dll
│       ├── osx-x64/native/
│       │   ├── libscintilla.dylib
│       │   └── liblexilla.dylib
│       └── osx-arm64/native/
│           ├── libscintilla.dylib
│           └── liblexilla.dylib
├── TestApp/
│   ├── Program.cs
│   ├── TestApp.csproj
│   ├── TestApp.Linux/
│   ├── TestApp.Windows/
│   └── TestApp.Mac/
├── Tests/
│   ├── Scintilla.NET.Gtk.Tests/
│   │   ├── PlatformDetectorTests.cs
│   │   ├── NativeLoaderTests.cs
│   │   └── ScintillaTests.cs
│   └── Scintilla.NET.Gtk.IntegrationTests/
└── .github/
    └── workflows/
        ├── build-linux.yml
        ├── build-windows.yml
        └── build-macos.yml
```

---

## Appendix B: API Changes

### No Breaking Changes for Common Usage

```csharp
// This code works in 1.x and 2.x
var scintilla = new Scintilla();
scintilla.Text = "Hello";
scintilla.LexerName = "cpp";
```

### New Features in 2.x

```csharp
// Platform detection
var platform = PlatformDetector.CurrentPlatform;
Console.WriteLine($"Running on: {platform}");

// Explicit native control (advanced)
var native = NativeLibraryLoader.CreateScintillaNative();
var scintilla = new Scintilla(native);

// Check platform capabilities
if (PlatformDetector.CurrentPlatform == PlatformOS.MacOSX)
{
    // macOS-specific adjustments
}
```

### Internal Changes (Not User-Visible)

- `IScintillaLinux` → `IScintillaGtk`
- Direct P/Invoke → Interface-based native calls
- Hardcoded library names → Platform-specific loading

---

## Appendix C: FAQ

### Q: Will my existing Linux code break?
**A**: No. The public API remains unchanged. Only internal implementation changes.

### Q: Do I need to install anything extra on Windows/Mac?
**A**: You need GTK# runtime installed. We'll document installation for each platform.

### Q: Will performance be affected?
**A**: Negligible. The abstraction layer adds one interface call, which is insignificant compared to native calls.

### Q: Can I still use the library on Linux only?
**A**: Absolutely. The library will work exactly as before on Linux.

### Q: What about mobile platforms (iOS/Android)?
**A**: Not in scope for this refactor. GTK# has limited mobile support.

### Q: Will the NuGet package size increase?
**A**: Yes, it will include native binaries for all platforms. Estimated 3-4x larger, but still reasonable.

### Q: How do I report platform-specific bugs?
**A**: Use GitHub Issues with platform tag: `[Linux]`, `[Windows]`, `[macOS]`

---

## Conclusion

This refactoring plan transforms **Scintilla.NET.Gtk** from a Linux-only library into a truly cross-platform solution while maintaining the GTK# framework across all platforms. The approach is conservative, prioritizing backward compatibility while enabling exciting new scenarios.

**Key Benefits**:
- ✅ Single codebase for all platforms
- ✅ Consistent GTK# UI experience
- ✅ Minimal breaking changes
- ✅ Clear migration path
- ✅ Maintainable architecture
- ✅ Native performance on all platforms

**Next Steps**:
1. Review and approve this plan
2. Set up development environments for all platforms
3. Begin Phase 1 implementation
4. Regular progress reviews

**Questions or feedback?** Open an issue or discussion on GitHub!

---

*Document Version: 1.0*
*Last Updated: November 28, 2025*
*Author: Cross-Platform Refactoring Team*
