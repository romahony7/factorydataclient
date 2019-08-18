using Common;
using FactoryDataClient.Models;
using FactoryDataClient.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace FactoryDataClient.Controllers
{
    public class TagsController : Controller
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

        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole(RoleName.Administrator))
                return View("Index");

            return View("UserIndex");
        }

        [Authorize]
        public ActionResult Create()
        {

            var types = _context.TagTypes.ToList();
            var plcs = _context.Plcs.ToList();

            var viewModel = new TagFormViewModel
            {
                Tag = new Tag(),
                TagTypes = types,
                Plcs = plcs
            };

            if (User.IsInRole(RoleName.Administrator))
                return View("Create", viewModel);

            return View("Create", viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TagFormViewModel viewModel)
        {
            var tag = new Tag
            {
                Name = viewModel.Tag.Name,
                TagTypeId = viewModel.Tag.TagTypeId,
                PlcId = viewModel.Tag.PlcId,
                IsActive = viewModel.Tag.IsActive
            };

            _context.Tags.Add(tag);
            _context.SaveChanges();

            return RedirectToAction("Index", "Tags");
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            var tag = _context.Tags.Single(t => t.Id == id);
            var plc = _context.Plcs.Single(p => p.Id == tag.PlcId);
            var type = _context.TagTypes.Single(tt => tt.Id == tag.TagTypeId);

            if (tag == null)
                return HttpNotFound();

            var viewModel = new TagDetailViewModel
            {
                Tag = tag,
                Plc = plc,
                TagType = type
            };


            if (User.IsInRole(RoleName.Administrator))
                return View("Details", viewModel);

            return View("Details", viewModel);

        }

        [Authorize]
        public ActionResult Data(int id)
        {
            var tag = _context.Tags.Single(t => t.Id == id);

            if (tag.TagTypeId == TagTypeConstants.subscriptionType)
            {
                var records = new TagDataViewModel
                {
                    Id = tag.Id,
                    Url = "http://fd-webhost:8080/api/subscriptionTagRecords/" + tag.Id,
                    Name = tag.Name
                };

                return View("Records", records);

            }

            if (tag.TagTypeId == TagTypeConstants.eventType)
            {
                var records = new TagDataViewModel
                {
                    Id = tag.Id,
                    Url = "http://fd-webhost:8080/api/eventTagRecords/" + tag.Id,
                    Name = tag.Name
                };

                return View("Records", records);

            }

            return RedirectToAction("Index", "Tags");
        }
    }
}