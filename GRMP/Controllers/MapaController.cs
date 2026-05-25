using GRMP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GRMP.Controllers
{
    public class MapaController : Controller
    {
        private readonly IConfiguration _configuration;

        public MapaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =========================
        // MAPA GERAL
        // =========================
        public IActionResult Index(int? status)
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            var viewModel = new MapaViewModel();

            string connStr = _configuration.GetConnectionString("StringConexaoSQLServer");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string filtroStatus = "";

                if (status.HasValue)
                {
                    filtroStatus = " AND os.status = @status ";
                }

                // =========================
                // BLOCOS COM OS
                // =========================

                string sqlBlocos = $@"
            SELECT DISTINCT b.nome
            FROM OrdemServico os
            INNER JOIN Bloco b ON b.idBloco = os.Bloco
            WHERE os.ativo = 1
            {filtroStatus}
        ";

                using (SqlCommand cmd = new SqlCommand(sqlBlocos, conn))
                {
                    if (status.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@status", status.Value);
                    }

                    using SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        viewModel.BlocosComOs.Add(dr["nome"].ToString());
                    }
                }

                // =========================
                // CHAMADOS LATERAIS
                // =========================

                string sqlChamados = $@"
            SELECT TOP 10
                os.idOrdemServico,
                os.descricaoServico,
                b.nome AS bloco,
                os.status
            FROM OrdemServico os
            INNER JOIN Bloco b ON b.idBloco = os.Bloco
            WHERE os.ativo = 1
            {filtroStatus}
            ORDER BY os.idOrdemServico DESC
        ";

                using (SqlCommand cmd = new SqlCommand(sqlChamados, conn))
                {
                    if (status.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@status", status.Value);
                    }

                    using SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        int st = Convert.ToInt32(dr["status"]);

                        string statusTexto = st switch
                        {
                            1 => "Aberto",
                            2 => "Em andamento",
                            3 => "Resolvido",
                            _ => "Desconhecido"
                        };

                        viewModel.Chamados.Add(new ChamadoMapa
                        {
                            Id = Convert.ToInt32(dr["idOrdemServico"]),
                            Titulo = dr["descricaoServico"].ToString(),
                            Bloco = dr["bloco"].ToString(),
                            Status = st,
                            StatusTexto = statusTexto
                        });
                    }
                }
            }

            viewModel.StatusSelecionado = status;

            return View(viewModel);
        }

        // =========================
        // TELA DO BLOCO
        // =========================
        public IActionResult Bloco(string id)
        {
            ViewBag.Bloco = id;

            var locais = new List<dynamic>();

            string connStr = _configuration.GetConnectionString("StringConexaoSQLServer");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT
                        l.idLocal,
                        l.nome
                    FROM Local l
                    INNER JOIN Bloco b ON b.idBloco = l.fk_idBloco
                    WHERE b.nome = @bloco
                    ORDER BY l.nome
                ";

                using SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@bloco", id);

                using SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    locais.Add(new
                    {
                        Id = Convert.ToInt32(dr["idLocal"]),
                        Nome = dr["nome"].ToString()
                    });
                }
            }

            return View(locais);
        }
    }
}