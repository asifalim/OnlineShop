using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext dbContext;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment he;
        public ProductController(ApplicationDbContext _dbContext, Microsoft.AspNetCore.Hosting.IHostingEnvironment _he)
        {
            dbContext = _dbContext;
            he = _he;
        }
        public IActionResult Index()
        {
            return View(dbContext.products.Include(c=>c.ProductTypes).ToList());
        }
        [HttpPost]
        public IActionResult Index(decimal? lowAmount, decimal? highAmount)
        {
            if (lowAmount == null || highAmount == null) return View(dbContext.products.Include(c => c.ProductTypes).ToList());
            var products = dbContext.products.Include(c=>c.ProductTypes).Where(c=>c.Price >= lowAmount  && c.Price <= highAmount).ToList();
            return View(products);
        }

        //Get Create Method
        public IActionResult Create()
        {
            ViewData["productTypeId"] = new SelectList(dbContext.productTypes.ToList(), "Id", "ProductType");
            return View();
        }
        //Post Create Method
        [HttpPost]
        public async Task<IActionResult>Create(Products products, IFormFile Image)
        {

            //if(ModelState.IsValid)
            //{
                var searchProduct = dbContext.products.FirstOrDefault(c=>c.Name == products.Name);
                if(searchProduct != null)
                {
                ViewBag.Msg = "This product is already exist";
                ViewData["productTypeId"] = new SelectList(dbContext.productTypes.ToList(), "Id", "ProductType");
                return View(products);
                }

                if(Image!=null)
                {
                    var name = Path.Combine(he.WebRootPath+"/Images", Path.GetFileName(Image.FileName));
                    await Image.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.Image = "Images/" + Image.FileName;
                }
                if (Image == null)
                {
                   products.Image = "Images/noimage.jpg";
                }

                dbContext.products.Add(products);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            return View(products);
        }

        //Get Edit
        public ActionResult Edit(int? id)
        {
            ViewData["productTypeId"] = new SelectList(dbContext.productTypes.ToList(), "Id", "ProductType");
            if (id == null) return NotFound();
            var product = dbContext.products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult>Edit(Products products, IFormFile Image)
        {
            var searchProduct = dbContext.products.FirstOrDefault(c => c.Name == products.Name);
            if(searchProduct != null)
            {
                ViewBag.Msg = "This product is already exist";
                ViewData["productTypeId"] = new SelectList(dbContext.productTypes.ToList(), "Id", "ProductType");
                return View(products);
            }
            var originalProduct = await dbContext.products.AsNoTracking().FirstOrDefaultAsync(c=>c.Id == products.Id);
            if (Image != null)
            {
                var name = Path.Combine(he.WebRootPath + "/Images", Path.GetFileName(Image.FileName));
                await Image.CopyToAsync(new FileStream(name, FileMode.Create));
                products.Image = "Images/" + Image.FileName;
            }
            else
            {
                // products.Image = "Images/noimage.jpg";
                products.Image = originalProduct?.Image ?? "Images/noimage.jpg";
            }
            dbContext.products.Update(products);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        //Get Details Action Method
        public ActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var products = dbContext.products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (products == null) return NotFound();
            return View(products);
        }

        //Get Delete Action Method
        public ActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var products = dbContext.products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (products == null) return NotFound();
            return View(products);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult>DeleteConfirm(int? id)
        {
            if (id == null) return NotFound();
            var products = dbContext.products.FirstOrDefault(c => c.Id == id);
            if (products == null) return NotFound();
            dbContext.Remove(products);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
