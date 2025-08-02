# 📝 Trinket

> **A lightning-fast note-taking app with global hotkeys for instant text capture and retrieval**

Trinket is a minimalist Windows application designed for developers, writers, and power users who need to quickly capture text snippets and find them later. With system-wide hotkeys and instant search, your notes are always just a keystroke away.

## ✨ Features

### 🚀 **Instant Access**
- **Global Hotkeys**: Access your notes from anywhere in Windows
- **Lightning Fast**: Opens and closes in milliseconds
- **Always Available**: Runs quietly in the background, ready when you need it

### 📝 **Smart Note Taking**
- **Quick Capture**: Win+Ctrl+PageUp to instantly add notes
- **Auto-Save**: Notes are automatically saved with unique identifiers
- **Rich Text Support**: Handles multi-line text and special characters

### 🔍 **Powerful Search**
- **Instant Search**: Win+Ctrl+PageDown for immediate access to all notes
- **Live Filtering**: Type to filter results in real-time
- **Keyboard Navigation**: Navigate search results without touching the mouse
- **Smart Selection**: First result is automatically selected

### ⌨️ **Keyboard-First Design**
- **Add Notes**: `Ctrl+Enter` to save and close
- **Search Navigation**: `↑/↓` for single items, `PgUp/PgDn` for 5 items at a time
- **Quick Copy**: `Enter` to copy selected note to clipboard
- **Escape Anywhere**: Quick exit from any dialog

## 🎯 Quick Start

### Installation
1. Download the latest release from the [Releases](../../releases) page
2. Extract `trinket.exe` to your preferred location
3. Run `trinket.exe` - it will start minimized in the system tray

### Basic Usage

| Action | Hotkey | Description |
|--------|--------|-------------|
| **Add Note** | `Win+Ctrl+PageUp` | Open quick note dialog |
| **Find Notes** | `Win+Ctrl+PageDown` | Open search dialog |
| **Save & Close** | `Ctrl+Enter` | Save current note and close |
| **Copy & Close** | `Enter` | Copy selected note to clipboard |
| **Cancel** | `Escape` | Close dialog without saving |

## 🛠️ Development

### Requirements
- .NET 6.0 or later
- Windows 10/11
- Visual Studio 2022 or VS Code

### Building from Source

```bash
# Clone the repository
git clone https://github.com/claudehenchoz/trinket.git
cd trinket

# Build the solution
dotnet build trinket.sln --configuration Release

# Create single-file executable
dotnet publish trinket.sln --configuration Release --output ./publish
```

### Project Structure

```
trinket/
├── trinket/
│   ├── Add.cs              # Quick note input dialog
│   ├── Get.cs              # Search and retrieval dialog  
│   ├── Form1.cs            # Main application window
│   ├── Hotkey.cs           # Global hotkey management
│   ├── Program.cs          # Application entry point
│   └── trinket.csproj      # Project configuration
├── trinket.sln             # Solution file
├── CLAUDE.md              # Development guidance
└── README.md              # This file
```

## 🎨 User Experience

### Adding Notes
1. Press `Win+Ctrl+PageUp` from anywhere
2. Type your note in the text area
3. Press `Ctrl+Enter` to save, or `Escape` to cancel
4. Note is saved automatically with a unique filename

### Finding Notes  
1. Press `Win+Ctrl+PageDown` from anywhere
2. Start typing to filter your notes instantly
3. Use `↑/↓` or `PgUp/PgDn` to navigate results
4. Press `Enter` to copy the selected note to clipboard
5. The dialog closes automatically

### Pro Tips
- **Stay Focused**: Search box always maintains focus for continuous typing
- **Quick Navigation**: Page Up/Down moves 5 items at a time for faster browsing  
- **Instant Copy**: Selected text is immediately available in your clipboard
- **Visual Feedback**: Selected rows are highlighted for clear indication

## 🔧 Technical Details

- **Framework**: .NET 6.0 with Windows Forms
- **Architecture**: Single-file executable with minimal dependencies
- **Storage**: Plain text files with GUID filenames for uniqueness
- **Hotkeys**: Windows API integration for system-wide accessibility
- **Search**: Real-time DataTable filtering for instant results

## 📊 File Management

Notes are stored as individual `.txt` files in the application directory:
- **Naming**: Each note gets a unique GUID filename (e.g., `a1b2c3d4-e5f6-7890-abcd-ef1234567890.txt`)
- **Content**: Plain text with preserved formatting
- **Metadata**: File modification time used for chronological sorting
- **Portability**: Easy to backup, sync, or migrate by copying `.txt` files

## 🤝 Contributing

Contributions are welcome! Here are some ways you can help:

- 🐛 **Report Bugs**: Found an issue? [Open an issue](../../issues)
- 💡 **Feature Requests**: Have an idea? We'd love to hear it
- 🔧 **Code Contributions**: Submit pull requests for improvements
- 📖 **Documentation**: Help improve docs and examples

---

<div align="center">

**Made with ❤️ for efficient note-taking**

[Report Bug](../../issues) • [Request Feature](../../issues) • [Contribute](../../pulls)

</div>