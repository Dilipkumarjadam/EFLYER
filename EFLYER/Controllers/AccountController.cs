using EFLYER.DTOs;
using EFLYER.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace EFLYER.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEflyerRepository _repository;

        public AccountController(IEflyerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                return RedirectToAction("Index", "Home");

            }
            else if (HttpContext.Session.GetString("UserSession") == null)
            {
                return View();
            }
            return View();

        }

        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            var AdminUser = _repository.GetAdminData().Where(A => A.AdminEmail == Email && A.AdminPassword == Password).FirstOrDefault();
            if (AdminUser != null)
            {
                return RedirectToAction("AdminIndex", "Admin");
            }
            else
            {
                var MyUsers = _repository.GetUserData().Where(U => U.Email == Email && U.Password == Password).FirstOrDefault();
                if (MyUsers != null)
                {
                    HttpContext.Session.SetString("UserSession", MyUsers.FullName);
                    HttpContext.Session.SetInt32("UserId", MyUsers.RegId);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "Login Failed...";
                }
                return View();
            }

        }

        public ActionResult Logout()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                HttpContext.Session.Remove("UserSession");
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult RegisterUser()
        {
            var drop = _repository.GetCountry();
            ViewBag.DROP = new SelectList(drop, "CountryId", "CountryName");
            return View();
        }

        // POST: AccountController/Create
        [HttpPost]

        public ActionResult RegisterUser(RegisteredUserDTO dTO, IFormFile IMAGE)
        {
            var drop = _repository.GetCountry();
            ViewBag.DROP = new SelectList(drop, "CountryId", "CountryName");
            try
            {
                var CheckEmail = _repository.CheckEmail(dTO.Email, 0, "INSERT");
                if (CheckEmail == true)
                {
                    ViewBag.EmailError = "Email Already Exist Try Other.";
                    return View(dTO);
                }
                else
                {
                    if (IMAGE != null && IMAGE.Length > 0)
                    {
                        // Determine the file path and ensure it exists
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserImage");
                        Directory.CreateDirectory(uploadsFolder);

                        var fileName = Path.GetFileName(IMAGE.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save the file synchronously
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            IMAGE.CopyTo(stream); // Synchronously copy the file
                        }

                        dTO.ImagePath = $"/UserImage/{fileName}"; // Set image path in DTO
                    }

                    _repository.AddUserData(dTO);

                    string subject = "Successfully Registered";
                    string body = $"Dear {dTO.FullName},<br/>" +
                                  "Your account has been successfully added to our system.<br/>" +
                                  $"Email Id: {dTO.Email}<br/>" +
                                  $"Password: {dTO.Password}<br/>" +
                                  "You can Login With This EmailId and Password to access the site." +
                                  "Thank you!";

                    _repository.SendEmail(dTO.Email, subject, body);
                    return RedirectToAction(nameof(Login));
                }

            }
            catch
            {
                ViewBag.ErrorMessage = "An error occurred while registering. Please try again.";
                return View(dTO);
            }
        }

    }
}
