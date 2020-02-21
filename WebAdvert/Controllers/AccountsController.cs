using Amazon.AspNetCore.Identity.Cognito;
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
                var user = _pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with same email already exists");
                    return View(model);
                }

                user.Attributes.Add("name", model.Email);
                user.Attributes.Add("email", model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password);

                if (createdUser.Succeeded)
                {
                    return RedirectToAction("Confirm");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> Confirm_post(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null)
                    {
                        ModelState.AddModelError("NotFound", "User with given email was not found");
                        return View(model);
                    }

                    var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);

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
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            return View("Confirm", model);
        }

        [HttpGet]
        public async Task<IActionResult> Login(LoginModel model)
        {
            return View(model);
        }

        [ActionName("Login")]
        [HttpPost]
        public async Task<IActionResult> Login_post(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var loggedInUser = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (loggedInUser.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("LoginError", "Email and password do not match");
                }
            }
            return View("Login", model);
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            return View(model);
        }

        [ActionName("ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> ForgotPasswordPost(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "User with given email was not found");
                    return View("ForgotPassword", model);
                }
                await user.ForgotPasswordAsync();
                return RedirectToAction("ResetPassword");
            }
            return View("ForgotPassword", model);
        }


        [HttpGet]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            return View(model);
        }

        [ActionName("ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPasswordPost(ResetPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "User with given email was not found");
                    return View("ForgotPassword", model);
                }
                var result = user.ConfirmForgotPasswordAsync(model.Code, model.Password);

                return View("Home", "Index");
            }
            return View("ResetPassword", model);
        }
    }
}
