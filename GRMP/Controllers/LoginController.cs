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
        public IActionResult LoginExibir()
        {
            return View("LoginExibirView");
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
                        HttpContext.Session.SetString("nvAcesso", row["nvAcesso"].ToString());

                        string nvAcesso = row["nvAcesso"].ToString();

                        if (nvAcesso == "3" || nvAcesso == "2")
                        {
                            return RedirectToAction("MapaExibir", "Mapa");
                        }
                        else
                        {
                            return RedirectToAction("ListarOSExibir", "Usuario");
                        }
                    }
                }

                ViewBag.Erro = "Email ou senha inválidos." + senhaHash;
                return View("LoginExibirView");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = ex.Message;
                return View("LoginExibirView");
            }
        }

        public IActionResult SairProcessar()
        {
            HttpContext.Session.Clear();

            return View("LoginExibirView");
        }
        private string GerarHash(string senha)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
