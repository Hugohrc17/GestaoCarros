using System.Linq.Expressions;
using GestãoCarros.Data;
using ImobiFlow.Api.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoCarros.Services.Repo
{
    public class RepositorioGenerico : IRepositorioGenerico
    {
        private readonly AppDbContext _contexto;

        public RepositorioGenerico(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<T?> ObterPorIdAsync<T>(Guid id) where T : class
        {
            return await _contexto.Set<T>().FindAsync(id);
        }

        public async Task<T?> ObterPorAsync<T>(Expression<Func<T, bool>> filtro) where T : class
        {
            return await _contexto.Set<T>().FirstOrDefaultAsync(filtro);
        }

        public async Task<IEnumerable<T>> ObterTodosPorIdAsync<T>(Expression<Func<T, bool>> filtro) where T : class
        {
            return await _contexto.Set<T>()
                .Where(filtro)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> ObterTodosAsync<T>(Expression<Func<T, bool>> filtro) where T : class
        {
            return await _contexto.Set<T>().Where(filtro).ToListAsync();
        }

        public async Task<IEnumerable<T>> ObterTodosSemFiltrosAsync<T>() where T : class
        {
            return await _contexto.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> ObterComIncludeAsync<T>(params Expression<Func<T, object>>[] includes) where T : class
        {
            var query = _contexto.Set<T>().AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> ObterComFiltroEIncludeAsync<T>(
            Expression<Func<T, bool>> filtro,
            params Expression<Func<T, object>>[] includes) where T : class
        {
            var query = _contexto.Set<T>().AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(filtro).ToListAsync();
        }

        public async Task<bool> ExisteAsync<T>(Guid id) where T : class
        {
            return await _contexto.Set<T>().FindAsync(id) != null;
        }

        public async Task<bool> ExisteAsync<T>(Expression<Func<T, bool>> filtro) where T : class
        {
            return await _contexto.Set<T>().AnyAsync(filtro);
        }


        // Métodos de Escrita

        public async Task<T> AdicionarAsync<T>(T entidade) where T : class
        {
            await _contexto.Set<T>().AddAsync(entidade);
            await _contexto.SaveChangesAsync();
            return entidade;
        }

        public async Task<IEnumerable<T>> AdicionarVariosAsync<T>(IEnumerable<T> entidades) where T : class
        {
            await _contexto.Set<T>().AddRangeAsync(entidades);
            await _contexto.SaveChangesAsync();
            return entidades;
        }

        public async Task AtualizarAsync<T>(T entidade) where T : class
        {
            _contexto.Set<T>().Update(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task AtualizarVariosAsync<T>(IEnumerable<T> entidades) where T : class
        {
            _contexto.Set<T>().UpdateRange(entidades);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirAsync<T>(Guid id) where T : class
        {
            var entidade = await _contexto.Set<T>().FindAsync(id);
            if (entidade != null)
            {
                _contexto.Set<T>().Remove(entidade);
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task ExcluirAsync<T>(T entidade) where T : class
        {
            _contexto.Set<T>().Remove(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirVariosAsync<T>(IEnumerable<T> entidades) where T : class
        {
            _contexto.Set<T>().RemoveRange(entidades);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirVariosAsync<T>(Expression<Func<T, bool>> filtro) where T : class
        {
            var entidades = await _contexto.Set<T>().Where(filtro).ToListAsync();
            if (entidades.Any())
            {
                _contexto.Set<T>().RemoveRange(entidades);
                await _contexto.SaveChangesAsync();
            }
        }

        // Operações de Transação

        public async Task SalvarMudancasAsync()
        {
            await _contexto.SaveChangesAsync();
        }

        public Task<object?> ObterTodosPorIdAsync<T>(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}