using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webApiAghos.Repositories;

namespace webApiAghos.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class EmployeeController : Controller
    {
        ITUserRepository repository;

        public EmployeeController(ITUserRepository _tUserRepository)
        {
            repository = _tUserRepository;
        }

        [HttpGet("")]
        //[HttpGet("Home")]
        //[HttpGet("Home/Index")]
        public string Index(string user="paulo")
        {
            if  (user=="paulo")
                {
                return "Salve Paulo antonio";
            }
            return "Conectado: "+user;
        }



        [HttpGet]
        //[Authorize]
        [Route("api/GetExamesListOff/{tmpOffset?}")]
        public ActionResult GetExamesList(int tmpOffset)
        {
            var result = repository.GetTExamesList(tmpOffset);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("api/GetExamesList")]
        public ActionResult GetExamesList()
        {
            var result = repository.GetTExamesList3();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("api/GetExamesDetail/{tmpId}")]
        public ActionResult GetExamesDetail(int tmpId)
        {
            var result = repository.GetTexamesDetail(tmpId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("api/GetExamesDetail2/{tmpId}")]
        public ActionResult GetExamesDetail2(int tmpId)
        {
            var result = repository.GetTexamesDetail2(tmpId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("api/GetExamesList3")]
        public ActionResult GetExamesList3()
        {
            var result = repository.GetTExamesList3();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}