using MongoDB.Driver;
using Counter.Db;
using CounterRule;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Counter.Entity;
using MongoDB.Bson;
using System.Linq;
using System;

namespace Counter
{
  public  class MeterService:IMeterService<MeterDocument, Document>
    {
        Database _database;
        private int _time; 
        public int EllipseTime
        {
            get { return _time; } set{_time = value;}
        }
        public MeterService(string Connectionstring, int key)
        {
            _time = key;
            _database = new Database(Connectionstring);
            CacheData.Data = new System.Collections.Generic.Dictionary<string, string[]>();

        }

        public async Task Request(string url,HttpContext context,Document document)
        {
           var key= CacheData.Data.FirstOrDefault(m => m.Key == url);
            if(key.Key== null)
            {
                Add(url, document);   
            };
            AddDocument(url, key.Value[0], document);
        }
        private void GeTime(string url)
        {
           var timer= CacheData.Data.FirstOrDefault(m => m.Key == url);
            
           var sds= DateTime.Now.ToUniversalTime().Second;
            
            //if(timer)
        }
        private void Add(string url, Document document)
        {
            try
            {
                var id = ObjectId.GenerateNewId().ToString();
                _database.MongoDatabase.GetCollection<MeterDocument>(url).InsertOne(new MeterDocument()
                {
                    Id = id,
                    MethodUrl = url,
                    Request = new System.Collections.Generic.List<Document>()
                {
                    document
                }
                });
                CacheData.Data.Add(url, new string[] { id, "sdsd"});
            }
            catch
            {

            }
            
        }
        public void AddDocument(string url, string id, Document document)
        {
            try
            {
                _database.MongoDatabase.GetCollection<MeterDocument>(url).FindOneAndUpdate(mbox => mbox.Id == id, Builders<MeterDocument>.Update.AddToSet(m => m.Request, document));
            }
            catch
            {

            }
            
        }
        public Task PostRequest(HttpContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task ErrorResponse(string url, HttpContext context, Document document)
        {
            throw new System.NotImplementedException();
        }
    }
}
