using HR.Website.Infrastructure;
using HR.Website.Models;
using HR.Website.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HR.Website.Controllers
{
    public class VacationController : BaseController
    {
        [CustomAuthorizationFilter("0", "1")]
        public ActionResult DisplayVacations()
        {
            return View();
        }
        [CustomAuthorizationFilter("1")]
        public ActionResult AddNewVacation()
        {

            return View();
        }
        public ActionResult UpdateVacation(int id = 0)
        {
            try
            {
                if (id != 0)
                {
                    HttpResponseMessage response = GlobalVariable.ApiClient.GetAsync($"vacations/GetVacationById/{id}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var vacation = response.Content.ReadAsAsync<VacationInsertAndEdit>().Result;

                        return View(vacation);
                    }

                }
            }
            catch (Exception ex) { 
                GlobalVariable.log.Error(ex);
                ViewBag.Message = $"Oops! Something went wrong, please try again later"; }
            return View();
        }
        // GET: Vacation
        [HandleError]
        public ActionResult Vacation()
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

            List<VacationView> vacations = null;
            var userID = Session["UserID"];
            var role = Convert.ToInt32(Session["Role"]);
            try
            {
                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = GlobalVariable.ApiClient.PostAsJsonAsync<PagingModel>($"vacations/GetVacations?id={userID}&role={role}", paging).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsAsync<PageRecord<VacationView>>().Result;
                        vacations = content.Data;
                        int totalRecord = content.TotalRecord;
                        int totalFilteredRecord = content.TotalFilteredRecord;

                        var result = from c in vacations
                                     select new[]
                            { Convert.ToString(c.VacationID),
                        c.Name, c.LastName, Convert.ToString(c.Type), Convert.ToString(c.StartDate),
                        Convert.ToString(c.EndDate), Convert.ToString(c.Status), c.Attatchment, c.Description ,c.RejectionReason};
                        return Json(new { aaData = result.ToArray(), recordsTotal = totalRecord, draw = Request["draw"], recordsFiltered = totalFilteredRecord }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception ex) {     
                GlobalVariable.log.Error(ex);
                return Json(new { aaData = string.Empty.ToArray(), JsonRequestBehavior.AllowGet });
            }
            return View("DisplayVacations");
        }
        public bool CheckContentType(string contentType) {
            return contentType.Equals("application/pdf");
        }
        public ActionResult LoadFile(string path)
        {
        
            //path = Path.Combine(@"C:\Users\Ghadir SSD\source\repos\HR.Website\HR.Website\Content\Files");

            return File(path, "application/pdf");

        }
        
        [HttpPost]
        [CustomAuthorizationFilter("1")]
        public ActionResult AddVacation(VacationView vacation, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                vacation.EmployeeID = Convert.ToInt32(Session["UserID"]);
                if (file != null && !CheckContentType(file.ContentType))
                {
                    return Json(new { success = false, message = "File format is not supported, please upload a PDF file!" }, JsonRequestBehavior.AllowGet);
                }
                else if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string path = vacation.Attatchment;

                        path = Path.Combine(Server.MapPath("/Content/Files"),
                                                   Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        vacation.Attatchment = path;

                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        GlobalVariable.log.Error(ex);
                       // ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                //else
                //{
                //    ViewBag.Message = "You have not specified a file.";
                //}
                try
                {
                    HttpResponseMessage response = GlobalVariable.ApiClient.PostAsJsonAsync<VacationView>("vacations/PostVacation", vacation).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var totalDays = (vacation.EndDate - vacation.StartDate).TotalDays;
                        var totalHours = (vacation.EndDate - vacation.StartDate).TotalHours;
                        if (Math.Abs(totalDays) > 14 && vacation.Type == Types.Annual)
                        {
                            return Json(new { success = false, message = "Only 14 days or less are allowed for Annual vacation!" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (Math.Abs(totalHours) > 8 && vacation.Type == Types.Leave)
                        {
                            return Json(new { success = false, message = "Only 8 hours or less are allowed for Leave Request!" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { success = false, message = "You have exceeded the limit of annual vacation days" }, JsonRequestBehavior.AllowGet);

                        // return Json(new { success = false, message = "Only 14 days or less are allowed for Annual vacation!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex) { 
                    GlobalVariable.log.Error(ex.Message); 
                    return Json(new { success = false, message = "Oops! Something went wrong, please try again later" }, JsonRequestBehavior.AllowGet); }
            }
            return View("AddNewVacation", vacation);

        }

        public ActionResult UpdateVacations(VacationInsertAndEdit vacation, HttpPostedFileBase file) {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null && !CheckContentType(file.ContentType))
                    {
                        return Json(new { success = false, message = "File format is not supported, please upload a PDF file!" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            string path = vacation.Attatchment;

                            path = Path.Combine(Server.MapPath("/Content/Files"),
                                                       Path.GetFileName(file.FileName));
                            file.SaveAs(path);
                            vacation.Attatchment = path;

                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            GlobalVariable.log.Error(ex);
                         //   ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    if (vacation.RejectionReason == null && vacation.Status == Statuses.Rejected)
                    {
                        return Json(new { success = false, message = "Reason field is required" });
                    }
                        HttpResponseMessage response = GlobalVariable.ApiClient.PutAsJsonAsync<VacationInsertAndEdit>($"vacations/PutVacation/{vacation.VacationID}", vacation).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Updated" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Not Found" });

                    }
                }
            }
            catch (Exception ex) {
                GlobalVariable.log.Error(ex.Message); 
                return Json(new { success = false, message = "Oops! Something went wrong, please try again later" }, JsonRequestBehavior.AllowGet); 
            }
            return View("UpdateVacation");
        }
        [HttpPost]
        public ActionResult DeleteVacation(int id)
        {
            try
            {
                HttpResponseMessage response = GlobalVariable.ApiClient.DeleteAsync($"vacations/DeleteVacation/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { error = true, message = "Failed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                GlobalVariable.log.Error(ex); 
                return Json(new { success = false, message = "Oops! Something went wrong, please try again later" }, JsonRequestBehavior.AllowGet); }
        }
        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    Exception exception = filterContext.Exception;
        //    //Logging the Exception
        //    filterContext.ExceptionHandled = true;

        //    var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");
        //    filterContext.Result = new ViewResult()
        //    {
        //        ViewName = "Error",
        //        ViewData = new ViewDataDictionary(model)
        //    };
        // //   filterContext.Result = RedirectToAction("UnAuthorized", "User");
        //    //var Result = this.View("Error", new HandleErrorInfo(exception,
        //    //    filterContext.RouteData.Values["controller"].ToString(),
        //    //    filterContext.RouteData.Values["action"].ToString()));

        //    //filterContext.Result = Result;

      //  }
    }
}
