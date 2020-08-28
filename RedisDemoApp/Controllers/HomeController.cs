using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RedisDemoApp.Models;

using System.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;


namespace RedisDemoApp.Controllers
{    
    public class HomeController : Controller
    {
        //Shoaib Changes
        public Lazy<ConnectionMultiplexer> connection;
        public IDatabase cache;

        public HomeController()
        {
            connection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }
        //Shoaib Changes
        public ActionResult Index()
        {
            try
            {
                cache = connection.Value.GetDatabase();
                var result = cache.StringGet("ListItems");
                ViewBag.Result = "Drop Down Data";
                if (result.HasValue)
                {
                    List<SelectListItem>  _Sellist = JsonConvert.DeserializeObject<List<SelectListItem>>(result);
                    //ViewBag.MySkills = _Sellist;
                    ViewData["MySkills"] = _Sellist;
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message.ToString();
            }
            finally
            {
                connection.Value.Dispose();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string txtCommand, string txtKey, string txtKeyValue, string txtGetKey)
        {
            try
            {
                List<ItemsList> _SellistItems = new List<ItemsList>();
                //_SellistItems.Add(new ItemsList() { Id = 1, Name = "A", DisplayText = "A" });
                //_SellistItems.Add(new ItemsList() { Id = 2, Name = "B", DisplayText = "BB" });
                //_SellistItems.Add(new ItemsList() { Id = 3, Name = "A", DisplayText = "CCC" });
                //txtKeyValue = JsonConvert.SerializeObject(_SellistItems);

                List<SelectListItem> _slItems = new List<SelectListItem>();
                _slItems.Add(new SelectListItem() { Value = "1", Text = "M", Selected = false });
                _slItems.Add(new SelectListItem() { Value = "2", Text = "S",  Selected = true });
                _slItems.Add(new SelectListItem() { Value = "3", Text = "K",  Selected = false });
                txtKeyValue = JsonConvert.SerializeObject(_slItems);

                cache = connection.Value.GetDatabase();
                if (txtCommand != null && txtCommand.Trim().Length > 0)
                    ViewBag.Result = cache.Execute(txtCommand).ToString();

                if (txtKey != null && txtKey.Trim().Length > 0 && txtKeyValue != null && txtKeyValue.Trim().Length > 0)
                    ViewBag.Result = cache.StringSet(txtKey, txtKeyValue);

                if (txtGetKey != null && txtGetKey.Trim().Length > 0)
                {
                    List<RedisModel> objlist = new List<RedisModel>();
                    List<SelectListItem> _Sellist = new List<SelectListItem>();
                    var result = cache.StringGet(txtGetKey);
                    ViewBag.Result = result;
                    Type T = result.GetType();
                    if (result.HasValue){
                        //objlist = JsonConvert.DeserializeObject<List<RedisModel>>(result);
                        //_SellistItems = JsonConvert.DeserializeObject<List<ItemsList>>(result);
                        _Sellist = JsonConvert.DeserializeObject<List<SelectListItem>>(result);
                        //ViewBag.MySkills = _Sellist;
                        ViewData["MySkills"] = _Sellist;
                    }
                }  
            }
            catch(Exception ex) {
                //ViewBag.Result = ex.Message.ToString();
            }
            finally
            {
                connection.Value.Dispose();
            }
            
            return View();
        }
    }
}