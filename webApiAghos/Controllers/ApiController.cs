using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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

        [HttpGet]
        [Route("api/GetEscalaExames/{idEscalaExame:int}")]
        public ActionResult GetEscalaExames(int idEscalaExame)
        {
            var result = repository.GetEscalaExames(idEscalaExame);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/AgendamentosDoDia")]
        public ActionResult GetAgendamentosDoDia([FromQuery] string? data)
        {
            if (!DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
            {
                return BadRequest(new { mensagem = "Informe a data no formato DD/MM/AAAA." });
            }

            return Ok(repository.GetAgendamentosDoDia(data!));
        }

        [HttpGet]
        [Route("api/AgendamentosPorPeriodo")]
        public ActionResult GetAgendamentosPorPeriodo(
            [FromQuery] string? dataInicial,
            [FromQuery] string? dataFinal,
            [FromQuery] int? idHospital)
        {
            const string dateFormat = "dd/MM/yyyy";

            if (!DateTime.TryParseExact(dataInicial, dateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var inicio) ||
                !DateTime.TryParseExact(dataFinal, dateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var fim))
            {
                return BadRequest(new { mensagem = "Informe dataInicial e dataFinal no formato DD/MM/AAAA." });
            }

            if (inicio > fim)
            {
                return BadRequest(new { mensagem = "dataInicial não pode ser posterior a dataFinal." });
            }

            if (idHospital is <= 0)
            {
                return BadRequest(new { mensagem = "Informe um idHospital válido." });
            }

            return Ok(repository.GetAgendamentosPorPeriodo(inicio, fim, idHospital));
        }
    }
}
