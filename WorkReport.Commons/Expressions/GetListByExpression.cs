using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Expressions
{
    public class GetListByExpression
    {
        public static List<string> GetList(Expression columns)
        {
            ListBuilderVisitor vistor = new ListBuilderVisitor();
            vistor.Visit(columns);
            return vistor.Condition();
        }
    }
}
