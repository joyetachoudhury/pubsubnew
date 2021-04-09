using System;
using Cassandra;

namespace petmanagement.database
{
    public interface IDbConnection
    {
        public Cassandra.Session GetCassandrasession();
    }
}
