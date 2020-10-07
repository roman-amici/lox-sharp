using System;
using System.Collections.Generic;

namespace lox_sharp
{
    public class ParserError : Exception
    {
        public Token ErrorToken { get; set; }

        public override string Message { get; }

        public ParserError(Token errorToken, string message)
        {
            ErrorToken = errorToken;
            Message = message;
        }
    }

    public class Parser
    {
        private List<Token> _tokens;

        //State machine
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        bool Check(TokenType t)
        {
            if (IsAtEnd())
            {
                return false;
            }
            else
            {
                return Peek().Type == t;
            }
        }

        Token Advance()
        {
            if (!IsAtEnd())
            {
                _current++;
            }

            return Previous();
        }

        bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        Token Peek()
        {
            return _tokens[_current];
        }

        Token Previous()
        {
            return _tokens[_current - 1];
        }

        bool Match(params TokenType[] toMatch)
        {
            foreach (TokenType t in toMatch)
            {
                if (Check(t))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        Token Consume(TokenType expected, string errorMessage)
        {
            if (Check(expected)) return Advance();

            throw new ParserError(Peek(), errorMessage);

        }

        void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.Semicolon) return;

                switch (Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;
                }

                Advance();
            }
        }

        //Grammar
        Expr Primary()
        {
            if (Match(TokenType.False)) return new Literal() { Value = false };
            if (Match(TokenType.True)) return new Literal() { Value = true };
            if (Match(TokenType.Nil)) return new Literal() { Value = null };

            if (Match(TokenType.Number, TokenType.String))
            {
                return new Literal() { Value = Previous().Literal };
            }

            if (Match(TokenType.LeftParen))
            {
                Expr expr = Expression();
                Consume(TokenType.RightParen, "Expected ')' after expression.");
                return new Grouping() { Expression = expr };
            }

            throw new Exception("Expected expression.");
        }

        Expr Unary()
        {
            if (Match(TokenType.Bang, TokenType.Minus))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Unary()
                {
                    Operator = op,
                    Right = right
                };
            }
            else
            {
                return Primary();
            }
        }

        Expr Factor()
        {
            Expr expr = Unary();

            while (Match(TokenType.Slash, TokenType.Star))
            {
                Token op = Previous();
                Expr right = Unary();
                expr = new Binary()
                {
                    Left = expr,
                    Right = right,
                    Operator = op,
                };
            }

            return expr;
        }

        Expr Term()
        {
            Expr expr = Factor();

            while (Match(TokenType.Minus, TokenType.Plus))
            {
                Token op = Previous();
                Expr right = Factor();
                expr = new Binary()
                {
                    Left = expr,
                    Right = right,
                    Operator = op,
                };
            }

            return expr;
        }

        //Grammar
        Expr Comparison()
        {
            Expr expr = Term();

            while (Match(
                TokenType.Greater,
                TokenType.GreaterEqual,
                TokenType.Less,
                TokenType.LessEqual))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new Binary()
                {
                    Left = expr,
                    Right = right,
                    Operator = op,
                };
            }

            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Binary()
                {
                    Left = expr,
                    Operator = op,
                    Right = right,
                };
            }

            return expr;
        }

        Expr Expression()
        {
            return Equality();
        }

        //Interface
        static void ReportError(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Console.WriteLine($"{token.Line}, at end {message}");
            }
            else
            {
                Console.WriteLine($"{token.Line} at {message}");
            }
        }

        public Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParserError error)
            {
                ReportError(error.ErrorToken, error.Message);
                return null;
            }
        }
    }
}