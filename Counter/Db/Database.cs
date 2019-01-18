using Counter.Entity;
using MongoDB.Driver;

namespace Counter.Db
{
    public  class Database
    {
        public IMongoDatabase MongoDatabase;
        public Database(string connectionString)
        {
            MongoClient client = new MongoClient(connectionString);
            var url= new    MongoUrl(connectionString);
            var databaseName = "";
            if (string.IsNullOrEmpty(url.DatabaseName))
                databaseName = url.DatabaseName;
            else
                databaseName = "meter";
            MongoDatabase = client.GetDatabase(databaseName);           

        }
        public IMongoCollection<ErrorDocument> ErrodDocuments { get { return MongoDatabase.GetCollection<ErrorDocument>("ErrorDocument"); } }
    }
}
