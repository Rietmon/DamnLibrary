using System;
using System.Text;
using System.Collections;
using DamnLibrary.Debugging;

namespace Tiny
{
    public class JsonBuilder
    {
        private readonly StringBuilder builder = new StringBuilder();
        private readonly bool pretty;
        private int level;

        public JsonBuilder()
        {
        }

        public JsonBuilder(bool pretty)
        {
            this.pretty = pretty;
        }

        private void AppendPrettyLineBreak()
        {
            builder.Append("\n");
            for (var i = 0; i < level; i++)
            {
                builder.Append("\t");
            }
        }

        private bool HasPrettyLineBreak()
        {
            return builder.ToString().EndsWith("\t", StringComparison.InvariantCulture) ||
                   builder.ToString().EndsWith("\n", StringComparison.InvariantCulture);
        }

        private void RemovePrettyLineBreak()
        {
            while (HasPrettyLineBreak())
            {
                builder.Remove(builder.Length - 1, 1);
            }
        }

        public void AppendBeginObject()
        {
            level++;
            builder.Append("{");
            if (pretty) AppendPrettyLineBreak();
        }

        public void AppendEndObject()
        {
            level--;
            if (pretty) RemovePrettyLineBreak();
            if (pretty) AppendPrettyLineBreak();
            builder.Append("}");
            if (pretty) AppendPrettyLineBreak();
        }

        public void AppendBeginArray()
        {
            level++;
            builder.Append("[");
            if (pretty) AppendPrettyLineBreak();
        }

        public void AppendEndArray()
        {
            level--;
            if (pretty) RemovePrettyLineBreak();
            if (pretty) AppendPrettyLineBreak();
            builder.Append("]");
            if (pretty) AppendPrettyLineBreak();
        }

        public void AppendSeperator()
        {
            if (pretty) RemovePrettyLineBreak();
            builder.Append(",");
            if (pretty) AppendPrettyLineBreak();
        }

        public void AppendNull()
        {
            builder.Append("null");
        }

        public void AppendBool(bool b)
        {
            builder.Append(b ? "true" : "false");
        }

        public void AppendNumber(object number)
        {
            if (number != null)
            {
                var numberString = number.ToString();
                if (number.GetType().IsFloatingPoint())
                {
                    numberString = numberString.Replace(',', '.');
                    if (!numberString.Contains(".")) numberString += ".0";
                }

                builder.Append(numberString);
            }
            else
            {
                AppendNull();
            }
        }

        public void AppendString(string str)
        {
            if (str != null)
            {
                builder.Append('\"');
                foreach (var c in str)
                {
                    switch (c)
                    {
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            var codepoint = Convert.ToInt32(c);
                            if (pretty || (codepoint >= 32 && codepoint <= 126))
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                            }

                            break;
                    }
                }

                builder.Append('\"');
            }
            else
            {
                AppendNull();
            }
        }

        public void AppendArray(IEnumerable enumerable)
        {
            if (enumerable != null)
            {
                AppendBeginArray();
                var first = true;
                foreach (var item in enumerable)
                {
                    if (first) first = false;
                    else AppendSeperator();
                    AppendValue(item);
                }

                AppendEndArray();
            }
            else
            {
                AppendNull();
            }
        }

        public void AppendDictionary(IDictionary dict)
        {
            if (dict != null)
            {
                AppendBeginObject();
                var first = true;
                foreach (DictionaryEntry entry in dict)
                {
                    if (first) first = false;
                    else AppendSeperator();
                    AppendString(entry.Key.ToString());
                    builder.Append(pretty ? " : " : ":");
                    AppendValue(entry.Value);
                }

                AppendEndObject();
            }
            else
            {
                AppendNull();
            }
        }

        public void AppendValue(object value)
        {
            if (value == null)
            {
                AppendNull();
            }
            else if (value is bool)
            {
                AppendBool((bool)value);
            }
            else if (value is string)
            {
                AppendString((string)value);
            }
            else if (value is char)
            {
                AppendString("" + value);
            }
            else if (IsEnum(value))
            {
                AppendNumber((int)value);
            }
            else if (IsNumber(value))
            {
                AppendNumber(value);
            }
            else
            {
                UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(AppendValue)}) Type " + value.GetType() + " not supported!");
            }
        }

        public void AppendName(string name)
        {
            AppendString(name);
            builder.Append(pretty ? " : " : ":");
        }

        internal static bool IsNumber(object value)
        {
            return value != null && value.GetType().IsNumeric();
        }

        internal static bool IsEnum(object value)
        {
            return value != null && value.GetType().IsEnum;
        }

        internal static bool IsSupported(object obj)
        {
            switch (obj)
            {
                case null:
                case bool _:
                case string _:
                case char _:
                    return true;
            }

            return IsEnum(obj) || IsNumber(obj);
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}