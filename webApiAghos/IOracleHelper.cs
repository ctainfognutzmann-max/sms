// IOracleHelper.cs
using System.Data;

namespace webApiAghos
{
    public interface IOracleHelper
    {
        IDbConnection GetConnection();
    }
}
