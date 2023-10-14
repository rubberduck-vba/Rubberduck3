using Antlr4.Runtime;
using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing.Expressions;

public sealed class DateValue : IValue
{
    private readonly System.DateTime _value;

    public DateValue(System.DateTime value)
    {
        _value = value;
    }

    public ValueType ValueType
    {
        get
        {
            return ValueType.Date;
        }
    }

    public bool AsBool
    {
        get
        {
            return new DecimalValue(AsDecimal).AsBool;
        }
    }

    public byte AsByte
    {
        get
        {
            return new DecimalValue(AsDecimal).AsByte;
        }
    }

    public System.DateTime AsDate
    {
        get
        {
            return _value;
        }
    }

    public decimal AsDecimal
    {
        get
        {
            return (decimal)(_value).ToOADate();
        }
    }

    public string AsString
    {
        get
        {
            if (_value.Date == VBADateConstants.EPOCH_START.Date)
            {
                return _value.ToLongTimeString();
            }
            return _value.ToShortDateString();
        }
    }

    public IEnumerable<IToken> AsTokens
    {
        get
        {
            return new List<IToken>();
        }
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}
