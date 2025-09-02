using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using System.Data.Entity;
using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.Migrations;

namespace BugTrackerWithLayout
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
        
            // Code First Migrations aktif
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<BugTrackerDbContext, Configuration>());

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
