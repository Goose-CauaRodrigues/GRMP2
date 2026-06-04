namespace GRMP.Models
{
    public class RelatorioModel
    {
        public int idOrdemServico { get; set; }

        public string solicitante { get; set; }

        public string executor { get; set; }

        public string descricaoServico { get; set; }

        public string categoria { get; set; }

        public string numeroPatrimonio { get; set; }

        public string bloco { get; set; }

        public string local { get; set; }

        public string prioridade { get; set; }

        public string status { get; set; }

        public string observacoes { get; set; }

        public DateTime dataSolicitacao { get; set; }

        public DateTime? dataInicio { get; set; }

        public DateTime? dataFinalizacao { get; set; }

        public int? tempoConclusaoHoras { get; set; }
    }
}
