using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test.Controllers
{
    public class JohaController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult getJoha(string id)
        {
            return Json("joha");
        }
        public JsonResult PostJoha([FromBody] JohaModal modal)
        {
            return Json(modal);
        }
        [HttpPost]
        public  JsonResult Bed([FromBody] JohaModal modal)
        {
            this.HttpContext.Response.StatusCode = 418;
            Console.WriteLine(modal);
            return Json("sdcsd");
        }
        [Route("/api/joha/bad")]
        public string bad([FromBody]JohaModal modal) {
            this.HttpContext.Response.StatusCode = 418;
            Console.Write(modal.name);
            return "sdsd"; }

    }
    public class JohaModal{
        public string name { get; set; }
    }
}
