using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using netcore2._2_auth0.Models;

namespace netcore2._2_auth0.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      if (User.Identity.IsAuthenticated)
      {
        return Redirect("~/home/app");
      }


      return View();
    }

    public IActionResult App()
    {
      return View();
    }

  }
}
