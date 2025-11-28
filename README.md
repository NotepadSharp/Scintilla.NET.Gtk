# Scintilla.NET.Gtk
A cross-platform GTK# implementation of Scintilla.NET for **Linux, Windows, and macOS**

[![.NET](https://github.com/VPKSoft/Scintilla.NET.Gtk/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VPKSoft/Scintilla.NET.Gtk/actions/workflows/dotnet.yml)
 [![.NET NuGet Release](https://github.com/VPKSoft/Scintilla.NET.Gtk/actions/workflows/dotnet_nuget.yml/badge.svg)](https://github.com/VPKSoft/Scintilla.NET.Gtk/actions/workflows/dotnet_nuget.yml) ![Nuget](https://img.shields.io/nuget/v/Scintilla.NET.Gtk)

This is a [GtkSharp](https://github.com/GtkSharp/GtkSharp) implementation of [Scintilla.NET](https://github.com/VPKSoft/Scintilla.NET) with **cross-platform support**.

## Platform Support

| Platform | Status | Architecture | Native Libraries |
|----------|--------|--------------|------------------|
| 🐧 Linux | ✅ Fully Supported | x64 | libscintilla.so, liblexilla.so |
| 🪟 Windows | ✅ Ready (binaries needed) | x64, x86 | scintilla.dll, lexilla.dll |
| 🍎 macOS | ✅ Ready (binaries needed) | x64, ARM64 | libscintilla.dylib, liblexilla.dylib |

The library automatically detects your platform and loads the appropriate native libraries at runtime.

## Features

- 🎯 **Cross-Platform**: Single codebase runs on Linux, Windows, and macOS
- 🔄 **100% Backward Compatible**: Existing Linux code works without changes
- 🚀 **Automatic Platform Detection**: No manual configuration needed
- 📦 **NuGet Ready**: Runtime-specific native library packaging
- 🎨 **Full Scintilla Features**: Syntax highlighting, code folding, auto-completion, and more

## Quick Start

```csharp
using ScintillaNet.Gtk;
using Gtk;

Application.Init();

var window = new Window("Cross-Platform Editor");
var scintilla = new Scintilla();

scintilla.Text = "// Works on Linux, Windows, and macOS!\n";
scintilla.StyleSetFont(Style.Default, "Consolas");

window.Add(scintilla);
window.ShowAll();

Application.Run();
```

## Installation

```bash
dotnet add package Scintilla.NET.Gtk
```

**Note**: The project is currently in beta. Please report bugs and issues via [pull requests](https://github.com/pdavis68/Scintilla.NET.Gtk/pulls).

## Documentation

- 📖 [Developer Guide](Documentation/Scintilla.NET%20Developer%20Guide.md) - Complete API reference and usage examples
- 🔄 [Cross-Platform Migration Guide](Documentation/Cross-Platform%20Migration%20Guide.md) - Detailed migration and deployment instructions
- 🏗️ [Cross-Platformization Plan](Documentation/X-Platformization%20Plan.md) - Technical architecture and implementation details

## Architecture

The library uses a **platform abstraction layer** that:
- Detects the runtime platform using `System.Runtime.InteropServices.RuntimeInformation`
- Loads platform-specific native libraries dynamically
- Provides a unified API across all platforms

See [Platform/README.md](Scintilla.NET.Gtk/Platform/README.md) for technical details.

## Version Information

**Current Version**: 2.0.0 (Cross-Platform Release)

The package uses semantic versioning with a fourth number indicating the native Scintilla version:
- Example: `2.0.0.5320` → Scintilla 5.3.2.0

### What's New in 2.0

- ✅ Cross-platform support (Linux, Windows, macOS)
- ✅ Automatic platform detection
- ✅ Runtime-specific native library packaging
- ✅ Enhanced build system
- ✅ Comprehensive documentation
- ✅ 100% backward compatibility

## Native Libraries

### Linux (Included)
The embedded `lib*.so` libraries have been built on Linux Mint. They are automatically copied to the output directory.

### Windows & macOS
Native binaries for Windows and macOS can be built from the official Scintilla source or obtained from pre-built packages. See the [Cross-Platform Migration Guide](Documentation/Cross-Platform%20Migration%20Guide.md) for build instructions.

## Test Application

![image](https://user-images.githubusercontent.com/40712699/218978784-e7b5e233-ebe0-4fdb-92ee-cbc0180f15b4.png)

### Thanks to
* [JetBrains](https://www.jetbrains.com/?from=Scintilla.NET.Gtk) for their open source license(s).

![JetBrains Logo (Main) logo](https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg)
