using AutoMapper;
using FactoryDataClient.Dtos;
using FactoryDataClient.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace FactoryDataClient.Controllers.Api
{
    [Authorize]
    public class EventTagRecordsController : ApiController
    {
        private ApplicationDbContext _context;

        public EventTagRecordsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [HttpGet]
        public IHttpActionResult GetRecords(int id)
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime yesterday = now.AddHours(-24);
                var records = _context.EventTagRecords
                    .Where(r => r.TagId == id && r.RecordTS > yesterday)
                     .OrderByDescending(r => r.RecordTS)
                    .ToList()
                    .Select(Mapper.Map<EventTagRecord, EventTagRecordDto>);

                return Ok(records);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest("Database Query Fail" + ex.Message);
            }
        }
    }
}
