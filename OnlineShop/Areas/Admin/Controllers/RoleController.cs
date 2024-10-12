
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Areas.Admin.Models;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        ApplicationDbContext dbContext;
        UserManager<IdentityUser> userManager;
        public RoleController(RoleManager<IdentityRole> _roleManager, ApplicationDbContext _dbcontext, UserManager<IdentityUser> _userManager)
        {
            roleManager = _roleManager;
            dbContext = _dbcontext;
            userManager = _userManager;
        }
        public IActionResult Index()
        {
            var role = roleManager.Roles.ToList();
            ViewBag.Roles = role;
            return View();
        }

        public async Task<IActionResult>Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Create(string name)
        {
            IdentityRole role = new IdentityRole();
            role.Name = name;
            var isExist = await roleManager.RoleExistsAsync(role.Name);
            if(isExist)
            {
                ViewBag.Msg = "This role is already exist";
                ViewBag.name = name;
                return View();
            }
            var result = await roleManager.CreateAsync(role);
            if(result.Succeeded)
            {
                TempData["Save"] = "Role has been Created";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            ViewBag.Id = role.Id; 
            ViewBag.Name = role.Name;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string name)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            role.Name = name;
            var isExist = await roleManager.RoleExistsAsync(role.Name);
            if (isExist)
            {
                ViewBag.Msg = "This role is already Exist";
                ViewBag.name = name;
                return View();
            }
            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                TempData["Save"] = "Role has been Updated";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            ViewBag.Id = role.Id;
            ViewBag.Name = role.Name;
            return View();
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult>DeleteConfirm(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            var result = await roleManager.DeleteAsync(role);
            if(result.Succeeded)
            {
                TempData["delete"] = "Role has been Deleted";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        
        public async Task<IActionResult>Assign()
        {
            ViewData["UserId"] = new SelectList(dbContext.applicationUsers.Where(c=>c.LockoutEnd<DateTime.Now || c.LockoutEnd == null).ToList(), "Id", "UserName");
            ViewData["RoleId"] = new SelectList(roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Assign(RoleUservm roleUser)
        {
            var user = dbContext.applicationUsers.FirstOrDefault(c => c.Id == roleUser.UserId);
            var isExist = await userManager.IsInRoleAsync(user, roleUser.RoleId);
            if(isExist)
            {
                ViewBag.Msg = "This User already assign this role";
                ViewData["UserId"] = new SelectList(dbContext.applicationUsers.Where(c => c.LockoutEnd < DateTime.Now || c.LockoutEnd == null).ToList(), "Id", "UserName");
                ViewData["RoleId"] = new SelectList(roleManager.Roles.ToList(), "Name", "Name");
                return View();
            }
            var role = await userManager.AddToRoleAsync(user, roleUser.RoleId);
            if(role.Succeeded)
            {
                TempData["Save"] = "User role assigned";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public ActionResult AssignUserRole()
        {
            var result = from ur in dbContext.UserRoles
                         join r in dbContext.Roles on ur.RoleId equals r.Id
                         join a in dbContext.applicationUsers on ur.UserId equals a.Id
                         select new UserRoleMapping()
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserName = a.UserName,
                             RoleName = r.Name
                         };
            ViewBag.Roles = result;
            return View();
        }
    }
}
