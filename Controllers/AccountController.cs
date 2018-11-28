using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FYP.Models;
using System.Security.Claims;
using System.Data;

namespace FYP.Controllers
{
    public class AccountController : Controller
    {
        private static string AuthScheme = "UserSecurity";

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["TheLayout"] = "_Layout";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginUser user,
                                    string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ClaimsPrincipal principal = null;
            if (SecureValidUser(user.UserId, user.Password, out principal))
            {
                HttpContext.Authentication.SignInAsync(AuthScheme, principal);
                return RedirectToAction("MemberIndex", "PSS");
            }
            else
            {
                ViewData["Message"] = "Incorrect User ID or Password";
                ViewData["TheLayout"] = "_Layout";
                return View("Login");
            }
        }

        public IActionResult Logoff(string returnUrl = null)
        {
            HttpContext.Authentication.SignOutAsync(AuthScheme);
            return Redirect("http://localhost:53011/PSS");
        }

        public IActionResult Forbidden(string returnUrl = null)
        {
            ViewData["TheLayout"] = "_Layout";
            return View();
        }

        private bool SecureValidUser(string uid,
                                     string pw,
                                     out ClaimsPrincipal principal)
        {
            string sql = "";
            string returnUrl = ViewData["ReturnUrl"] as string;
            sql = @"SELECT * FROM Users WHERE Email='{0}' AND Password = HASHBYTES('SHA1','{1}')";

            DataTable ds = DBUtl.GetTable(sql, uid, pw);

            principal = null;
            if (ds.Rows.Count == 1)
            {
                string uname = ds.Rows[0]["FullName"].ToString();
                string userid = ds.Rows[0]["MembershipNo"].ToString();
                string role = ds.Rows[0]["RoleID"].ToString();
               

                principal =
                   new ClaimsPrincipal(
                   new ClaimsIdentity(
                      new Claim[] {
                     new Claim(ClaimTypes.NameIdentifier,userid),
                     new Claim(ClaimTypes.Name, uname),
                     new Claim(ClaimTypes.Role, role)
                      },
                      "Basic"));
                return true;
            }
            else
            {
                return false;
            }
        }
         
    }
}