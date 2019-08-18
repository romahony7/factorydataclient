using AutoMapper;
using FactoryDataClient.Dtos;
using FactoryDataClient.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace FactoryDataClient.Controllers.Api
{
    [Authorize]
    public class TagsController : ApiController
    {
        private ApplicationDbContext _context;

        public TagsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        //Get/api/tags
        [HttpGet]
        public IHttpActionResult GetTags(string query = null)
        {
            var tagsQuery = _context.Tags
                .Include(t => t.TagType)
                .Include(t => t.Plc);

            if (!string.IsNullOrWhiteSpace(query))
                tagsQuery = tagsQuery.Where(t => t.Name.Contains(query));

            var tagDto = tagsQuery
               .ToList()
               .Select(Mapper.Map<Tag, TagDto>);

            return Ok(tagDto);
        }

        //Get/api/tags/1
        [HttpGet]
        public IHttpActionResult GetTag(int id)
        {
            var tag = _context.Tags.SingleOrDefault(t => t.Id == id);
  
            if (tag == null)
                return NotFound();

            return Ok(Mapper.Map<Tag,TagDto>(tag));
        }

        //Post/api/tags
        [HttpPost]
        public IHttpActionResult CreateTag(TagDto tagDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var tag = Mapper.Map<TagDto, Tag>(tagDto);
            _context.Tags.Add(tag);
            _context.SaveChanges();

            tagDto.Id = tag.Id;

            return Created(new Uri(Request.RequestUri + "/" + tag.Id), tagDto);

        }


        //DELETE/api/tags/1
        [HttpDelete]
        public IHttpActionResult DeleteTag(int id)
        {
            var TagInDb = _context.Tags.SingleOrDefault(t => t.Id == id);

            if (TagInDb == null)
                return NotFound();

            _context.Tags.Remove(TagInDb);
            _context.SaveChanges();

            return Ok();
        }

    }
}

