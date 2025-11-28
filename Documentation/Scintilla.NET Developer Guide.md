# Scintilla.NET.Gtk Developer Guide

## Table of Contents
1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Getting Started](#getting-started)
4. [Basic Usage](#basic-usage)
5. [Text Editing](#text-editing)
6. [Styling and Syntax Highlighting](#styling-and-syntax-highlighting)
7. [Collections](#collections)
8. [Margins](#margins)
9. [Markers](#markers)
10. [Indicators](#indicators)
11. [Code Folding](#code-folding)
12. [Auto-Completion](#auto-completion)
13. [Call Tips](#call-tips)
14. [Search and Replace](#search-and-replace)
15. [Multiple Selections](#multiple-selections)
16. [Events](#events)
17. [Advanced Features](#advanced-features)
18. [Best Practices](#best-practices)

---

## Introduction

**Scintilla.NET.Gtk** is a GTK/Linux port of the popular Scintilla.NET library, providing a powerful source code editing component for GTK# applications. Built on top of the Scintilla editing component, it offers advanced features like syntax highlighting, code folding, auto-completion, and much more.

### Key Features
- Full-featured source code editor
- Syntax highlighting with lexer support
- Code folding
- Multiple selections and rectangular selections
- Auto-completion and call tips
- Line numbering and margins
- Markers and indicators
- Search and replace with regular expressions
- Undo/redo support
- Virtual space support

---

## Installation

Add the Scintilla.NET.Gtk NuGet package to your project:

```bash
dotnet add package Scintilla.NET.Gtk
```

Or via Package Manager:
```
Install-Package Scintilla.NET.Gtk
```

### Requirements
- .NET Standard 2.1 or higher
- GtkSharp 3.x
- Linux-based operating system

---

## Getting Started

### Creating a Basic Application

```csharp
using Gtk;
using ScintillaNet.Gtk;

class Program
{
    static void Main()
    {
        Application.Init();

        var window = new Window("Scintilla Editor");
        window.DeleteEvent += (o, args) => Application.Quit();
        window.Resize(800, 600);

        // Create the Scintilla control
        var scintilla = new Scintilla();
        
        window.Add(scintilla);
        window.ShowAll();

        Application.Run();
    }
}
```

---

## Basic Usage

### Setting and Getting Text

```csharp
// Set text
scintilla.Text = "Hello, World!";

// Get text
string content = scintilla.Text;

// Get text length
int length = scintilla.TextLength;
```

### Adding Text

```csharp
// Add text at current position
scintilla.AddText("Additional text");

// Append text to end of document
scintilla.AppendText("Text at end\n");

// Insert text at specific position
scintilla.InsertText(0, "Text at beginning\n");
```

### Clipboard Operations

```csharp
// Cut, copy, paste
scintilla.Cut();
scintilla.Copy();
scintilla.Paste();

// Clear selection
scintilla.Clear();

// Check if paste is available
if (scintilla.CanPaste)
{
    scintilla.Paste();
}
```

### Undo/Redo

```csharp
// Undo and redo
scintilla.Undo();
scintilla.Redo();

// Check availability
if (scintilla.CanUndo)
    scintilla.Undo();

if (scintilla.CanRedo)
    scintilla.Redo();

// Clear undo buffer
scintilla.EmptyUndoBuffer();

// Group operations
scintilla.BeginUndoAction();
// ... multiple operations ...
scintilla.EndUndoAction();
```

### Read-Only Mode

```csharp
// Make document read-only
scintilla.ReadOnly = true;

// Check if modified
if (scintilla.Modified)
{
    // Document has unsaved changes
}

// Mark as saved
scintilla.SetSavePoint();
```

---

## Text Editing

### Selection

```csharp
// Select all text
scintilla.SelectAll();

// Get selected text
string selected = scintilla.SelectedText;

// Set selection
scintilla.SetSelection(startPos, endPos);

// Get/set selection bounds
int start = scintilla.SelectionStart;
int end = scintilla.SelectionEnd;

// Set selection without scrolling
scintilla.SetEmptySelection(position);

// Replace selection
scintilla.ReplaceSelection("New text");
```

### Caret and Anchor

```csharp
// Get/set current position
int pos = scintilla.CurrentPosition;
scintilla.CurrentPosition = 100;

// Get/set anchor position
int anchor = scintilla.AnchorPosition;
scintilla.AnchorPosition = 50;

// Scroll caret into view
scintilla.ScrollCaret();

// Set caret color
Color caretColor = new Color();
Color.Parse("#000000", ref caretColor);
scintilla.CaretForeColor = caretColor;

// Highlight current line
scintilla.CaretLineVisible = true;
Color lineColor = new Color();
Color.Parse("#FFFFCC", ref lineColor);
scintilla.CaretLineBackColor = lineColor;
```

### Line Operations

```csharp
// Get line count
int lineCount = scintilla.Lines.Count;

// Access a specific line
var line = scintilla.Lines[0];

// Get line text
string lineText = line.Text;

// Get line length
int lineLength = line.Length;

// Go to line
scintilla.GotoPosition(line.Position);

// Delete a line
line.Delete();
```

---

## Styling and Syntax Highlighting

### Setting Up a Lexer

```csharp
using ScintillaNet.Abstractions.Classes.Lexers;
using ScintillaNet.Abstractions.Enumerations;

// Set lexer by name
scintilla.LexerName = "cpp";

// Configure lexer properties
scintilla.SetProperty("fold", "1");
scintilla.SetProperty("fold.compact", "1");
scintilla.SetProperty("fold.preprocessor", "1");

// Set keywords
scintilla.SetKeywords(0, 
    "if else while for return break continue switch case default");
scintilla.SetKeywords(1, 
    "int char float double void bool");
```

### Configuring Styles

```csharp
Color FromHtml(string value)
{
    var color = new Color();
    Color.Parse(value, ref color);
    return color;
}

// Configure individual styles
scintilla.Styles[Cpp.Default].ForeColor = FromHtml("#000000");
scintilla.Styles[Cpp.Default].BackColor = FromHtml("#FFFFFF");

scintilla.Styles[Cpp.Word].ForeColor = FromHtml("#0000FF");
scintilla.Styles[Cpp.Word].Bold = true;

scintilla.Styles[Cpp.Number].ForeColor = FromHtml("#FF8000");

scintilla.Styles[Cpp.String].ForeColor = FromHtml("#008000");

scintilla.Styles[Cpp.Comment].ForeColor = FromHtml("#808080");
scintilla.Styles[Cpp.Comment].Italic = true;

scintilla.Styles[Cpp.Operator].ForeColor = FromHtml("#000080");
scintilla.Styles[Cpp.Operator].Bold = true;

// Set font properties
scintilla.Styles[Cpp.Default].Font = "Monospace";
scintilla.Styles[Cpp.Default].Size = 10;
```

### Common Style Properties

```csharp
var style = scintilla.Styles[0];

// Colors
style.ForeColor = myColor;
style.BackColor = myColor;

// Font properties
style.Font = "Monospace";
style.Size = 10;
style.Bold = true;
style.Italic = true;
style.Underline = true;

// Visibility
style.Visible = true;

// Case
style.Case = StyleCase.Mixed;

// Hotspot
style.Hotspot = false;
```

---

## Collections

### StyleCollection

The `Styles` collection provides access to all 256 style definitions.

```csharp
// Access by index
var style = scintilla.Styles[0];

// Common predefined styles
scintilla.Styles[Style.Default].ForeColor = myColor;
scintilla.Styles[Style.LineNumber].BackColor = myColor;
scintilla.Styles[Style.CallTip].Font = "Sans";
```

### LineCollection

The `Lines` collection provides access to all lines in the document.

```csharp
// Iterate through lines
foreach (var line in scintilla.Lines)
{
    Console.WriteLine($"Line {line.Index}: {line.Text}");
}

// Access specific line
var firstLine = scintilla.Lines[0];
var lastLine = scintilla.Lines[scintilla.Lines.Count - 1];

// Line properties
int position = line.Position;      // Start position
int length = line.Length;          // Line length in characters
string text = line.Text;           // Line text
bool visible = line.Visible;       // Visibility
int height = line.Height;          // Height in pixels
```

### MarginCollection

```csharp
// Configure margins (0-4 available)
var margin = scintilla.Margins[0];

// Common margin configurations
// Line numbers
scintilla.Margins[0].Type = MarginType.Number;
scintilla.Margins[0].Width = 40;

// Symbols (for markers)
scintilla.Margins[1].Type = MarginType.Symbol;
scintilla.Margins[1].Width = 16;
scintilla.Margins[1].Mask = MarkerConstants.MaskAll;
scintilla.Margins[1].Sensitive = true;
```

### MarkerCollection

```csharp
// Configure marker appearance
scintilla.Markers[0].Symbol = MarkerSymbol.Circle;
scintilla.Markers[0].SetForeColor(myColor);
scintilla.Markers[0].SetBackColor(myColor);

// Add marker to a line
int markerHandle = line.MarkerAdd(0);

// Remove marker
line.MarkerDelete(0);

// Check if marker exists
bool hasMarker = (line.MarkerGet() & (1 << 0)) != 0;
```

### IndicatorCollection

```csharp
// Configure indicator appearance
scintilla.Indicators[0].Style = IndicatorStyle.Squiggle;
scintilla.Indicators[0].ForeColor = myColor;
scintilla.Indicators[0].Alpha = 128;
scintilla.Indicators[0].OutlineAlpha = 255;

// Apply indicator to range
scintilla.IndicatorCurrent = 0;
scintilla.IndicatorFillRange(startPos, length);

// Clear indicator from range
scintilla.IndicatorClearRange(startPos, length);
```

### SelectionCollection

```csharp
// Access selections
int selCount = scintilla.Selections.Count;

// Get main selection
var mainSel = scintilla.Selections[scintilla.MainSelection];

// Selection properties
int start = mainSel.Start;
int end = mainSel.End;
int caret = mainSel.Caret;
int anchor = mainSel.Anchor;
```

---

## Margins

### Margin Types

```csharp
// Line number margin
scintilla.Margins[0].Type = MarginType.Number;
scintilla.Margins[0].Width = 40;

// Symbol margin (for markers)
scintilla.Margins[1].Type = MarginType.Symbol;
scintilla.Margins[1].Width = 16;

// Background color margin
scintilla.Margins[2].Type = MarginType.Color;
scintilla.Margins[2].Width = 4;

// Text margin
scintilla.Margins[3].Type = MarginType.Text;
scintilla.Margins[3].Width = 100;

// Right-justified text
scintilla.Margins[4].Type = MarginType.RightText;
```

### Margin Properties

```csharp
var margin = scintilla.Margins[0];

// Width
margin.Width = 40;

// Sensitive to mouse clicks
margin.Sensitive = true;

// Marker mask (which markers can display)
margin.Mask = MarkerConstants.MaskFolders;

// Cursor style
margin.Cursor = MarginCursor.Arrow;

// Background color (for color type)
margin.BackColor = myColor;
```

### Margin Events

```csharp
scintilla.MarginClick += (sender, e) =>
{
    if (e.Margin == 2) // Fold margin
    {
        var line = scintilla.Lines[e.Line];
        line.ToggleFold();
    }
};

scintilla.MarginRightClick += (sender, e) =>
{
    // Handle right-click on margin
};
```

---

## Markers

### Marker Symbols

```csharp
// Built-in symbols
scintilla.Markers[0].Symbol = MarkerSymbol.Circle;
scintilla.Markers[1].Symbol = MarkerSymbol.RoundRect;
scintilla.Markers[2].Symbol = MarkerSymbol.Arrow;
scintilla.Markers[3].Symbol = MarkerSymbol.SmallRect;
scintilla.Markers[4].Symbol = MarkerSymbol.ShortArrow;
scintilla.Markers[5].Symbol = MarkerSymbol.Empty;
scintilla.Markers[6].Symbol = MarkerSymbol.ArrowDown;
scintilla.Markers[7].Symbol = MarkerSymbol.Minus;
scintilla.Markers[8].Symbol = MarkerSymbol.Plus;
```

### Marker Colors

```csharp
var marker = scintilla.Markers[0];

Color foreColor = new Color();
Color.Parse("#FFFFFF", ref foreColor);
marker.SetForeColor(foreColor);

Color backColor = new Color();
Color.Parse("#FF0000", ref backColor);
marker.SetBackColor(backColor);

// Alpha transparency
marker.SetAlpha(128);
```

### Using Markers

```csharp
// Add marker to a line
var line = scintilla.Lines[10];
int handle = line.MarkerAdd(0);

// Delete marker
line.MarkerDelete(0);

// Delete all markers of a type
scintilla.MarkerDeleteAll(0);

// Get markers on a line
uint markerMask = line.MarkerGet();

// Find next line with marker
int nextLine = scintilla.Lines[currentLine].MarkerNext(1 << 0);

// Find previous line with marker
int prevLine = scintilla.Lines[currentLine].MarkerPrevious(1 << 0);
```

### Custom Marker Images

```csharp
using Gtk;

// Load image
var image = new Image("marker.png");

// Define as marker
scintilla.Markers[0].DefineRgbaImage(image);
```

---

## Indicators

### Indicator Styles

```csharp
// Squiggle underline (like spell-check)
scintilla.Indicators[0].Style = IndicatorStyle.Squiggle;

// Straight underline
scintilla.Indicators[1].Style = IndicatorStyle.Plain;

// TT style (wavy)
scintilla.Indicators[2].Style = IndicatorStyle.TT;

// Diagonal hatching
scintilla.Indicators[3].Style = IndicatorStyle.Diagonal;

// Strike-out
scintilla.Indicators[4].Style = IndicatorStyle.Strike;

// Highlight box
scintilla.Indicators[5].Style = IndicatorStyle.Box;

// Rounded box
scintilla.Indicators[6].Style = IndicatorStyle.RoundBox;

// Straight box
scintilla.Indicators[7].Style = IndicatorStyle.StraightBox;

// Dotted underline
scintilla.Indicators[8].Style = IndicatorStyle.Dots;

// Dashed underline
scintilla.Indicators[9].Style = IndicatorStyle.Dash;
```

### Applying Indicators

```csharp
// Configure indicator
scintilla.Indicators[0].Style = IndicatorStyle.Squiggle;
Color indicatorColor = new Color();
Color.Parse("#FF0000", ref indicatorColor);
scintilla.Indicators[0].ForeColor = indicatorColor;

// Set as current
scintilla.IndicatorCurrent = 0;

// Fill range with indicator
scintilla.IndicatorFillRange(startPos, length);

// Clear range
scintilla.IndicatorClearRange(startPos, length);

// Get indicator value at position
int value = scintilla.IndicatorValueAt(0, position);
```

### Indicator Events

```csharp
scintilla.IndicatorClick += (sender, e) =>
{
    Console.WriteLine($"Indicator clicked at position {e.Position}");
};

scintilla.IndicatorRelease += (sender, e) =>
{
    Console.WriteLine($"Indicator released at position {e.Position}");
};
```

---

## Code Folding

### Setting Up Folding

```csharp
// Enable folding in lexer
scintilla.SetProperty("fold", "1");
scintilla.SetProperty("fold.compact", "1");
scintilla.SetProperty("fold.comment", "1");
scintilla.SetProperty("fold.preprocessor", "1");

// Configure fold margin
scintilla.Margins[2].Type = MarginType.Symbol;
scintilla.Margins[2].Mask = MarkerConstants.MaskFolders;
scintilla.Margins[2].Sensitive = true;
scintilla.Margins[2].Width = 20;
```

### Fold Markers

```csharp
Color foldFore = new Color();
Color.Parse("#F5F3ED", ref foldFore);
Color foldBack = new Color();
Color.Parse("#5E5C56", ref foldBack);

// Configure folding markers
for (int i = 25; i <= 31; i++)
{
    scintilla.Markers[i].SetForeColor(foldFore);
    scintilla.Markers[i].SetBackColor(foldBack);
}

// Set marker symbols
scintilla.Markers[MarkerConstants.Folder].Symbol = MarkerSymbol.BoxPlus;
scintilla.Markers[MarkerConstants.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
scintilla.Markers[MarkerConstants.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
scintilla.Markers[MarkerConstants.FolderMidTail].Symbol = MarkerSymbol.TCorner;
scintilla.Markers[MarkerConstants.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
scintilla.Markers[MarkerConstants.FolderSub].Symbol = MarkerSymbol.VLine;
scintilla.Markers[MarkerConstants.FolderTail].Symbol = MarkerSymbol.LCorner;
```

### Automatic Folding

```csharp
using ScintillaNet.Abstractions.Enumerations;

// Enable automatic folding
scintilla.AutomaticFold = AutomaticFold.Show | 
                          AutomaticFold.Click | 
                          AutomaticFold.Change;
```

### Manual Folding Operations

```csharp
var line = scintilla.Lines[10];

// Toggle fold
line.ToggleFold();

// Expand/collapse
line.FoldExpanded = false; // Collapse
line.FoldExpanded = true;  // Expand

// Check fold level
int foldLevel = line.FoldLevel;
FoldLevelFlags flags = line.FoldLevelFlags;

// Check if header
bool isHeader = (flags & FoldLevelFlags.Header) != 0;

// Fold all
scintilla.FoldAll(FoldAction.Contract);

// Expand all
scintilla.FoldAll(FoldAction.Expand);

// Toggle all
scintilla.FoldAll(FoldAction.Toggle);
```

### Fold Display Text

```csharp
// Set text shown when folded
line.ToggleFoldShowText("...");

// Configure fold text style
scintilla.FoldDisplayTextSetStyle(FoldDisplayText.Standard);
```

---

## Auto-Completion

### Basic Auto-Completion

```csharp
// Show auto-completion list
scintilla.AutoCShow(0, "apple banana cherry date elderberry");

// With custom separator
scintilla.AutoCSeparator = ' ';
scintilla.AutoCShow(0, "apple banana cherry");

// Cancel auto-completion
scintilla.AutoCCancel();

// Check if active
if (scintilla.AutoCActive)
{
    // Auto-completion is showing
}
```

### Auto-Completion Configuration

```csharp
// Case sensitivity
scintilla.AutoCIgnoreCase = true;

// Auto-hide when no match
scintilla.AutoCAutoHide = true;

// Cancel at start of word
scintilla.AutoCCancelAtStart = false;

// Choose single match automatically
scintilla.AutoCChooseSingle = false;

// Delete word after insertion
scintilla.AutoCDropRestOfWord = false;

// Maximum height in rows
scintilla.AutoCMaxHeight = 10;

// Maximum width in characters
scintilla.AutoCMaxWidth = 0; // 0 = auto-size

// Sort order
scintilla.AutoCOrder = Order.Presorted;
```

### Auto-Completion with Images

```csharp
// Set type separator (default is '?')
scintilla.AutoCTypeSeparator = '?';

// Register images (type 1, 2, 3, etc.)
var image1 = new Image("class.png");
scintilla.RegisterRgbaImage(1, image1);

var image2 = new Image("method.png");
scintilla.RegisterRgbaImage(2, image2);

// Show with images
scintilla.AutoCShow(0, "MyClass?1 MyMethod?2 MyProperty?1");
```

### Auto-Completion Events

```csharp
scintilla.AutoCSelection += (sender, e) =>
{
    Console.WriteLine($"Selected: {e.Text}");
};

scintilla.AutoCCompleted += (sender, e) =>
{
    Console.WriteLine($"Completed: {e.Text}");
};

scintilla.AutoCCancelled += (sender, e) =>
{
    Console.WriteLine("Auto-completion cancelled");
};

scintilla.AutoCCharDeleted += (sender, e) =>
{
    Console.WriteLine("Character deleted during auto-completion");
};

scintilla.AutoCSelectionChange += (sender, e) =>
{
    Console.WriteLine($"Selection changed to: {e.Text}");
};
```

### Fill-Up Characters

```csharp
// Set characters that accept the current selection
scintilla.AutoCSetFillUps("([");
```

### Stop Characters

```csharp
// Set characters that cancel auto-completion
scintilla.AutoCStops(" .,;:!?");
```

---

## Call Tips

### Showing Call Tips

```csharp
// Show call tip at position
scintilla.CallTipShow(position, "function(int param1, string param2)");

// Cancel call tip
scintilla.CallTipCancel();

// Check if active
if (scintilla.CallTipActive)
{
    // Call tip is showing
}
```

### Call Tip Styling

```csharp
// Set background color
Color bgColor = new Color();
Color.Parse("#FFFFCC", ref bgColor);
scintilla.CallTipSetBackColor(bgColor);

// Set foreground color
Color fgColor = new Color();
Color.Parse("#000000", ref fgColor);
scintilla.CallTipSetForeColor(fgColor);

// Set highlight color
Color hlColor = new Color();
Color.Parse("#0000FF", ref hlColor);
scintilla.CallTipSetForeHighlightColor(hlColor);
```

### Call Tip Highlighting

```csharp
// Highlight portion of call tip
scintilla.CallTipShow(position, "function(int param1, string param2)");
scintilla.CallTipSetHighlight(9, 19); // Highlights "int param1"
```

### Call Tip Position

```csharp
// Set position relative to text
scintilla.CallTipPosStart = position;

// Use tabs in call tip
scintilla.CallTipUseStyle = 0; // Use default style
```

### Call Tip Events

```csharp
scintilla.CallTipClick += (sender, e) =>
{
    switch (e.CallTipClickType)
    {
        case CallTipClickType.UpArrow:
            // Show previous signature
            break;
        case CallTipClickType.DownArrow:
            // Show next signature
            break;
        default:
            // Clicked elsewhere
            break;
    }
};
```

---

## Search and Replace

### Simple Text Search

```csharp
// Find first occurrence
scintilla.TargetStart = 0;
scintilla.TargetEnd = scintilla.TextLength;
scintilla.SearchFlags = SearchFlags.None;

int pos = scintilla.SearchInTarget("searchText");
if (pos != -1)
{
    // Found at position
    scintilla.SetSelection(scintilla.TargetStart, scintilla.TargetEnd);
}
```

### Search Flags

```csharp
using ScintillaNet.Abstractions.Enumerations;

// Match case
scintilla.SearchFlags = SearchFlags.MatchCase;

// Match whole word
scintilla.SearchFlags = SearchFlags.WholeWord;

// Regular expression
scintilla.SearchFlags = SearchFlags.RegEx;

// POSIX regular expression
scintilla.SearchFlags = SearchFlags.Posix;

// Combined flags
scintilla.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;
```

### Replace in Target

```csharp
// Set target range
scintilla.TargetStart = 0;
scintilla.TargetEnd = scintilla.TextLength;

// Find and replace
int pos = scintilla.SearchInTarget("oldText");
if (pos != -1)
{
    scintilla.ReplaceTarget("newText");
}
```

### Regular Expression Search

```csharp
// Enable regex
scintilla.SearchFlags = SearchFlags.RegEx;

// Search with regex
scintilla.TargetStart = 0;
scintilla.TargetEnd = scintilla.TextLength;
int pos = scintilla.SearchInTarget(@"\b\w+@\w+\.\w+\b"); // Email pattern

if (pos != -1)
{
    // Get capture group
    string tag = scintilla.GetTag(1);
    
    // Replace with backreferences
    scintilla.ReplaceTargetRe(@"<\1>");
}
```

### Find All Occurrences

```csharp
List<int> FindAll(string searchText)
{
    var results = new List<int>();
    scintilla.TargetStart = 0;
    scintilla.TargetEnd = scintilla.TextLength;
    scintilla.SearchFlags = SearchFlags.None;

    while (true)
    {
        int pos = scintilla.SearchInTarget(searchText);
        if (pos == -1)
            break;

        results.Add(pos);
        
        // Move past this match
        scintilla.TargetStart = scintilla.TargetEnd;
        scintilla.TargetEnd = scintilla.TextLength;
    }

    return results;
}
```

### Replace All

```csharp
void ReplaceAll(string searchText, string replaceText)
{
    scintilla.TargetStart = 0;
    scintilla.TargetEnd = scintilla.TextLength;
    scintilla.SearchFlags = SearchFlags.None;

    while (true)
    {
        int pos = scintilla.SearchInTarget(searchText);
        if (pos == -1)
            break;

        scintilla.ReplaceTarget(replaceText);
        
        // Move past the replacement
        scintilla.TargetStart = scintilla.TargetEnd;
        scintilla.TargetEnd = scintilla.TextLength;
    }
}
```

---

## Multiple Selections

### Enabling Multiple Selections

```csharp
// Enable multiple selections
scintilla.MultipleSelection = true;

// Allow rectangular selection with Alt+Mouse
scintilla.MouseSelectionRectangularSwitch = true;

// Allow typing in multiple selections
scintilla.AdditionalSelectionTyping = true;

// Make additional carets visible
scintilla.AdditionalCaretsVisible = true;

// Set transparency of additional selections
scintilla.AdditionalSelAlpha = 128; // 0-255, or 256 for opaque
```

### Adding Selections

```csharp
// Add a new selection
scintilla.AddSelection(caretPos, anchorPos);

// Set main selection
scintilla.SetSelection(startPos, endPos);

// Get number of selections
int count = scintilla.Selections.Count;

// Get/set main selection index
int mainSel = scintilla.MainSelection;
scintilla.MainSelection = 0;
```

### Working with Multiple Selections

```csharp
// Iterate through selections
foreach (var selection in scintilla.Selections)
{
    int start = selection.Start;
    int end = selection.End;
    int caret = selection.Caret;
    int anchor = selection.Anchor;
}

// Remove a selection
scintilla.DropSelection(selectionIndex);

// Clear all but main
scintilla.ClearSelections();

// Rotate main selection
scintilla.RotateSelection();
```

### Rectangular Selection

```csharp
// Get/set rectangular selection bounds
int rectAnchor = scintilla.RectangularSelectionAnchor;
int rectCaret = scintilla.RectangularSelectionCaret;

scintilla.RectangularSelectionAnchor = startPos;
scintilla.RectangularSelectionCaret = endPos;

// Virtual space in rectangular selection
scintilla.RectangularSelectionAnchorVirtualSpace = 5;
scintilla.RectangularSelectionCaretVirtualSpace = 10;
```

### Multiple Selection Colors

```csharp
// Set additional selection colors
Color selColor = new Color();
Color.Parse("#0000FF", ref selColor);
scintilla.SetAdditionalSelFore(selColor);

Color.Parse("#CCCCFF", ref selColor);
scintilla.SetAdditionalSelBack(selColor);
```

---

## Events

### Text Modification Events

```csharp
// Before text is inserted
scintilla.BeforeInsert += (sender, e) =>
{
    Console.WriteLine($"About to insert {e.Text.Length} bytes at {e.Position}");
};

// After text is inserted
scintilla.Insert += (sender, e) =>
{
    Console.WriteLine($"Inserted: {e.Text}");
};

// Before text is deleted
scintilla.BeforeDelete += (sender, e) =>
{
    Console.WriteLine($"About to delete {e.Text.Length} bytes at {e.Position}");
};

// After text is deleted
scintilla.Delete += (sender, e) =>
{
    Console.WriteLine($"Deleted: {e.Text}");
};

// Character added (typed)
scintilla.CharAdded += (sender, e) =>
{
    char ch = (char)e.Char;
    Console.WriteLine($"Character added: {ch}");
};

// Attempt to modify read-only document
scintilla.ModifyAttempt += (sender, e) =>
{
    Console.WriteLine("Attempted to modify read-only document");
};
```

### Save Point Events

```csharp
// Document saved
scintilla.SavePointReached += (sender, e) =>
{
    Console.WriteLine("Document saved");
};

// Document modified after save
scintilla.SavePointLeft += (sender, e) =>
{
    Console.WriteLine("Document has unsaved changes");
};
```

### UI Events

```csharp
// UI updated (selection, scroll, etc.)
scintilla.UpdateUi += (sender, e) =>
{
    if ((e.Change & UpdateChange.Selection) != 0)
    {
        // Selection changed
    }
    if ((e.Change & UpdateChange.Content) != 0)
    {
        // Content changed
    }
};

// Editor painted
scintilla.Painted += (sender, e) =>
{
    // Drawing complete
};

// Zoom changed
scintilla.ZoomChanged += (sender, e) =>
{
    Console.WriteLine($"Zoom level: {scintilla.Zoom}");
};
```

### User Interaction Events

```csharp
// Double-click
scintilla.DoubleClick += (sender, e) =>
{
    Console.WriteLine($"Double-clicked at line {e.Line}, position {e.Position}");
};

// Margin clicked
scintilla.MarginClick += (sender, e) =>
{
    if (e.Margin == 2) // Fold margin
    {
        var line = scintilla.Lines[e.Line];
        line.ToggleFold();
    }
};

scintilla.MarginRightClick += (sender, e) =>
{
    // Right-click on margin
};

// Mouse dwelling
scintilla.DwellStart += (sender, e) =>
{
    // Show tooltip or hint
};

scintilla.DwellEnd += (sender, e) =>
{
    // Hide tooltip
};
```

### Hotspot Events

```csharp
// Hotspot clicked
scintilla.HotspotClick += (sender, e) =>
{
    Console.WriteLine($"Hotspot clicked at {e.Position}");
};

scintilla.HotspotDoubleClick += (sender, e) =>
{
    Console.WriteLine($"Hotspot double-clicked at {e.Position}");
};

scintilla.HotspotReleaseClick += (sender, e) =>
{
    Console.WriteLine($"Hotspot released at {e.Position}");
};
```

### Styling Events

```csharp
// Styling needed
scintilla.StyleNeeded += (sender, e) =>
{
    int startPos = scintilla.GetEndStyled();
    int endPos = e.Position;
    
    // Perform custom styling
    // ...
};
```

---

## Advanced Features

### Virtual Space

```csharp
// Enable virtual space
scintilla.VirtualSpaceOptions = VirtualSpace.RectangularSelection | 
                                 VirtualSpace.UserAccessible;

// Rectangular selection only
scintilla.VirtualSpaceOptions = VirtualSpace.RectangularSelection;

// Allow in all selections
scintilla.VirtualSpaceOptions = VirtualSpace.UserAccessible;
```

### Annotations

```csharp
// Add annotation to a line
var line = scintilla.Lines[10];
line.AnnotationText = "This is an annotation";

// Set annotation style
line.AnnotationStyle = 0;

// Configure annotation visibility
scintilla.AnnotationVisible = Annotation.Boxed;
// Options: Hidden, Standard, Boxed, Indented

// Clear all annotations
scintilla.AnnotationClearAll();
```

### Document Management

```csharp
// Create a new document
var doc = scintilla.CreateDocument();

// Switch to document
scintilla.Document = doc;

// Release document (decrease reference count)
scintilla.ReleaseDocument(doc);

// Add reference to document
scintilla.AddRefDocument(doc);
```

### Scrolling and View

```csharp
// Scroll to position
scintilla.ScrollCaret();

// Scroll range into view
scintilla.ScrollRange(startPos, endPos);

// Set scroll width
scintilla.ScrollWidth = 2000;
scintilla.ScrollWidthTracking = true; // Auto-expand

// Get lines on screen
int linesOnScreen = scintilla.LinesOnScreen;

// First visible line
int firstLine = scintilla.FirstVisibleLine;

// Line from visible line
int docLine = scintilla.DocLineFromVisible(visibleLine);
```

### Zoom

```csharp
// Set zoom level (-10 to 20)
scintilla.Zoom = 2;

// Zoom in/out
scintilla.ZoomIn();
scintilla.ZoomOut();

// Reset zoom
scintilla.Zoom = 0;
```

### End-of-Line Mode

```csharp
// Set EOL mode
scintilla.EolMode = EolMode.CrLf;  // Windows
scintilla.EolMode = EolMode.Lf;    // Unix/Linux
scintilla.EolMode = EolMode.Cr;    // Mac

// Convert all line endings
scintilla.ConvertEols(EolMode.Lf);

// Show EOL characters
scintilla.ViewEol = true;
```

### Whitespace Display

```csharp
// Show whitespace
scintilla.ViewWhitespace = WhitespaceMode.VisibleAlways;
scintilla.ViewWhitespace = WhitespaceMode.VisibleAfterIndent;
scintilla.ViewWhitespace = WhitespaceMode.Invisible;

// Set whitespace colors
Color wsColor = new Color();
Color.Parse("#808080", ref wsColor);
scintilla.SetWhitespaceForeColor(true, wsColor);
scintilla.SetWhitespaceBackColor(true, wsColor);

// Whitespace size
scintilla.WhitespaceSize = 2;
```

### Indentation

```csharp
// Tab width
scintilla.TabWidth = 4;

// Use tabs or spaces
scintilla.UseTabs = false;

// Indentation width (for spaces)
scintilla.IndentWidth = 4;

// Tab indents
scintilla.TabIndents = true;

// Backspace unindents
scintilla.BackspaceUnIndents = true;

// Indentation guides
scintilla.IndentationGuides = IndentView.Real;
// Options: None, Real, LookForward, LookBoth
```

### Long Lines

```csharp
// Edge mode
scintilla.EdgeMode = EdgeMode.Line;  // Vertical line
scintilla.EdgeMode = EdgeMode.Background;  // Background color
scintilla.EdgeMode = EdgeMode.MultiLine;  // Multiple lines

// Edge column
scintilla.EdgeColumn = 80;

// Edge color
Color edgeColor = new Color();
Color.Parse("#C0C0C0", ref edgeColor);
scintilla.EdgeColor = edgeColor;
```

### Word Wrap

```csharp
// Enable word wrap
scintilla.WrapMode = WrapMode.Word;
scintilla.WrapMode = WrapMode.Char;
scintilla.WrapMode = WrapMode.Whitespace;
scintilla.WrapMode = WrapMode.None;

// Wrap visual flags
scintilla.WrapVisualFlags = WrapVisualFlags.End;
scintilla.WrapVisualFlags = WrapVisualFlags.Start;
scintilla.WrapVisualFlags = WrapVisualFlags.Margin;

// Wrap indent mode
scintilla.WrapIndentMode = WrapIndentMode.Fixed;
scintilla.WrapIndentMode = WrapIndentMode.Same;
scintilla.WrapIndentMode = WrapIndentMode.Indent;
```

### Lexer Properties

```csharp
// Get lexer language
string language = scintilla.LexerLanguage;

// Describe keyword sets
string keywordSets = scintilla.DescribeKeywordSets();

// Get/set property
scintilla.SetProperty("fold", "1");
string value = scintilla.GetScintillaProperty("fold");

// Get property expanded (with macros)
string expanded = scintilla.GetPropertyExpanded("fold");

// Get property as int
int intValue = scintilla.GetPropertyInt("fold", 0);

// Describe property
string description = scintilla.DescribeProperty("fold");

// Get property names
string[] names = scintilla.PropertyNames();

// Get property type
PropertyType type = scintilla.PropertyType("fold");
```

### Direct Messaging

```csharp
// For advanced users - direct Scintilla messages
IntPtr result = scintilla.DirectMessage(SCI_GETLENGTH);

// With parameters
result = scintilla.DirectMessage(SCI_SETTEXT, IntPtr.Zero, textPtr);
```

---

## Best Practices

### Performance Optimization

```csharp
// Disable drawing during bulk updates
scintilla.BeginUndoAction();
try
{
    // Bulk operations here
}
finally
{
    scintilla.EndUndoAction();
}

// Use target ranges for search/replace instead of repeated searches

// Clear unnecessary styling/indicators when not needed
```

### Memory Management

```csharp
// Properly manage document references
var doc = scintilla.CreateDocument();
try
{
    // Use document
}
finally
{
    scintilla.ReleaseDocument(doc);
}

// Clear large text when done
scintilla.ClearAll();
```

### Styling Best Practices

```csharp
// Set up base styles first
scintilla.StyleResetDefault();
scintilla.Styles[Style.Default].Font = "Monospace";
scintilla.Styles[Style.Default].Size = 10;
scintilla.StyleClearAll(); // Copy to all styles

// Then configure specific styles
// ...

// Use style hotspots sparingly
// Configure colors in RGB format for consistency
```

### Event Handling

```csharp
// Unsubscribe from events when disposing
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        scintilla.CharAdded -= OnCharAdded;
        scintilla.TextChanged -= OnTextChanged;
        // ... other events
    }
    base.Dispose(disposing);
}

// Use BeginInvoke for UI updates from events if needed
```

### Error Handling

```csharp
// Check status after operations
scintilla.Status = Status.Ok;
// ... operations
if (scintilla.Status != Status.Ok)
{
    // Handle error
}

// Validate input ranges
int pos = Math.Min(position, scintilla.TextLength);
```

### Cross-Platform Considerations

```csharp
// Use Gdk.Color for colors (not System.Drawing.Color)
Color color = new Color();
Color.Parse("#FF0000", ref color);

// Be aware of native library dependencies
// Ensure libscintilla.so is available

// Test on target Linux distributions
```

---

## Example: Complete Code Editor

Here's a complete example putting it all together:

```csharp
using Gtk;
using ScintillaNet.Gtk;
using ScintillaNet.Abstractions.Classes.Lexers;
using ScintillaNet.Abstractions.Enumerations;
using Gdk;
using Color = Gdk.Color;

class CodeEditor : Window
{
    private Scintilla scintilla;
    
    public CodeEditor() : base("Code Editor")
    {
        SetDefaultSize(800, 600);
        DeleteEvent += (o, args) => Application.Quit();
        
        // Create editor
        scintilla = new Scintilla();
        
        // Configure editor
        SetupEditor();
        SetupStyling();
        SetupMargins();
        SetupFolding();
        SetupEvents();
        
        Add(scintilla);
        ShowAll();
    }
    
    private void SetupEditor()
    {
        // Basic settings
        scintilla.TabWidth = 4;
        scintilla.UseTabs = false;
        scintilla.IndentWidth = 4;
        scintilla.TabIndents = true;
        scintilla.BackspaceUnIndents = true;
        
        // Whitespace
        scintilla.ViewWhitespace = WhitespaceMode.Invisible;
        scintilla.IndentationGuides = IndentView.LookBoth;
        
        // EOL
        scintilla.EolMode = EolMode.Lf;
        scintilla.ViewEol = false;
        
        // Caret
        scintilla.CaretLineVisible = true;
        Color lineColor = new Color();
        Color.Parse("#FFFFEE", ref lineColor);
        scintilla.CaretLineBackColor = lineColor;
        
        // Selection
        scintilla.MultipleSelection = true;
        scintilla.AdditionalSelectionTyping = true;
        
        // Scrolling
        scintilla.ScrollWidth = 1;
        scintilla.ScrollWidthTracking = true;
    }
    
    private void SetupStyling()
    {
        // Set lexer
        scintilla.LexerName = "cpp";
        
        // Configure lexer
        scintilla.SetProperty("fold", "1");
        scintilla.SetProperty("fold.compact", "1");
        scintilla.SetProperty("fold.preprocessor", "1");
        
        // Keywords
        scintilla.SetKeywords(0, 
            "if else while for return break continue switch case default");
        scintilla.SetKeywords(1, 
            "int char float double void bool");
        
        // Styles
        Color FromHtml(string hex)
        {
            var color = new Color();
            Color.Parse(hex, ref color);
            return color;
        }
        
        scintilla.Styles[Cpp.Default].ForeColor = FromHtml("#000000");
        scintilla.Styles[Cpp.Word].ForeColor = FromHtml("#0000FF");
        scintilla.Styles[Cpp.Word].Bold = true;
        scintilla.Styles[Cpp.Number].ForeColor = FromHtml("#FF8000");
        scintilla.Styles[Cpp.String].ForeColor = FromHtml("#008000");
        scintilla.Styles[Cpp.Comment].ForeColor = FromHtml("#808080");
        scintilla.Styles[Cpp.CommentLine].ForeColor = FromHtml("#808080");
        scintilla.Styles[Cpp.Operator].ForeColor = FromHtml("#000080");
    }
    
    private void SetupMargins()
    {
        // Line numbers
        scintilla.Margins[0].Type = MarginType.Number;
        scintilla.Margins[0].Width = 40;
        
        // Symbols
        scintilla.Margins[1].Type = MarginType.Symbol;
        scintilla.Margins[1].Width = 16;
        scintilla.Margins[1].Mask = MarkerConstants.MaskAll & ~MarkerConstants.MaskFolders;
        scintilla.Margins[1].Sensitive = true;
    }
    
    private void SetupFolding()
    {
        // Fold margin
        scintilla.Margins[2].Type = MarginType.Symbol;
        scintilla.Margins[2].Mask = MarkerConstants.MaskFolders;
        scintilla.Margins[2].Sensitive = true;
        scintilla.Margins[2].Width = 20;
        
        // Fold markers
        Color FromHtml(string hex)
        {
            var color = new Color();
            Color.Parse(hex, ref color);
            return color;
        }
        
        var foldFore = FromHtml("#F5F3ED");
        var foldBack = FromHtml("#5E5C56");
        
        for (int i = 25; i <= 31; i++)
        {
            scintilla.Markers[i].SetForeColor(foldFore);
            scintilla.Markers[i].SetBackColor(foldBack);
        }
        
        scintilla.Markers[MarkerConstants.Folder].Symbol = MarkerSymbol.BoxPlus;
        scintilla.Markers[MarkerConstants.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
        scintilla.Markers[MarkerConstants.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
        scintilla.Markers[MarkerConstants.FolderMidTail].Symbol = MarkerSymbol.TCorner;
        scintilla.Markers[MarkerConstants.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
        scintilla.Markers[MarkerConstants.FolderSub].Symbol = MarkerSymbol.VLine;
        scintilla.Markers[MarkerConstants.FolderTail].Symbol = MarkerSymbol.LCorner;
        
        // Automatic folding
        scintilla.AutomaticFold = AutomaticFold.Show | 
                                  AutomaticFold.Click | 
                                  AutomaticFold.Change;
    }
    
    private void SetupEvents()
    {
        // Margin clicks for folding
        scintilla.MarginClick += (sender, e) =>
        {
            if (e.Margin == 2)
            {
                var line = scintilla.Lines[e.Line];
                line.ToggleFold();
            }
        };
        
        // Update title on modification
        scintilla.SavePointReached += (sender, e) =>
        {
            Title = "Code Editor";
        };
        
        scintilla.SavePointLeft += (sender, e) =>
        {
            Title = "Code Editor *";
        };
    }
    
    public static void Main()
    {
        Application.Init();
        new CodeEditor();
        Application.Run();
    }
}
```

---

## Additional Resources

### Scintilla Documentation
- Official Scintilla documentation: https://www.scintilla.org/ScintillaDoc.html
- Lexilla (lexer library): https://www.scintilla.org/Lexilla.html

### Related Projects
- Scintilla.NET (Windows): https://github.com/VPKSoft/Scintilla.NET
- Original Scintilla: https://www.scintilla.org/
- GtkSharp: https://github.com/GtkSharp/GtkSharp

### Community
- GitHub Issues: https://github.com/VPKSoft/Scintilla.NET.Gtk/issues
- Pull Requests: https://github.com/VPKSoft/Scintilla.NET.Gtk/pulls

---

## Conclusion

Scintilla.NET.Gtk provides a comprehensive and powerful text editing component for GTK# applications on Linux. This guide has covered the major features and usage patterns, but the library offers even more capabilities for advanced use cases. Refer to the XML documentation and Scintilla documentation for additional details.

Happy coding!
