using HR.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Linq.Dynamic;
//using CustomAuthorizationFilter.Infrastructure;
using HR.Website.Infrastructure;
using System.Web.Security;

namespace HR.Website.Controllers
{
    public class UserController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            //if (string.IsNullOrEmpty(Convert.ToString(Session["UserId"])))
            //{
            //    return View();
            //}
            //if (Convert.ToString(Session["Role"]) == "0")
            //    return RedirectToAction("UserManager");
            //if (Convert.ToString(Session["Role"]) == "1")
            //    return RedirectToAction("UserEmployee");
            return View();
        }

        [CustomAuthorizationFilter("0")]
        public ActionResult UserManager()
        {
            if (Session["Username"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [CustomAuthorizationFilter("1")]
        public ActionResult UserEmployee()
        {
            if (Session["Username"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [CustomAuthorizationFilter("0")]
        public ActionResult AddNewEmployee()
        {
            return View();
        }
        public ActionResult UpdateEmployee(int id = 0)
        {
            try
            {
                if (id != 0)
                {
                    HttpResponseMessage response = GlobalVariable.ApiClient.GetAsync($"users/GetUserById/{id}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var user = response.Content.ReadAsAsync<UserInsertAndEdit>().Result;

                        return View(user);
                    }

                }
            }
            catch (Exception ex) {
                ViewBag.Message = $"Oops! Something went wrong, please try again later";
                GlobalVariable.log.Error(ex);
            }
            return View();
        }
        [CustomAuthorizationFilter("0")]
        public ActionResult DisplayUsers()
        {
            return View();
        }
        public ActionResult UnAuthorized()
        {
            ViewBag.Message = "You are not authorized to access this page!";

            return View();
        }
        [HttpPost]
        public ActionResult GetUsers()
        {
            PagingModel paging = new PagingModel
            {
                // server side parameter
                DisplayStart = Convert.ToInt32(Request["start"]),
                DisplayLength = Convert.ToInt32(Request["length"]),
                SearchValue = Request["search[value]"],
                SortCol = Request["columns[" + Request["order[0][column]"] + "][name]"],
                SortDir = Request["order[0][dir]"]
            };
            List<UserView> users = null;
            var userID = Session["UserID"];
            try
            {
                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = GlobalVariable.ApiClient.PostAsJsonAsync<PagingModel>($"users/GetManagerEmployees/{userID}", paging).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsAsync<PageRecord<UserView>>().Result;
                        users = content.Data;
                        int totalRecord = content.TotalRecord;
                        int totalFilteredRecord = content.TotalFilteredRecord;
     

                        var result = from c in users select new[] { Convert.ToString(c.UserID), c.Name, c.LastName, c.JobTitle, c.Mobile, c.Email };
                        return Json(new { aaData = result.ToArray(), recordsTotal = totalRecord, draw = Request["draw"], recordsFiltered = totalFilteredRecord }, JsonRequestBehavior.AllowGet);
                    }
                }
                //  ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                GlobalVariable.log.Error(ex);
             return Json(new { aaData = string.Empty.ToArray(), JsonRequestBehavior.AllowGet });
            }
          //  return View("DisplayUsers");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = GlobalVariable.ApiClient.PostAsJsonAsync<LoginModel>("users/login", user).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var users = response.Content.ReadAsAsync<LoginModel[]>().Result;
                    //    Session["User"] = user;      
                      foreach (var u in users)
                        {
                            Session["UserID"] = u.UserID.ToString();
                            Session["Username"] = u.Username.ToString();
                         //   Session["Password"] = u.Password.ToString();
                            Session["Role"] = u.Role.ToString();
                            if (u.Role == 0)
                            {
                                return RedirectToAction("UserManager");
                            }
                            else
                                return RedirectToAction("UserEmployee");
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("", "Check the username and password !");
                    }

                }
            }
            catch (Exception ex) {
                 ViewBag.Message = $"Oops! Something went wrong, please try again later";
                GlobalVariable.log.Error(ex);
            }
            return View("Index");

        }
      
        [HttpPost]
        public ActionResult AddUser(UserInsertAndEdit user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    user.ManagerID = Convert.ToInt32(Session["UserID"]);
                    HttpResponseMessage response = GlobalVariable.ApiClient.PostAsJsonAsync<UserInsertAndEdit>("users/PostUser", user).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Username, Email, or Phone number is alreay exist!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex) {
                GlobalVariable.log.Error(ex);
                return Json(new { success = false, message = "Oops! Something went wrong, please try again later" }, JsonRequestBehavior.AllowGet);}
            return View("AddNewEmployee", user);
        }

        [HttpPost]
        [CustomAuthorizationFilter("0")]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                HttpResponseMessage response = GlobalVariable.ApiClient.DeleteAsync($"users/DeleteUser/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { error = true, message = "Failed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                GlobalVariable.log.Error(ex);
                return Json(new { success = false, message = "Oops! Something went wrong, please try again later" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateUser(UserInsertAndEdit user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = GlobalVariable.ApiClient.PutAsJsonAsync<UserInsertAndEdit>($"users/PutUser/{user.UserID}", user).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Updated" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Username, Email, or Phone number is alreay exist!" });
                    }
                }
                return Json(new { success = false, message = "ERROR" });
            }
            catch (Exception ex) {
                GlobalVariable.log.Error(ex);
                return Json(new { success = false, message = "Oops! Something went wrong, please try again later" }, JsonRequestBehavior.AllowGet); }
        }


        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index","User");
        }
        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    Exception exception = filterContext.Exception;
        //    Logging the Exception
        //    filterContext.ExceptionHandled = true;

        //    var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");
        //    filterContext.Result = new ViewResult()
        //    {
        //        ViewName = "Error",
        //        ViewData = new ViewDataDictionary(model)
        //    };
        //    filterContext.Result = RedirectToAction("UnAuthorized", "User");
        //    var Result = this.View("Error", new HandleErrorInfo(exception,
        //        filterContext.RouteData.Values["controller"].ToString(),
        //        filterContext.RouteData.Values["action"].ToString()));

        //    filterContext.Result = Result;

        //}

    }
}