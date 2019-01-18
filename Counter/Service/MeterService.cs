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
        private IMeterService<IMeter, IMeterDocument> _next;
        public int EllipseTime
        {
            get { return _time; } set{_time = value;}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Connectionstring">Connecstrion  string for Mongodb Database</param>
        /// <param name="key"> Key is perriut time in secunds for creating New Document in MongodDb </param>
        public MeterService(string Connectionstring, int key)
        {
            _time = key;
            _database = new Database(Connectionstring);
        }
        public MeterService(string ConnectionString, int key, IMeterService<IMeter,IMeterDocument> next):this(ConnectionString, key)
        {
            _next = next;
        }

        public async Task Request(string url,HttpContext context,Document document)
        {
           var data=  GetTime(url);
            if(data!= null)
            {
                AddDocument(url, data.Id, document);
                data.Count++;
                return;
            }
            Add(url, document);
            if (_next != null) await _next.Request(url,context,document);
        }
        #region
        /// <summary>
        /// Get Data from Cache if not exist return null
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Data GetTime(string url)
        {
           var timer= CacheData.Data.FirstOrDefault(m => m.Key == url);
            if (string.IsNullOrEmpty(timer.Key))
            {
                return null;
            }
            var currentTime= DateTime.Now.ToUniversalTime().Second;
            if (timer.Value.EndTime >= currentTime)
            {
                return timer.Value;
            }
            FinishMeter(timer.Value, timer.Key);
            CacheData.Data.Remove(timer.Key);
            return  null;
        }
        /// <summary>
        /// When Meter Timer is Finished this Function update some Params in meter 
        /// </summary>
        /// <param name="data"></param>
        private void FinishMeter(Data data, string url)
        {
            _database.MongoDatabase.GetCollection<MeterDocument>(url)
                .FindOneAndUpdate(mbox => mbox.Id == data.Id, Builders<MeterDocument>
                .Update.Set(m => m.End , DateTime.Now.ToUniversalTime().Second)
                .Set(m => m.RequestCount, data.Count));
        }
        /// <summary>
        /// Create Meter document use it if you wont Create Meter Document
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private MeterDocument CreateDocument(string url)
        {
            return new MeterDocument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                MethodUrl = url,
                Start = DateTime.Now.ToUniversalTime().Second,
                Request = new System.Collections.Generic.List<Document>()
            };
        }
        /// <summary>
        ///  Add Data For Cache Dictionary
        /// </summary>
        /// <param name="document"></param>
        /// 
        private void AddCacheData(MeterDocument document)
        {
            CacheData.Data.Add(document.MethodUrl,
                new Data
                {
                    Count = 1,
                    EndTime = DateTime.Now.ToUniversalTime().Second + EllipseTime,
                    Id = document.Id,
                    StartTimer = DateTime.Now.ToUniversalTime().Second
                });

        }
        /// <summary>
        /// Add new Meter Document To Mongodb And Add Cache
        /// </summary>
        /// <param name="url"></param>
        /// <param name="document"></param>
        private void Add(string url, Document document)
        {
            try
            {
                var newMeter = CreateDocument(url);
                newMeter.Request.Add(document);
                _database.MongoDatabase.GetCollection<MeterDocument>(url).InsertOne(newMeter);
                AddCacheData(newMeter);
            }
            catch
            {

            }

        }
        /// <summary>
        /// Push Document Exist Meter Document
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <param name="document"></param>
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
        #endregion

        public Task ErrorResponse(string url, HttpContext context, Document document)
        {
            //_database.ErrodDocuments.
            if (_next != null)
                _next.ErrorResponse(url, context, document);
        }
    }
}
