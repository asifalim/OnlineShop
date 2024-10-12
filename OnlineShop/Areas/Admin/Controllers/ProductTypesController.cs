using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductTypesController : Controller
    {
        ApplicationDbContext dbContext;
        public ProductTypesController(ApplicationDbContext _dbContext) 
        {
           dbContext = _dbContext;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(dbContext.productTypes.ToList());
        }

        //Create Get Action Method
       
        public ActionResult Create()
        {
            return View();
        }

        //Create Post Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if(ModelState.IsValid)
            {
                dbContext.productTypes.Add(productTypes);
                await dbContext.SaveChangesAsync();
                TempData["save"] = "Product saved successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

        //Create Edit Action
        public ActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var productType = dbContext.productTypes.Find(id);
            if(productType== null) return NotFound();
            return View(productType);
        }

        //Post Edit Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                dbContext.Update(productTypes);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

        //Create Details Action
        public ActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var productType = dbContext.productTypes.Find(id);
            if (productType == null) return NotFound();
            return View(productType);
        }

        //Post Details Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ProductTypes productTypes)
        {
            return RedirectToAction(nameof(Index));
        }

        //Delete Action
        public ActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var productType = dbContext.productTypes.Find(id);
            if (productType == null) return NotFound();
            return View(productType);
        }

        //Post Edit Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, ProductTypes productTypes)
        {
            if (id == null || id != productTypes.Id) return NotFound();
            var producttype = dbContext.productTypes.Find(id);
            if(producttype == null) return NotFound();
            if (ModelState.IsValid)
            {
                dbContext.Remove(producttype);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }
    }
}
