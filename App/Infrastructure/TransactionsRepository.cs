using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace BankFlowAnalyzer.Infrastructure
{
    public class TransactionsRepository
    {
        private readonly IDbConnection _conn;
        public TransactionsRepository(IDbConnection conn) => _conn = conn;

        public Task<IEnumerable<dynamic>> QueryPagedAsync(int offset, int pageSize, string? keyword)
        {
            string where = string.IsNullOrWhiteSpace(keyword) ? "" :
                "WHERE (summary LIKE @kw OR counterparty_name LIKE @kw OR counterparty_account LIKE @kw)";
            string sql = $@"
SELECT id, tx_time, CASE direction WHEN 1 THEN '收入' ELSE '支出' END AS direction, amount, summary, counterparty_name, counterparty_account, bank_name
FROM transactions
{where}
ORDER BY tx_time DESC
LIMIT @pageSize OFFSET @offset";
            return _conn.QueryAsync(sql, new { offset, pageSize, kw = "%" + keyword + "%" });
        }

        public Task<IEnumerable<dynamic>> MonthlyNetAsync()
        {
            string sql = @"
SELECT substr(tx_time,1,7) AS ym,
       SUM(CASE WHEN direction=1 THEN amount ELSE -amount END) AS net
FROM transactions
GROUP BY ym
ORDER BY ym";
            return _conn.QueryAsync(sql);
        }
    }
}