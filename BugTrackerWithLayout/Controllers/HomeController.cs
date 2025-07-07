using System.Web.Mvc;

namespace BugTrackerWithLayout.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
