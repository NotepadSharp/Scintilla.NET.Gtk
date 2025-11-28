# Quick Reference: Cross-Platform Scintilla.NET.Gtk

## Installation

```bash
dotnet add package Scintilla.NET.Gtk
```

## Basic Usage (Works on All Platforms)

```csharp
using ScintillaNet.Gtk;
using Gtk;

Application.Init();

var window = new Window("Editor");
var scintilla = new Scintilla();

scintilla.Text = "Hello, cross-platform world!";
window.Add(scintilla);
window.ShowAll();

Application.Run();
```

## Platform Detection

```csharp
using ScintillaNet.Gtk.Platform;

// Check current platform
if (PlatformDetector.IsLinux)
    Console.WriteLine("Running on Linux");
else if (PlatformDetector.IsWindows)
    Console.WriteLine("Running on Windows");
else if (PlatformDetector.IsMacOS)
    Console.WriteLine("Running on macOS");

// Get runtime identifier
Console.WriteLine(PlatformDetector.RuntimeIdentifier);
// Output: "linux-x64", "win-x64", "osx-x64", or "osx-arm64"

// Get platform enum
var platform = PlatformDetector.CurrentPlatform;
// PlatformOS.Linux, PlatformOS.Windows, or PlatformOS.MacOSX
```

## Native Library Requirements

### Linux
```
libscintilla.so  (included)
liblexilla.so    (included)
```

### Windows
```
scintilla.dll    (build or obtain separately)
lexilla.dll      (build or obtain separately)
```
Dependencies: GTK+ 3 for Windows runtime

### macOS
```
libscintilla.dylib  (build or obtain separately)
liblexilla.dylib    (build or obtain separately)
```
Dependencies: `brew install gtk+3`

## Common Syntax Highlighting Setup

```csharp
// C# syntax highlighting
scintilla.StyleResetDefault();
scintilla.Styles[Style.Default].Font = "Consolas";
scintilla.Styles[Style.Default].Size = 10;
scintilla.StyleClearAll();

// Line numbers
scintilla.Margins[0].Width = 30;
scintilla.Margins[0].Type = MarginType.Number;

// Keywords
scintilla.SetKeywords(0, "class using namespace public private static void");
scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Parse("blue");
scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.Parse("green");
scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.Parse("green");
scintilla.Styles[Style.Cpp.String].ForeColor = Color.Parse("red");

scintilla.Lexer = Lexer.Cpp;
```

## Auto-Completion

```csharp
// Register auto-completion keywords
var keywords = new[] { "Console", "WriteLine", "ReadLine" };
scintilla.AutoCShow(0, string.Join(" ", keywords));

// Configure auto-completion behavior
scintilla.AutoCSetIgnoreCase(true);
scintilla.AutoCSetMaxHeight(10);
```

## Code Folding

```csharp
scintilla.SetProperty("fold", "1");
scintilla.SetProperty("fold.compact", "1");

// Configure fold margin
scintilla.Margins[2].Type = MarginType.Symbol;
scintilla.Margins[2].Mask = Marker.MaskFolders;
scintilla.Margins[2].Sensitive = true;
scintilla.Margins[2].Width = 20;

// Set fold markers
for (int i = 25; i <= 31; i++)
{
    scintilla.Markers[i].Symbol = MarkerSymbol.BoxPlus;
}
```

## Search and Replace

```csharp
// Find text
var pos = scintilla.SearchInTarget("search term");
if (pos != -1)
{
    scintilla.SetSel(pos, pos + "search term".Length);
}

// Replace
scintilla.TargetStart = 0;
scintilla.TargetEnd = scintilla.TextLength;
scintilla.SearchFlags = SearchFlags.None;
scintilla.ReplaceTarget("replacement");
```

## Events

```csharp
// Text changed
scintilla.TextChanged += (sender, e) => {
    Console.WriteLine("Text modified");
};

// Character added
scintilla.CharAdded += (sender, e) => {
    Console.WriteLine($"Char added: {(char)e.Char}");
};

// Save point reached/left
scintilla.SavePointReached += (sender, e) => {
    Console.WriteLine("Document saved");
};

scintilla.SavePointLeft += (sender, e) => {
    Console.WriteLine("Document modified");
};
```

## Performance Tips

```csharp
// Disable redraw during bulk updates
scintilla.BeginUndoAction();
try
{
    // Multiple text operations
    scintilla.Text = largeText;
    // Apply styles, etc.
}
finally
{
    scintilla.EndUndoAction();
}
```

## Troubleshooting

### Native library not found

**Linux:**
```bash
# Ensure libraries are in output directory
ls bin/Debug/net*/libscintilla.so
```

**Windows:**
```powershell
# Check GTK+ 3 is installed
# Libraries should be in bin\Debug\net*\
dir bin\Debug\net*\scintilla.dll
```

**macOS:**
```bash
# Install GTK+ 3
brew install gtk+3

# Check libraries
ls bin/Debug/net*/libscintilla.dylib
```

### Platform detection issues

```csharp
// Debug platform detection
using ScintillaNet.Gtk.Platform;
using System.Runtime.InteropServices;

Console.WriteLine($"Detected: {PlatformDetector.CurrentPlatform}");
Console.WriteLine($"Runtime: {PlatformDetector.RuntimeIdentifier}");
Console.WriteLine($"Arch: {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
```

## Project Configuration

### .csproj for consuming projects

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Scintilla.NET.Gtk" Version="2.0.0" />
  </ItemGroup>
</Project>
```

### Runtime-specific builds

```bash
# Build for specific platform
dotnet build -r linux-x64
dotnet build -r win-x64
dotnet build -r osx-x64
dotnet build -r osx-arm64

# Publish self-contained
dotnet publish -r linux-x64 --self-contained
```

## API Compatibility

All existing Scintilla.NET.Gtk 1.x code works unchanged in 2.0:

```csharp
// ✅ Works in both 1.x and 2.x
var scintilla = new Scintilla();
scintilla.Text = "test";
scintilla.Margins[0].Width = 30;
scintilla.Lexer = Lexer.Cpp;
```

No code changes needed for migration!

## Resources

- [Complete Developer Guide](Scintilla.NET%20Developer%20Guide.md)
- [Migration Guide](Cross-Platform%20Migration%20Guide.md)
- [Implementation Details](Implementation%20Summary.md)
- [Scintilla Documentation](https://www.scintilla.org/ScintillaDoc.html)

## Support

- GitHub Issues: https://github.com/pdavis68/Scintilla.NET.Gtk/issues
- Pull Requests Welcome!

---

*Version 2.0.0 | Cross-Platform Release*
