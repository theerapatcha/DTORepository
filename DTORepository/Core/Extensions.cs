using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Core
{
    public static class Extensions
    {
        public static TResult MapTo<TResult>(this object entity, DbContext context)
        {
            return DTORepositoryContainer.Mapper.Map<TResult>(entity, opts =>
            {
                opts.Items["DbContext"] = context;
            });
        }
    }
}
