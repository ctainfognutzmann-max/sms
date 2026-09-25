
// ITUserRepository.cs
namespace webApiAghos.Repositories
{
    public interface ITUserRepository
    {
        object? GetTExamesList(int tmpOffset = 0, int tmpFecht = 10);
        object? GetTExamesList3();
        object? GetTexamesDetail(int tmpId);
        object? GetTexamesDetail2(int tmpId);
        IEnumerable<EscalaExameDto> GetEscalaExames(int idEscalaExame);
        IEnumerable<dynamic> GetAgendamentosDoDia(string data);
        IEnumerable<dynamic> GetAgendamentosPorPeriodo(DateTime dataInicial, DateTime dataFinal, int? idHospital);

    }
}
