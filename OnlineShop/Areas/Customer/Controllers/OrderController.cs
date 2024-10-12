using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        ApplicationDbContext dbContext;
        public OrderController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public IActionResult CheckOut()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(Order order)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                foreach (var product in products)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.ProductId = product.Id;

                    order.OrderDetails.Add(orderDetails);
                }
            }
            order.OrderNo = GetOrderNo();
            dbContext.orders.Add(order);
            await dbContext.SaveChangesAsync();
            HttpContext.Session.Set("products", new List<Products>());
            return View();
        }

        public string GetOrderNo()
        {
            int rowCount = dbContext.orders.ToList().Count() + 1;
            return rowCount.ToString("000");
        }
    }
}
