namespace GRMP.Models
{
    public class MapaViewModel
    {
        public List<string> BlocosComOs { get; set; } = new();

        public List<ChamadoMapa> Chamados { get; set; } = new();

        public int? StatusSelecionado { get; set; }
    }

    public class ChamadoMapa
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Bloco { get; set; }

        public string StatusTexto { get; set; }

        public int Status { get; set; }
    }
}
