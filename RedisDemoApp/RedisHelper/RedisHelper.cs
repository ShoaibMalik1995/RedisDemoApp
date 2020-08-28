using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using StackExchange.Redis;

namespace RedisDemoApp.RedisHelper
{
    public class RedisHelper
    {
        private static Lazy<ConnectionMultiplexer> connection;
        private static IDatabase cache;

        public RedisHelper()
        {
            connection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        public static void ReConnect()
        {
            connection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        public static string GetKeyVal(string key)
        {
            try
            {
                cache = connection.Value.GetDatabase();
                var result = cache.StringGet(key);
                if (result.HasValue)
                    return JsonConvert.DeserializeObject<string>(result);
            }
            catch (Exception ex) 
            {

            }
            finally
            {
                connection.Value.Dispose();
            }
            return string.Empty;
        }

        //public static List<T> GetObjectListByKey(string key)
        //{
        //    try
        //    {
        //        cache = connection.Value.GetDatabase();
        //        var result = cache.StringGet(key);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return new List<T>();
        //}
    }
}