using System.Data;
using System.Data.SQLite;
using System.IO;
using Dapper;

namespace BankFlowAnalyzer.Infrastructure
{
    public static class Db
    {
        public static IDbConnection Open(string dbPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            var cs = new SQLiteConnectionStringBuilder
            {
                DataSource = dbPath,
                JournalMode = SQLiteJournalModeEnum.Wal,
                SyncMode = SynchronizationModes.Normal,
                ForeignKeys = true
            }.ToString();

            var conn = new SQLiteConnection(cs);
            conn.Open();
            return conn;
        }

        public static void Initialize(IDbConnection conn)
        {
            conn.Execute(@"
CREATE TABLE IF NOT EXISTS project_meta (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  case_no TEXT,
  title TEXT,
  reason TEXT,
  created_at TEXT,
  notes TEXT
);
CREATE TABLE IF NOT EXISTS transactions (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  tx_time TEXT NOT NULL,
  direction INTEGER NOT NULL,
  amount REAL NOT NULL,
  balance REAL NULL,
  bank_name TEXT,
  counterparty_name TEXT,
  counterparty_account TEXT,
  summary TEXT,
  source_file TEXT,
  source_row INTEGER,
  account_no TEXT,
  account_name TEXT,
  hash_dedup TEXT UNIQUE,
  tags TEXT NULL,
  ext TEXT NULL
);
CREATE INDEX IF NOT EXISTS idx_tx_time ON transactions(tx_time);
CREATE INDEX IF NOT EXISTS idx_cp ON transactions(counterparty_account, tx_time);
CREATE INDEX IF NOT EXISTS idx_amount ON transactions(amount);
CREATE INDEX IF NOT EXISTS idx_direction ON transactions(direction);
");
        }
    }
}