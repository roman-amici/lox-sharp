using System;

namespace lox_sharp
{
    public class AstPrinter : IVisitor
    {
        public object CallBinary(Binary b)
        {
            string l_str = (string)b.Left.Visit(this);
            string r_str = (string)b.Right.Visit(this);

            return $"({b.Operator.Display()} {l_str} {r_str})";
        }

        public object CallGrouping(Grouping g)
        {
            string inner = (string)g.Expression.Visit(this);
            return $"({inner})";
        }

        public object CallLiteral(Literal l)
        {
            return l.Value.ToString();
        }

        public object CallUnary(Unary u)
        {
            return $"({u.Operator.Display()} {u.Right.Visit(this)}";
        }

        public string ToString(Expr e)
        {
            return (string)e.Visit(this);
        }
    }

}