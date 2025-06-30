using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Shared.Abstractions
{
    public interface IDatabaseHelper
    {
        void EnsureDatabaseExists(string connectionString);
    }
}