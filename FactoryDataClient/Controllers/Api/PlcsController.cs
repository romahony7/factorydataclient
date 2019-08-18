using AutoMapper;
using FactoryDataClient.Dtos;
using FactoryDataClient.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace FactoryDataClient.Controllers.Api
{
    [Authorize]
    public class PlcsController : ApiController
    {
        private ApplicationDbContext _context;

        public PlcsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        //Get/api/plcs
        [HttpGet]
        public IHttpActionResult GetPlcs()
        {
            try
            {
                var plcDtos = _context.Plcs.ToList().Select(Mapper.Map<Plc, PlcDto>);

                return Ok(plcDtos);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest("Database Query Fail"+ ex.Message);
            }
            
        }

        //Get/api/plcs/1
        [HttpGet]
        public IHttpActionResult GetPlc(int id)
        {
            var plc = _context.Plcs.SingleOrDefault(p => p.Id == id);

            if (plc == null)
                return NotFound();

            return Ok (Mapper.Map<Plc, PlcDto>(plc));
        }

        //Post/api/plcs
        [HttpPost]
        public IHttpActionResult CreatePlc(PlcDto plcDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var plc = Mapper.Map<PlcDto, Plc>(plcDto);
            _context.Plcs.Add(plc);
            _context.SaveChanges();

            plcDto.Id = plc.Id;

            return Created(new Uri(Request.RequestUri + "/" + plc.Id),plcDto);

        }

        //Put/api/plcs/1
        [HttpPut]
        public IHttpActionResult UpdatePlc(PlcDto plcDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var plcInDb = _context.Plcs.SingleOrDefault(p => p.Id == plcDto.Id);

            if (plcInDb == null)
                return NotFound();

            Mapper.Map(plcDto, plcInDb);
            
            _context.SaveChanges();

            return Ok();
        }

        //DELETE/api/plcs/1
        [HttpDelete]
        public IHttpActionResult DeletePlc(int id)
        {
            var plcInDb = _context.Plcs.SingleOrDefault(p => p.Id == id);

            if (plcInDb == null)
                return NotFound();

            _context.Plcs.Remove(plcInDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}
