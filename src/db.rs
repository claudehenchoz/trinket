use rusqlite::{Connection, Result, params};
use serde::Serialize;
use std::sync::{Arc, Mutex};

pub struct DbState(pub Arc<Mutex<Connection>>);

const LIST_LIMIT: i64 = 500;
const SEARCH_LIMIT: i64 = 200;

#[derive(Debug, Serialize, Clone)]
pub struct Snippet {
    pub id: i64,
    pub content: String,
    pub preview: String,
    pub created_at: i64,  // Unix ms
    pub modified_at: i64, // Unix ms
}

pub fn init_db(conn: &Connection) -> Result<()> {
    conn.execute_batch(
        "
        PRAGMA journal_mode=WAL;
        PRAGMA synchronous=NORMAL;
        PRAGMA cache_size=-8000;

        CREATE TABLE IF NOT EXISTS snippets (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            content     TEXT    NOT NULL,
            created_at  INTEGER NOT NULL,
            modified_at INTEGER NOT NULL
        );
        CREATE INDEX IF NOT EXISTS idx_snippets_modified
            ON snippets(modified_at DESC);

        CREATE VIRTUAL TABLE IF NOT EXISTS snippets_fts
            USING fts5(content, content='snippets', content_rowid='id');

        CREATE TRIGGER IF NOT EXISTS snippets_ai
        AFTER INSERT ON snippets BEGIN
            INSERT INTO snippets_fts(rowid, content)
                VALUES (new.id, new.content);
        END;

        CREATE TRIGGER IF NOT EXISTS snippets_ad
        AFTER DELETE ON snippets BEGIN
            INSERT INTO snippets_fts(snippets_fts, rowid, content)
                VALUES ('delete', old.id, old.content);
        END;

        CREATE TRIGGER IF NOT EXISTS snippets_au
        AFTER UPDATE ON snippets BEGIN
            INSERT INTO snippets_fts(snippets_fts, rowid, content)
                VALUES ('delete', old.id, old.content);
            INSERT INTO snippets_fts(rowid, content)
                VALUES (new.id, new.content);
        END;
        ",
    )?;
    Ok(())
}

pub fn save_snippet(conn: &Connection, content: &str) -> Result<i64> {
    let now = chrono::Utc::now().timestamp_millis();
    conn.execute(
        "INSERT INTO snippets (content, created_at, modified_at) VALUES (?1, ?2, ?2)",
        params![content, now],
    )?;
    Ok(conn.last_insert_rowid())
}

/// Filters blank/quote-only lines, takes up to 7, appends " [...]" if truncated.
pub fn create_preview(content: &str) -> String {
    let lines: Vec<&str> = content
        .lines()
        .filter(|l| {
            let t = l.trim();
            !t.is_empty() && !t.trim_matches('>').trim().is_empty()
        })
        .collect();
    let take = lines.len().min(7);
    let truncated = lines.len() > 7;
    let mut preview = lines[..take].join(" ↵ ");
    if truncated {
        preview.push_str(" [...]");
    }
    preview
}

pub fn list_snippets(conn: &Connection, query: Option<&str>) -> Result<Vec<Snippet>> {
    let q = query.and_then(|s| {
        let s = s.trim();
        if s.is_empty() { None } else { Some(s) }
    });

    match q {
        None => {
            let mut stmt = conn.prepare(
                "SELECT id, content, created_at, modified_at \
                 FROM snippets ORDER BY modified_at DESC LIMIT ?1",
            )?;
            let rows = stmt
                .query_map(params![LIST_LIMIT], |row| {
                    let content: String = row.get(1)?;
                    let preview = create_preview(&content);
                    Ok(Snippet {
                        id: row.get(0)?,
                        content,
                        preview,
                        created_at: row.get(2)?,
                        modified_at: row.get(3)?,
                    })
                })?
                .filter_map(|r| r.ok())
                .collect();
            Ok(rows)
        }
        Some(q) => match build_fts_query(q) {
            None => list_snippets(conn, None),
            Some(fts) => {
                let mut stmt = conn.prepare(
                    "SELECT s.id, s.content, s.created_at, s.modified_at \
                     FROM snippets s \
                     JOIN snippets_fts f ON f.rowid = s.id \
                     WHERE snippets_fts MATCH ?1 \
                     ORDER BY rank LIMIT ?2",
                )?;
                let rows = stmt
                    .query_map(params![fts, SEARCH_LIMIT], |row| {
                        let content: String = row.get(1)?;
                        let preview = create_preview(&content);
                        Ok(Snippet {
                            id: row.get(0)?,
                            content,
                            preview,
                            created_at: row.get(2)?,
                            modified_at: row.get(3)?,
                        })
                    })?
                    .filter_map(|r| r.ok())
                    .collect();
                Ok(rows)
            }
        },
    }
}

/// Wraps each whitespace-delimited token in double-quotes + `*` for safe FTS5 prefix matching.
fn build_fts_query(input: &str) -> Option<String> {
    let tokens: Vec<String> = input
        .split_whitespace()
        .filter(|t| !t.is_empty())
        .map(|t| format!("\"{}\"*", t.replace('"', "\"\"")))
        .collect();
    if tokens.is_empty() {
        None
    } else {
        Some(tokens.join(" "))
    }
}
