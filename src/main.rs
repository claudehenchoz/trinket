// Suppress console window on Windows in release builds
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

mod commands;
mod db;

use commands::{cmd_get_clipboard_blockquote, cmd_get_snippets, cmd_save_snippet};
use db::DbState;
use std::sync::{Arc, Mutex};
use tauri::{App, AppHandle, Emitter, Manager};
use tauri::menu::{Menu, MenuItem, PredefinedMenuItem};
use tauri::tray::TrayIconBuilder;
use tauri_plugin_global_shortcut::{GlobalShortcutExt, ShortcutState};

const ICON_BYTES: &[u8] = include_bytes!("../icons/icon.png");

fn main() {
    tauri::Builder::default()
        .plugin(tauri_plugin_global_shortcut::Builder::new().build())
        .setup(|app| {
            setup(app)?;
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![
            cmd_save_snippet,
            cmd_get_snippets,
            cmd_get_clipboard_blockquote,
        ])
        .on_window_event(|window, event| {
            // Intercept close requests — hide instead of destroying the window.
            // This keeps the pre-created windows alive for instant re-show.
            if let tauri::WindowEvent::CloseRequested { api, .. } = event {
                api.prevent_close();
                let _ = window.hide();
            }
        })
        .run(tauri::generate_context!())
        .expect("error while running trinket");
}

fn setup(app: &mut App) -> Result<(), Box<dyn std::error::Error>> {
    // ── Database ────────────────────────────────────────────────────────────
    let db_path = {
        let mut p = dirs::document_dir().unwrap_or_else(|| std::path::PathBuf::from("."));
        p.push("Trinket");
        std::fs::create_dir_all(&p)?;
        p.push("trinket.db");
        p
    };
    let conn = rusqlite::Connection::open(&db_path)?;
    db::init_db(&conn)?;
    app.manage(DbState(Arc::new(Mutex::new(conn))));

    // ── System Tray ─────────────────────────────────────────────────────────
    let add_item = MenuItem::with_id(app, "add", "Add  WIN+Ctrl+PgUp", true, None::<&str>)?;
    let get_item = MenuItem::with_id(app, "get", "Get  WIN+Ctrl+PgDown", true, None::<&str>)?;
    let sep = PredefinedMenuItem::separator(app)?;
    let quit_item = MenuItem::with_id(app, "quit", "Quit", true, None::<&str>)?;
    let menu = Menu::with_items(app, &[&add_item, &get_item, &sep, &quit_item])?;

    let icon = tauri::image::Image::from_bytes(ICON_BYTES)?;
    let handle = app.handle().clone();

    TrayIconBuilder::new()
        .icon(icon)
        .tooltip("Trinket")
        .menu(&menu)
        .show_menu_on_left_click(false)
        .on_menu_event(move |_tray, event| {
            let h = handle.clone();
            match event.id().as_ref() {
                "add" => {
                    let _ = show_window(&h, "add");
                }
                "get" => {
                    let _ = show_window(&h, "get");
                }
                "quit" => {
                    h.exit(0);
                }
                _ => {}
            }
        })
        .build(app)?;

    // ── Global Shortcuts ────────────────────────────────────────────────────
    // "Super" maps to: Win key on Windows, Cmd on macOS, Super/Meta on Linux
    let h_add = app.handle().clone();
    let h_get = app.handle().clone();

    app.global_shortcut()
        .on_shortcut("Super+Ctrl+PageUp", move |_app, _sc, ev| {
            if ev.state() == ShortcutState::Pressed {
                let _ = show_window(&h_add, "add");
            }
        })?;

    app.global_shortcut()
        .on_shortcut("Super+Ctrl+PageDown", move |_app, _sc, ev| {
            if ev.state() == ShortcutState::Pressed {
                let _ = show_window(&h_get, "get");
            }
        })?;

    Ok(())
}

/// Show a pre-created window: center it, bring to front, emit "reset" so the
/// frontend clears its state (textarea emptied, search cleared, snippets reloaded).
fn show_window(app: &AppHandle, label: &str) -> Result<(), tauri::Error> {
    if let Some(win) = app.get_webview_window(label) {
        win.show()?;
        win.center()?;
        win.set_focus()?;
        win.emit("reset", ())?;
    }
    Ok(())
}
