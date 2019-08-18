using FactoryDataClient.Models;
using FactoryDataClient.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace FactoryDataClient.Controllers
{
    public class PlcsController : Controller
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

        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole(RoleName.Administrator))
            {
                return View("Index");
            }

            return View("UserIndex");
             
        }

        [Authorize]
        public ActionResult Create()
        {
           
            if (User.IsInRole(RoleName.Administrator))

                return View("Create");

            return View("UserIndex");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlcFormViewModel viewModel)
        {
            var plc = new Plc
            {
                Name = viewModel.Name,
                IPAddress = viewModel.IPAddress,
                DisableSubscriptions = viewModel.DisableSubscriptions,
                PollRateOverride = viewModel.PollRateOverride,
                Port = viewModel.Port,
                ProcessorSlot = viewModel.ProcessorSlot,
                EventPollRate = viewModel.EventPollRate,
                SubscriptionPollRate = viewModel.SubscriptionPollRate,
                TransactionPollRate = viewModel.TransactionPollRate
            };
          
            _context.Plcs.Add(plc);
            _context.SaveChanges();

            return RedirectToAction("Index", "Plcs");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Details(int id)
        {
            
            var plc = _context.Plcs.Single(p => p.Id == id);

            if (plc == null)
                return HttpNotFound();

            var viewModel = new PlcDetailViewModel
            {
                Id = plc.Id,
                Name = plc.Name,
                IPAddress = plc.IPAddress,
                DisableSubscriptions = plc.DisableSubscriptions,
                PollRateOverride = plc.PollRateOverride,
                Port = plc.Port,
                ProcessorSlot = plc.ProcessorSlot,
                EventPollRate = plc.EventPollRate,
                SubscriptionPollRate = plc.SubscriptionPollRate,
                TransactionPollRate = plc.TransactionPollRate
            };


            if (User.IsInRole(RoleName.Administrator))
                return View("Details",viewModel);

            return View("UserDetails", viewModel);

        }

        [Authorize]
        public ActionResult Edit(int id)
        {

            var plc = _context.Plcs.Single(p => p.Id == id);

            if (plc == null)
                return HttpNotFound();

            var viewModel = new PlcFormViewModel
            {
                Id = plc.Id,
                Name = plc.Name,
                IPAddress = plc.IPAddress,
                DisableSubscriptions = plc.DisableSubscriptions,
                PollRateOverride = plc.PollRateOverride,
                Port = plc.Port,
                ProcessorSlot = plc.ProcessorSlot,
                EventPollRate = plc.EventPollRate,
                SubscriptionPollRate = plc.SubscriptionPollRate,
                TransactionPollRate = plc.TransactionPollRate
            };


            if (User.IsInRole(RoleName.Administrator))
                return View("Edit", viewModel);

            return View("UserIndex");

        }

    }

}
    