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

        public IActionResult CriarOSExibir()
        {
            OrdemServicoViewModel OsVm = new OrdemServicoViewModel();
            OsVm.DtBlocos = BuscarBlocos();

            return View("CriarOSExibirView", OsVm);
        }

        public IActionResult CriarOSProcessar(OrdemServicoViewModel OsVM)
        {
            



            return View("CriarOSExibirView");
        }

        public DataTable BuscarBlocos()
        {
            try
            {
                Bloco bloco = new Bloco();

                return bloco.Selecionar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //---------------------------------
        // Buscar locais por bloco
        //---------------------------------
        [HttpGet]
        public JsonResult BuscarLocaisPorBloco(int idBloco)
        {
            try
            {
                Local local = new Local();

                local.fk_idBloco = idBloco;

                DataTable dt = local.BuscarLocaisPorBloco();

                List<object> lista = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new
                    {
                        idLocal = row["idLocal"],
                        nome = row["nome"]
                    });
                }

                return Json(lista);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    erro = ex.Message
                });
            }
        }

    }
}
