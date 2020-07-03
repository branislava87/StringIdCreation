using System.Collections.Generic;
using System.Web;

namespace StringIdCreation.Models
{
    public class MarketModel
    {
        public string Id { get; set; }
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public string Environment { get; set; }
        public string StringType { get; set; }
        public HttpPostedFileBase File { get; set; }
        public object Error { get; set; }

        public List<MarketModel> ResultList { get; set; }
    }
}