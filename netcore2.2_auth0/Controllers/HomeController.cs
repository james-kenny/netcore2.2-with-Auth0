using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi;
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
      // Get User information from Auth0
      if (User.Identity.IsAuthenticated)
      {
        string sUserId = User.Claims.Last()?.Value;


        Task<Auth0.ManagementApi.Models.User> _taskUserInfo = GetUserInformationAsync(sUserId);
      }

      return View();
    }

    private async Task<Auth0.ManagementApi.Models.User> GetUserInformationAsync(string sUserId)
    {
      var client = new ManagementApiClient("YOURTOKEN", new Uri("https://YOURDOMAIN.eu.auth0.com/api/v2/"));

      return client.Users.GetAsync(sUserId).Result;
    }

  }
}
