using GRMP.Classes;
using GRMP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProjBancoDados.BancoDados;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace GRMP.Controllers
{
    public class OrdemServicoController : Controller
    {
        private readonly IConfiguration _configuration;

        public OrdemServicoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult ListarOSExibir()
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("LoginExibir", "Login");
            }

            Os Os = new Os();
            DataTable dt = Os.SelecionarOS();

            return View("ListarOSExibirView" , dt);// teste 234
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
                    return RedirectToAction("LoginExibir", "Login");
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

                //---------------------------------
                // Inserir
                //---------------------------------

                os.Inserir();

                //---------------------------------
                // Redireciona
                //---------------------------------

                return RedirectToAction("ListarOSExibir");
            }
            catch (Exception ex)
            {
                OrdemServicoViewModel model = new OrdemServicoViewModel();

                model.DtBlocos = BuscarBlocos();

                ViewBag.Erro = ex.Message;

                return View("CriarOSExibirView", model);
            }
        }

        public DataTable BuscarExecutores()
        {
            Usuario usuario = new Usuario();

            DataTable dtUsuarios = usuario.SelecionarSeguro();

            DataTable dtExecutores = dtUsuarios.Clone();

            foreach (DataRow row in dtUsuarios.Rows)
            {
                if (Convert.ToInt32(row["nvAcesso"]) == 2)
                {
                    dtExecutores.ImportRow(row);
                }
            }

            return dtExecutores;
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
                    "LoginExibir",
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
                        "ListarOSExibir",
                        "OrdemServico"
                    );
                }

                else if (nvAcesso == 2)
                {
                    return RedirectToAction(
                        "InicioNivelDoisMapa",
                        "OrdemServico"
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
                    "ListarOSExibir",
                    "OrdemServico"
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

            //-----------------------------------
            // DROPDOWN BLOCO
            //-----------------------------------

            osVm.DtBlocos =
                BuscarBlocos();
            osVm.DtLocais =
                BuscarLocaisPorBlocoAlterado(osVm.Bloco);
            osVm.DtExecutores = BuscarExecutores();


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
                        "LoginExibir",
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
                // ALTERAR
                //---------------------------------

                os.Alterar();

                //---------------------------------
                // REDIRECIONA
                //---------------------------------

                return RedirectToAction(
                    "ListarOSExibir"
                );
            }
            catch (Exception ex)
            {
                OrdemServicoViewModel model =
                    new OrdemServicoViewModel();

                model.DtBlocos =
                    BuscarBlocos();

                model.DtLocais =
    BuscarLocaisPorBlocoAlterado(
        OsVM.Bloco
    );

                model.DtExecutores =
                    BuscarExecutores();

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

        //<<<<<<< HEAD
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
            //=======
        public IActionResult BaixarOSWord(int id)
        {
            string connStr = _configuration.GetConnectionString("StringConexaoSQLServer");

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                SELECT
                    os.*,

                    b.nome AS nomeBloco,
                    l.nome AS nomeLocal,

                    uc.nome AS nomeCriador,
                    uc.email AS emailCriador,

                    ue.nome AS nomeExecutor,
                    ue.email AS emailExecutor

                FROM OrdemServico os

                LEFT JOIN Bloco b
                    ON b.idBloco = os.Bloco

                LEFT JOIN Local l
                    ON l.idLocal = os.Local

                LEFT JOIN Usuario uc
                    ON uc.idUsuario = os.fk_idUsuario

                LEFT JOIN Usuario ue
                    ON ue.idUsuario = os.fk_executor

                WHERE os.idOrdemServico = @id
                ";

                using SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", id);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                return NotFound();
            }

            DataRow os = dt.Rows[0];

            string caminho = Path.Combine(Path.GetTempPath(), $"OS_{id}.docx");

            using (var document = DocX.Create(caminho))
            {
                string logoPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "img",
                    "senai-logo.png"
                );

                var imagem = document.AddImage(logoPath);

                var picture = imagem.CreatePicture();

                picture.Width = 140;
                picture.Height = 60;

                var pLogo = document.InsertParagraph();

                pLogo.AppendPicture(picture);

                pLogo.Alignment = Alignment.center;

                document.InsertParagraph("");

                var titulo = document.InsertParagraph();

                titulo.Append("ORDEM DE SERVIÇO");

                titulo.Bold();

                titulo.FontSize(22);

                titulo.Alignment = Alignment.center;

                titulo.SpacingAfter(20d);

                document.InsertParagraph("INFORMAÇÕES GERAIS")
                .Bold()
                .FontSize(16);

                document.InsertParagraph("-----------------------------------");

                document.InsertParagraph("");

                document.InsertParagraph($"ID: {os["idOrdemServico"]}");

                document.InsertParagraph($"Descrição: {os["descricaoServico"]}");

                document.InsertParagraph($"Categoria: {os["categoria"]}");

                document.InsertParagraph($"Número Patrimônio: {os["numeroPatrimonio"]}");

                var p = document.InsertParagraph();

                p.Append("Bloco: ").Bold();

                p.Append(os["nomeBloco"].ToString());

                document.InsertParagraph($"Local: {os["nomeLocal"]}");

                document.InsertParagraph($"Prioridade: {os["prioridade"]}");

                document.InsertParagraph($"Observações: {os["observacoes"]}");

                document.InsertParagraph($"Status: {os["status"]}");

                document.InsertParagraph($"Data Solicitação: {os["dataSolicitacao"]}");

                document.InsertParagraph($"Data Início: {os["dataInicio"]}");

                document.InsertParagraph($"Data Finalização: {os["dataFinalizacao"]}");

                document.InsertParagraph("");

                document.InsertParagraph("SOLICITANTE")
                    .Bold();

                document.InsertParagraph($"Nome: {os["nomeCriador"]}");

                document.InsertParagraph($"Email: {os["emailCriador"]}");

                document.InsertParagraph("");

                document.InsertParagraph("EXECUTOR")
                    .Bold();

                document.InsertParagraph($"Nome: {os["nomeExecutor"]}");

                document.InsertParagraph($"Email: {os["emailExecutor"]}");

                document.AddFooters();

                document.Footers.Odd.InsertParagraph(
                    "Sistema de Gerenciamento de Manutenção Predial - SENAI"
                );

                document.Save();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(caminho);

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"OS_{id}.docx"
            );
        }
    }
}
