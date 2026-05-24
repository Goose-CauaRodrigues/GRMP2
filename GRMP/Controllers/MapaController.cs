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
        public IActionResult Index()
        {
            var blocosComOs = new List<string>();

            string connStr = _configuration.GetConnectionString("StringConexaoSQLServer");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT DISTINCT b.nome
                    FROM OrdemServico os
                    INNER JOIN Bloco b ON b.idBloco = os.Bloco
                    WHERE os.status != 3
                    AND os.ativo = 1";

                using SqlCommand cmd = new SqlCommand(sql, conn);
                using SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    blocosComOs.Add(dr["nome"].ToString());
                }
            }

            return View(blocosComOs);
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