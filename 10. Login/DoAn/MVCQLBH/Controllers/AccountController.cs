﻿using BotDetect.Web.Mvc;
using MVCQLBH.Models;
using MVCQLBH.Ultilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCQLBH.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POSST: Account/Login
        [HttpPost]
        public ActionResult Login(UserInfo ui)
        {
            var pass = Ulti.Md5Hash(ui.Password);
            using (var dc = new QLBHEntities())
            {
                var user = dc.Users.Where(u => u.f_Username == ui.Username && u.f_Password == pass).FirstOrDefault();
                if(user != null)
                {
                    Session["Logged"] = ui;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMsg = "Thông tin đăng nhập chưa đúng";
                }
            }
            return View();
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            var u = new UserRegisting
            {
                Username = "user001",
                Password = "1234",
                PasswordRetype = "1234",
                Name = "user 0009",
                DOB = "12/6/1999",
            };
            return View(u);
            //return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "Incorrect CAPTCHA code!")]

        public ActionResult Register(UserRegisting user)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Captcha validation failed, show error message
                //ViewBag.ErrorMsg = "Incorrect CAPTCHA code!";
                ViewBag.ErrorMsg = "CAPTCHA không hợp lệ!";
            }
            else
            {
                var u = new User
                {
                    f_Username = user.Username,
                    f_Password = Ulti.Md5Hash(user.Password),
                    f_Name = user.Name,
                    f_Email = user.Email,
                    f_DOB = DateTime.ParseExact(user.DOB, "d/m/yyyy", null)
                };

                using (var dc = new QLBHEntities())
                {
                    dc.Users.Add(u);
                    dc.SaveChanges();
                }
                var ui = new UserInfo { Username = u.f_Username };
                Session["Logged"] = ui;
                //return View("RegisterSuccess", user);
                return RedirectToAction("RegisterSuccess", "Home");
            }
            return View();
        }

    }
}