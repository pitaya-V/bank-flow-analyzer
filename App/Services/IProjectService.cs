using System.Data;

namespace BankFlowAnalyzer.Services
{
    public record ProjectInfo(string CaseNo, string Title, string DbPath, DateTime LastOpenedUtc);

    public interface IProjectService
    {
        ProjectInfo? Current { get; }
        string? CurrentDbPath { get; }
        Task<ProjectInfo> CreateAsync(string caseNo, string title, string? notes = null);
        Task<ProjectInfo> OpenAsync(string dbPath);
        Task<IReadOnlyList<ProjectInfo>> GetRecentAsync(int top = 10);
        Task RemoveRecentAsync(string dbPath);
        IDbConnection OpenConnection();
    }
}