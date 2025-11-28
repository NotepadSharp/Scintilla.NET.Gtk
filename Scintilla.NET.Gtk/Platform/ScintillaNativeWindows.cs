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
/// Windows implementation of Scintilla native library bindings.
/// </summary>
public class ScintillaNativeWindows : IScintillaNative
{
    private const string LibScintilla = "scintilla";
    
    /// <summary>
    /// Create a new Scintilla widget. The returned pointer can be added to a container and displayed in the same way as other widgets.
    /// </summary>
    /// <returns>IntPtr.</returns>
    [DllImport(LibScintilla, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern IntPtr scintilla_new();

    /// <summary>
    /// The main entry point allows sending any of the messages described in this document.
    /// </summary>
    /// <param name="ptr">The ScintillaObject pointer.</param>
    /// <param name="iMessage">The message identifier to send to the control.</param>
    /// <param name="wParam">The message <c>wParam</c> field.</param>
    /// <param name="lParam">The message <c>lParam</c> field.</param>
    /// <returns>IntPtr.</returns>
    [DllImport(LibScintilla, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern IntPtr scintilla_send_message(IntPtr ptr, int iMessage, IntPtr wParam, IntPtr lParam);

    /// <inheritdoc />
    public IntPtr CreateScintilla() => scintilla_new();
    
    /// <inheritdoc />
    public IntPtr SendMessage(IntPtr ptr, int message, IntPtr wParam, IntPtr lParam)
        => scintilla_send_message(ptr, message, wParam, lParam);
    
    /// <inheritdoc />
    public void Dispose()
    {
        // No cleanup needed for Windows
    }
}
