using GRMP.Classes;
using GRMP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProjBancoDados.BancoDados;
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

        public IActionResult AlterarOSExibir(int id)
        {
            string idUsuario =
                HttpContext.Session.GetString("idUsuario");

            //-----------------------------------
            // VALIDAR LOGIN
            //-----------------------------------

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction(
                    "Login",
                    "Login"
                );
            }

            //-----------------------------------
            // VALIDAR NÍVEL
            //-----------------------------------

            Usuario us =
                new Usuario();

            DataTable dtUsuario =
                us.BuscarPorID(
                    int.Parse(idUsuario)
                );

            foreach (DataRow dr in dtUsuario.Rows)
            {
                int nvAcesso =
                    Convert.ToInt32(
                        dr["nvAcesso"]
                    );

                if (nvAcesso == 1)
                {
                    return RedirectToAction(
                        "InicioExibir",
                        "Usuario"
                    );
                }

                else if (nvAcesso == 2)
                {
                    return RedirectToAction(
                        "InicioNivelDoisMapa",
                        "Usuario"
                    );
                }
            }

            //-----------------------------------
            // BUSCAR OS
            //-----------------------------------

            Os os =
                new Os();

            DataTable dtOS =
                os.BuscarPorId(id);

            //-----------------------------------
            // VALIDAR EXISTÊNCIA
            //-----------------------------------

            if (dtOS == null ||
                dtOS.Rows.Count == 0)
            {
                return RedirectToAction(
                    "InicioExibir",
                    "Usuario"
                );
            }

            //-----------------------------------
            // MODEL
            //-----------------------------------

            OrdemServicoViewModel osVm =
                new OrdemServicoViewModel();

            DataRow linha =
                dtOS.Rows[0];

            //-----------------------------------
            // PREENCHER MODEL
            //-----------------------------------

            osVm.IdOrdemServico =
                Convert.ToInt32(
                    linha["idOrdemServico"]
                );

            osVm.FkIdUsuario =
                Convert.ToInt32(
                    linha["fk_idUsuario"]
                );

            if (linha["fk_executor"] != DBNull.Value)
            {
                osVm.FkExecutor =
                    Convert.ToInt32(
                        linha["fk_executor"]
                    );
            }

            osVm.DescricaoServico =
                linha["descricaoServico"]
                .ToString();

            osVm.Categoria =
                Convert.ToInt32(
                    linha["categoria"]
                );

            osVm.NumeroPatrimonio =
                linha["numeroPatrimonio"]
                .ToString();

            osVm.Bloco =
                Convert.ToInt32(
                    linha["bloco"]
                );

            osVm.Local =
                Convert.ToInt32(
                    linha["local"]
                );

            if (linha["prioridade"] != DBNull.Value)
            {
                osVm.Prioridade =
                    Convert.ToInt32(
                        linha["prioridade"]
                    );
            }

            osVm.Observacoes =
                linha["observacoes"]
                .ToString();

            osVm.DataSolicitacao =
                Convert.ToDateTime(
                    linha["dataSolicitacao"]
                );

            if (linha["dataInicio"] != DBNull.Value)
            {
                osVm.DataInicio =
                    Convert.ToDateTime(
                        linha["dataInicio"]
                    );
            }

            if (linha["dataFinalizacao"] != DBNull.Value)
            {
                osVm.DataFinalizacao =
                    Convert.ToDateTime(
                        linha["dataFinalizacao"]
                    );
            }

            if (linha["status"] != DBNull.Value)
            {
                osVm.Status =
                    Convert.ToInt32(
                        linha["status"]
                    );
            }

            osVm.Ativo =
                Convert.ToBoolean(
                    linha["ativo"]
                );

            //-----------------------------------
            // DROPDOWN BLOCO
            //-----------------------------------

            osVm.DtBlocos =
                BuscarBlocos();
            osVm.DtLocais =
                BuscarLocaisPorBlocoAlterado(osVm.Bloco);


            //-----------------------------------
            // VIEW
            //-----------------------------------

            return View(
                "AlterarOSExibirView",
                osVm
            );
        }

      
        [HttpPost]
        public IActionResult AlterarOSProcessar(OrdemServicoViewModel OsVM)
        {
            try
            {
                //---------------------------------
                // Verifica sessão
                //---------------------------------

                string idUsuario =
                    HttpContext.Session.GetString(
                        "idUsuario"
                    );

                if (string.IsNullOrEmpty(idUsuario))
                {
                    return RedirectToAction(
                        "Login",
                        "Login"
                    );
                }

                //---------------------------------
                // Cria objeto OS
                //---------------------------------

                Os os =
                    new Os();

                //---------------------------------
                // ID DA OS
                //---------------------------------

                os.idOrdemServico =
                    OsVM.IdOrdemServico;

                //---------------------------------
                // Usuário criador
                //---------------------------------

                os.fk_idUsuario =
                    OsVM.FkIdUsuario;

                //---------------------------------
                // Executor
                //---------------------------------

                os.fk_executor =
                    OsVM.FkExecutor;

                //---------------------------------
                // Descrição
                //---------------------------------

                os.descricaoServico =
                    OsVM.DescricaoServico;

                //---------------------------------
                // Categoria
                //---------------------------------

                os.categoria =
                    OsVM.Categoria;

                //---------------------------------
                // Patrimônio
                //---------------------------------

                os.numeroPatrimonio =
                    string.IsNullOrEmpty(
                        OsVM.NumeroPatrimonio
                    )
                    ? null
                    : OsVM.NumeroPatrimonio;

                //---------------------------------
                // Localização
                //---------------------------------

                os.bloco =
                    OsVM.Bloco;

                os.local =
                    OsVM.Local;

                //---------------------------------
                // Prioridade
                //---------------------------------

                os.prioridade =
                    OsVM.Prioridade;

                //---------------------------------
                // Observações
                //---------------------------------

                os.observacoes =
                    OsVM.Observacoes;

                //---------------------------------
                // Datas
                //---------------------------------

                os.dataSolicitacao =
                    OsVM.DataSolicitacao;

                os.dataInicio =
                    OsVM.DataInicio;

                os.dataFinalizacao =
                    OsVM.DataFinalizacao;

                //---------------------------------
                // Status
                //---------------------------------

                os.status =
                    OsVM.Status;

                //---------------------------------
                // Ativo
                //---------------------------------

                os.ativo =
                    OsVM.Ativo;

                //---------------------------------
                // ALTERAR
                //---------------------------------

                os.Alterar();

                //---------------------------------
                // REDIRECIONA
                //---------------------------------

                return RedirectToAction(
                    "InicioExibir"
                );
            }
            catch (Exception ex)
            {
                OrdemServicoViewModel model =
                    new OrdemServicoViewModel();

                model.DtBlocos =
                    BuscarBlocos();

                ViewBag.Erro =
                    ex.Message;

                return View(
                    "AlterarOSExibirView",
                    model
                );
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

        //---------------------------------
        // Buscar locais por bloco
        //---------------------------------
        [HttpGet]
        public DataTable BuscarLocaisPorBlocoAlterado(int idBloco)
        {
            try
            {
                Local Local = new Local();


                Local.fk_idBloco = idBloco;

                return Local.BuscarLocaisPorBloco();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
