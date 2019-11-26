using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFactorAuthenticationDemo.Models;

namespace TwoFactorAuthenticationDemo.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        private const string key = "qazqaz12345"; //You can add your own Key
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel login)
        {
            bool status = false;
            string message;
            //check UserName and password form our database here
            if (login.UserName == "Admin" && login.Password == "12345") // Admin as user name and 12345 as Password
            {
                status = true;
                message = "Two Factor Authentication Verification";
                Session["UserName"] = login.UserName;

                //Two Factor Authentication Setup
                TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                string UserUniqueKey = (login.UserName + key);
                Session["UserUniqueKey"] = UserUniqueKey;
                var setupInfo = TwoFacAuth.GenerateSetupCode("Dot Net Detail", login.UserName, UserUniqueKey, 300, 300);
                ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                ViewBag.SetupCode = setupInfo.ManualEntryKey;
            }
            else
            {
                message = "Please Enter the Valid Credential!";
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }

        public ActionResult TwoFactorAuthenticate()
        {
            var token = Request["CodeDigit"];
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            string UserUniqueKey = Session["UserUniqueKey"].ToString();
            bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, token);
            if (isValid)
            {
                Session["IsValidTwoFactorAuthentication"] = true;
                return RedirectToAction("UserProfile", "Home");
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult UserProfile()
        {
            if (Session["Username"] == null || Session["IsValidTwoFactorAuthentication"] == null || !(bool)Session["IsValidTwoFactorAuthentication"])
            {
                return RedirectToAction("Login");
            }
            ViewBag.Message = "Welcome to Mr. " + Session["Username"].ToString();
            return View();
        }
    }
}