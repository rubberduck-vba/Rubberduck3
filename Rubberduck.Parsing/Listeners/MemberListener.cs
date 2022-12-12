using Antlr4.Runtime.Misc;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Parsing.Listeners
{
    public class MemberListener : VBAParserBaseListener
    {
        private readonly IList<MemberInfo> _members = new List<MemberInfo>();
        public IEnumerable<MemberInfo> Members => _members;

        private IEnumerable<ParameterInfo> GetParameterInfo(VBAParser.ArgListContext argList)
        {
            var args = argList?.arg() ?? Enumerable.Empty<VBAParser.ArgContext>();

            var ordinal = 0;
            foreach (var arg in args)
            {
                ordinal++;
                yield return new ParameterInfo(ordinal, arg);
            }
        }

        public override void EnterModule([NotNull] VBAParser.ModuleContext context)
        {
            base.EnterModule(context);
            _members.Clear();
        }

        private bool _inModuleDeclarationsSection = true;
        public override void EnterModuleBody([NotNull] VBAParser.ModuleBodyContext context)
        {
            _inModuleDeclarationsSection = false;
        }

        public override void ExitVariableStmt([NotNull] VBAParser.VariableStmtContext context)
        {
            if (!_inModuleDeclarationsSection)
            {
                return;
            }

            var isPrivate = context.DIM() != null || Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.FieldFriend
                : isPrivate ? MemberType.FieldPrivate : MemberType.Field;

            foreach (var variable in context.variableListStmt()?.variableSubStmt() ?? Enumerable.Empty<VBAParser.VariableSubStmtContext>())
            {
                _members.Add(new TypedMemberInfo(
                    variable.identifier()?.GetText()?? string.Empty,
                    variable.Offset,
                    memberType,
                    variable.asTypeClause()?.type()?.GetText() ?? Tokens.Variant));
            }
        }

        public override void ExitConstStmt([NotNull] VBAParser.ConstStmtContext context)
        {
            if (!_inModuleDeclarationsSection)
            {
                return;
            }

            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.ConstFriend
                : isPrivate ? MemberType.ConstPrivate : MemberType.Const;

            foreach (var constant in context.constSubStmt() ?? Enumerable.Empty<VBAParser.ConstSubStmtContext>())
            {
                _members.Add(new ValuedMemberInfo(
                    constant.identifier()?.GetText()?? string.Empty,
                    constant.Offset,
                    memberType,
                    constant.asTypeClause()?.type()?.GetText() ?? Tokens.Variant,
                    constant.expression()?.GetText() ?? Tokens.Empty));
            }
        }

        public override void ExitDeclareStmt([NotNull] VBAParser.DeclareStmtContext context)
        {
            if (!_inModuleDeclarationsSection)
            {
                return;
            }

            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.ProcedureFriend
                : isPrivate ? MemberType.ProcedurePrivate : MemberType.Procedure;

            var parameters = new List<ParameterInfo>(GetParameterInfo(context.argList()));

            _members.Add(new TypedMemberInfo(
                context.identifier()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                context.asTypeClause()?.type()?.GetText() ?? Tokens.Variant,
                parameters));
        }

        public override void ExitEventStmt([NotNull] VBAParser.EventStmtContext context)
        {
            if (!_inModuleDeclarationsSection)
            {
                return;
            }

            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.EventFriend
                : isPrivate ? MemberType.EventPrivate : MemberType.Event;

            var parameters = new List<ParameterInfo>(GetParameterInfo(context.argList()));

            _members.Add(new MemberInfo(
                context.identifier()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                parameters));
        }

        public override void ExitSubStmt([NotNull] VBAParser.SubStmtContext context)
        {
            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.ProcedureFriend
                : isPrivate ? MemberType.ProcedurePrivate : MemberType.Procedure;

            var parameters = new List<ParameterInfo>(GetParameterInfo(context.argList()));

            _members.Add(new MemberInfo(
                context.subroutineName()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                parameters));
        }

        public override void ExitFunctionStmt([NotNull] VBAParser.FunctionStmtContext context)
        {
            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.FunctionFriend
                : isPrivate ? MemberType.FunctionPrivate : MemberType.Function;

            var parameters = new List<ParameterInfo>(GetParameterInfo(context.argList()));

            _members.Add(new TypedMemberInfo(
                context.functionName()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                context.asTypeClause()?.type()?.GetText() ?? Tokens.Variant,
                parameters));
        }

        public override void ExitPropertyGetStmt([NotNull] VBAParser.PropertyGetStmtContext context)
        {
            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.PropertyGetFriend
                : isPrivate ? MemberType.PropertyGetPrivate : MemberType.PropertyGet;

            var parameters = new List<ParameterInfo>(GetParameterInfo(context.argList()));

            _members.Add(new TypedMemberInfo(
                context.functionName()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                context.asTypeClause()?.type()?.GetText() ?? Tokens.Variant,
                parameters));
        }

        public override void ExitPropertyLetStmt([NotNull] VBAParser.PropertyLetStmtContext context)
        {
            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.PropertyLetFriend
                : isPrivate ? MemberType.PropertyLetPrivate : MemberType.PropertyLet;

            var parameters = new List<ParameterInfo>(GetParameterInfo(context.argList()));

            _members.Add(new MemberInfo(
                context.subroutineName()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                parameters));
        }

        public override void ExitPropertySetStmt([NotNull] VBAParser.PropertySetStmtContext context)
        {
            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.PropertySetFriend
                : isPrivate ? MemberType.PropertySetPrivate : MemberType.PropertySet;

            var parameters = new List<ParameterInfo>(GetParameterInfo(context.argList()));

            _members.Add(new MemberInfo(
                context.subroutineName()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                parameters));
        }

        public override void ExitUdtDeclaration([NotNull] VBAParser.UdtDeclarationContext context)
        {
            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.UserDefinedTypeFriend
                : isPrivate ? MemberType.UserDefinedTypePrivate: MemberType.UserDefinedType;

            var members = GetMembers(context.udtMemberList()?.udtMember() ?? Enumerable.Empty<VBAParser.UdtMemberContext>());
            _members.Add(new MemberInfo(
                context.untypedIdentifier()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                members: members));
        }

        private IEnumerable<MemberInfo> GetMembers(IEnumerable<VBAParser.UdtMemberContext> members)
        {
            foreach (var member in members)
            {
                var name = member.untypedNameMemberDeclaration()?.untypedIdentifier()?.GetText()?? string.Empty;
                yield return new TypedMemberInfo(
                    name, 
                    member.Offset, 
                    MemberType.UserDefinedTypeMember,
                    member.reservedNameMemberDeclaration()?.asTypeClause()?.type()?.GetText() ?? Tokens.Variant);
            }
        }

        public override void ExitEnumerationStmt([NotNull] VBAParser.EnumerationStmtContext context)
        {
            var isPrivate = Tokens.Private.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var isFriend = Tokens.Friend.Equals(context.visibility()?.GetText(), StringComparison.InvariantCultureIgnoreCase);
            var memberType = isFriend ? MemberType.EnumFriend
                : isPrivate ? MemberType.EnumPrivate : MemberType.Enum;

            var members = GetMembers(context.enumerationStmt_Constant() ?? Enumerable.Empty<VBAParser.EnumerationStmt_ConstantContext>());
            _members.Add(new MemberInfo(
                context.identifier()?.GetText()?? string.Empty,
                context.Offset,
                memberType,
                members: members));
        }

        private IEnumerable<ValuedMemberInfo> GetMembers(IEnumerable<VBAParser.EnumerationStmt_ConstantContext> members)
        {
            foreach (var member in members)
            {
                var name = member.identifier()?.GetText()?? string.Empty;
                yield return new ValuedMemberInfo(
                    name,
                    member.Offset,
                    MemberType.EnumMember,
                    Tokens.Long,
                    member.expression()?.GetText() ?? string.Empty);
            }
        }
    }
}
