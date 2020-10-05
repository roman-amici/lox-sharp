namespace lox_sharp
{
    public interface IVisitor
    {
        object CallBinary(Binary b);
        object CallGrouping(Grouping g);
        object CallLiteral(Literal l);
        object CallUnary(Unary u);
    }

    public abstract class Expr
    {
        public abstract object Visit(IVisitor visitor);
    }

    public class Binary : Expr
    {
        public Expr Left { get; set; }
        public Token Operator { get; set; }
        public Expr Right { get; set; }

        public override object Visit(IVisitor visitor)
        {
            return visitor.CallBinary(this);
        }
    }

    public class Grouping : Expr
    {
        public Expr Expression { get; set; }

        public override object Visit(IVisitor visitor)
        {
            return visitor.CallGrouping(this);
        }
    }

    public class Literal : Expr
    {
        public object Value { get; set; }

        public override object Visit(IVisitor visitor)
        {
            return visitor.CallLiteral(this);
        }
    }

    public class Unary : Expr
    {
        public Token Operator { get; set; }
        public Expr Right { get; set; }

        public override object Visit(IVisitor visitor)
        {
            return visitor.CallUnary(this);
        }
    }

}