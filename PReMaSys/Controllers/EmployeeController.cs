﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PReMaSys.Data;

using PReMaSys.Models;

using PagedList;
using System.Collections.Generic;


namespace PReMaSys.Controllers
{
    [Authorize(Roles = "Sales")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult EmployeeRank()
        {
            return View();
        }

        public IActionResult TransactionHistory()
        {
            return View();
        }
        public IActionResult RewardStatus()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult EmployeeHomePage(String searchby, String search)
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var balance = _context.SERecord.FirstOrDefault(c => c.SERId == user).EmployeePoints;

            //My Points
            ViewBag.Balance = balance;

            var list = _context.Rewards.ToList();
            if (searchby == "RewardName" && search != null)
            {
                return View(list.Where(x => x.RewardName.Contains(search)).ToList());
            }
            /*else if(list != null)
            {
                return View(list.Where(x => ((int)x.Category) == 1).ToList()); // Returns Display Product Per Category
            }*/
            else
            {
                return View(list);
            }
        }

        public IActionResult Food()
        {
            var list = _context.Rewards.ToList();
            return View(list.Where(x => ((int)x.Category) == 1).ToList()); // Returns Display Product Per Category
        }
        public IActionResult Travel()
        {
            var list = _context.Rewards.ToList();
            return View(list.Where(x => ((int)x.Category) == 2).ToList()); // Returns Display Product Per Category
        }
        public IActionResult Discounts()
        {
            var list = _context.Rewards.ToList();
            return View(list.Where(x => ((int)x.Category) == 3).ToList()); // Returns Display Product Per Category
        }
        public IActionResult Others()
        {
            var list = _context.Rewards.ToList();
            return View(list.Where(x => ((int)x.Category) == 4).ToList()); // Returns Display Product Per Category
        }


        //Display Add To Cart Rewards
        public IActionResult AddToCartDisplay()
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));


            var list = _context.AddToCart.Where(c => c.ApplicationUser == user).ToList();

            decimal x = 0;
            if (list != null)
            {
                foreach (var item in list)
                {
                    x += item.TotalCost;
                }
                TempData["Total"] = x;
            }

            return View(list);
        }

        //Purchase Button
        public IActionResult AddToCart(int? id)
        {
            Rewards reward = _context.Rewards.Where(r => r.RewardsInformationId == id).SingleOrDefault();
            return View(reward);
        }

        [HttpPost]
        public IActionResult AddToCart(string qty, int id)
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            /*// this retrieves the foreign key id from customer table
            var latestCId = _context.Customers.FirstOrDefault(c => c.ApplicationUser == user).CustomerId;
            Customer cust = _context.Customers.FirstOrDefault(c => c.CustomerId == latestCId);*/

            Rewards reward = _context.Rewards.Where(r => r.RewardsInformationId == id).SingleOrDefault();


            AddToCart cart = new AddToCart();

            cart.ApplicationUser = user;
            cart.Reward = reward;
            cart.RewardImage = reward.Picture;
            cart.RewardName = reward.RewardName;
            cart.Category = reward.Category;
            cart.RewardDescription = reward.Description;
            cart.RewardPrice = reward.PointsCost;
            cart.TotalCost = reward.PointsCost;

            _context.AddToCart.Add(cart);
            _context.SaveChanges();

            return RedirectToAction("EmployeeHomePage");
        }

        //Delete Item from Cart
        public IActionResult DeleteItem(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("AddToCartDisplay");
            }

            var cart = _context.AddToCart.Where(i => i.CartId == id).SingleOrDefault();
            if (cart == null)
            {
                return RedirectToAction("AddToCartDisplay");
            }

            _context.AddToCart.Remove(cart);
            _context.SaveChanges();

            return RedirectToAction("AddToCartDisplay");         
        }

        public IActionResult PurchaseView()
        {
            ViewBag.userId = _userManager.GetUserName(HttpContext.User);

            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            var list = _context.Purchase.Where(c => c.ApplicationUser == user).ToList();
            return View(list);
        }

        public IActionResult Transaction()
        {
            ViewBag.userId = _userManager.GetUserName(HttpContext.User);

            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            var list = _context.Purchase.Where(c => c.ApplicationUser == user).ToList();
            return View(list);
        }

        //Check Out
        public IActionResult Purchase(int? id)
        {
            AddToCart purchase = _context.AddToCart.Where(c => c.CartId == id).SingleOrDefault();
            return View(purchase);
        }
        [HttpPost]
        public IActionResult Purchase(Purchase record, int id)
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            AddToCart addCart = _context.AddToCart.Where(a => a.CartId == id).SingleOrDefault();

            Purchase purchase = new Purchase();

            var check = _context.SERecord.FirstOrDefault(c => c.SERId == user).EmployeePoints;

            if(Convert.ToDecimal(check) >= addCart.RewardPrice)
            {
                purchase.ApplicationUser = user;
                purchase.EmployeeName = user.UserName;
                purchase.AddToCart = addCart;
                purchase.RewardImage = addCart.RewardImage;
                purchase.RewardName = addCart.RewardName;
                purchase.RewardPrice = addCart.RewardPrice;
                purchase.TotalPayment = addCart.RewardPrice;
                purchase.DateAdded = DateTime.Now;
                purchase.Stat = record.Stat;
                
                var temp = Convert.ToDecimal(check) - addCart.RewardPrice;

                var SEmployees = _context.SERecord.Where(s => s.SERId == user).SingleOrDefault();
                SEmployees.EmployeePoints = temp.ToString();


                
                _context.SERecord.Update(SEmployees);
                _context.Purchase.Add(purchase);
                _context.SaveChanges();

                TempData["ResultMessage"]= "Thank you for your Purchase!";
                return RedirectToAction("Purchase");
               
            }
            else
            {
                TempData["ResultMessage2"] = "Sorry! Insufficient Points!";
                return RedirectToAction("Purchase");
            }

       
        }

    }
}
