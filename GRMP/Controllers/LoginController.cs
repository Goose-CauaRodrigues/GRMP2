using GRMP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjBancoDados.BancoDados;
using System.Data;

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
            try
            { 
                Usuario usuario = new Usuario();
                DataTable dt = usuario.BuscarPorEmail(Vm_Login.Email);

                if (dt != null)
                {

                    DataRow row = dt.Rows[0];


                    var passwordHasher = new PasswordHasher<Usuario>();

                    string senhaBanco = row["Senha"].ToString();

                    var resultado = passwordHasher.VerifyHashedPassword(
                        null,
                        senhaBanco,
                        Vm_Login.Senha
                    );

                    if (resultado == PasswordVerificationResult.Success)
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
                            return RedirectToAction("ListarOSExibir", "OrdemServico");
                        }
                    }                    
                }

                ViewBag.Erro = "Email ou senha inválidos.";
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
    }
}
