using System.Data;
using Microsoft.Data.SqlClient;

namespace GRMP.Classes
{
    public class Bloco
    {
        public int idBloco;
        public string nome;

        SqlConnection con;

        public Bloco()
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
                string cmdSQL = @"INSERT INTO Bloco(nome)
                                  VALUES(@nome)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@nome", nome);

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
                string cmdSQL = @"UPDATE Bloco SET
                                    nome = @nome
                                  WHERE idBloco = @idBloco";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idBloco", idBloco);
                cmd.Parameters.AddWithValue("@nome", nome);

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
                string cmdSQL = "DELETE FROM Bloco WHERE idBloco = @idBloco";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idBloco", idBloco);

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
                string cmdSQL = "SELECT * FROM Bloco ORDER BY idBloco";

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
                string cmdSQL = @"SELECT * FROM Bloco WHERE idBloco = @idBloco";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idBloco", id);

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
    }
}