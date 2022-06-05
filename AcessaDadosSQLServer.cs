using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Using para o BD
using System.Data;
using System.Data.SqlClient;
using AcessoBancoDados.Properties;


namespace AcessoBancoDados
{
    class AcessaDadosSQLServer
    {
        //Criar a conexão com BD
        public SqlConnection CriarConexao()
        {
            return new SqlConnection(Settings.Default.StringConnection);
        }

        //parâmetros para o BD
        private SqlParameterCollection sqlparametercollection = new SqlCommand().Parameters;

        public void LimparParametros() 
        {
            sqlparametercollection.Clear();
        }

        public void AdicionarParametro(string nomeParametro, object valorParametro)  
        {
            sqlparametercollection.Add(new SqlParameter(nomeParametro, valorParametro));
        }

        private SqlCommand PreencheSqlCommand(CommandType commandType, string MinhaUsp)
        {
            try
            {
                //Criar e abrir Conexão com o BD
                SqlConnection sqlConnection = CriarConexao();
                sqlConnection.Open();

                //Criar e enviar os Comandos ao BD
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = commandType;
                sqlCommand.CommandText = MinhaUsp;
                //TimeOut
                sqlCommand.CommandTimeout = 3600; //<--- Medido em Segundos

                //Adicionar os parâmetros da Strored Procedure
                foreach  (SqlParameter item in sqlparametercollection)
                {
                    sqlCommand.Parameters.Add(new SqlParameter(item.ParameterName, item.Value));  
                }
                //Retorno da SQLCommand
                return sqlCommand;

            }
            catch (Exception ex)
            {
                //Dispara um novo efeito de exceção para ser manipulado na camada superior
                //O formato de saída é diferente do exigido
                throw new Exception(ex.Message);
            }
        }

        //Inserir,Alterar e Excluir
        public object ManipulaDados(CommandType commandType, string minhUsp) 
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                //Chamar a função que preenche a SqlCommand
                sqlCommand = PreencheSqlCommand(commandType, minhUsp);
                //Executar a rotina do BD
                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        //Consultar os Registros
        public DataTable ExecutaConsulta(CommandType commandType, string minhaUsp) 
        {
            try
            {
                //Chama a função que preenche o SqlCommand
                SqlCommand sqlCommand = PreencheSqlCommand(commandType, minhaUsp);
                //Criar um tradutor para o retorno do BD
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                //Criar a DataTable(retorno)
                DataTable dataTable = new DataTable();
                //Preencher o dataTable com os dados do BD
                sqlDataAdapter.Fill(dataTable);
                //Retornar a consulta
                return dataTable;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
