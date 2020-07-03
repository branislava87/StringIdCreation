using StringIdCreation.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace StringIdCreation.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            MarketModel market = new MarketModel();
            ViewBag.Message = market;
            return View();
        }

        [HttpPost]
        public ActionResult Index(MarketModel market)
        {
            MarketModel marketModel = new MarketModel
            {
                TemplateId = market.TemplateId,
                Name = market.Name,
                Environment = market.Environment,
                StringType = market.StringType
            };

            var url = GetUrl(marketModel.Environment);

            if (string.IsNullOrEmpty(url))
            {
                marketModel.Environment = "Wrong";
                ViewBag.Message = marketModel;
                return View();
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var obj = new
                {
                    strings = new[] {
                        new {
                            itemId = "1",
                            name = marketModel.Name,
                            tags  =
                                new { marketModel.StringType,
                                    sport = "4",
                                    domain = "Trading"
                            }
                        }
                 },
                    correlationId = "34690f9d-1835-4b1c-bdd3-86bfa625a9a9",
                    user = "ICEPOR\\btomic"
                };

                var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                json = json.Replace("name", "string");
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var resultList = JsonConvert.DeserializeObject<List<MarketModel>>(result);
                marketModel.Id = resultList[0].Id;
                marketModel.Error = resultList[0].Error;
            }

            ViewBag.Message = marketModel;
            return View();
        }
    }
}