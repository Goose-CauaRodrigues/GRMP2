namespace GRMP.Models
{
    public class ChamadoBlocoViewModel
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public int Status { get; set; }

        public string Local { get; set; }

        public string StatusTexto
        {
            get
            {
                return Status switch
                {
                    1 => "Aberto",
                    2 => "Em andamento",
                    3 => "Resolvido",
                    _ => "Desconhecido"
                };
            }
        }
    }
}