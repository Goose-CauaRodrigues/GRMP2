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

            return View("ListarOSExibirView", dt);
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
                // Verifica a sessão.

                string idUsuario = HttpContext.Session.GetString("idUsuario");

                if (string.IsNullOrEmpty(idUsuario))
                {
                    return RedirectToAction("LoginExibir", "Login");
                }

                Os os = new Os();

                os.fk_idUsuario = Convert.ToInt32(idUsuario);
                os.descricaoServico = OsVM.DescricaoServico;
                os.categoria = OsVM.Categoria;
                os.numeroPatrimonio = string.IsNullOrEmpty(OsVM.NumeroPatrimonio) ? null : OsVM.NumeroPatrimonio;
                os.bloco = OsVM.Bloco;
                os.local = OsVM.Local;
                os.dataSolicitacao = DateTime.Now;
                os.status = 0;

                os.Inserir();

                TempData["Sucesso"] = "Ordem de serviço criada com sucesso!";

                return RedirectToAction("ListarOSExibir");
            }
            catch (Exception ex)
            {
                OrdemServicoViewModel model = new OrdemServicoViewModel();

                model.DtBlocos = BuscarBlocos();

                ViewBag.Erro = ex.Message;

                TempData["Erro"] = ex.Message;

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
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            // Valida o Login.

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("LoginExibir", "Login");
            }

            // Valida o Nível.

            Usuario us = new Usuario();

            DataTable dtUsuario = us.BuscarPorID(int.Parse(idUsuario));

            foreach (DataRow dr in dtUsuario.Rows)
            {
                int nvAcesso = Convert.ToInt32(dr["nvAcesso"]);

                if (nvAcesso == 1)
                {
                    return RedirectToAction("ListarOSExibir", "OrdemServico");
                }

                else if (nvAcesso == 2)
                {
                    return RedirectToAction("InicioNivelDoisMapa", "OrdemServico");
                }
            }

            // Busca a OS.

            Os os = new Os();

            DataTable dtOS = os.BuscarPorId(id);

            // Valida a existência.

            if (dtOS == null || dtOS.Rows.Count == 0)
            {
                return RedirectToAction("ListarOSExibir", "OrdemServico");
            }

            OrdemServicoViewModel osVm = new OrdemServicoViewModel();

            DataRow linha = dtOS.Rows[0];

            osVm.IdOrdemServico = Convert.ToInt32(linha["idOrdemServico"]);

            osVm.FkIdUsuario = Convert.ToInt32(linha["fk_idUsuario"]);

            if (linha["fk_executor"] != DBNull.Value)
            {
                osVm.FkExecutor = Convert.ToInt32(linha["fk_executor"]);
            }

            osVm.DescricaoServico = linha["descricaoServico"].ToString();

            osVm.Categoria = Convert.ToInt32(linha["categoria"]);

            osVm.NumeroPatrimonio = linha["numeroPatrimonio"].ToString();

            osVm.Bloco = Convert.ToInt32(linha["bloco"]);

            osVm.Local = Convert.ToInt32(linha["local"]);

            if (linha["prioridade"] != DBNull.Value)
            {
                osVm.Prioridade = Convert.ToInt32(linha["prioridade"]);
            }

            osVm.Observacoes = linha["observacoes"].ToString();

            osVm.DataSolicitacao = Convert.ToDateTime(linha["dataSolicitacao"]);

            if (linha["dataInicio"] != DBNull.Value)
            {
                osVm.DataInicio = Convert.ToDateTime(linha["dataInicio"]);
            }

            if (linha["dataFinalizacao"] != DBNull.Value)
            {
                osVm.DataFinalizacao = Convert.ToDateTime(linha["dataFinalizacao"]);
            }

            if (linha["status"] != DBNull.Value)
            {
                osVm.Status = Convert.ToInt32(linha["status"]);
            }

            osVm.DtBlocos = BuscarBlocos();
            osVm.DtLocais = BuscarLocaisPorBlocoAlterado(osVm.Bloco);
            osVm.DtExecutores = BuscarExecutores();

            return View("AlterarOSExibirView", osVm);
        }

        [HttpPost]
        public IActionResult AlterarOSProcessar(OrdemServicoViewModel OsVM)
        {
            try
            {
                // Verifica a sessão.

                string idUsuario = HttpContext.Session.GetString("idUsuario");

                if (string.IsNullOrEmpty(idUsuario))
                {
                    return RedirectToAction("LoginExibir", "Login");
                }

                Os os = new Os();

                os.idOrdemServico = OsVM.IdOrdemServico;

                os.fk_idUsuario = OsVM.FkIdUsuario;

                os.fk_executor = OsVM.FkExecutor;

                os.descricaoServico = OsVM.DescricaoServico;

                os.categoria = OsVM.Categoria;

                os.numeroPatrimonio = string.IsNullOrEmpty(OsVM.NumeroPatrimonio) ? null : OsVM.NumeroPatrimonio;

                os.bloco = OsVM.Bloco;

                os.local = OsVM.Local;

                os.prioridade = OsVM.Prioridade;

                os.observacoes = OsVM.Observacoes;

                os.dataSolicitacao = OsVM.DataSolicitacao;

                os.dataInicio = OsVM.DataInicio;

                os.dataFinalizacao = OsVM.DataFinalizacao;

                os.status = OsVM.Status;

                os.Alterar();

                TempData["Sucesso"] = "Ordem de serviço atualizada com sucesso!";

                return RedirectToAction("ListarOSExibir");
            }
            catch (Exception ex)
            {
                OrdemServicoViewModel model = new OrdemServicoViewModel();

                model.DtBlocos = BuscarBlocos();

                model.DtLocais = BuscarLocaisPorBlocoAlterado(OsVM.Bloco);

                model.DtExecutores = BuscarExecutores();

                ViewBag.Erro = ex.Message;

                TempData["Erro"] = ex.Message;

                return View("AlterarOSExibirView", model);
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
                    lista.Add(new { idLocal = row["idLocal"], nome = row["nome"] });
                }

                return Json(lista);
            }
            catch (Exception ex)
            {
                return Json(new { erro = ex.Message });
            }
        }

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

        public IActionResult BaixarOSWord(int id)
        {
            Os ordemServico = new Os();

            DataTable dt =
                ordemServico.BuscarDadosWord(id);

            if (dt.Rows.Count == 0)
            {
                return NotFound();
            }

            DataRow os = dt.Rows[0];

            string caminho = Path.Combine(Path.GetTempPath(), $"OS_{id}.docx");

            int status = Convert.ToInt32(os["status"]);

            string statusTexto = status switch
            {
                0 => "Aberto",
                1 => "Em andamento",
                2 => "Concluída",
                3 => "Cancelada",
                4 => "Em pausa",
                _ => "Desconhecido"
            };

            string prioridadeTexto = "Não definida";

            if (os["prioridade"] != DBNull.Value)
            {
                int prioridade = Convert.ToInt32(os["prioridade"]);

                prioridadeTexto = prioridade switch
                {
                    1 => "Baixa",
                    2 => "Média",
                    3 => "Alta",
                    _ => "Não definida"
                };
            }

            int categoria = Convert.ToInt32(os["categoria"]);

            string categoriaTexto = categoria switch
            {
                1 => "Infraestrutura",
                2 => "Manutenção",
                _ => "Desconhecida"
            };

            using (var document = DocX.Create(caminho))
            {
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "senai-logo.png");

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

                document.InsertParagraph("INFORMAÇÕES GERAIS").Bold().FontSize(16);

                document.InsertParagraph("-----------------------------------");

                document.InsertParagraph("");

                document.InsertParagraph($"ID: {os["idOrdemServico"]}");

                document.InsertParagraph($"Descrição: {os["descricaoServico"]}");

                document.InsertParagraph($"Categoria: {categoriaTexto}");

                document.InsertParagraph($"Número Patrimônio: {os["numeroPatrimonio"]}");

                var p = document.InsertParagraph();

                p.Append("Bloco: ").Bold();

                p.Append(os["nomeBloco"].ToString());

                document.InsertParagraph($"Local: {os["nomeLocal"]}");

                document.InsertParagraph($"Prioridade: {prioridadeTexto}");

                document.InsertParagraph($"Observações: {os["observacoes"]}");

                document.InsertParagraph($"Status: {statusTexto}");

                document.InsertParagraph($"Data Solicitação: {os["dataSolicitacao"]}");

                string dataInicio = os["dataInicio"] != DBNull.Value ? Convert.ToDateTime(os["dataInicio"]).ToString("dd/MM/yyyy HH:mm") : "Não iniciada";

                string dataFinalizacao = os["dataFinalizacao"] != DBNull.Value ? Convert.ToDateTime(os["dataFinalizacao"]).ToString("dd/MM/yyyy HH:mm") : "Não finalizada";

                document.InsertParagraph("");

                document.InsertParagraph("SOLICITANTE").Bold();

                document.InsertParagraph($"Nome: {os["nomeCriador"]}");

                document.InsertParagraph($"Email: {os["emailCriador"]}");

                document.InsertParagraph("");

                document.InsertParagraph("EXECUTOR").Bold();

                document.InsertParagraph($"Nome: {os["nomeExecutor"]}");

                document.InsertParagraph($"Email: {os["emailExecutor"]}");

                document.AddFooters();

                document.Footers.Odd.InsertParagraph("Sistema de Gerenciamento de Manutenção Predial - SENAI");

                document.Save();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(caminho);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"OS_{id}.docx");
        }
    }
}
