using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using webApiAghos.Repositories;
using webApiAghos;
using System.Text.RegularExpressions;
using System.Drawing;
using Oracle.ManagedDataAccess.Types;

namespace webApiAghos.Repositories
{
    public class TUserRepository : ITUserRepository
    {
        IOracleHelper oracleHelper;

        public TUserRepository(IOracleHelper _oracleHelper)
        {
            oracleHelper = _oracleHelper;
        }
        //lista um registro com procedute identificado com tmpId
        public object? GetTuserDetail(int tmpId)
        {
            object? result = null;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input, tmpId);
                dyParam.Add("P_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);

                var conn = oracleHelper.GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "USP_GET_TUSER_DETAIL";

                    result = SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public object? GetTExamesList(int tmpOffset=0, int tmpFecht=5)
        {
            object? result = null;

            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_OFFSET", OracleDbType.Int32, ParameterDirection.Input, tmpOffset);   //0 é o offset pula numero de rows, posso colocar em variavel
                dyParam.Add("P_FECHT", OracleDbType.Int32, ParameterDirection.Input, tmpFecht);   //10 é o limite registros, posso colocar em variavel

                dyParam.Add("P_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);

                var conn = oracleHelper.GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "USP_GET_TUSER_LIST";
                    result = SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public object? GetTExamesList3()
        {
            //esta listando 10 registros apartir do inicio
            //O WITH TIES retorna linhas adicionais com a mesma chave de classificação da última linha buscada. Observe que se você usar WITH TIES , deverá especificar uma cláusula ORDER BY na consulta. Caso contrário, a consulta não retornará as linhas adicionais.

            object? result = null;
            var query = "select NOME,ID_EXAME,SOLICITADO,STATUS,PROCEDIMENTO,CPF,CARTAO_SUS,TELEFONES"
                +" from V_EXAME_LISTA_ESPERA FETCH NEXT 10 ROWS WITH TIES";
            try
            {

                var conn = oracleHelper.GetConnection();
                if (conn.State == ConnectionState.Closed) { conn.Open();}
                if (conn.State == ConnectionState.Open)
                {
                    result = conn.Query<ApiaghosDTO>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

//        var ids = new[] { 3, 7, 12 };
//        var sql = "SELECT * FROM Products WHERE ProductId IN @Ids;";
//using (var connection = new SqlConnection(connectionString))
//{
//	connection.Open();
//	var products = connection.Query<Product>(sql, new { Ids = ids }).ToList();
//}

//lista um registro identificado por tmpId
public object? GetTexamesDetail(int tmpId)
        {
            object? result = null;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input, tmpId);
                dyParam.Add("P_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);

                var conn = oracleHelper.GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "USP_GET_TEXAME_DETAIL";

                    result = SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }
        //lista um registro com comando SQL direto sem procedure
        public object? GetTexamesDetail2(int tmpId)
        {
            object? result = null;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input, tmpId);

                var conn = oracleHelper.GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    string query = $"select NOME,ID_EXAME,SOLICITADO,STATUS,PROCEDIMENTO,CPF,CARTAO_SUS,TELEFONES from V_EXAME_LISTA_ESPERA where id_exame={tmpId}";                   
                    result = SqlMapper.Query(conn, query, commandType: CommandType.Text);
                }

            }
            catch (Exception ex)
            {
                return 0;
                
            }

            return result;
        }
    }
}
// select* from gsh_usuario INNER JOIN gsh_pessoas USING(id_pessoa)  ORDER BY  nome ASC OFFSET 9 ROWS FETCH NEXT 10 ROWS WITH TIES; --WITH TIES repete ultimos registros com o mesmo valor do ultimo;  ROWS ONLY somente o número registros
//var parameters = new { UserName = username, Password = password };
//var sql = "SELECT * from users where username = @UserName and password = @Password";
//var result = connection.Query(sql, parameters);