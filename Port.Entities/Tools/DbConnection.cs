using System.Configuration;
using System.Data.SqlClient;
using MongoDB.Driver;
using Port.Entities.Entities;

namespace Port.Entities.Tools
{
    public sealed class DbConnection
    {
        public static string ConnectionString { get; set; }
        public static object Padlock { get; } = new object();
        public static object PadlockMongo { get; } = new object();

        public static SqlConnection SqlCon;
        public static MongoClient MongoClient ;
        public static IMongoCollection<PortLog> LogCollection;

        public static string SqlConnectionString
        {
            get
            {
                lock (Padlock)
                {
                    if (SqlCon == null)
                    {
                        SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["LaPortalContext"].ConnectionString);
                    }
                    ConnectionString = SqlCon.ConnectionString;
                }
                return ConnectionString;
            }
        }
        public static MongoClient MongoDbConnection
        {
            get
            {
                lock (PadlockMongo)
                {
                    if (MongoClient == null)
                    {
                     //  var  mongoClient = new MongoClient("mongodb://localhost:27017");
                    }
                }
                return MongoClient;

            }

        }
    }
}
