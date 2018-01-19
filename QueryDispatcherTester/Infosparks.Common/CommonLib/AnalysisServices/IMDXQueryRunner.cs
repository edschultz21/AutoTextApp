using Microsoft.AnalysisServices.AdomdClient;

namespace TenK.InfoSparks.Common.AnalysisServices
{
    public interface IMDXQueryRunner
    {
        // Run query and return cellset
        CellSet LocalRunQueryCS(string dataSource, string key, string query);

        // Run query and return MDX
        MDXQueryResult DispatcherRunQuery(string dataSource, string key, string query, bool quickMode = false, int priority = -1);

    }
}
