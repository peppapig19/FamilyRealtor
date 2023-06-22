using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Areas.Admin.Models.ViewModels;
using FamilyRealtor.Data;
using FamilyRealtor.Helpers;
using FamilyRealtor.Models;

namespace FamilyRealtor.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "администратор")]
	public class UserController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly UserManager<User> _userMngr;
		readonly RoleManager<IdentityRole> _roleMngr;
		readonly IConfiguration _conf;

		public UserController(RealtorContext ctx, UserManager<User> userMngr, RoleManager<IdentityRole> roleMngr, IConfiguration conf)
		{
			_ctx = ctx;
			_userMngr = userMngr;
			_roleMngr = roleMngr;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/[controller]s")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			ViewData["PageCount"] = await _userMngr.Users.CountPagesAsync(_pageSize);
			List<User> users = await _userMngr.Users.GetPage(page, _pageSize).ToListAsync();

			foreach (User user in users)
			{
				user.RoleNames = await _userMngr.GetRolesAsync(user);
			}

			ViewData["Roles"] = await _roleMngr.Roles.OrderBy(r => r.Name).ToListAsync();
			return View(users);
		}

		[HttpGet]
		[Route("/[area]/[controller]s/[action]")]
		public async Task<IActionResult> SearchAsync(string? sEmail, string? sRole, int page = 1)
		{
			IQueryable<User> query = _userMngr.Users;
			if (sEmail != null) query = query.Where(u => u.Email!.ToLower().Contains(sEmail.ToLower()));
			List<User> users = await query.ToListAsync();

			foreach (User user in users)
			{
				user.RoleNames = await _userMngr.GetRolesAsync(user);
			}

			if (sRole != null) users = users.Where(u => u.RoleNames!.Contains(sRole)).ToList();

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			users = users.Skip((page - 1) * _pageSize).Take(_pageSize).ToList();

			ViewData["Roles"] = await _roleMngr.Roles.OrderBy(r => r.Name).ToListAsync();
			return View("Index", users);
		}

		[HttpGet]
		public async Task<IActionResult> CreateAsync()
		{
			ViewData["Roles"] = await _roleMngr.Roles.OrderBy(r => r.Name).ToListAsync();
			return View("Form");
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(UserViewModel vm)
		{
			List<IdentityResult> errorResults = new();

			if (ModelState.IsValid)
			{
				User user = new() { UserName = vm.Email, Email = vm.Email };

				if (vm.Password != null)
				{
					IdentityResult userResult = await _userMngr.CreateAsync(user, vm.Password);
					if (!userResult.Succeeded) errorResults.Add(userResult);
				}

				if (errorResults.Count == 0 && vm.Roles != null)
				{
					foreach (string role in vm.Roles)
					{
						IdentityResult roleResult = await _userMngr.AddToRoleAsync(user, role);
						if (!roleResult.Succeeded) errorResults.Add(roleResult);
					}
				}
			}

			foreach (IdentityResult result in errorResults)
			{
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}

			if (ModelState.IsValid)
			{
				await _ctx.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			ViewData["Roles"] = await _roleMngr.Roles.OrderBy(r => r.Name).ToListAsync();
			return View("Form", vm);
		}

		[HttpGet]
		public async Task<IActionResult> UpdateAsync(string id)
		{
			User? user = await _userMngr.FindByIdAsync(id);
			if (user == null || user.Email == null) return NotFound();

			IList<string> roles = await _userMngr.GetRolesAsync(user);
			UserViewModel vm = new() { Id = user.Id, Email = user.Email, Roles = roles };

			ViewData["Roles"] = await _roleMngr.Roles.OrderBy(r => r.Name).ToListAsync();
			return View("Form", vm);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAsync(UserViewModel vm)
		{
			if (vm.Id == null) return BadRequest();

			User? user = await _userMngr.FindByIdAsync(vm.Id);
			if (user == null) return View("Form", vm);

			List<IdentityResult> errorResults = new();

			if (ModelState.IsValid)
			{
				user.Email = vm.Email;

				if (vm.Password != null)
				{
					string token = await _userMngr.GeneratePasswordResetTokenAsync(user);
					IdentityResult passwordResult = await _userMngr.ResetPasswordAsync(user, token, vm.Password);
					if (!passwordResult.Succeeded) errorResults.Add(passwordResult);
				}

				IList<string> currentRoles = await _userMngr.GetRolesAsync(user);

				if (vm.Roles != currentRoles) await _userMngr.RemoveFromRolesAsync(user, currentRoles);

				if (vm.Roles != null)
				{
					foreach (string role in vm.Roles)
					{
						IdentityResult roleResult = await _userMngr.AddToRoleAsync(user, role);
						if (!roleResult.Succeeded) errorResults.Add(roleResult);
					}
				}
			}

			foreach (IdentityResult result in errorResults)
			{
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}

			if (ModelState.IsValid)
			{
				IdentityResult result = await _userMngr.UpdateAsync(user);

				if (result.Succeeded)
				{
					await _ctx.SaveChangesAsync();
					return RedirectToAction("Index");
				}
				else
				{
					foreach (IdentityError error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
			}

			ViewData["Roles"] = await _roleMngr.Roles.OrderBy(r => r.Name).ToListAsync();
			return View("Form", vm);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(string id)
		{
			User? user = await _userMngr.FindByIdAsync(id);
			if (user == null) return NotFound();
			
			await _userMngr.DeleteAsync(user);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "User", new { Area = "Admin" }));
		}
	}
}