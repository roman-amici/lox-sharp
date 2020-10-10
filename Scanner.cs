using System.Collections.Generic;
using System;

namespace lox_sharp
{
    public class Scanner
    {
        static Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>()
        {
            {"and", TokenType.And },
            {"class", TokenType.Class},
            {"else", TokenType.Else},
            {"false", TokenType.False},
            {"for", TokenType.For},
            {"fun", TokenType.Fun},
            {"if", TokenType.If},
            {"nil", TokenType.Nil},
            {"or", TokenType.Or},
            {"print", TokenType.Print},
            {"return", TokenType.Return},
            {"super", TokenType.Super},
            {"this", TokenType.This},
            {"true", TokenType.True},
            {"var", TokenType.Var},
            {"while", TokenType.While}
        };

        string SourceLine;
        List<Token> Tokens = new List<Token>();

        public bool ShouldExecute = true;

        int Start = 0;
        int Current = 0;
        int Line = 1;

        public Scanner(string source)
        {
            SourceLine = source;
        }

        public bool IsAtEnd()
        {
            return Current >= SourceLine.Length;
        }

        char Advance()
        {
            Current++;
            return SourceLine[Current - 1];
        }

        void AddToken(TokenType type, Object literal = null)
        {
            string text = SourceLine.Substring(Start, Current - Start);
            Tokens.Add(new Token(type, text, literal, Line));
        }

        bool Match(char expected)
        {
            if (IsAtEnd())
                return false;
            if (SourceLine[Current] != expected)
                return false;

            Current++;
            return true;
        }

        char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }
            else
            {
                return SourceLine[Current];
            }
        }

        void ParseString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') Line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.ReportError(Line, "Unterminated String!");
                return;
            }

            Advance(); //Consume the final "

            string inner = SourceLine.Substring(Start + 1, Current - Start - 2);
            AddToken(TokenType.String, inner);
        }

        bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        char PeekNext()
        {
            if (Current + 1 >= SourceLine.Length)
            {
                return '\0';
            }
            else
            {
                return SourceLine[Current + 1];
            }
        }

        void ParseNumber()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();
            }

            while (IsDigit(Peek())) Advance();

            string span = SourceLine.Substring(Start, Current - Start);
            double d = double.Parse(span);
            AddToken(TokenType.Number, d);
        }

        bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        void ParseIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = SourceLine.Substring(Start, Current - Start);
            if (Keywords.ContainsKey(text))
            {
                AddToken(Keywords[text]);
            }
            else
            {
                AddToken(TokenType.Identifier);
            }

        }

        void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(':
                    AddToken(TokenType.LeftParen); break;
                case ')':
                    AddToken(TokenType.RightParen); break;
                case '{':
                    AddToken(TokenType.LeftBrace); break;
                case '}':
                    AddToken(TokenType.RightBrace); break;
                case ',':
                    AddToken(TokenType.Comma); break;
                case '.':
                    AddToken(TokenType.Dot); break;
                case '-':
                    AddToken(TokenType.Minus); break;
                case '+':
                    AddToken(TokenType.Plus); break;
                case ';':
                    AddToken(TokenType.Semicolon); break;
                case '*':
                    AddToken(TokenType.Star); break;
                case '<':
                    AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '=':
                    AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '>':
                    AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(TokenType.Slash);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break; //Consume whitespace without doing anything
                case '\n':
                    Line++;
                    break;
                case '"':
                    ParseString();
                    break;
                case var _ when IsDigit(c):
                    ParseNumber();
                    break;
                case var _ when IsAlpha(c):
                    ParseIdentifier();
                    break;
                default:
                    ShouldExecute = false;
                    Lox.ReportError(Line, $"Unexpected Character '{c}'");
                    break;
            };
        }

        public List<Token> Scan()
        {
            while (!IsAtEnd())
            {
                Start = Current;
                ScanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, Line));
            return Tokens;
        }
    }
}