using GRMP.Classes;
using GRMP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjBancoDados.BancoDados;
using System.Data;

namespace GRMP.Controllers
{
    public class GerenciarUsuariosController : Controller
    {
        public IActionResult ListaUsuariosExibir()
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("LoginExibir", "Login");
            }

            Usuario Us = new Usuario();

            DataTable dt = Us.BuscarPorID(int.Parse(idUsuario));

            foreach (DataRow dr in dt.Rows)
            {
                int nvAcesso = Convert.ToInt32(dr["nvAcesso"]);

                if (nvAcesso == 3)
                {
                    break;
                }
                else if (nvAcesso == 2)
                {
                    return RedirectToAction("Index", "Mapa");
                }
                else
                {
                    return RedirectToAction("ListarOSExibir", "Usuario");
                }
            }

            dt = Us.SelecionarSeguro();

            return View("ListaUsuarioExibirView", dt);
        }

        public IActionResult CriarUsuarioExibir()
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("LoginExibir", "Login");
            }

            return View("CriarUsuarioExibirView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarUsuarioProcessar(UsuarioViewModel USVM)
        {
            try
            {
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

                var passwordHasher = new PasswordHasher<Usuario>();

                Us.senha = passwordHasher.HashPassword(
                    Us,
                    "Senha123"
                );

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
                    "CriarUsuarioExibirView",
                    USVM
                );
            }
        }

        public IActionResult AlterarUsuarioExibir(int id)
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("LoginExibir", "Login");
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

                // NÃO envie hash para a view
                USVM.Senha = "";

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
                if (!ModelState.IsValid)
                {
                    return View(
                        "AlterarUsuarioExibirView",
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

                var passwordHasher = new PasswordHasher<Usuario>();

                Us.senha = passwordHasher.HashPassword(
                    Us,
                    USVM.Senha
                );

                Us.nvAcesso = USVM.NvAcesso;

                //-----------------------------------
                // ALTERAR
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
                    "AlterarUsuarioExibirView",
                    USVM
                );
            }
        }
        public IActionResult AlterarSenhaExibir()
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("LoginExibir", "Login");
            }

            UsuarioViewModel USVM = new UsuarioViewModel();

            Usuario Us = new Usuario();

            DataTable dt = Us.BuscarPorID(int.Parse(idUsuario));

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                USVM.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                USVM.Nome = Convert.ToString(dr["Nome"]);
                USVM.Email = Convert.ToString(dr["Email"]);

                // NÃO envie hash para a view
                USVM.Senha = "";
            }

            return View("AlterarSenhaExibirView", USVM);
        }
        public IActionResult AlterarSenhaProcessar(UsuarioViewModel USVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction(
                        "ListarOSExibir", "Usuario",
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

                var passwordHasher = new PasswordHasher<Usuario>();

                Us.senha = passwordHasher.HashPassword(
                    Us,
                    USVM.Senha
                );

                Us.nvAcesso = int.Parse(HttpContext.Session.GetString("nvAcesso"));

                //-----------------------------------
                // ALTERAR
                //-----------------------------------

                Us.Alterar();

                //-----------------------------------
                // REDIRECIONAR
                //-----------------------------------

                return RedirectToAction(
                    "ListarOSExibir","Usuario"
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                return RedirectToAction(
                    "ListarOSExibir", "Usuario",
                    USVM
                );
            }
        }
    }
}