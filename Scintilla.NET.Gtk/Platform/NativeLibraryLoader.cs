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

namespace ScintillaNet.Gtk.Platform;

/// <summary>
/// Factory for creating platform-specific native library instances.
/// </summary>
public static class NativeLibraryLoader
{
    private static IScintillaNative? _scintillaInstance;
    private static ILexillaNative? _lexillaInstance;
    private static readonly object _lock = new object();
    
    /// <summary>
    /// Creates or gets the singleton Scintilla native library instance for the current platform.
    /// </summary>
    /// <returns>Platform-specific Scintilla native instance.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
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
                        $"Platform {PlatformDetector.CurrentPlatform} ({PlatformDetector.RuntimeIdentifier}) is not supported.")
                };
            }
            return _scintillaInstance;
        }
    }
    
    /// <summary>
    /// Creates or gets the singleton Lexilla native library instance for the current platform.
    /// </summary>
    /// <returns>Platform-specific Lexilla native instance.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
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
                        $"Platform {PlatformDetector.CurrentPlatform} ({PlatformDetector.RuntimeIdentifier}) is not supported.")
                };
            }
            return _lexillaInstance;
        }
    }
    
    /// <summary>
    /// Resets the cached native library instances. Mainly used for testing.
    /// </summary>
    internal static void Reset()
    {
        lock (_lock)
        {
            _scintillaInstance?.Dispose();
            _lexillaInstance?.Dispose();
            _scintillaInstance = null;
            _lexillaInstance = null;
        }
    }
}
