use crate::db::{list_snippets, save_snippet, DbState, Snippet};
use std::sync::Arc;
use tauri::State;

#[tauri::command]
pub async fn cmd_save_snippet(
    state: State<'_, DbState>,
    content: String,
) -> Result<i64, String> {
    let db = Arc::clone(&state.inner().0);
    tauri::async_runtime::spawn_blocking(move || {
        let conn = db.lock().map_err(|e| e.to_string())?;
        save_snippet(&conn, &content).map_err(|e| e.to_string())
    })
    .await
    .map_err(|e| e.to_string())?
}

#[tauri::command]
pub async fn cmd_get_snippets(
    state: State<'_, DbState>,
    query: Option<String>,
) -> Result<Vec<Snippet>, String> {
    let db = Arc::clone(&state.inner().0);
    tauri::async_runtime::spawn_blocking(move || {
        let conn = db.lock().map_err(|e| e.to_string())?;
        list_snippets(&conn, query.as_deref()).map_err(|e| e.to_string())
    })
    .await
    .map_err(|e| e.to_string())?
}

#[tauri::command]
pub async fn cmd_get_clipboard_blockquote() -> Result<String, String> {
    tauri::async_runtime::spawn_blocking(|| {
        let mut cb = arboard::Clipboard::new().map_err(|e| e.to_string())?;

        // arboard 3.x: get().html() -> Result<String, Error>
        match cb.get().html() {
            Ok(html) => {
                let source_url = get_source_url_windows();
                let markdown = htmd::convert(&html).unwrap_or(html);
                Ok(format_blockquote(&markdown, source_url.as_deref()))
            }
            Err(_) => {
                let text = cb.get_text().map_err(|e| e.to_string())?;
                Ok(format_blockquote(&text, None))
            }
        }
    })
    .await
    .map_err(|e| e.to_string())?
}

fn format_blockquote(text: &str, source_url: Option<&str>) -> String {
    let quoted = text
        .lines()
        .map(|l| format!("> {}", l))
        .collect::<Vec<_>>()
        .join("\n");
    match source_url {
        Some(url) => format!("{}\n>\n> Source: {}", quoted, url),
        None => quoted,
    }
}

fn parse_source_url(raw: &str) -> Option<String> {
    raw.lines()
        .find_map(|l| l.strip_prefix("SourceURL:").map(|u| u.trim().to_string()))
        .filter(|u| !u.is_empty())
}

/// On Windows: reads the raw CF_HTML clipboard format via direct Win32 API calls
/// to extract the SourceURL header line. No extra crate needed — links to user32/kernel32.
/// On other platforms: always returns None.
#[cfg(target_os = "windows")]
fn get_source_url_windows() -> Option<String> {
    #[link(name = "user32")]
    extern "system" {
        fn RegisterClipboardFormatA(lpszFormat: *const u8) -> u32;
        fn OpenClipboard(hWndNewOwner: isize) -> i32;
        fn CloseClipboard() -> i32;
        fn GetClipboardData(uFormat: u32) -> isize;
    }
    #[link(name = "kernel32")]
    extern "system" {
        fn GlobalLock(hMem: isize) -> *mut std::ffi::c_void;
        fn GlobalUnlock(hMem: isize) -> i32;
    }

    unsafe {
        let format_name = b"HTML Format\0";
        let format_id = RegisterClipboardFormatA(format_name.as_ptr());
        if format_id == 0 {
            return None;
        }
        if OpenClipboard(0) == 0 {
            return None;
        }
        let handle = GetClipboardData(format_id);
        if handle == 0 {
            CloseClipboard();
            return None;
        }
        let ptr = GlobalLock(handle) as *const u8;
        if ptr.is_null() {
            CloseClipboard();
            return None;
        }
        // CF_HTML data is a null-terminated UTF-8 string
        let mut len = 0usize;
        while *ptr.add(len) != 0 {
            len += 1;
        }
        let bytes = std::slice::from_raw_parts(ptr, len);
        let raw = String::from_utf8_lossy(bytes).into_owned();
        GlobalUnlock(handle);
        CloseClipboard();
        parse_source_url(&raw)
    }
}

#[cfg(not(target_os = "windows"))]
fn get_source_url_windows() -> Option<String> {
    None
}
