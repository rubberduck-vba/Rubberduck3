using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;
using Rubberduck.VBEditor;
using System;

namespace Rubberduck.Parsing
{
    public readonly struct QualifiedContext<TContext> : IEquatable<QualifiedContext<TContext>>
        where TContext : VBABaseParserRuleContext
    {
        public QualifiedContext(QualifiedMemberName member, TContext context)
            : this(member.QualifiedModuleName, context)
        {
            MemberName = member;
        }

        public QualifiedContext(QualifiedModuleName module, TContext context)
        {
            MemberName = QualifiedMemberName.None;
            ModuleName = module;
            Context = context;
            Offset = new QualifiedDocumentOffset(module, context.Offset);
        }

        public QualifiedDocumentOffset Offset { get; } 

        public QualifiedModuleName ModuleName { get; }
        public QualifiedMemberName MemberName { get; }
        public TContext Context { get; }

        public bool Equals(QualifiedContext<TContext> other)
        {
            return ModuleName.Equals(other.ModuleName)
                && MemberName.Equals(other.MemberName);
        }

        public override bool Equals(object obj)
        {
            return Equals((QualifiedContext<TContext>)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModuleName, MemberName);
        }
    }
}
