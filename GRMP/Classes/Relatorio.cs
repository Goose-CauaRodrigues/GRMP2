using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GRMP.Classes
{
    public class Relatorio
    {
        SqlConnection con;

        //-----------------------------
        // Construtor
        //-----------------------------
        public Relatorio()
        {
            try
            {
                IConfigurationRoot o_Config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@".\Configuration\GRMPBD.json")
                    .Build();

                string strConexao = o_Config.GetConnectionString("StringConexaoSQLServer");

                con = new SqlConnection(strConexao);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //-----------------------------
        // Selecionar Filtro
        //-----------------------------
        public DataTable SelecionarFiltro(
            DateTime? dataInicial,
            DateTime? dataFinal,
            string? executor,
            string? bloco,
            string? local,
            int? tempoConclusaoHoras,
            string? status)
        {
            try
            {
                string cmdSQL = @"
                    SELECT 
                        idOrdemServico,
                        solicitante,
                        executor,
                        descricaoServico,
                        categoria,
                        numeroPatrimonio,
                        bloco,
                        local,
                        prioridade,
                        status,
                        observacoes,
                        dataSolicitacao,
                        dataInicio,
                        dataFinalizacao,
                        tempoConclusaoHoras,
                        ativo
                    FROM vw_OrdemServico
                    WHERE 1 = 1";

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = con;

                // =========================
                // DATA SOLICITAÇÃO
                // =========================

                if (dataInicial.HasValue)
                {
                    cmdSQL += " AND dataSolicitacao >= @dataInicial";

                    cmd.Parameters.Add("@dataInicial", SqlDbType.DateTime).Value = dataInicial.Value;
                }

                if (dataFinal.HasValue)
                {
                    cmdSQL += " AND dataSolicitacao <= @dataFinal";

                    cmd.Parameters.Add("@dataFinal", SqlDbType.DateTime).Value = dataFinal.Value;
                }

                // =========================
                // EXECUTOR
                // =========================

                if (!string.IsNullOrEmpty(executor))
                {
                    cmdSQL += " AND executor LIKE @executor";

                    cmd.Parameters.Add("@executor", SqlDbType.VarChar).Value = "%" + executor + "%";
                }

                // =========================
                // BLOCO
                // =========================

                if (!string.IsNullOrEmpty(bloco))
                {
                    cmdSQL += " AND bloco LIKE @bloco";

                    cmd.Parameters.Add("@bloco", SqlDbType.VarChar).Value = "%" + bloco + "%";
                }

                // =========================
                // LOCAL
                // =========================

                if (!string.IsNullOrEmpty(local))
                {
                    cmdSQL += " AND local LIKE @local";

                    cmd.Parameters.Add("@local", SqlDbType.VarChar).Value = "%" + local + "%";
                }

                // =========================
                // TEMPO CONCLUSÃO
                // =========================

                if (tempoConclusaoHoras.HasValue)
                {
                    cmdSQL += " AND tempoConclusaoHoras = @tempoConclusaoHoras";

                    cmd.Parameters.Add("@tempoConclusaoHoras", SqlDbType.Int).Value = tempoConclusaoHoras.Value;
                }

                // =========================
                // STATUS
                // =========================

                if (!string.IsNullOrEmpty(status))
                {
                    cmdSQL += " AND status LIKE @status";

                    cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "%" + status + "%";
                }

                cmdSQL += " ORDER BY idOrdemServico DESC";

                cmd.CommandText = cmdSQL;

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt : null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}