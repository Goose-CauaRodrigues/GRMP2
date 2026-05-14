using Microsoft.AspNetCore.Mvc;
using GRMP.Classes;
using GRMP.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace GRMP.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View("LoginView");
        }

        public IActionResult LoginProcessar(LoginViewModel Vm_Login)
        {

            
            return View();
        }

        private string GerarHash(string senha)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
