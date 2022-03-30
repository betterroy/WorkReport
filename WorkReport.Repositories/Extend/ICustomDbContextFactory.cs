using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Repositories.Extend
{
    public interface ICustomDbContextFactory
    {
        public DbContext ConnWriteOrRead(WriteAndReadEnum writeAndRead);
    }
}
