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
            // Verificação se o usuário está logado
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            Usuario Us = new Usuario();

            // Busca dados do usuário logado
            DataTable dt = Us.BuscarPorID(int.Parse(idUsuario));

            foreach (DataRow dr in dt.Rows)
            {
                int nvAcesso = Convert.ToInt32(dr["nvAcesso"]);

                // Se for nível 1 -> redireciona
                if (nvAcesso == 1)
                {
                    return RedirectToAction("InicioNivelDoisMapa", "Usuario");
                }

                // Se for nível 3 -> continua normalmente
                if (nvAcesso == 3)
                {
                    break;
                }

                // Qualquer outro nível sem permissão
                return RedirectToAction("Login", "Login");
            }

            // Carrega lista de usuários
            dt = Us.SelecionarSeguro();

            return View("ListaUsuariosExibirView", dt);
        }
    }
}
