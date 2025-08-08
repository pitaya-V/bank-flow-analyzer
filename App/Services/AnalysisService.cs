using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using BankFlowAnalyzer.Infrastructure;

namespace BankFlowAnalyzer.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly IProjectService _project;
        public AnalysisService(IProjectService project) => _project = project;

        public async Task<IReadOnlyList<(string ym, double net)>> MonthlyNetAsync()
        {
            using var conn = _project.OpenConnection();
            var repo = new TransactionsRepository(conn);
            var rows = await repo.MonthlyNetAsync();
            var list = new List<(string ym, double net)>();
            foreach (var r in rows) list.Add(((string)r.ym, Convert.ToDouble(r.net)));
            return list;
        }

        public async Task<IReadOnlyList<dynamic>> PagedAsync(int page, int pageSize, string? keyword)
        {
            using var conn = _project.OpenConnection();
            var repo = new TransactionsRepository(conn);
            var rows = await repo.QueryPagedAsync((page - 1) * pageSize, pageSize, keyword);
            return rows.AsList();
        }

        public async Task SeedIfEmptyAsync()
        {
            using var conn = _project.OpenConnection();
            var count = await conn.ExecuteScalarAsync<long>("SELECT COUNT(1) FROM transactions");
            if (count > 0) return;
            await conn.ExecuteAsync(@"
INSERT INTO transactions(tx_time,direction,amount,balance,bank_name,counterparty_name,counterparty_account,summary,source_file,source_row,hash_dedup)
VALUES
('2025-01-01T10:00:00',1,1000,5000,'演示银行','某公司','6222****1234','工资入账','seed',1,'seed-1'),
('2025-01-03T15:30:00',0,200,4800,'演示银行','便利店','6222****5678','超市消费','seed',2,'seed-2'),
('2025-02-05T09:20:00',1,300,5100,'演示银行','好友A','6222****9999','转账','seed',3,'seed-3'),
('2025-02-06T12:10:00',0,100,5000,'演示银行','餐馆','6222****8888','工作餐','seed',4,'seed-4');");
        }
    }
}