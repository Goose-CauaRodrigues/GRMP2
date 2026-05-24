using GRMP.Classes;
using GRMP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProjBancoDados.BancoDados;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GRMP.Controllers
{
    public class GerenciarUsuariosController : Controller
    {
        public IActionResult ListaUsuariosExibir()
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");
            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Login");
            }
            

            Usuario Us = new Usuario();

            Us.idUsuario = int.Parse(idUsuario);
            DataTable dt = Us.lece




            dt = Us.SelecionarSeguro();

            return View("ListaUsuariosExibirView" , dt);
        }
    }
}
