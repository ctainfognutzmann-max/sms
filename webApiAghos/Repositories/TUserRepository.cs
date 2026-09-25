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

        public IEnumerable<EscalaExameDto> GetEscalaExames(int idEscalaExame)
        {
            const string query = """
                SELECT ID_EXAME AS IdExame,
                       ID_ESCALA_EXAME AS IdEscalaExame,
                       ID_ESCALA_GRADE_EXAME AS IdEscalaGradeExame,
                       ID_SALA AS IdSala,
                       PERIODO_INICIAL AS PeriodoInicial,
                       PERIODO_FINAL AS PeriodoFinal,
                       DESCR_SALA AS DescrSala,
                       ID_GRADE_EXAME_ESTADO AS IdGradeExameEstado,
                       PACIENTE AS Paciente,
                       DESCR_EXAME_ESTADO AS DescrExameEstado,
                       EXTRA AS Extra,
                       COR AS Cor
                  FROM (
                        SELECT E.ID_EXAME,
                               E.ID_ESCALA_EXAME,
                               E.ID_ESCALA_GRADE_EXAME,
                               E.ID_SALA,
                               TO_CHAR(E.PERIODO_INICIAL, 'HH24:MI') PERIODO_INICIAL,
                               TO_CHAR(E.PERIODO_FINAL, 'HH24:MI') PERIODO_FINAL,
                               S.DESCR_SALA,
                               E.ID_GRADE_EXAME_ESTADO,
                               (SELECT B.NOME
                                  FROM GSH_PRONTUARIO A, GSH_PESSOAS B, GSH_EXAME C
                                 WHERE A.ID_PESSOA = B.ID_PESSOA
                                   AND A.ID_PRONTUARIO = C.PRONTUARIO_SOLICITANTE
                                   AND C.ID_EXAME = E.ID_EXAME) PACIENTE,
                               EXE.DESCR_EXAME_ESTADO,
                               DECODE(E.EXTRA, 1, 'SIM', 'NÃO') EXTRA,
                               CASE WHEN E.EXTRA = 1 THEN 'clRed' ELSE 'clBlack' END COR
                          FROM GSH_ESCALA_GRADE_EXAME E,
                               GSH_SALA S,
                               GSH_GRADE_EXAME_ESTADO EE,
                               GSH_EXAME EX,
                               GSH_EXAME_ESTADO EXE
                         WHERE E.ID_SALA = S.ID_SALA
                           AND E.ID_GRADE_EXAME_ESTADO = EE.ID_GRADE_EXAME_ESTADO
                           AND E.ID_EXAME = EX.ID_EXAME
                           AND EX.ID_EXAME_ESTADO = EXE.ID_EXAME_ESTADO
                           AND E.ID_ESCALA_EXAME = :idEscalaExame
                           AND EE.ID_GRADE_EXAME_ESTADO NOT IN (6)

                        UNION

                        SELECT GE.ID_EXAME,
                               GE.ID_ESCALA_EXAME,
                               GE.ID_ESCALA_GRADE_EXAME,
                               GE.ID_SALA,
                               TO_CHAR(GE.PERIODO_INICIAL, 'HH24:MI') PERIODO_INICIAL,
                               TO_CHAR(GE.PERIODO_FINAL, 'HH24:MI') PERIODO_FINAL,
                               SA.DESCR_SALA,
                               GE.ID_GRADE_EXAME_ESTADO,
                               PE.NOME PACIENTE,
                               GEE.DESCR_GRADE_EXAME_ESTADO DESCR_EXAME_ESTADO,
                               DECODE(GE.EXTRA, 1, 'SIM', 'NÃO') EXTRA,
                               CASE WHEN GE.EXTRA = 1 THEN 'clRed' ELSE 'clBlack' END COR
                          FROM GSH_ESCALA_GRADE_EXAME GE
                         INNER JOIN GSH_SALA SA ON SA.ID_SALA = GE.ID_SALA
                         INNER JOIN GSH_EXAME EX ON EX.ID_EXAME = GE.ID_EXAME
                         INNER JOIN GSH_PRONTUARIO PR ON PR.ID_PRONTUARIO = EX.PRONTUARIO_SOLICITANTE
                         INNER JOIN GSH_PESSOAS PE ON PE.ID_PESSOA = PR.ID_PESSOA
                         INNER JOIN GSH_GRADE_EXAME_ESTADO GEE
                            ON GEE.ID_GRADE_EXAME_ESTADO = GE.ID_GRADE_EXAME_ESTADO
                         WHERE GE.ID_ESCALA_EXAME = :idEscalaExame
                           AND GE.ID_GRADE_EXAME_ESTADO = 6
                       )
                 ORDER BY PERIODO_FINAL
                """;

            using var conn = oracleHelper.GetConnection();
            return conn.Query<EscalaExameDto>(query, new { idEscalaExame }).ToList();
        }

        public IEnumerable<dynamic> GetAgendamentosDoDia(string data)
        {
            const string query = """
                SELECT *
                  FROM GSH_ESCALA_GRADE_EXAME E
                 WHERE E.PERIODO_INICIAL >= TO_DATE(:data, 'DD/MM/YYYY')
                   AND E.PERIODO_INICIAL < TO_DATE(:data, 'DD/MM/YYYY') + 1
                   AND E.ID_GRADE_EXAME_ESTADO = 2
                """;

            using var conn = oracleHelper.GetConnection();
            return conn.Query(query, new { data }).ToList();
        }

        public IEnumerable<dynamic> GetAgendamentosPorPeriodo(DateTime dataInicial, DateTime dataFinal, int? idHospital)
        {
            const string query = """
                SELECT DISTINCT
                       B.ID_ESCALA_EXAME, A.ID_EXAME, B.ID_ESCALA_GRADE_EXAME, B.PERIODO_INICIAL,
                       TRIM(TO_CHAR(B.PERIODO_INICIAL, 'DD/MM/YY')) DATA,
                       TRIM(TO_CHAR(B.PERIODO_INICIAL, 'HH24:MI')) HORA,
                       TRIM(TO_CHAR(B.PERIODO_FINAL, 'HH24:MI')) HORAFIM,
                       HO.ID_HOSPITAL, A.PRONTUARIO_SOLICITANTE, C.NOME,
                       TRIM(TO_CHAR(C.CPF, '00000000000')) CPF,
                       TRIM(TO_CHAR(C.NASCIMENTO, 'DD/MM/YY')) NASCIMENTO,
                       A.ID_EXAME_ESTADO, EC.DESCR_EXAME_ESTADO DESCR, S.DESCR_SALA, S.ID_SALA,
                       MED.NOME NOME_MEDICO, MED.CPF CPF_MEDICO, HO.CNES, EN.NOME_FANTASIA,
                       C.NOME AS PACIENTE,
                       SUBSTR(DECODE(A.ID_PROCEDIMENTO_UNIFICADO,
                              NULL, PROC.ID_PROCEXAME || '-' || PROC.DESCR_PROCEXAME,
                              (SELECT DISTINCT UU.CODPROC || '-' || UU.NOME
                                 FROM GSH_PROCEDIMENTO_UNIFICADO UU
                                WHERE UU.CODPROC = A.ID_PROCEDIMENTO_UNIFICADO)), 1, 30) AS PROCEDIMENTO,
                       TO_CHAR(B.PERIODO_INICIAL, 'DY, DD/MM/YYYY') || ' - INICIO : ' ||
                           TO_CHAR(B.PERIODO_INICIAL, 'HH24:MI') AS ESCALA,
                       (SELECT DISTINCT CID.CID
                          FROM GSH_APAC AP, GSH_CID CID
                         WHERE AP.CID_PRINCIPAL = CID.ID_CID
                           AND AP.ID_APAC = A.ID_APAC) CID,
                       COALESCE(
                           (SELECT MAX(F.FONE) FROM GSH_FONE F WHERE F.ID_TIPO_FONE = 1 AND F.ID_PESSOA = D.ID_PESSOA),
                           (SELECT MAX(F.FONE) FROM GSH_FONE F WHERE F.ID_TIPO_FONE = 2 AND F.ID_PESSOA = D.ID_PESSOA),
                           (SELECT MAX(F.FONE) FROM GSH_FONE F WHERE F.ID_TIPO_FONE = 3 AND F.ID_PESSOA = D.ID_PESSOA),
                           (SELECT MAX(F.FONE) FROM GSH_FONE F WHERE F.ID_TIPO_FONE = 8 AND F.ID_PESSOA = D.ID_PESSOA),
                           (SELECT MAX(F.FONE) FROM GSH_FONE F WHERE F.ID_TIPO_FONE = 7 AND F.ID_PESSOA = D.ID_PESSOA),
                           (SELECT MAX(F.FONE) FROM GSH_FONE F WHERE F.ID_TIPO_FONE = 6 AND F.ID_PESSOA = D.ID_PESSOA),
                           '00000000') FONE
                  FROM GSH_EXAME A, GSH_ESCALA_GRADE_EXAME B, GSH_ESCALA_EXAME ES,
                       GSH_PESSOAS C, GSH_PESSOAS MED, GSH_PRONTUARIO D, GSH_EXAME_ESTADO EC,
                       GSH_SALA S, GSH_HOSPITAL HO, GSH_ENTIDADE EN, GSH_PROCEXAME PROC
                 WHERE A.ID_EXAME = B.ID_EXAME
                   AND C.ID_PESSOA(+) = D.ID_PESSOA
                   AND A.PRONTUARIO_SOLICITANTE = D.ID_PRONTUARIO(+)
                   AND EC.ID_EXAME_ESTADO = A.ID_EXAME_ESTADO
                   AND S.ID_SALA = B.ID_SALA
                   AND A.PROFISSIONAL_SOLICITANTE = MED.ID_PESSOA
                   AND HO.ID_HOSPITAL = EN.ID_ENTIDADE
                   AND TRUNC(B.PERIODO_INICIAL) BETWEEN :dataInicial AND :dataFinal
                   AND PROC.ID_PROCEXAME(+) = B.ID_PROCEXAME
                   AND ES.ID_HOSPITAL = HO.ID_HOSPITAL
                   AND ES.ID_ESCALA_EXAME = B.ID_ESCALA_EXAME
                   AND ES.ID_HOSPITAL = NVL(:idHospital, ES.ID_HOSPITAL)
                   AND A.FLAG_APAC = 0
                   AND A.ID_EXAME_ESTADO = 2
                 ORDER BY B.PERIODO_INICIAL
                """;

            using var conn = oracleHelper.GetConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("dataInicial", OracleDbType.Date, ParameterDirection.Input, dataInicial.Date);
            parameters.Add("dataFinal", OracleDbType.Date, ParameterDirection.Input, dataFinal.Date);
            parameters.Add("idHospital", OracleDbType.Int32, ParameterDirection.Input,
                idHospital.HasValue ? idHospital.Value : DBNull.Value);

            return conn.Query(query, parameters).ToList();
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
