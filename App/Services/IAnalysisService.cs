using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankFlowAnalyzer.Services
{
    public interface IAnalysisService
    {
        Task<IReadOnlyList<(string ym, double net)>> MonthlyNetAsync();
        Task<IReadOnlyList<dynamic>> PagedAsync(int page, int pageSize, string? keyword);
        Task SeedIfEmptyAsync();
    }
}