using AutoMapper;
using FactoryDataClient.Dtos;
using FactoryDataClient.Models;
using System.Linq;
using System.Web.Http;

namespace FactoryDataClient.Controllers.Api
{
    public class TypesController : ApiController
    {
        private ApplicationDbContext _context;

        public TypesController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        //Get/api/types
        [HttpGet]
        public IHttpActionResult GetTypes()
        {
            var tagTypeQuery = _context.TagTypes;

            var tagTypeDto = tagTypeQuery
               .ToList()
               .Select(Mapper.Map<TagType, TagTypeDto>);

            return Ok(tagTypeDto);

        }

        //Get/api/types/1
        [HttpGet]
        public IHttpActionResult GetTypes(int id)
        {
            var tagType = _context.TagTypes.SingleOrDefault(t => t.Id == id);

            if (tagType == null)
               return NotFound();

            Mapper.Map<TagType, TagTypeDto>(tagType);

            return Ok(tagType);

        }
    }
}
