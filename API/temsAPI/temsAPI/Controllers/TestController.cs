using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                ID = "1"
            };
        }
    }
}
