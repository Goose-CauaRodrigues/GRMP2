using GRMP.Classes;
using GRMP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace GRMP.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _configuration;

        public UsuarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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

                document.InsertParagraph($"Ativo: {os["ativo"]}");

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
