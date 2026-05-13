using System.Data;
using Microsoft.Data.SqlClient;

namespace ProjBancoDados.BancoDados
{
    public class Usuario
    {
        //-----------------------------
        // Atributos
        //-----------------------------
        public int idUsuario;
        public string nome;
        public string senha;
        public string cpf;
        public int nvAcesso;

        SqlConnection con;

        //-----------------------------
        // Construtor
        //-----------------------------
        public Usuario()
        {
            try
            {
                IConfigurationRoot o_Config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@".\Configuration\SistemaRH.json")
                    .Build();

                string strConexao = o_Config.GetConnectionString(@"StringConexaoSQLServer");

                // Prepara a conexão com o BD
                con = new SqlConnection(strConexao);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //-----------------------------
        // Métodos
        //-----------------------------
        public void Inserir()
        {
            try
            {
                // Prepara o comando SQL
                string cmdSQL = "Insert Into Usuario(Nome, Senha, CPF, NvAcesso) " +
                                "Values(@Nome, @Senha, @CPF, @NvAcesso)";

                // Prepara SqlCommand
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Senha", senha);
                cmd.Parameters.AddWithValue("@CPF", cpf);
                cmd.Parameters.AddWithValue("@NvAcesso", nvAcesso);

                // Abre a conexão com o BD
                con.Open();

                // Executa o comando SQL
                cmd.ExecuteNonQuery();

                // Fecha a conexão com o BD
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Alterar()
        {
            try
            {
                // Prepara o comando SQL
                string cmdSQL = "Update Usuario Set " +
                                "Nome = @Nome, " +
                                "Senha = @Senha, " +
                                "CPF = @CPF, " +
                                "NvAcesso = @NvAcesso " +
                                "Where IdUsuario = @IdUsuario";

                // Prepara SqlCommand
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Senha", senha);
                cmd.Parameters.AddWithValue("@CPF", cpf);
                cmd.Parameters.AddWithValue("@NvAcesso", nvAcesso);

                // Abre a conexão com o BD
                con.Open();

                // Executa o comando SQL
                cmd.ExecuteNonQuery();

                // Fecha a conexão com o BD
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Excluir()
        {
            try
            {
                // Prepara o comando SQL
                string cmdSQL = "Delete From Usuario Where IdUsuario = @IdUsuario";

                // Prepara SqlCommand
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                // Abre a conexão com o BD
                con.Open();

                // Executa o comando SQL
                cmd.ExecuteNonQuery();

                // Fecha a conexão com o BD
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable Selecionar()
        {
            try
            {
                // Prepara o comando SQL
                string cmdSQL = "Select * From Usuario Order By IdUsuario";

                // Prepara SqlDataAdapter
                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);

                // Abre a conexão com o BD
                con.Open();

                DataTable dtPesquisa = new DataTable();

                // Executa o Select no banco de dados
                int qtdLinhasAfetada = o_DataAdapter.Fill(dtPesquisa);

                // Fecha a conexão com o BD
                con.Close();

                if (qtdLinhasAfetada > 0)
                {
                    return dtPesquisa;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

}