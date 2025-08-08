using System.Data;
using System.Text.Json;
using Dapper;
using BankFlowAnalyzer.Infrastructure;
using System.IO;

namespace BankFlowAnalyzer.Services
{
    public class ProjectService : IProjectService
    {
        private static readonly string Root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "H_BankFlowAnalyzer");
        private static readonly string ProjectsRoot = Path.Combine(Root, "Projects");
        private static readonly string RecentFile = Path.Combine(Root, "recent_projects.json");

        private ProjectInfo? _current;
        public ProjectInfo? Current => _current;
        public string? CurrentDbPath => _current?.DbPath;

        public async Task<ProjectInfo> CreateAsync(string caseNo, string title, string? notes = null)
        {
            Directory.CreateDirectory(ProjectsRoot);
            var dir = Path.Combine(ProjectsRoot, caseNo);
            var dbPath = Path.Combine(dir, $"{caseNo}.db");

            if (File.Exists(dbPath))
                throw new InvalidOperationException($"案号已存在：{caseNo}");

            Directory.CreateDirectory(dir);
            using var conn = Db.Open(dbPath);
            Db.Initialize(conn);
            await conn.ExecuteAsync(
                "INSERT INTO project_meta(case_no,title,created_at,notes) VALUES(@a,@b,@c,@d)",
                new { a = caseNo, b = title, c = DateTime.Now.ToString("s"), d = notes });

            _current = new ProjectInfo(caseNo, title, dbPath, DateTime.UtcNow);
            await AddRecentAsync(_current);
            return _current;
        }

        public async Task<ProjectInfo> OpenAsync(string dbPath)
        {
            using var conn = Db.Open(dbPath);
            Db.Initialize(conn);
            var meta = await conn.QueryFirstOrDefaultAsync("SELECT case_no AS CaseNo, title AS Title FROM project_meta ORDER BY id LIMIT 1");
            _current = new ProjectInfo(
                meta?.CaseNo ?? Path.GetFileNameWithoutExtension(dbPath),
                meta?.Title ?? Path.GetFileNameWithoutExtension(dbPath),
                dbPath,
                DateTime.UtcNow);
            await AddRecentAsync(_current);
            return _current;
        }

        public IDbConnection OpenConnection()
        {
            if (_current is null) throw new InvalidOperationException("未选择项目");
            return Db.Open(_current.DbPath);
        }

        public async Task<IReadOnlyList<ProjectInfo>> GetRecentAsync(int top = 10)
        {
            if (!File.Exists(RecentFile)) return Array.Empty<ProjectInfo>();
            try
            {
                var json = await File.ReadAllTextAsync(RecentFile);
                var list = JsonSerializer.Deserialize<List<ProjectInfo>>(json) ?? new();
                return list.OrderByDescending(x => x.LastOpenedUtc).Take(top).ToList();
            }
            catch { return Array.Empty<ProjectInfo>(); }
        }

        public async Task RemoveRecentAsync(string dbPath)
        {
            var list = (await GetRecentAsync(100)).ToList();
            list.RemoveAll(x => string.Equals(x.DbPath, dbPath, StringComparison.OrdinalIgnoreCase));
            await SaveRecentAsync(list);
        }

        private async Task AddRecentAsync(ProjectInfo info)
        {
            var list = (await GetRecentAsync(100)).ToList();
            list.RemoveAll(x => string.Equals(x.DbPath, info.DbPath, StringComparison.OrdinalIgnoreCase));
            list.Add(info with { LastOpenedUtc = DateTime.UtcNow });
            await SaveRecentAsync(list);
        }

        private async Task SaveRecentAsync(List<ProjectInfo> list)
        {
            Directory.CreateDirectory(Root);
            var json = JsonSerializer.Serialize(list);
            await File.WriteAllTextAsync(RecentFile, json);
        }
    }
}