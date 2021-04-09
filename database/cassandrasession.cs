using Cassandra;

namespace petmanagement.database
{
    public class cassandrasession:IDbConnection
    {
        public Cassandra.Session GetCassandrasession()
        {
            
            Cassandra.Session  session =
                (Session)Cluster.Builder()
                       .WithCloudSecureConnectionBundle("secure-connect-petmanagement.zip")
                       .WithCredentials("petuser", "petpwd12")
                       .Build()
                       .Connect();

            return session;
        }

    }
}