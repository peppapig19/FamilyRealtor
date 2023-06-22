using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FamilyRealtor.Data;
using FamilyRealtor.Models;
using FamilyRealtor.Models.DomainModels;
using FamilyRealtor.Models.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace FamilyRealtor.Controllers
{
	public class AccountController : Controller
	{
		readonly RealtorContext _ctx;
		readonly UserManager<User> _userMngr;
		readonly SignInManager<User> _signInMngr;

		public AccountController(RealtorContext ctx, UserManager<User> userMngr, SignInManager<User> signInMngr)
		{
			_ctx = ctx;
			_userMngr = userMngr;
			_signInMngr = signInMngr;
		}

		[HttpGet]
		[Authorize]
		[Route("/[controller]")]
		public async Task<IActionResult> Index()
		{
			Client? client = null;

			if (_signInMngr.IsSignedIn(User))
			{
				User? user = await _userMngr.GetUserAsync(User);
				client = user?.Client;
			}

			if (client == null) return Unauthorized();

			AccountViewModel vm = new()
			{
				FirstName = client.FirstName,
				LastName = client.LastName,
				Patronymic = client.Patronymic,
				Phone = client.Phone
			};

			return View(vm);
		}

		[HttpPost]
		[Authorize]
		[Route("/[controller]")]
		public async Task<IActionResult> Index(AccountViewModel vm)
		{
			User? user = null;
			Client? client = null;

			if (_signInMngr.IsSignedIn(User))
			{
				user = await _userMngr.GetUserAsync(User);
				client = user?.Client;
			}

			if (user == null || client == null) return Unauthorized();

			if (ModelState.IsValid)
			{
				if (vm.NewPassword != null)
				{
					IdentityResult result = await _userMngr.ChangePasswordAsync(user, vm.OldPassword!, vm.NewPassword);

					if (!result.Succeeded)
					{
						foreach (IdentityError error in result.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}

						return View(vm);
					}
				}

				client.FirstName = vm.FirstName;
				client.LastName = vm.LastName;
				client.Patronymic = vm.Patronymic;
				client.Phone = vm.Phone;

				_ctx.Clients.Update(client);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			return View(vm);
		}

		[HttpGet]
		[Route("/[action]")]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[Route("/[action]")]
		public async Task<IActionResult> Register(RegisterViewModel client)
		{
			if (ModelState.IsValid)
			{
				User user = new() { UserName = client.Email, Email = client.Email };
				IdentityResult result = await _userMngr.CreateAsync(user, client.Password!);

				if (result.Succeeded)
				{
					client.User = user;
					_ctx.Clients.Add(client);
					await _ctx.SaveChangesAsync();

					await _signInMngr.SignInAsync(user, isPersistent: false);
					return RedirectToAction("Index", "Home");
				}
				else
				{
					foreach (IdentityError error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
			}

			return View(client);
		}

		[HttpGet]
		[Route("/[action]")]
		public IActionResult Login(string? ReturnUrl)
		{
			LoginViewModel vm = new() { ReturnUrl = ReturnUrl };
			return View(vm);
		}

		[HttpPost]
		[Route("/[action]")]
		public async Task<IActionResult> Login(LoginViewModel vm, string? ReturnUrl)
		{
			if (ModelState.IsValid)
			{
				SignInResult result = await _signInMngr.PasswordSignInAsync(vm.Email, vm.Password,
					isPersistent: vm.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)) return Redirect(ReturnUrl);
					else return RedirectToAction("Index", "Home");
				}
			}

			ModelState.AddModelError("", "Неправильные электронная почта или пароль.");
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInMngr.SignOutAsync();
			HttpContext.Session.Clear();

			return RedirectToAction("Index", "Home");
		}
	}
}