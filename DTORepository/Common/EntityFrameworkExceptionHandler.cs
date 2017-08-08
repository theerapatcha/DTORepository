using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Common
{
    public static class EntityFrameworkExceptionHandler
    {
        public static ISuccessOrErrors HandleException(Exception ex)
        {
            var errors = new SuccessOrErrors();
            if(ex is DbEntityValidationException)
            {
                var evx = (DbEntityValidationException)ex;
                foreach (var eve in evx.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errors.AddNamedParameterError(ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            else if (ex is DbUpdateException)
            {
                SqlException s = ex.InnerException.InnerException as SqlException;
                var uex = (DbUpdateException)ex;
                foreach (var eve in uex.Entries)
                {
                    errors.AddNamedParameterError(
                        eve.Entity.GetType().Name,
                        $"Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated, [{ex.InnerException.ToString()}]"
                    );
                }
            }
            else
            {
                errors.AddSingleError(ex.Message, ex);
            }
            return errors;
        }
    }
}
