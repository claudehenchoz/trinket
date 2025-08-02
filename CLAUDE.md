# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Trinket is a Windows Forms application built in C# (.NET Framework 4.5) that provides a simple note-taking system with global hotkeys. Users can quickly add text snippets and search through them using keyboard shortcuts.

## Build Commands

- **Build solution**: `dotnet build trinket.sln` or open in Visual Studio and build
- **Debug build**: `dotnet build trinket.sln --configuration Debug`
- **Release build**: `dotnet build trinket.sln --configuration Release`

## Architecture

### Core Components

1. **MainForm (Form1.cs)**: Main application window with menu buttons for Add/Get operations
2. **Add Form**: Dialog for creating new text snippets - saves to GUID-named .txt files
3. **Get Form**: Search interface with DataGrid showing all text files, supports live filtering
4. **Hotkey System**: Global hotkey registration using Windows API
   - Win+PageUp: Opens Add dialog
   - Win+PageDown: Opens Get dialog

### Key Classes

- `MainForm`: Entry point form with hotkey registration in Form_Load
- `Add`: Text input form that saves content to .txt files with GUID names
- `Get`: Search form that loads all .txt files into a DataGrid with filtering
- `Hotkey`: Windows API wrapper for global hotkey registration (MovablePython namespace)

### Data Storage

- Text snippets stored as individual .txt files in application directory
- File names are GUIDs to avoid conflicts
- Get form reads all .txt files and displays in sortable grid by modification date

### UI Patterns

- All forms support Esc key to close
- Forms activate and focus appropriate controls when shown
- Search box in Get form provides live filtering of results