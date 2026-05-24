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

        [HttpPost]
        public IActionResult CriarOSProcessar(OrdemServicoViewModel OsVM)
        {
            try
            {
                //---------------------------------
                // Verifica sessão
                //---------------------------------

                string idUsuario = HttpContext.Session.GetString("idUsuario");

                if (string.IsNullOrEmpty(idUsuario))
                {
                    return RedirectToAction("Login", "Login");
                }

                //---------------------------------
                // Cria objeto OS
                //---------------------------------

                Os os = new Os();

                os.fk_idUsuario = Convert.ToInt32(idUsuario);

                os.descricaoServico = OsVM.DescricaoServico;

                //---------------------------------
                // Categoria
                //---------------------------------

                os.categoria = OsVM.Categoria;
                //---------------------------------
                // Patrimônio
                //---------------------------------

                os.numeroPatrimonio = string.IsNullOrEmpty(OsVM.NumeroPatrimonio)
                    ? null
                    : OsVM.NumeroPatrimonio;

                //---------------------------------
                // Localização
                //---------------------------------

                os.bloco = OsVM.Bloco;

                os.local = OsVM.Local;

                //---------------------------------
                // Dados padrão
                //---------------------------------

                os.dataSolicitacao = DateTime.Now;

                os.status = 0;

                os.ativo = true;

                //---------------------------------
                // Inserir
                //---------------------------------

                os.Inserir();

                //---------------------------------
                // Redireciona
                //---------------------------------

                return RedirectToAction("InicioExibir");
            }
            catch (Exception ex)
            {
                OrdemServicoViewModel model = new OrdemServicoViewModel();

                model.DtBlocos = BuscarBlocos();

                ViewBag.Erro = ex.Message;

                return View("CriarOSExibirView", model);
            }
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
