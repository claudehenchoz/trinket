# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
cargo build                  # debug build
cargo build --release        # release build (enables windows_subsystem = "windows")
cargo run                    # run in debug mode (console window visible)
cargo check                  # type-check without linking
```

There is no test suite and no frontend build step. The `ui/` directory is served as static files directly from disk via `tauri.conf.json` → `build.frontendDist: "ui"`.

`tauri-build` runs automatically via `build.rs` and validates `tauri.conf.json` and `capabilities/` at compile time. Build errors from `tauri-build` appear as `failed to run custom build command` — check the `--- stdout` section for the actual message (e.g. unknown permission name, bad platform casing).

**Known gotchas:**
- Platform names in `capabilities/*.json` are case-sensitive: `macOS` not `macos`, `windows` not `Windows`.
- `icons/icon.ico` must exist before the first build (required by `tauri-build` for Windows resource embedding). `icons/icon.png` is embedded via `include_bytes!` in `main.rs` for the tray icon.
- Tauri permissions are validated at build time. Use only names from the enumerated list in the build error output — there is no `core:clipboard:*` permission; use `navigator.clipboard` (Web API) from JS instead.

## Architecture

### Process model
Trinket is a **tray-only** app with no main window. Both popup windows (`add` and `get`) are **pre-created hidden at startup** in `tauri.conf.json` and never destroyed — `CloseRequested` events are intercepted in `main.rs` and converted to `.hide()` calls. This gives instant show latency.

### Show/hide flow
`show_window(app, label)` in `main.rs` calls `.show()`, `.center()`, `.set_focus()`, then emits a `"reset"` Tauri event to the window's WebView. Each HTML page listens for `"reset"` and clears its state (empty textarea / reload snippets list).

### Tauri command pattern
All three commands in `src/commands.rs` are `async fn` and use `tauri::async_runtime::spawn_blocking` to run SQLite on the blocking thread pool without holding a tokio worker:
```rust
let db = Arc::clone(&state.inner().0);  // clone Arc before move
spawn_blocking(move || { let conn = db.lock()?; ... }).await?
```
`tauri::State<'_, T>` is not `Send`, so the Arc must be cloned outside the closure.

### Database (`src/db.rs`)
- Storage: `~/Documents/Trinket/trinket.db` (created automatically)
- Schema: `snippets` table + `snippets_fts` FTS5 **external content table** backed by `snippets`. Three triggers (`snippets_ai/ad/au`) keep the FTS index in sync.
- FTS5 is available without a separate feature flag — `rusqlite`'s `bundled` feature compiles SQLite from source with FTS5 enabled.
- Search query safety: user input is split on whitespace, each token wrapped as `"token"*` (quoted prefix match). Special chars inside tokens are escaped by doubling `"`.
- Limits: 500 rows for list-all (by `modified_at DESC`), 200 for FTS5 search (by `rank`/BM25).

### Frontend (`ui/`)
Pure HTML/CSS/JS, no framework, no build tool. `window.__TAURI__` is injected globally (`withGlobalTauri: true`).

**Tauri v2 JS namespaces:**
- `window.__TAURI__.core.invoke(cmd, args)` — call Rust commands
- `window.__TAURI__.window.getCurrentWindow()` — WebviewWindow handle
- `window.__TAURI__.event.listen(event, handler)` — event subscription

**`get.html` virtual scroll:** Fixed `ROW_H = 38px` rows. On each scroll event, only rows within the viewport ± `BUFFER = 6` are rendered as DOM nodes; spacer `<div>`s above/below maintain correct scroll height. A `reqId` counter discards stale responses when the user types faster than results arrive (50ms debounce).

**Clipboard write** uses `navigator.clipboard.writeText()` (standard Web API), not a Tauri plugin.

### Clipboard HTML reading (`src/commands.rs`)
`cmd_get_clipboard_blockquote` uses `arboard` (cross-platform) to read HTML. On Windows, SourceURL is additionally extracted from the raw CF_HTML clipboard format via raw Win32 FFI (`RegisterClipboardFormatA` / `GetClipboardData` / `GlobalLock` — declared inline with `#[link(name = "user32")]`, no extra crate). On non-Windows, SourceURL is silently skipped.

### Theming
Both HTML pages use `color-scheme: light dark` and the CSS `light-dark()` function for all colors. No JS is needed — the WebView inherits the OS dark/light mode automatically.
