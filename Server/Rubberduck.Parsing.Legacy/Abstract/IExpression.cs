namespace Rubberduck.Parsing.Abstract
{
    public interface IExpression
    {
        IValue Evaluate();
        bool EvaluateCondition();
    }
}
