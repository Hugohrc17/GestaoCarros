using System.Linq.Expressions;

namespace ImobiFlow.Api.Core.Interfaces
{
    public interface IRepositorioGenerico
    {
        // Métodos de Leitura
        Task<T?> ObterPorIdAsync<T>(Guid id) where T : class;
        Task<T?> ObterPorAsync<T>(Expression<Func<T, bool>> filtro) where T : class;
        Task<IEnumerable<T>> ObterTodosPorIdAsync<T>(Expression<Func<T, bool>> filtro) where T : class;
        Task<IEnumerable<T>> ObterTodosAsync<T>(Expression<Func<T, bool>> filtro) where T : class;
        Task<IEnumerable<T>> ObterTodosSemFiltrosAsync<T>() where T : class;
        Task<IEnumerable<T>> ObterComIncludeAsync<T>(params Expression<Func<T, object>>[] includes) where T : class;
        Task<IEnumerable<T>> ObterComFiltroEIncludeAsync<T>(
            Expression<Func<T, bool>> filtro, 
            params Expression<Func<T, object>>[] includes) where T : class;
        Task<bool> ExisteAsync<T>(Guid id) where T : class;
        Task<bool> ExisteAsync<T>(Expression<Func<T, bool>> filtro) where T : class;

        // Métodos de Escrita
        Task<T> AdicionarAsync<T>(T entidade) where T : class;
        Task<IEnumerable<T>> AdicionarVariosAsync<T>(IEnumerable<T> entidades) where T : class;
        Task AtualizarAsync<T>(T entidade) where T : class;
        Task AtualizarVariosAsync<T>(IEnumerable<T> entidades) where T : class;
        Task ExcluirAsync<T>(Guid id) where T : class;
        Task ExcluirAsync<T>(T entidade) where T : class;
        Task ExcluirVariosAsync<T>(IEnumerable<T> entidades) where T : class;
        Task ExcluirVariosAsync<T>(Expression<Func<T, bool>> filtro) where T : class;
        
        // Operações de Transação
        Task SalvarMudancasAsync();
    }
}