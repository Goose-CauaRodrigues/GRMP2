using System.Data;
using Microsoft.Data.SqlClient;

namespace GRMP.Classes
{
    public class Os
    {

        //-----------------------------
        // Atributos
        //-----------------------------
        public int idOrdemServico;
        public int fk_idUsuario;
        public int? fk_executor;
        public string descricaoServico;
        public int categoria;
        public string? numeroPatrimonio;
        public int bloco;
        public int local;
        public int? prioridade;
        public string? observacoes;
        public DateTime dataSolicitacao;
        public DateTime? dataInicio;
        public DateTime? dataFinalizacao;
        public int? status;
        public bool ativo;

        SqlConnection con;

        //-----------------------------
        // Construtor
        //-----------------------------
        public Os()
        {
            try
            {
                IConfigurationRoot o_Config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@".\Configuration\GRMPBD.json")
                    .Build();

                string strConexao = o_Config.GetConnectionString(@"StringConexaoSQLServer");

                con = new SqlConnection(strConexao);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //-----------------------------
        // Inserir
        //-----------------------------
        public void Inserir()
        {
            try
            {
                string cmdSQL = @"
                    INSERT INTO OrdemServico
                    (
                        fk_idUsuario,
                        fk_executor,
                        descricaoServico,
                        categoria,
                        numeroPatrimonio,
                        bloco,
                        local,
                        prioridade,
                        observacoes,
                        dataSolicitacao,
                        dataInicio,
                        dataFinalizacao,
                        status,
                        ativo
                    )
                    VALUES
                    (
                        @fk_idUsuario,
                        @fk_executor,
                        @descricaoServico,
                        @categoria,
                        @numeroPatrimonio,
                        @bloco,
                        @local,
                        @prioridade,
                        @observacoes,
                        @dataSolicitacao,
                        @dataInicio,
                        @dataFinalizacao,
                        @status,
                        @ativo
                    )";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@fk_idUsuario", fk_idUsuario);
                cmd.Parameters.AddWithValue("@fk_executor", (object?)fk_executor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@descricaoServico", descricaoServico);
                cmd.Parameters.AddWithValue("@categoria", categoria);
                cmd.Parameters.AddWithValue("@numeroPatrimonio", (object?)numeroPatrimonio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@bloco", bloco);
                cmd.Parameters.AddWithValue("@local", local);
                cmd.Parameters.AddWithValue("@prioridade", (object?)prioridade ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@observacoes", (object?)observacoes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dataSolicitacao", dataSolicitacao);
                cmd.Parameters.AddWithValue("@dataInicio", (object?)dataInicio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dataFinalizacao", (object?)dataFinalizacao ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@status", (object?)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ativo", ativo);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        //-----------------------------
        // Alterar
        //-----------------------------
        public void Alterar()
        {
            try
            {
                string cmdSQL = @"
                    UPDATE OrdemServico SET
                        fk_idUsuario = @fk_idUsuario,
                        fk_executor = @fk_executor,
                        descricaoServico = @descricaoServico,
                        categoria = @categoria,
                        numeroPatrimonio = @numeroPatrimonio,
                        bloco = @bloco,
                        local = @local,
                        prioridade = @prioridade,
                        observacoes = @observacoes,
                        dataSolicitacao = @dataSolicitacao,
                        dataInicio = @dataInicio,
                        dataFinalizacao = @dataFinalizacao,
                        status = @status,
                        ativo = @ativo
                    WHERE idOrdemServico = @idOrdemServico";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idOrdemServico", idOrdemServico);
                cmd.Parameters.AddWithValue("@fk_idUsuario", fk_idUsuario);
                cmd.Parameters.AddWithValue("@fk_executor", (object?)fk_executor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@descricaoServico", descricaoServico);
                cmd.Parameters.AddWithValue("@categoria", categoria);
                cmd.Parameters.AddWithValue("@numeroPatrimonio", (object?)numeroPatrimonio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@bloco", bloco);
                cmd.Parameters.AddWithValue("@local", local);
                cmd.Parameters.AddWithValue("@prioridade", (object?)prioridade ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@observacoes", (object?)observacoes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dataSolicitacao", dataSolicitacao);
                cmd.Parameters.AddWithValue("@dataInicio", (object?)dataInicio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dataFinalizacao", (object?)dataFinalizacao ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@status", (object?)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ativo", ativo);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        //-----------------------------
        // Excluir
        //-----------------------------
        public void Excluir()
        {
            try
            {
                string cmdSQL = "DELETE FROM OrdemServico WHERE idOrdemServico = @idOrdemServico";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idOrdemServico", idOrdemServico);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        //-----------------------------
        // Selecionar
        //-----------------------------
        public DataTable Selecionar()
        {
            try
            {
                string cmdSQL = "SELECT * FROM OrdemServico ORDER BY idOrdemServico";

                SqlDataAdapter da = new SqlDataAdapter(cmdSQL, con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt : null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //-----------------------------
        // Buscar por ID
        //-----------------------------
        public DataTable BuscarPorId(int id)
        {
            try
            {
                string cmdSQL = @"
                    SELECT * 
                    FROM OrdemServico
                    WHERE idOrdemServico = @idOrdemServico";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idOrdemServico", id);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt : null;
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable SelecionarOS()
        {
            try
            {
                string sql = @"
            SELECT
                os.idOrdemServico,
                os.descricaoServico,
                os.categoria,
                os.numeroPatrimonio,

                os.bloco,
                b.nome AS nomeBloco,

                os.local,
                l.nome AS nomeLocal,

                os.prioridade,
                os.status,
                os.dataSolicitacao,
                os.dataInicio,
                os.dataFinalizacao,
                os.observacoes,
                os.ativo,

                criador.nome AS nomeCriador,
                criador.email AS emailCriador,

                executor.nome AS nomeExecutor,
                executor.email AS emailExecutor

            FROM OrdemServico os

            INNER JOIN Usuario criador
                ON os.fk_idUsuario = criador.idUsuario

            LEFT JOIN Usuario executor
                ON os.fk_executor = executor.idUsuario

            LEFT JOIN Bloco b
                ON os.bloco = b.idBloco

            LEFT JOIN Local l
                ON os.local = l.idLocal

            ORDER BY os.idOrdemServico";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}


