using GRMP.Classes;
using GRMP.Models;
using Microsoft.AspNetCore.Mvc;
using ProjBancoDados.BancoDados;
using System.Data;

namespace GRMP.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult DashboardExibir(string solicitante, string executor, string periodo, string status,
                                             string prioridade, string bloco, string dataDe, string dataAte, int pagina = 1)
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("LoginExibir", "Login");
            }

            Usuario Us = new Usuario();

            // Busca dados do usuário logado
            DataTable dt = Us.BuscarPorID(int.Parse(idUsuario));

            foreach (DataRow dr in dt.Rows)
            {
                int nvAcesso = Convert.ToInt32(dr["nvAcesso"]);

                // Se for nível de acesso 3 continua normalmente.

                if (nvAcesso == 3)
                {
                    break;
                }

                // Se for nível de acesso 2 ou 1 redireciona.

                else if (nvAcesso == 2)
                {
                    return RedirectToAction("MapaExibir", "Mapa");
                }
                else
                {
                    return RedirectToAction("ListarOSExibir", "Usuario");

                }
            }

            Relatorio relatorio = new Relatorio();

            var dados = relatorio.SelecionarFiltro(null, null, null, null, null, null, null);

            if (string.IsNullOrWhiteSpace(periodo)) periodo = "todos";

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (!string.IsNullOrWhiteSpace(dataDe) && DateTime.TryParse(dataDe, out var dtDe)) dataInicio = dtDe;
            if (!string.IsNullOrWhiteSpace(dataAte) && DateTime.TryParse(dataAte, out var dtAte)) dataFim = dtAte.Date.AddDays(1).AddTicks(-1);

            if (dataInicio == null && dataFim == null && periodo != "todos")
            {
                int? dias = periodo switch { "mensal" => 30, "trimestral" => 90, "semestral" => 180, "anual" => 365, _ => (int?)null };
                if (dias.HasValue) dataInicio = DateTime.Now.Date.AddDays(-dias.Value);
            }

            var solicitantes = dados.Select(x => x.solicitante).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList();
            var executores = dados.Select(x => x.executor).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList();
            var blocos = dados.Select(x => x.bloco).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList();

            var filtradas = dados.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(solicitante)) filtradas = filtradas.Where(x => x.solicitante == solicitante);
            if (!string.IsNullOrWhiteSpace(executor)) filtradas = filtradas.Where(x => x.executor == executor);
            if (!string.IsNullOrWhiteSpace(status)) filtradas = filtradas.Where(x => x.status == status);
            if (!string.IsNullOrWhiteSpace(prioridade)) filtradas = filtradas.Where(x => x.prioridade == prioridade);
            if (!string.IsNullOrWhiteSpace(bloco)) filtradas = filtradas.Where(x => x.bloco == bloco);
            if (dataInicio.HasValue) filtradas = filtradas.Where(x => x.dataSolicitacao >= dataInicio.Value);
            if (dataFim.HasValue) filtradas = filtradas.Where(x => x.dataSolicitacao <= dataFim.Value);

            var lista = filtradas.ToList();

            int paginaAtual = pagina;

            int itensPorPagina = 50;

            int totalItens = lista.Count;

            int totalPaginas = Math.Max(1, (int)Math.Ceiling((double)totalItens / itensPorPagina));

            if (paginaAtual < 1) paginaAtual = 1;

            if (paginaAtual > totalPaginas) paginaAtual = totalPaginas;

            var listaPaginada = lista.OrderByDescending(x => x.dataSolicitacao).Skip((paginaAtual - 1) * itensPorPagina).Take(itensPorPagina).ToList();
            var total = lista.Count;
            var abertas = lista.Count(x => x.status == "Aberta");
            var emAndamento = lista.Count(x => x.status == "Em andamento");
            var concluidas = lista.Count(x => x.status == "Concluída");
            var canceladas = lista.Count(x => x.status == "Cancelada");
            var emPausa = lista.Count(x => x.status == "Em pausa");

            var prBaixa = lista.Count(x => x.prioridade == "Baixa");
            var prMedia = lista.Count(x => x.prioridade == "Média");
            var prAlta = lista.Count(x => x.prioridade == "Alta");

            var filtrosAtivos = (!string.IsNullOrWhiteSpace(solicitante) ? 1 : 0) + (!string.IsNullOrWhiteSpace(executor) ? 1 : 0) + (periodo != "todos" ? 1 : 0) + 
                                (!string.IsNullOrWhiteSpace(status) ? 1 : 0) + (!string.IsNullOrWhiteSpace(prioridade) ? 1 : 0) + 
                                (!string.IsNullOrWhiteSpace(bloco) ? 1 : 0) + (!string.IsNullOrWhiteSpace(dataDe) ? 1 : 0) + 
                                (!string.IsNullOrWhiteSpace(dataAte) ? 1 : 0);

            var vm = new DashboardViewModel
            {
                ListaOS = listaPaginada,

                Solicitantes = solicitantes,
                Executores = executores,
                Blocos = blocos,

                SolicitanteSelecionado = solicitante,
                ExecutorSelecionado = executor,
                StatusSelecionado = status,
                PrioridadeSelecionada = prioridade,
                BlocoSelecionado = bloco,
                PeriodoSelecionado = periodo,

                DataDe = dataDe,
                DataAte = dataAte,

                PaginaAtual = paginaAtual,
                TotalPaginas = totalPaginas,
                TotalItens = totalItens,

                Total = total,

                Abertas = abertas,
                EmAndamento = emAndamento,
                Concluidas = concluidas,
                Canceladas = canceladas,
                EmPausa = emPausa,

                PrioridadeBaixa = prBaixa,
                PrioridadeMedia = prMedia,
                PrioridadeAlta = prAlta,

                FiltrosAtivos = filtrosAtivos,

                PercentualAbertas = total > 0 ? Math.Round(abertas * 100.0 / total) + "%" : "-",
                PercentualEmAndamento = total > 0 ? Math.Round(emAndamento * 100.0 / total) + "%" : "-",
                PercentualConcluidas = total > 0 ? Math.Round(concluidas * 100.0 / total) + "%" : "-",
                PercentualCanceladas = total > 0 ? Math.Round(canceladas * 100.0 / total) + "%" : "-",
                PercentualEmPausa = total > 0 ? Math.Round(emPausa * 100.0 / total) + "%" : "-"
            };

            return View("DashboardExibirView", vm);
        }
    }
}
