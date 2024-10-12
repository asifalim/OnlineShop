
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        UserManager<IdentityUser> userManager;
        ApplicationDbContext dbcontext;
        public UserController(UserManager<IdentityUser> _userManager, ApplicationDbContext _dbcontext)
        {
            userManager = _userManager; 
            dbcontext = _dbcontext;
        }
        public IActionResult Index()
        {
            return View(dbcontext.applicationUsers.ToList());
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Create(ApplicationUser applicationUser)
        {
            if (ModelState.IsValid) 
            {
                var result = await userManager.CreateAsync(applicationUser, applicationUser.PasswordHash);
                if (result.Succeeded) {
                    var saveUserRole = await userManager.AddToRoleAsync(applicationUser, "Customer");
                    TempData["Save"] = "User has been created";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(); 
        }

        public async Task<IActionResult>Edit(string id)
        {
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult>Edit(ApplicationUser applicationuser)
        {
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == applicationuser.Id);
            if (user == null) return NotFound();
            user.FirstName = applicationuser.FirstName;
            user.LastName = applicationuser.LastName;
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Save"] = "User has been updated";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        public async Task<IActionResult> lockout(string id)
        {
            if (id == null) return NotFound();
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult>lockout(ApplicationUser applicationUser)
        {
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == applicationUser.Id);
            if (user == null) return NotFound();
            user.LockoutEnd = DateTime.Now.AddYears(100);
            int rowaffected = dbcontext.SaveChanges();
            if(rowaffected > 0)
            {
                TempData["Save"] = "User has been Lockout";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        public async Task<IActionResult>Active(string id)
        {
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == id);
            if(user == null)return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult>Active(ApplicationUser applicationUser)
        {
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == applicationUser.Id);
            if (user == null) return NotFound();
            user.LockoutEnd = null;
            int rowaffected = dbcontext.SaveChanges();
            if(rowaffected > 0)
            {
                TempData["Save"] = "User has been Activated";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult>Delete(string id)
        {
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser applicationUser)
        {
            var user = dbcontext.applicationUsers.FirstOrDefault(c => c.Id == applicationUser.Id);
            if (user == null) return NotFound();
            dbcontext.applicationUsers.Remove(user);
            int rowaffected = dbcontext.SaveChanges();
            if (rowaffected > 0)
            {
                TempData["Save"] = "User has been Deleted";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
    }
}
