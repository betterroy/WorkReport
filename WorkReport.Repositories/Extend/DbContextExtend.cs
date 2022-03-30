using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkReport.Repositories.Extend
{
    public static class DbContextExtend
    {
        public static DbContext ToWriteOrRead(this DbContext dbContext, string conn)
        {
            if (dbContext is WorkReportContext)
            {

                WorkReportContext context = (WorkReportContext)dbContext; // context 是 EFCoreContext 实例； 
                return context.ToWriteOrRead(conn);
            }
            else
                throw new Exception();
        }
    }
}
