namespace GRMP.Models
{
    public class BlocoViewModel
    {
        public string NomeBloco { get; set; }

        public int? LocalSelecionado { get; set; }

        public List<LocalBlocoViewModel> Locais { get; set; } = new();

        public List<ChamadoBlocoViewModel> Chamados { get; set; } = new();
    }
}