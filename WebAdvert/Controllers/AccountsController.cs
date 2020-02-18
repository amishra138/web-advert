using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAdvert.Models.Accounts;

namespace WebAdvert.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;
        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SignUp()
        {
            var model = new SignUpModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _pool.GetUser(model.Email);
                    if (user.Status != null)
                    {
                        ModelState.AddModelError("UserExists", "User with same email already exists");
                        return View(model);
                    }

                    user.Attributes.Add("Name", model.Email);
                    var createdUser = await _userManager.CreateAsync(user, model.Password);

                    if (createdUser.Succeeded)
                    {
                        RedirectToAction("Confirm");
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            return View("SignUp");
        }

        public async Task<IActionResult> Confirm()
        {
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "User with given email was not found");
                    return View(model);
                }

                var result = await _userManager.ConfirmEmailAsync(user, model.Code);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }

                    return View(model);
                }
            }
            return View(model);
        }
    }
}
