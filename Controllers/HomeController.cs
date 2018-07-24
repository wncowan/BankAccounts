using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private BankContext _context;

        public HomeController(BankContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel model)
        {

            if(ModelState.IsValid)
            {
                User CheckUser = _context.Users.SingleOrDefault(u => u.Email == model.Email);
                if(CheckUser != null)
                {
                    TempData["EmailInUseError"] = "Email Aleady in use";
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    User user = new User(){
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        created_at = DateTime.Now
                    };
                    user.Password = Hasher.HashPassword(user, model.Password);
                    _context.Add(user);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("currentUserId", user.UserId);
                    HttpContext.Session.SetString("currentFirstName", user.FirstName);
                    return RedirectToAction("Dashboard");
                }
            }
            else{
                return View("Index");
            }
        }
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginViewModel model)
        {
            
            User logUser = _context.Users.SingleOrDefault(_user => _user.Email == model.Email);
            if (logUser == null)
            {
                TempData["EmailError"] = "Email Not Found!";
                return RedirectToAction("Index");
            }
            else
            {
                if(logUser != null && model.Password != null)
                {
                    var Hasher = new PasswordHasher<User>();
                    // Pass the user object, the hashed password, and the PasswordToCheck
                    if(0 != Hasher.VerifyHashedPassword(logUser, logUser.Password, model.Password))
                    {
                        //Handle success
                        HttpContext.Session.SetInt32("currentUserId", logUser.UserId);
                        HttpContext.Session.SetString("currentFirstName", logUser.FirstName);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        TempData["PasswordError"] = "Invalid password";
                        return View("Index");
                    }
                }
                return View("Index");
                // if (logUser.Password == user.Password)
                // {
                //     return RedirectToAction("Success");
                // }
                // else
                // {
                //     TempData["PasswordError"] = "Invalid password";
                //     return RedirectToAction("Index");
                // }
            }
        }
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                List<Transaction> userTransactions = _context.Transactions.Where(t => t.UserId == (int)userId).ToList();
                ViewBag.currentUser = currentUser;
                ViewBag.userTransactions = userTransactions;
                return View();
            }
        }

        [HttpPost]
        [Route("process")]
        public IActionResult Process(TransactionViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
            if(ModelState.IsValid)
            {
                
                if((model.Amount + currentUser.Balance) < 0)
                {
                    TempData["Overdraft"] = "Insufficient Funds";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    Transaction transaction = new Transaction()
                    {
                        Amount = model.Amount,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                        UserId = (int)userId,

                    };
                    if(transaction.Amount < 0)
                    {
                        transaction.TransactionType = "Withdrawal";
                    }
                    else
                    {
                        transaction.TransactionType = "Deposit";
                    }
                    currentUser.Balance += transaction.Amount;
                    transaction.CurrentBalance = currentUser.Balance;
                    _context.Add(transaction);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
    
            else
            {
                return View("Dashboard");
            }
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
