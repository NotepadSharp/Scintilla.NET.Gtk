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
/// Interface for platform-specific Lexilla native library bindings.
/// </summary>
public interface ILexillaNative : IDisposable
{
    /// <summary>
    /// Gets the number of available lexers.
    /// </summary>
    int LexerCount { get; }
    
    /// <summary>
    /// Gets the name of a lexer by index.
    /// </summary>
    /// <param name="index">The lexer index.</param>
    /// <returns>The lexer name.</returns>
    string GetLexerName(uint index);
    
    /// <summary>
    /// Creates a lexer instance by name.
    /// </summary>
    /// <param name="name">The lexer name.</param>
    /// <returns>Pointer to the lexer instance.</returns>
    IntPtr CreateLexer(string name);
}
