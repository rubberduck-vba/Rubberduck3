using System.Collections.Concurrent;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Parsing.VBA.Parsing;

public class ParserStateEventArgs : EventArgs
{
    public ParserStateEventArgs(QualifiedModuleName module, ParserState state, ParserState oldState, CancellationToken token)
    {
        Module = module;
        State = state;
        OldState = oldState;
        Token = token;
    }

    public QualifiedModuleName Module { get; }
    public ParserState State { get; }
    public ParserState OldState { get; }
    public CancellationToken Token { get; }

    public bool IsError => 
        State == ParserState.ResolverError 
        || State == ParserState.Error 
        || State == ParserState.UnexpectedError;
}

public interface IParserStateManager
{
    ParserState GetModuleParserState(QualifiedModuleName module);
    void SetModuleParserState(QualifiedModuleName module, ParserState state, CancellationToken token);
    void SetModuleState(ModuleState state, CancellationToken token);

    event EventHandler<ParserStateEventArgs> StateChanged;
}

public class ParserStateManager : IParserStateManager
{
    private readonly ConcurrentDictionary<QualifiedModuleName, ModuleState> _moduleStates = new();

    public ParserState GetModuleParserState(QualifiedModuleName module)
    {
        return _moduleStates.TryGetValue(module, out var state) ? state.Status : ParserState.None;
    }

    public event EventHandler<ParserStateEventArgs> StateChanged;
    private void OnStateChanged(QualifiedModuleName module, ParserState newState, ParserState oldState, CancellationToken token) 
        => StateChanged?.Invoke(this, new ParserStateEventArgs(module, newState, oldState, token));

    public void SetModuleParserState(QualifiedModuleName module, ParserState state, CancellationToken token)
    {
        var oldState = (ModuleState)null;
        if (_moduleStates.TryGetValue(module, out var existing))
        {
            oldState = existing;
        }

        token.ThrowIfCancellationRequested();

        var moduleState = _moduleStates.GetOrAdd(module, qmn => new ModuleState(qmn, null));
        moduleState.Status = state;

        if (state != oldState?.Status)
        {
            token.ThrowIfCancellationRequested();
            OnStateChanged(module, state, oldState?.Status ?? ParserState.None, token);
        }
    }

    public void SetModuleState(ModuleState state, CancellationToken token)
    {
        var oldState = (ModuleState)null;
        if (_moduleStates.TryGetValue(state.Module, out var existing))
        {
            oldState = existing;
        }

        token.ThrowIfCancellationRequested();

        _moduleStates.AddOrUpdate(state.Module, state, (qmn, s) => state);

        if (state.Status != oldState?.Status)
        {
            token.ThrowIfCancellationRequested();
            OnStateChanged(state.Module, state.Status, oldState?.Status ?? ParserState.None, token);
        }
    }
}
