using Microsoft.EntityFrameworkCore;
using movieapp.data.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace movieapp.data.Concrete.EFCore
{
    public class EfCoreGenericRepository<TEntity, TContext> : IRepository<TEntity>
        where TContext : DbContext, new()
        where TEntity : class
    {
        private readonly TContext context;

        public EfCoreGenericRepository(TContext context)
        {
            this.context = context;
        }
        public async Task Create(TEntity entity)
        {   
                await context.Set<TEntity>().AddAsync(entity);
                await context.SaveChangesAsync();
            
        }

        public async Task Delete(TEntity entity)
        {
          
                context.Set<TEntity>().Remove(entity);
                await context.SaveChangesAsync();
            
        }

        public async Task<List<TEntity>> GetAll()
        {
           
                return await context.Set<TEntity>().ToListAsync();
            
        }

        public async Task<TEntity> GetById(int id)
        {
           
                return await context.Set<TEntity>().FindAsync(id);
            
        }


        public async Task Update(TEntity entity)
        {
          
                context.Entry(entity).State = EntityState.Modified;
                await context.SaveChangesAsync();
            
        }
    }
}
