using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using DamnLibrary.Debugs;

namespace DamnLibrary.TinyJson
{
    public class JsonParser : IDisposable
    {
        private enum Token
        {
            None,
            CurlyOpen,
            CurlyClose,
            SquareOpen,
            SquareClose,
            Colon,
            Comma,
            String,
            Number,
            BoolOrNull
        };

        private StringReader json;

        // Temporary allocated
        private StringBuilder sb = new();

        public static object ParseValue(string jsonString)
        {
            using (var parser = new JsonParser(jsonString))
            {
                return parser.ParseValue();
            }
        }

        internal JsonParser(string jsonString)
        {
            json = new StringReader(jsonString);
        }

        public void Dispose()
        {
            json.Dispose();
            json = null;
        }

        //** Reading Token **//

        private bool EndReached()
        {
            return json.Peek() == -1;
        }

        private bool PeekWordbreak()
        {
            var c = PeekChar();
            return c == ' ' || c == ',' || c == ':' || c == '\"' || c == '{' || c == '}' || c == '[' || c == ']' ||
                   c == '\t' || c == '\n' || c == '\r';
        }

        private bool PeekWhitespace()
        {
            var c = PeekChar();
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        private char PeekChar()
        {
            return Convert.ToChar(json.Peek());
        }

        private char ReadChar()
        {
            return Convert.ToChar(json.Read());
        }

        private string ReadWord()
        {
            sb.Clear();
            while (!PeekWordbreak() && !EndReached())
            {
                sb.Append(ReadChar());
            }

            return EndReached() ? null : sb.ToString();
        }

        private void EatWhitespace()
        {
            while (PeekWhitespace())
            {
                json.Read();
            }
        }

        private Token PeekToken()
        {
            EatWhitespace();
            if (EndReached()) return Token.None;
            switch (PeekChar())
            {
                case '{':
                    return Token.CurlyOpen;
                case '}':
                    return Token.CurlyClose;
                case '[':
                    return Token.SquareOpen;
                case ']':
                    return Token.SquareClose;
                case ',':
                    return Token.Comma;
                case '"':
                    return Token.String;
                case ':':
                    return Token.Colon;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return Token.Number;
                case 't':
                case 'f':
                case 'n':
                    return Token.BoolOrNull;
                default:
                    return Token.None;
            }
        }

        //** Parsing Parts **//

        private object ParseBoolOrNull()
        {
            if (PeekToken() == Token.BoolOrNull)
            {
                var boolValue = ReadWord();
                if (boolValue == "true") return true;
                if (boolValue == "false") return false;
                if (boolValue == "null") return null;
                Console.WriteLine("Unexpected bool value: " + boolValue);
                return null;
            }
            else
            {
                Console.WriteLine("Unexpected bool token: " + PeekToken());
                return null;
            }
        }

        private object ParseNumber()
        {
            if (PeekToken() == Token.Number)
            {
                var number = ReadWord();
                if (number.Contains("."))
                {
                    if (double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                        return parsed;
                }
                else
                {
                    if (long.TryParse(number, out var parsed)) return parsed;
                }

                UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(ParseNumber)}) Unexpected number value: " + number);
                return null;
            }

            UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(ParseNumber)}) Unexpected number token: " + PeekToken());
            return null;
        }

        private string ParseString()
        {
            if (PeekToken() == Token.String)
            {
                ReadChar(); // ditch opening quote

                sb.Clear();
                char c;
                while (true)
                {
                    if (EndReached()) return null;

                    c = ReadChar();
                    switch (c)
                    {
                        case '"':
                            return sb.ToString();
                        case '\\':
                            if (EndReached()) return null;

                            c = ReadChar();
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    sb.Append(c);
                                    break;
                                case 'b':
                                    sb.Append('\b');
                                    break;
                                case 'f':
                                    sb.Append('\f');
                                    break;
                                case 'n':
                                    sb.Append('\n');
                                    break;
                                case 'r':
                                    sb.Append('\r');
                                    break;
                                case 't':
                                    sb.Append('\t');
                                    break;
                                case 'u':
                                    var hex = string.Concat(ReadChar(), ReadChar(), ReadChar(), ReadChar());
                                    sb.Append((char)Convert.ToInt32(hex, 16));
                                    break;
                            }

                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }
            }

            UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(ParseString)}) Unexpected string token: " + PeekToken());
            return null;
        }

        //** Parsing Objects **//

        private Dictionary<string, object> ParseObject()
        {
            if (PeekToken() == Token.CurlyOpen)
            {
                json.Read(); // ditch opening brace

                var table = new Dictionary<string, object>();
                while (true)
                {
                    switch (PeekToken())
                    {
                        case Token.None:
                            return null;
                        case Token.Comma:
                            json.Read();
                            continue;
                        case Token.CurlyClose:
                            json.Read();
                            return table;
                        default:
                            var name = ParseString();
                            if (string.IsNullOrEmpty(name)) return null;

                            if (PeekToken() != Token.Colon) return null;
                            json.Read(); // ditch the colon

                            table[name] = ParseValue();
                            break;
                    }
                }
            }

            UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(ParseObject)}) Unexpected object token: " + PeekToken());
            return null;
        }

        private List<object> ParseArray()
        {
            if (PeekToken() == Token.SquareOpen)
            {
                json.Read(); // ditch opening brace

                var array = new List<object>();
                while (true)
                {
                    switch (PeekToken())
                    {
                        case Token.None:
                            return null;
                        case Token.Comma:
                            json.Read();
                            continue;
                        case Token.SquareClose:
                            json.Read();
                            return array;
                        default:
                            array.Add(ParseValue());
                            break;
                    }
                }
            }

            UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(ParseArray)}) Unexpected array token: " + PeekToken());
            return null;
        }

        private object ParseValue()
        {
            switch (PeekToken())
            {
                case Token.String:
                    return ParseString();
                case Token.Number:
                    return ParseNumber();
                case Token.BoolOrNull:
                    return ParseBoolOrNull();
                case Token.CurlyOpen:
                    return ParseObject();
                case Token.SquareOpen:
                    return ParseArray();
            }

            UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(ParseValue)}) Unexpected value token: " + PeekToken());
            return null;
        }
    }
}