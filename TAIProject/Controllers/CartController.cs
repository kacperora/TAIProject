using TAIProject.Helpers;
using TAIProject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TAIProject.Data;

namespace LearnASPNETCoreMVC5.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly HttpClient client;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
            client = new();
        }
        [Route("index")]
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            try
            {
                ViewBag.total = cart.Sum(item => item.Product.UnitPrice * item.Quantity);
            }
            catch (ArgumentNullException e) {
                ViewBag.total = 0;
            }
            return View();
        }

        [Route("buy/{id}")]
        public IActionResult Buy(Guid id)
        {

            var products = _context.Product.Where(s => s.Id.Equals(id)).ToList();
            var categories = _context.Category.ToList();


            var view = from prod in products
                       join cat in categories on prod.CategoryID equals cat.Id
                       select new ProductIndexModel
                       {
                           Product = prod,
                           Category = cat
                       };
            var result = view.FirstOrDefault(m => m.Product.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            Product product= result.Product;
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Product = result.Product, Quantity = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Product = result.Product, Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        [Route("remove/{id}")]
        public IActionResult Remove(Guid id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult Pay(Guid id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        private int isExist(Guid id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

    }
}