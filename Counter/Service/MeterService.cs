using MongoDB.Driver;
using Counter.Db;
using CounterRule;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Counter.Entity;
using MongoDB.Bson;
using System.Linq;
using System.Collections.Generic;

namespace Counter
{
  public  class MeterService:IMeterService<MeterDocument, Document>
    {
        Database _database;
        private int _time;
        private IMeterService<IMeter<IMeterDocument>, IMeterDocument> _next;
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
        public MeterService(string ConnectionString, int key, IMeterService<IMeter<IMeterDocument>,IMeterDocument> next):this(ConnectionString, key)
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
        #region Success Request Response 
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
            var currentTime= CacheData.Now;
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
                .Update.Set(m => m.End , CacheData.Now)
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
                Start = CacheData.Now,
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
                    EndTime = CacheData.Now + EllipseTime,
                    Id = document.Id,
                    StartTimer = CacheData.Now
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

        #region Error Response Handler
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<Document> ErrorCache(string url)
        {
             if(CacheData.ErrorBase== null)
             {
               return AddUpdateErrorData(url);
             }
             if(CacheData.ErrorBase.EndTime>=CacheData.Now )
             {
               var err= CacheData.ErrorCache.FirstOrDefault(m => m.Key == url);
                if(err.Key== null)
                {
                    return AddErroCache(url);
                }
                return err.Value;
             }
            return AddUpdateErrorData(url);
        }
        public List<Document> AddUpdateErrorData(string url)
        {
            var id = CreateErrorMongo();
            CacheData.ErrorBase = CreateCacheError(id);
            CacheData.ErrorCache = new Dictionary<string, List<Document>>();
            return AddErroCache(url);
            
        }
        private List<Document> AddErroCache(string url)
        {
            CacheData.ErrorCache.Add(url, new List<Document>());
            return CacheData.ErrorCache[url];
        }
        private string CreateErrorMongo()
        {
            string Id = ObjectId.GenerateNewId().ToString();
            _database.ErrodDocuments.InsertOne(new ErrorDocument()
            {
                Documents = new System.Collections.Generic.List<Document>(),
                Start = CacheData.Now,
                Id = Id

            });
            return Id;
        }
        private ErrorBase CreateCacheError(string id)
        {
           return new ErrorBase()
            {
                Id = id,
                Start = CacheData.Now,
                EndTime= CacheData.Now + EllipseTime
            };
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">Exeption Url </param>
        /// <param name="context"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task ErrorResponse(string url, HttpContext context, Document document)
        {
            var ErrorList= ErrorCache(url);
            ErrorList.Add(document);
            _database.ErrodDocuments.FindOneAndUpdate(mbox => mbox.Id ==CacheData.ErrorBase.Id, 
                Builders<ErrorDocument>.Update.AddToSet(m => m.Documents, document));
            if (_next != null)
                Task.Run(() => { _next.ErrorResponse(url, context, document); });
        }

       
    }
}
