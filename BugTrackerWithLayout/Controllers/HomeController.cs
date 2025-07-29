using System.Web.Mvc;

namespace BugTrackerWithLayout.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult TestGrid()
        {
            return View();
        }

    }
}
