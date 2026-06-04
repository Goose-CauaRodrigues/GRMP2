using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GRMP.Models
{
    public class OrdemServicoViewModel
    {
        public int IdOrdemServico { get; set; }

        [Required]
        public int FkIdUsuario { get; set; }

        public int? FkExecutor { get; set; }

        [Required]
        [StringLength(255)]
        public string DescricaoServico { get; set; }

        [Required]
        public int Categoria { get; set; }

        public string? NumeroPatrimonio { get; set; }

        [Required]
        public int Bloco { get; set; }

        [Required]
        public int Local { get; set; }

        public int? Prioridade { get; set; }

        public string? Observacoes { get; set; }

        [Required]
        public DateTime DataSolicitacao { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFinalizacao { get; set; }

        public int? Status { get; set; }

        public DataTable DtBlocos { get; set; }
        public DataTable DtLocais { get; set; }
    }
}