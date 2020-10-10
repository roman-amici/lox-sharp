using System;

namespace lox_sharp
{
    public class RunTimeError : Exception
    {

        public Token ErrorToken { get; private set; }

        public RunTimeError(Token token, string message) : base(message)
        {
            ErrorToken = token;
        }

    }

    public class Interpreter : IVisitor
    {
        object evaluate(Expr e)
        {
            return e.Visit(this);
        }

        public object CallGrouping(Grouping g)
        {
            return evaluate(g.Expression);
        }

        public object CallLiteral(Literal l)
        {
            return l.Value;
        }

        public object CallUnary(Unary u)
        {
            object right = evaluate(u.Right);

            switch (u.Operator.Type)
            {
                case TokenType.Minus:
                    ExpectNumber(u.Operator, right);
                    return -(double)right;
                case TokenType.Bang:
                    return !IsTruthy(right);
                default:
                    throw new Exception("Unknown unary operator.");
            }
        }

        public object CallBinary(Binary b)
        {
            object left = evaluate(b.Left);
            object right = evaluate(b.Right);

            switch (b.Operator.Type)
            {
                case TokenType.Plus:
                    if ((left is string) && (right is string))
                    {
                        return (string)left + (string)right;
                    }
                    else
                    {
                        ExpectNumber(b.Operator, left); ExpectNumber(b.Operator, right);
                        return (double)left + (double)right;
                    }
                case TokenType.Minus:
                    ExpectNumber(b.Operator, left); ExpectNumber(b.Operator, right);
                    return (double)left - (double)right;
                case TokenType.Slash:
                    ExpectNumber(b.Operator, left); ExpectNumber(b.Operator, right);
                    return (double)left / (double)right;
                case TokenType.Star:
                    ExpectNumber(b.Operator, left); ExpectNumber(b.Operator, right);
                    return (double)left * (double)right;
                case TokenType.Greater:
                    ExpectNumber(b.Operator, left); ExpectNumber(b.Operator, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    ExpectNumber(b.Operator, left); ExpectNumber(b.Operator, right);
                    return (double)left >= (double)right;
                case TokenType.Less:
                    ExpectNumber(b.Operator, left); ExpectNumber(b.Operator, right);
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    ExpectNumber(b.Operator, left); ExpectNumber(b.Operator, right);
                    return (double)left <= (double)right;
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.EqualEqual:
                    return IsEqual(left, right);
                default:
                    throw new Exception("Unknown binary operator.");
            }
        }

        //Helper Methods
        private bool IsTruthy(object value)
        {
            if (value == null) return false;
            if (value is bool) return (bool)value;
            return true;
        }

        private void ExpectNumber(Token op, object value)
        {
            if (value is double) return;
            throw new RunTimeError(op, "Operand must be a number");
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        //Entrypoint
        public bool Interpret(Expr expression)
        {
            try
            {
                object result = evaluate(expression);
                Console.WriteLine(result.ToString());
            }
            catch (RunTimeError e)
            {
                Console.WriteLine($"{e.ErrorToken.Line} {e.Message}");
                return false;
            }

            return true;
        }

    }
}