using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Expressions
{
    public class ListBuilderVisitor : ExpressionVisitor
    {
        private Stack<string> _StringStack = new Stack<string>();

        public List<string> Condition()
        {
            var list = this._StringStack.ToList();
            this._StringStack.Clear();
            return list;
        }

        /// <summary>
        /// 解析属性
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null) throw new ArgumentNullException("MemberExpression");
            if (node.Expression is ConstantExpression)
            {
            }
            else
            {
                this._StringStack.Push(node.Member.Name);
            }
            return node;
        }

    }
}
