using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScamScraper.Models;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


// For HashSet


namespace ScamScraper.Controllers
{
    public class QAController : Controller
    {
        //-----VARIABLES-----
        private MyContext _context;
        public QAController(MyContext context)
        {   
            _context = context;
        }


        [HttpGet("qa/login")]
        public IActionResult LoginIndex()
        {
            return View("LoginIndex");
        }
        [HttpGet("qa/register")]
        public IActionResult RegisterIndex()
        {
            return View("RegisterIndex");
        }
        [HttpPost("register")]
        public IActionResult Register(User thisUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == thisUser.Email))
                {
                    ModelState.AddModelError("Email", "This email is already in use");
                    return View("RegisterIndex");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                thisUser.Password = Hasher.HashPassword(thisUser, thisUser.Password);
                _context.Add(thisUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("uuid", thisUser.UserId);
                return RedirectToAction("QAMain");
            }
            return View("RegisterIndex");
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser thisUser)
        {
            if(ModelState.IsValid)
            {
                User userInDb = _context.Users
                    .FirstOrDefault(u => u.Email == thisUser.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginError", "There was an error logging you in, please try again");
                    View("LoginIndex");
                }
                System.Console.WriteLine("The userInDb is not null");
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(thisUser, userInDb.Password, thisUser.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginError", "There was an error logging you in, please try again");
                    return View("LoginIndex");
                }
                HttpContext.Session.SetInt32("uuid", userInDb.UserId);
                return RedirectToAction("QAMain");
            }
            return View("LoginIndex");
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("LoginIndex");
        }

        

        [HttpGet("qa/home")]
        public IActionResult QAMain()
        {
            // DashWrapper WMod = new DashWrapper()
            //     {};

            if(HttpContext.Session.GetInt32("uuid") == null)
            {
                return RedirectToAction("RegisterIndex");
            }
            ViewBag.thisUser = _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("uuid"));
            ViewBag.AllMessages = _context.Messages
                .Include(m => m.Comments)
                .ThenInclude(c => c.Associations)
                .Include(m => m.User)
                .ToList();
            ViewBag.thisUserId = HttpContext.Session.GetInt32("uuid");
            return View("QAMain");
        }
    //--------- Add and remove Messages and Comments Logic-----------//
        [HttpPost("add/message")]
        public IActionResult AddMessage(Message FromForm)
        {
            if(ModelState.IsValid)
            {
                _context.Add(FromForm);
                _context.SaveChanges();
                return RedirectToAction("QAMain");
            }
            return RedirectToAction("QAMain");
        }
        [HttpPost("add/comment")]
        public IActionResult AddComment(Comment FromForm)
        {
            if(ModelState.IsValid)
            {
                _context.Add(FromForm);
                _context.SaveChanges();
                return RedirectToAction("QAMain");
            }
            return RedirectToAction("QAMain");
        }
        [HttpPost("delete/message")]
        public IActionResult DeleteMessage(int messageId)
        {
            Message thisMessage = _context.Messages
                .FirstOrDefault(m => m.MessageId == messageId);
            _context.Remove(thisMessage);
            _context.SaveChanges();
            return RedirectToAction("QAMain");
        }
        [HttpPost("delete/comment")]
        public IActionResult DeleteComment(int commentId)
        {
            Comment thisComment = _context.Comments
                .FirstOrDefault(c => c.CommentId == commentId);
            _context.Remove(thisComment);
            _context.SaveChanges();
            return RedirectToAction("QAMain");
        }
        
        // ------------------Add and Remove Likes Logic-------------------------//
        [HttpPost("add/like")]
        public IActionResult LikeComment(Association a)
        {
            int userId = (int)HttpContext.Session.GetInt32("uuid");
            a.UserId = userId;
            _context.Add(a);
            _context.SaveChanges();
            return RedirectToAction("QAMain");
        }
        public IActionResult RemoveLike(int commentId)
        {
            int userId = (int)HttpContext.Session.GetInt32("uuid");
            Association a = _context.Associations
                .FirstOrDefault(a => a.UserId == userId && a.CommentId == commentId);
            _context.Associations.Remove(a);
            _context.SaveChanges();
            return RedirectToAction("QAMain");
        }
        public IActionResult RemoveComment(int commentId)
        {
            System.Console.WriteLine(commentId);
            Comment thisComment = _context.Comments
                .Include(w => w.Associations)
                .ThenInclude(a => a.User)
                .FirstOrDefault(w => w.CommentId == commentId);
            System.Console.WriteLine(thisComment.CommentId);
            _context.Remove(thisComment);
            _context.SaveChanges();
            return RedirectToAction("QAMain");
        }

        [HttpGet("message/{messageId}")]
        public IActionResult ViewMessage(int messageId)
        {
            if(HttpContext.Session.GetInt32("uuid") == null)
            {
                return RedirectToAction("LoginIndex");
            }
            ViewBag.thisUser = _context.Users
                .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("uuid"));
            System.Console.WriteLine($"This is the User: {ViewBag.thisUser.UserId}");
            ViewBag.thisMessage = _context.Messages
                .Include(m => m.Comments)
                .ThenInclude(c => c.Associations)
                .Include(m => m.User)
                .FirstOrDefault(m => m.MessageId == messageId);
            return View("ViewMessage");
        }
        // -------------------------------------------------------------------//
    }
}