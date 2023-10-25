
using ApiPeliculas.Entities;
using Azure;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Collections;
using System.Text;

namespace ApiPeliculas.Services
{
    public class CacheService: ICacheService
    {
        private IDatabase cacheDb;
        //private readonly IConnectionMultiplexer connectionMultiplexer;

        //public CacheService(IConnectionMultiplexer connectionMultiplexer){
        //    this.connectionMultiplexer = connectionMultiplexer;
        //}

        public CacheService()
        {
           
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            cacheDb = redis.GetDatabase();

        }

        [Obsolete]
         public List<string> GetAllkeys(string patternStr)
         {

            

            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true"))
            {
               
                List<string> listKeys = new();
                var servidor = redis.GetServer("localhost:6379");
                var keys = servidor.Keys(pattern: patternStr);

                //foreach (var key in keys.OrderBy(k => int.Parse(k.ToString().Split(':')[1])))  > tiempo de respuesta
                foreach(var key in keys)
                {
                    listKeys.Add(key);
                }
                //servidor.FlushDatabase(); 
                return listKeys;                
            }

        }


        public T GetData<T>(string key)
        {
            var value = cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }

        public object RemoveData(string key)
        {
            var exist = cacheDb.KeyExists(key);
            if (exist) return cacheDb.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expireTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return cacheDb.StringSet(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expireTime);

        }


    }
}
