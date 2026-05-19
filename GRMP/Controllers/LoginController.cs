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
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View("LoginView");
        }



        public IActionResult LoginProcessar(LoginViewModel Vm_Login)
        {
            string senhaHash = GerarHash(Vm_Login.Senha);

            try
            {
                //string senhaHash = GerarHash(Vm_Login.Senha);
 
                Usuario usuario = new Usuario();
                DataTable dt = usuario.BuscarPorEmail(Vm_Login.Email);

                if (dt != null)
                {
                    DataRow row = dt.Rows[0];

                    if (row["Senha"].ToString() == senhaHash)
                    {
                        HttpContext.Session.SetString("idUsuario", row["idUsuario"].ToString());

                        string nvAcesso = row["nvAcesso"].ToString();

                        if (nvAcesso == "3")
                        {

                        }
                        else if (nvAcesso == "2")
                        {

                        }
                        else
                        {
                            return RedirectToAction("InicioExibir", "Usuario");
                        }
                    }
                }

                ViewBag.Erro = "Email ou senha inválidos.";
                return View("LoginView");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = ex.Message;
                return View("LoginView");
            }
        }

        private string GerarHash(string senha)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
