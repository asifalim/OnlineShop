using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;
using System.Diagnostics;
using X.PagedList;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        public ApplicationDbContext dbcontext;

        public HomeController(ApplicationDbContext _dbcontext)
        {
            dbcontext = _dbcontext;
        }

        public IActionResult Index(int? page)
        {
            return View(dbcontext.products.Include(c=>c.ProductTypes).ToList().ToPagedList(page??1,12));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Get product details action method
        public ActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var product = dbcontext.products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        //Post product details action method
        [HttpPost]
        [ActionName("Details")]
        public ActionResult ProductDetails(int? id)
        {
            List <Products > products = new List<Products>();
            if (id == null) return NotFound();
            var product = dbcontext.products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product == null) return NotFound();
            products = HttpContext.Session.Get<List<Products>>("products");
            if(products == null)
            {
                products = new List<Products>();
            }
            products.Add(product);
            HttpContext.Session.Set("products", products);
            return RedirectToAction(nameof(Index));
        }
        //GET Remove Action Method
        [ActionName("Remove")]
        public IActionResult RemoveFromCart(int? id)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }
            return RedirectToAction(nameof(Index));

        }

        //GET Product Cart action method
        public IActionResult Cart()
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products == null) products = new List<Products>();
            return View(products);
        }
    }
}
