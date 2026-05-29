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

                // Se for nível 3 -> continua normalmente
                if (nvAcesso == 3)
                {
                    break;
                }
                // Se for nível 1 -> redireciona
                else if(nvAcesso == 2)
                {
                    return RedirectToAction("Index", "Mapa");


                }
                else
                {
                    return RedirectToAction("InicioExibir", "Usuario");

                }
            }

            // Carrega lista de usuários
            dt = Us.SelecionarSeguro();

            return View("ListaUsuarioExibirView", dt);
        }

        public IActionResult CriarUsuarioExibir()
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Login");
            }
            return View("CriarUsuarioExibirView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarUsuarioProcessar(UsuarioViewModel USVM)
        {
            try
            {
                //-----------------------------------
                // VALIDAR MODEL
                //-----------------------------------

                if (!ModelState.IsValid)
                {
                    return View(
                        "CriarUsuarioExibirView",
                        USVM
                    );
                }
                
                Usuario Us = new Usuario();

                //-----------------------------------
                // PREENCHER
                //-----------------------------------

                

                Us.nome = USVM.Nome;

                Us.email = USVM.Email;

                Us.senha = GerarHash("Senha123");

                Us.nvAcesso = USVM.NvAcesso;

                //-----------------------------------
                // INSERIR
                //-----------------------------------

                Us.Inserir();

                //-----------------------------------
                // REDIRECIONAR
                //-----------------------------------

                return RedirectToAction(
                    "ListaUsuariosExibir"
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                return View(
                    "CriarUsuarioExibir",
                    USVM
                );
            }
        }

        public IActionResult AlterarUsuarioExibir(int id)
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            UsuarioViewModel USVM = new UsuarioViewModel();

            Usuario Us = new Usuario();

            DataTable dt = Us.BuscarPorID(id);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                USVM.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                USVM.Nome = Convert.ToString(dr["Nome"]);
                USVM.Email = Convert.ToString(dr["Email"]);
                USVM.Senha = Convert.ToString(dr["Senha"]);
                USVM.NvAcesso = Convert.ToInt32(dr["NvAcesso"]);
            }

            return View("AlterarUsuarioExibirView", USVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlterarUsuarioProcessar(UsuarioViewModel USVM)
        {
            try
            {
                //-----------------------------------
                // VALIDAR MODEL
                //-----------------------------------

                if (!ModelState.IsValid)
                {
                    return View(
                        "CriarUsuarioExibirView",
                        USVM
                    );
                }

                Usuario Us = new Usuario();

                //-----------------------------------
                // PREENCHER
                //-----------------------------------

                Us.idUsuario = USVM.IdUsuario;

                Us.nome = USVM.Nome;

                Us.email = USVM.Email;

                Us.senha = GerarHash(USVM.Senha);

                Us.nvAcesso = USVM.NvAcesso;

                //-----------------------------------
                // INSERIR
                //-----------------------------------

                Us.Alterar();

                //-----------------------------------
                // REDIRECIONAR
                //-----------------------------------

                return RedirectToAction(
                    "ListaUsuariosExibir"
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                return View(
                    "CriarUsuarioExibir",
                    USVM
                );
            }
        }
        private string GerarHash(string senha)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
