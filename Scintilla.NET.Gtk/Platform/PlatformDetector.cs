#region License
/*
MIT License

Copyright(c) 2023-2025 Petteri Kautonen, pdavis68

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
*/
#endregion

using System;
using System.Runtime.InteropServices;

namespace ScintillaNet.Gtk.Platform;

/// <summary>
/// Operating system platform types.
/// </summary>
public enum PlatformOS
{
    /// <summary>
    /// Linux operating system.
    /// </summary>
    Linux,
    
    /// <summary>
    /// Windows operating system.
    /// </summary>
    Windows,
    
    /// <summary>
    /// macOS operating system.
    /// </summary>
    MacOSX,
    
    /// <summary>
    /// Unknown or unsupported operating system.
    /// </summary>
    Unknown
}

/// <summary>
/// Detects the current platform at runtime.
/// </summary>
public static class PlatformDetector
{
    /// <summary>
    /// Gets the current operating system platform.
    /// </summary>
    public static PlatformOS CurrentPlatform { get; }
    
    /// <summary>
    /// Gets the runtime identifier for the current platform.
    /// </summary>
    public static string RuntimeIdentifier { get; }
    
    /// <summary>
    /// Gets a value indicating whether the current platform is Linux.
    /// </summary>
    public static bool IsLinux => CurrentPlatform == PlatformOS.Linux;
    
    /// <summary>
    /// Gets a value indicating whether the current platform is Windows.
    /// </summary>
    public static bool IsWindows => CurrentPlatform == PlatformOS.Windows;
    
    /// <summary>
    /// Gets a value indicating whether the current platform is macOS.
    /// </summary>
    public static bool IsMacOS => CurrentPlatform == PlatformOS.MacOSX;
    
    static PlatformDetector()
    {
        // Detect platform using RuntimeInformation
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CurrentPlatform = PlatformOS.Windows;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CurrentPlatform = PlatformOS.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            CurrentPlatform = PlatformOS.MacOSX;
        }
        else
        {
            CurrentPlatform = PlatformOS.Unknown;
        }
        
        RuntimeIdentifier = GetRuntimeIdentifier();
    }
    
    private static string GetRuntimeIdentifier()
    {
        var arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            Architecture.Arm => "arm",
            _ => "unknown"
        };
        
        return CurrentPlatform switch
        {
            PlatformOS.Linux => $"linux-{arch}",
            PlatformOS.Windows => $"win-{arch}",
            PlatformOS.MacOSX => $"osx-{arch}",
            _ => $"unknown-{arch}"
        };
    }
}
