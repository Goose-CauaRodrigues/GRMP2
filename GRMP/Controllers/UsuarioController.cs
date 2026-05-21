using Microsoft.AspNetCore.Mvc;
using GRMP.Classes;
using GRMP.Models;
using Microsoft.AspNetCore.Authentication;
using System.Data;
using System.Text;

namespace GRMP.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult InicioExibir()
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            Os Os = new Os();
            DataTable dt = Os.SelecionarOS();

            return View("InicioView" , dt);// teste 234
        }

    }
}
