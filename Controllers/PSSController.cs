using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FYP.Models;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FYP.Controllers
{
    public class PSSController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult MemberIndex()
        {
            DateTime dt = DateTime.Now;
            ViewData["today'sDate"] = dt;
            return View();
        }

        [AllowAnonymous]
        public ActionResult SignUp()
        {
            ViewData["mt"] = DBUtl.GetList(@"SELECT MembershipTypeName as text, MembershipTypeName as value FROM MembershipType");
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(Users newUser)
        {
            if (ModelState.IsValid)
            {
                if (DBUtl.ExecSQL(@"INSERT INTO Users
                                    (FullName, Email, RoleID, MembershipNo, Password) 
                                    VALUES
                                    ('{0}','{1}',{2}, HASHBYTES('SHA1', '{10}'))",
                                    newUser.FullName, newUser.Email, 1, 
                                    newUser.MembershipNo, newUser.Password) == 1)

                    TempData["MsgSignUp"] = "Your account has been created!";
                else
                    TempData["MsgSignUp"] = DBUtl.DB_Message + ", Please try again at later time, thank you.";
                return Redirect("SignUp");
            }
            else
            {
                TempData["MsgSignUp"] = "Invalid information entered!";
                return RedirectToAction("PSS/SignUp");
            }
        }

        public IActionResult EditProfile(string id)
        {
            DateTime dt = DateTime.Now;
            ViewData["today'sDate"] = dt;

            List<Users> data = DBUtl.GetList<Users>(@"SELECT * FROM Users WHERE FullName='{0}'", id);

            Users model = null;
            if (data.Count > 0)
            {
                model = data[0];
                return View("EditProfile", model);
            }
            else
            {
                TempData["MsgEditProfile"] = "User profile cannot be update, due to database error. Please try again later.";
                return RedirectToAction("EditProfile");
            }
        }

        [HttpPost]
        public IActionResult EditProfile(Users updateUser)
        {
         
                if (DBUtl.ExecSQL(@"UPDATE Users
                                    SET MembershipNo='{0}', FullName='{1}', Email='{2}', 
                                    WHERE FullName='{3}'", updateUser.MembershipNo, updateUser.FullName, updateUser.Email, updateUser.FullName) == 1)

                    TempData["MsgEditProfile"] = "Your profile has successfully updated!";
                else
                    TempData["MsgEditProfile"] = "ERROR: " + DBUtl.DB_Message;
                return RedirectToAction("MemberIndex");
         
        }

        public IActionResult Distinction(string id)
        {
            DateTime dt = DateTime.Now;
            ViewData["today'sDate"] = dt;

            ViewData["detail"] = DBUtl.GetList(@"SELECT MembershipNo as value, 
            FullName as value2, Email as value3, FROM Users WHERE FullName='{0}'", id);
            return View();
        }


        [Authorize]
        public IActionResult ViewMemberUser()
        {
            DateTime dt = DateTime.Now;
            ViewData["today'sDate"] = dt;

            List<Users> model = DBUtl.GetList<Users>("SELECT * FROM Users WHERE RoleID = 2 ORDER BY MembershipNo");
            return View(model);
        }

        [Authorize]
        public IActionResult UpdateMemberUser(int Id, bool? isDelete)
        {
            DateTime dt = DateTime.Now;
            ViewData["today'sDate"] = dt; 

            List<Users> MemberUser = DBUtl.GetList<Users>
                                       (@"SELECT * FROM Users WHERE MembershipNo={0}", Id);
            Users model = null;
            if (MemberUser.Count > 0)
            {
                if (isDelete.HasValue == false || isDelete == false)
                {
                    ViewData["PostTo"] = "UpdateMemberUser";
                    ViewData["ButtonText"] = "Update";
                }
                else
                {
                    ViewData["PostTo"] = "DeleteMemberUser";
                    ViewData["ButtonText"] = "Delete";
                }
                model = MemberUser[0];
                return View("Member", model);
            }
            else
            {
                TempData["Msg"] = $"Member ID of {Id} not found";
                return RedirectToAction("MemberIndex");
            }
        }

        

        [HttpPost]
        public IActionResult DeleteMemberUser(Users updateUser)
        {          
            if (DBUtl.ExecSQL(@"DELETE Users WHERE MembershipNo = {0}", updateUser.MembershipNo) == 1)
                TempData["Msg"] = "User " + updateUser.FullName + " Deleted!";
            else
                TempData["Msg"] = DBUtl.DB_Message;
            return RedirectToAction("ViewMemberUser");
        }

        [Authorize]
        public IActionResult ViewAdminUser()
        {
            DateTime dt = DateTime.Now;
            ViewData["today'sDate"] = dt;

            List<Users> model = DBUtl.GetList<Users>("SELECT * FROM Users WHERE RoleID = 1 ORDER BY MembershipNo");
            return View(model);
        }

        [Authorize]
        public IActionResult UpdateAdminUser(int Id, bool? isDelete)
        {
            DateTime dt = DateTime.Now;
            ViewData["today'sDate"] = dt; 

            List<Users> MemberUser = DBUtl.GetList<Users>
                                       (@"SELECT * FROM Users WHERE MembershipNo={0}", Id);
            Users model = null;
            if (MemberUser.Count > 0)
            {
                if (isDelete.HasValue == false || isDelete == false)
                {
                    ViewData["PostTo"] = "UpdateAdminUser";
                    ViewData["ButtonText"] = "Update";
                }
                else
                {
                    ViewData["PostTo"] = "DeleteAdminUser";
                    ViewData["ButtonText"] = "Delete";
                }
                model = MemberUser[0];
                return View("Admin", model);
            }
            else
            {
                TempData["MsgAdmin"] = $"Admin {Id} not found";
                return RedirectToAction("ViewAdminUser");
            }
        }

        [HttpPost]
        public IActionResult UpdateAdminUser(Users updateAdmin)
        {
          
                if (DBUtl.ExecSQL(@"UPDATE Users
                                    SET FullName='{0}', Email='{1}', 
                                     WHERE MembershipNo={2}",
                                       updateAdmin.FullName,  updateAdmin.Email,
                                        updateAdmin.MembershipNo) == 1)
                    TempData["MsgAdmin"] = "Admin " + updateAdmin.FullName + " Updated!";
                else
                    TempData["MsgAdmin"] = DBUtl.DB_Message;
                return RedirectToAction("ViewAdminUser");
           
        }

        [HttpPost]
        public IActionResult DeleteAdminUser(Users updateAdmin)
        {
            if (DBUtl.ExecSQL(@"DELETE Users WHERE MembershipNo = {0}", updateAdmin.MembershipNo) == 1)
                TempData["MsgAdmin"] = "User " + updateAdmin.FullName + " Deleted!";
            else
                TempData["MsgAdmin"] = DBUtl.DB_Message;
            return RedirectToAction("ViewAdminUser");
        }

        [Authorize]
        public ActionResult CreateAdmin()
        {
            DateTime dt = DateTime.Now;
            ViewData["today'sDate"] = dt;

            return View();
        }

        [HttpPost]
        public ActionResult CreateAdmin(Users newAdmin)
        {
          
                if (DBUtl.ExecSQL(@"INSERT INTO Users
                                    (FullName, Email, RoleID, Password) 
                                    VALUES
                                    ('{0}','{1}',{2}, HASHBYTES('SHA1', '{3}'))",
                                    newAdmin.FullName, newAdmin.Email, 1, newAdmin.Password) == 1)

                    TempData["MsgAdmin"] = "New Admin Account Created!";
                else
                    TempData["MsgAdmin"] = DBUtl.DB_Message;
                return Redirect("ViewAdminUser");
            
        }

        
        [Authorize]
        public IActionResult searchMember(string search)
        {
            List<Users> ListsOfMember = DBUtl.GetList<Users>
                                             (@"SELECT * FROM Users 
                                                WHERE RoleID = 2  
                                                AND (FullName = '{0}'
                                                OR MembershipNo = '{0}')", search.Trim());

            return View("ViewMemberUser", ListsOfMember);
        }

        [Authorize]
        public IActionResult searchAdmin(string search)
        {
            List<Users> ListsOfAdmin = DBUtl.GetList<Users>
                                             (@"SELECT * FROM Users WHERE RoleID = 1                    
                                                AND (FullName = '{0}'
                                                OR MembershipNo = '{0}')
                                                ORDER BY MembershipNo", search.Trim());

            return View("ViewAdminUser", ListsOfAdmin);
        }
    }
}