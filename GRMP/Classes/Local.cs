using System.Data;
using Microsoft.Data.SqlClient;

namespace GRMP.Classes
{
    public class Local
    {
        public int idLocal;
        public string nome;
        public int fk_idBloco;

        SqlConnection con;

        public Local()
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

        public void Inserir()
        {
            try
            {
                string cmdSQL = @"INSERT INTO Local (nome, fk_idBloco)
                                  VALUES (@nome, @fk_idBloco)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@fk_idBloco", fk_idBloco);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        public void Alterar()
        {
            try
            {
                string cmdSQL = @"UPDATE Local SET
                                    nome = @nome,
                                    fk_idBloco = @fk_idBloco
                                  WHERE idLocal = @idLocal";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idLocal", idLocal);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@fk_idBloco", fk_idBloco);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        public void Excluir()
        {
            try
            {
                string cmdSQL = "DELETE FROM Local WHERE idLocal = @idLocal";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idLocal", idLocal);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable Selecionar()
        {
            try
            {
                string cmdSQL = @"SELECT * FROM Local ORDER BY idLocal";

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

        public DataTable BuscarPorId(int id)
        {
            try
            {
                string cmdSQL = @"SELECT * FROM Local WHERE idLocal = @idLocal";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idLocal", id);

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

        public DataTable BuscarLocaisPorBloco()
        {
            try
            {
                string sql = @"SELECT
                                idLocal,
                                nome,
                                fk_idBloco
                               FROM Local
                               WHERE fk_idBloco = @fk_idBloco
                               ORDER BY nome";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@fk_idBloco", fk_idBloco);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt : null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable BuscarLocaisDoBloco(string nomeBloco)
        {
            try
            {
                string sql = @"SELECT DISTINCT
                                l.idLocal,
                                l.nome
                               FROM OrdemServico os

                               INNER JOIN Local l
                               ON l.idLocal = os.Local

                               INNER JOIN Bloco b
                               ON b.idBloco = l.fk_idBloco

                               WHERE b.nome = @bloco
                               AND os.status != 2
                               AND os.status != 3
                               ORDER BY l.nome";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@bloco", nomeBloco);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

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