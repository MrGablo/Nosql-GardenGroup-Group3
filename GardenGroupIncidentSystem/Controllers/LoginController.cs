using GardenGroupIncidentSystem.Models;
using GardenGroupIncidentSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GardenGroupIncidentSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthenticationService _authService;
        //private readonly EmployeeService _employeeService;

        public LoginController(AuthenticationService authService)
        {
            _authService = authService;
            //_employeeService = employeeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel loginModel)
        {
            try
            {
                var employee = _authService.Authenticate(loginModel.UserName, loginModel.Password);

                if (employee == null)
                {
                    ViewBag.ErrorMessage = "Invalid username or password!";
                    return View("Index", loginModel);
                }

                HttpContext.Session.SetObject("LoggedInUser", employee);

                // Redirect based on role
                if (employee.EmployeeRole == Role.regular)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return RedirectToAction("Index", "Employee");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("Index", loginModel);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

    }
}
