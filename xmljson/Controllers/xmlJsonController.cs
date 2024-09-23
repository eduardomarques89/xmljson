using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System;
using Newtonsoft.Json;
using System.Text;

namespace xmljson.Controllers
{
    [ApiController]
    [Route("convertJson")]
    public class xmlJsonController : Controller
    {
        private readonly ILogger<xmlJsonController> _logger;

        public xmlJsonController(ILogger<xmlJsonController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "XMLJson")]
        public IEnumerable<xmlJson> Get(string url)
        {
            var XML = @"" + url + "";
            XmlDocument Doc = new XmlDocument();
            Doc.Load(XML);

            return Enumerable.Range(1, 1).Select(index => new xmlJson { dados = JsonConvert.SerializeXmlNode(Doc, Newtonsoft.Json.Formatting.None, true) });
        }
    }
}
