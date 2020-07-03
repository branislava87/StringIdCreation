using StringIdCreation.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace StringIdCreation.Controllers
{
    public class MultipleCreationController : BaseController
    {
        private int n;

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
                Environment = market.Environment,
                StringType = market.StringType
            };

            var url = GetUrl(marketModel.Environment);

            if (string.IsNullOrEmpty(url))
            {
                ViewBag.Message = marketModel;
                return View();
            }

            var templatesToCreate = new List<Tuple<string, string>>();

            try
            {
                if (market.File.ContentLength == 0)
                {
                    marketModel.Error = "Zero length file!";
                    ViewBag.Message = marketModel;
                    return View();
                }
                else
                {
                    using (StreamReader file = new StreamReader(market.File.InputStream))
                    {

                        string ln;

                        while ((ln = file.ReadLine()) != null)
                        {
                            if (ln.Length == 0)
                                continue;
                            ln = ln.Trim();
                            ln = Regex.Replace(ln, @"\t|\n|\r", " ");
                            var firstSpaceIndex = ln.IndexOf(" ");
                            var templateId = ln.Substring(0, firstSpaceIndex);
                            var name = (ln.Substring(firstSpaceIndex + 1)).Trim();
                            templatesToCreate.Add(Tuple.Create(templateId, name));
                        }
                        file.Close();
                    }
                }
            }
            catch (Exception e)
            {
                marketModel.Error = e.Message;
                ViewBag.Message = marketModel;
                return View();
            }

            var resultList = new List<MarketModel>();

            foreach (var item in templatesToCreate)
            {
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
                            name = item.Item2,
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
                    var resultResponse = streamReader.ReadToEnd();
                    var results = JsonConvert.DeserializeObject<List<MarketModel>>(resultResponse);
                    marketModel.Id = results[0].Id;
                    marketModel.Error = results[0].Error;

                    if (Int32.TryParse(item.Item1, out n))
                    {
                        resultList.Add(new MarketModel
                        {
                            Id = results[0].Id,
                            TemplateId = item.Item1,
                            Error = results[0].Error,
                        });
                    }
                    else
                    {
                        resultList.Add(new MarketModel
                        {
                            Id = results[0].Id,
                            TemplateId = item.Item1,
                            Error = "Error: TemplateId wrong format!!",
                        });
                    }
                }
            }
            marketModel.ResultList = resultList;

            ViewBag.Message = marketModel;
            return View();
        }
    }
}