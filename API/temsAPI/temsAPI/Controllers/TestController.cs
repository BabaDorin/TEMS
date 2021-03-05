using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult<Personnel> GetPersonnel()
        {
            return new Personnel
            {
                Id = "1"
            };
        }
    }
}
