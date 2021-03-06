﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace netcore2._2_auth0
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      // Add authentication services
      services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect("Auth0", options =>
        {
          // Set the authority to your Auth0 domain
          options.Authority = $"https://contentappco.eu.auth0.com";

          // Configure the Auth0 Client ID and Client Secret
          options.ClientId = "Kmqrglz32JIS1GBZfg3beJ3k2FBH77Sa";
          options.ClientSecret = "uyquzDdbsoW5nYnWA06lmtcWI_fl6LdwP6jC1plosp7iuZS72l65b95DifQNU-qi";

          // Set response type to code
          options.ResponseType = "code";

          // Configure the scope
          options.Scope.Clear();
          options.Scope.Add("openid");

          // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
          // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
          options.CallbackPath = new PathString("/callback");

          // Configure the Claims Issuer to be Auth0
          options.ClaimsIssuer = "Auth0";

          options.Events = new OpenIdConnectEvents
          {
            OnRedirectToIdentityProvider = context =>
            {
              context.ProtocolMessage.SetParameter("audience", "5ac2295df4307b4e904d5bb9");

              return Task.FromResult(0);
            }
          };

          options.Events = new OpenIdConnectEvents
          {
            // handle the logout redirection
            OnRedirectToIdentityProviderForSignOut = (context) =>
            {
              var logoutUri =
                $"https://contentappco.eu.auth0.com/v2/logout?client_id=Kmqrglz32JIS1GBZfg3beJ3k2FBH77Sa";

              var postLogoutUri = context.Properties.RedirectUri;
              if (!string.IsNullOrEmpty(postLogoutUri))
              {
                if (postLogoutUri.StartsWith("/"))
                {
                  // transform to absolute
                  var request = context.Request;
                  postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                }

                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
              }

              context.Response.Redirect(logoutUri);
              context.HandleResponse();

              return Task.CompletedTask;
            }
          };
        });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {



      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
