﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TAIProject.Data;
using TAIProject.Helpers;
using TAIProject.Models;

namespace TAIProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "3e5cac39-7e38-4139-8fd6-30adc06a61bd");

        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            if (_context.Order != null)
            {
                if (User.IsInRole("Moderator") | User.IsInRole("Admin"))
                {
                    return View(await _context.Order.ToListAsync());
                } else if (User.IsInRole("Basic"))
                {
                    var id = _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name).Result.Id;
                    var ordersUser = await _context.Order.Where(o => o.UserID == id).ToListAsync();
                    return View(ordersUser);
                } else
                {
                    return NotFound();

                }
            } else
            {
                return NotFound();
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            var view = new OrderItemView();
            view.items = new();
            view.Adress = order.Adress;
            view.CreatedDate = order.CreatedDate;
            view.UserID = order.UserID;
            view.Total = order.Total;
            view.Id = order.Id;
            var items = await _context.OrderProduct.Where(o => o.OrderId == order.Id).ToListAsync();
            foreach(var item in items)
            {
                item.Product = await _context.Product.FindAsync(item.ProductID);
                view.items.Add(item);
            }
            return View(view);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Adress,CreatedDate,UserID")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid();
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Adress,CreatedDate")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            var items = _context.OrderProduct.Where(i => i.OrderId == id);
            foreach (var i in items)
            {
                _context.OrderProduct.Remove(i);
            }
            if (order != null)
            {
                _context.Order.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> AddOrderFromBasket()
        {

            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            var name = User.Identity.Name;
            var users = _context.Users.ToList();

            var user = users.FirstOrDefault(m => m.UserName == name);
            var order = new Order();
                order.Id = Guid.NewGuid();
                order.UserID = user.Id;
                order.CreatedDate = DateTime.Now;
            order.PaymentState = "New";
            order.Total = 0;
                foreach(var i in cart)
            {
                var o = new OrderProduct
                {
                    OrderId = order.Id,
                    ProductID = i.Product.Id,
                    Price = i.Product.UnitPrice,
                    ProductAmount = i.Quantity
                };
                _context.OrderProduct.Add(o);
                order.Total += i.Quantity * i.Product.UnitPrice;
            }
                _context.Add(order);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Pay(Guid id)
        {

            var order = await _context.Order.FindAsync(id);
            var customer = await _context.Users.FindAsync(order.UserID);
            var request = new JSONOrderCreateRequest();
            request.extOrderId = order.Id.ToString();
            request.notifyUrl = "";
            request.customerIP = HttpContext.Connection.RemoteIpAddress.ToString();
            request.totalAmount = (order.Total*100).ToString();
            request.buyer.email = customer.Email;
            request.buyer.phone = customer.PhoneNumber;
            request.buyer.firstName = customer.FirstName;
            request.buyer.lastName = customer.LastName;
            request.buyer.language = "pl";
            return View(order);

        }

        public async Task<IActionResult> PayU(Guid id)
        {
            var json = new JSONOrderCreateRequest();

            var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == id);
            var items = await _context.OrderProduct.Where(o => o.OrderId == order.Id).ToListAsync();
            var customer = await _context.Users.FindAsync(order.UserID);
            json.totalAmount = order.Total.ToString();
            json.extOrderId = order.Id.ToString();
            json.customerIP = Request.Host.Host;
            json.notifyUrl = "https://107f-46-204-44-227.eu.ngrok.io/Orders/RecieveConfirmPayU";
            json.buyer = new();
            json.buyer.email = customer.Email;
            try {
                json.buyer.phone = customer.PhoneNumber;
            }
            catch (NullReferenceException e) { 
                json.buyer.phone = "";
            }
            json.buyer.firstName = customer.FirstName;
            json.buyer.lastName = customer.LastName;
            json.buyer.language = "en";
            foreach (var item in items)
            {
                var product = await _context.Product.FindAsync(item.ProductID);
                json.products.Add(new OrderItem
                {
                    name = product.Name,
                    quantity = item.ProductAmount.ToString(),
                    unitPrice = (item.Price*item.ProductAmount*100).ToString()
                });
            }
            string jsonString = System.Text.Json.JsonSerializer.Serialize(json);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var result = await _httpClient.PostAsync("https://secure.snd.payu.com/api/v2_1/orders ", content);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecieveConfirmPayU()
        {
            Stream req = Request.Body;
            string bodyText = await new StreamReader(req).ReadToEndAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyText);
            if (json.status.statusCode == "SUCCESS")
            {
                var id = json.extOrderId;
                var url = json.redirectUri;
                var order = _context.Order.Find(new Guid((string)id));
                order.PaymentState = "Pending";
                _context.Update(order);
                _context.SaveChanges();
                return Redirect((string)url);
            }
            return RedirectToAction(nameof(Index));

        }

        private bool OrderExists(Guid id)
        {
          return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
