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
/// macOS implementation of Lexilla native library bindings.
/// </summary>
public class LexillaNativeMac : ILexillaNative
{
    private const string LibLexilla = "liblexilla";
    
    [DllImport(LibLexilla, CallingConvention = CallingConvention.Cdecl)]
    private static extern int GetLexerCount();

    [DllImport(LibLexilla, CallingConvention = CallingConvention.Cdecl)]
    private static extern void GetLexerName(uint index, IntPtr name, int buflength);

    [DllImport(LibLexilla, CallingConvention = CallingConvention.Cdecl, EntryPoint = "CreateLexer")]
    private static extern IntPtr CreateLexerNative([MarshalAs(UnmanagedType.LPStr)] string lexerName);

    /// <inheritdoc />
    public int LexerCount => GetLexerCount();

    /// <inheritdoc />
    public string GetLexerName(uint index)
    {
        var pointer = Marshal.AllocHGlobal(1024);
        try
        {
            GetLexerName(index, pointer, 1024);
            return Marshal.PtrToStringAnsi(pointer) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(pointer);
        }
    }

    /// <inheritdoc />
    public IntPtr CreateLexer(string name)
    {
        return CreateLexerNative(name);
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        // No cleanup needed for macOS
    }
}
