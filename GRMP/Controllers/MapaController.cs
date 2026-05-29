using GRMP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProjBancoDados.BancoDados;
using System.Data;

namespace GRMP.Controllers
{
    public class MapaController : Controller
    {
        private readonly IConfiguration _configuration;

        public MapaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult OsPorLocal(int localId)
        {
            var lista = new List<object>();

            string connStr =
                _configuration.GetConnectionString("StringConexaoSQLServer");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
            SELECT 
                os.idOrdemServico,
                os.descricaoServico,
                os.status
            FROM OrdemServico os
            WHERE os.local = @localId
            AND os.ativo = 1
            AND os.status != 2
            AND os.status != 3
            ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@localId", localId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int status = Convert.ToInt32(reader["status"]);

                            lista.Add(new
                            {
                                id = Convert.ToInt32(reader["idOrdemServico"]),
                                titulo = reader["descricaoServico"].ToString(),
                                statusTexto =
                                    status == 0 ? "Aberto" :
                                    status == 1 ? "Em andamento" :
                                    status == 2 ? "Concluída" :
                                    status == 3 ? "Cancelada" :
                                    "Pausado"
                            });
                        }
                    }
                }
            }

            return Json(lista);
        }

        // =========================
        // MAPA GERAL
        // =========================
        public IActionResult MapaExibir(int? status)
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            Usuario Us = new Usuario();

            // Busca dados do usuário logado
            DataTable dt = Us.BuscarPorID(int.Parse(idUsuario));

            foreach (DataRow dr in dt.Rows)
            {
                int nvAcesso = Convert.ToInt32(dr["nvAcesso"]);

                // Se for nível 3 -> continua normalmente
                if (nvAcesso == 3)
                {
                    break;
                }
                // Se for nível 1 -> redireciona
                else if (nvAcesso == 2)
                {
                    return RedirectToAction("MapaExibir", "Mapa");


                }
                else
                {
                    return RedirectToAction("InicioExibir", "Usuario");

                }
            }

            var viewModel = new MapaViewModel();

            string connStr =
                _configuration.GetConnectionString("StringConexaoSQLServer");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string filtroStatus = "";

                if (status.HasValue)
                    filtroStatus = " AND os.status = @status ";

                // BLOCO
                string sqlBlocos = $@"
                    SELECT DISTINCT b.nome
                    FROM OrdemServico os
                    INNER JOIN Bloco b ON b.idBloco = os.Bloco
                    WHERE os.ativo = 1
                    AND os.status != 2
                    AND os.status != 3
                    {filtroStatus}
                ";

                using (SqlCommand cmd = new SqlCommand(sqlBlocos, conn))
                {
                    if (status.HasValue)
                        cmd.Parameters.AddWithValue("@status", status.Value);

                    using SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                        viewModel.BlocosComOs.Add(dr["nome"].ToString());
                }

                // CHAMADOS
                string sqlChamados = $@"
                    SELECT TOP 10
                        os.idOrdemServico,
                        os.descricaoServico,
                        b.nome AS bloco,
                        os.status
                    FROM OrdemServico os
                    INNER JOIN Bloco b ON b.idBloco = os.Bloco
                    WHERE os.ativo = 1
                    AND os.status != 2
                    AND os.status != 3
                    {filtroStatus}
                    ORDER BY os.idOrdemServico DESC
                ";

                using (SqlCommand cmd = new SqlCommand(sqlChamados, conn))
                {
                    if (status.HasValue)
                        cmd.Parameters.AddWithValue("@status", status.Value);

                    using SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        int st = Convert.ToInt32(dr["status"]);

                        viewModel.Chamados.Add(new ChamadoMapa
                        {
                            Id = Convert.ToInt32(dr["idOrdemServico"]),
                            Titulo = dr["descricaoServico"].ToString(),
                            Bloco = dr["bloco"].ToString(),
                            Status = st,
                            StatusTexto = st switch
                            {
                                0 => "Aberto",
                                1 => "Em andamento",
                                2 => "Concluída",
                                3 => "Cancelada",
                                _ => "Desconhecido"
                            }
                        });
                    }
                }
            }

            viewModel.StatusSelecionado = status;

            return View("MapaView", viewModel);
        }

        // =========================
        // BLOCO
        // =========================
        public IActionResult Bloco(string id, int? localId)
        {
            ViewBag.Bloco = id;

            var vm = new BlocoViewModel
            {
                NomeBloco = id,
                LocalSelecionado = localId
            };

            string connStr =
                _configuration.GetConnectionString("StringConexaoSQLServer");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // LOCAIS
                string sqlLocais = @"
                    SELECT DISTINCT l.idLocal, l.nome
                    FROM OrdemServico os
                    INNER JOIN Local l ON l.idLocal = os.Local
                    INNER JOIN Bloco b ON b.idBloco = l.fk_idBloco
                    WHERE b.nome = @bloco
                    AND os.status != 2
                    AND os.status != 3
                    AND os.ativo = 1
                    ORDER BY l.nome
                ";

                using (SqlCommand cmd = new SqlCommand(sqlLocais, conn))
                {
                    cmd.Parameters.AddWithValue("@bloco", id);

                    using SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        vm.Locais.Add(new LocalBlocoViewModel
                        {
                            Id = Convert.ToInt32(dr["idLocal"]),
                            Nome = dr["nome"].ToString()
                        });
                    }
                }

                // CHAMADOS
                string sqlChamados = @"
                    SELECT
                        os.idOrdemServico,
                        os.descricaoServico,
                        os.status,
                        l.nome AS localNome
                    FROM OrdemServico os
                    INNER JOIN Local l ON l.idLocal = os.Local
                    INNER JOIN Bloco b ON b.idBloco = l.fk_idBloco
                    WHERE b.nome = @bloco
                    AND os.status != 2
                    AND os.status != 3
                    AND os.ativo = 1
                ";

                if (localId.HasValue)
                    sqlChamados += " AND l.idLocal = @localId";

                sqlChamados += " ORDER BY os.idOrdemServico DESC";

                using (SqlCommand cmd = new SqlCommand(sqlChamados, conn))
                {
                    cmd.Parameters.AddWithValue("@bloco", id);

                    if (localId.HasValue)
                        cmd.Parameters.AddWithValue("@localId", localId.Value);

                    using SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        vm.Chamados.Add(new ChamadoBlocoViewModel
                        {
                            Id = Convert.ToInt32(dr["idOrdemServico"]),
                            Titulo = dr["descricaoServico"].ToString(),
                            Status = Convert.ToInt32(dr["status"]),
                            Local = dr["localNome"].ToString()
                        });
                    }
                }
            }

            return View(vm);
        }
    }
}