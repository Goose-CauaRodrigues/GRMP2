namespace GRMP.Models
{
    public class DashboardViewModel
    {
        public List<RelatorioModel> ListaOS { get; set; } = new();
        public List<string> Solicitantes { get; set; } = new();
        public List<string> Executores { get; set; } = new();
        public List<string> Blocos { get; set; } = new();

        public string SolicitanteSelecionado { get; set; }

        public string ExecutorSelecionado { get; set; }

        public string StatusSelecionado { get; set; }

        public string PrioridadeSelecionada { get; set; }

        public string BlocoSelecionado { get; set; }

        public string PeriodoSelecionado { get; set; }

        public string DataDe { get; set; }

        public string DataAte { get; set; }

        public int PaginaAtual { get; set; }

        public int TotalPaginas { get; set; }

        public int TotalItens { get; set; }

        public int Total { get; set; }

        public int Abertas { get; set; }

        public int EmAndamento { get; set; }

        public int Concluidas { get; set; }

        public int Canceladas { get; set; }

        public int EmPausa { get; set; }

        public int PrioridadeBaixa { get; set; }

        public int PrioridadeMedia { get; set; }

        public int PrioridadeAlta { get; set; }

        public int FiltrosAtivos { get; set; }

        public string PercentualAbertas { get; set; }

        public string PercentualEmAndamento { get; set; }

        public string PercentualConcluidas { get; set; }

        public string PercentualCanceladas { get; set; }

        public string PercentualEmPausa { get; set; }
    }
}