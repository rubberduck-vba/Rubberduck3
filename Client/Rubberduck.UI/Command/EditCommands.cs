using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command
{
    public static class EditCommands
    {
        public static RoutedCommand GoToLineCommand { get; }
            = new RoutedCommand(nameof(GoToLineCommand), typeof(Window));
        public static RoutedCommand GoToAnythingCommand { get; }
            = new RoutedCommand(nameof(GoToAnythingCommand), typeof(Window));
        public static RoutedCommand GoToFileCommand { get; }
            = new RoutedCommand(nameof(GoToFileCommand), typeof(Window));
        public static RoutedCommand GoToMemberCommand { get; }
            = new RoutedCommand(nameof(GoToMemberCommand), typeof(Window));
        public static RoutedCommand GoToSymbolCommand { get; }
            = new RoutedCommand(nameof(GoToSymbolCommand), typeof(Window));
        public static RoutedCommand GoToNextIssueCommand { get; }
            = new RoutedCommand(nameof(GoToNextIssueCommand), typeof(Window));
        public static RoutedCommand GoToPreviousIssueCommand { get; }
            = new RoutedCommand(nameof(GoToPreviousIssueCommand), typeof(Window));
        public static RoutedCommand GoToPreviousLocationCommand { get; }
            = new RoutedCommand(nameof(GoToPreviousLocationCommand), typeof(Window));

        public static RoutedCommand FindCommand { get; }
            = new RoutedCommand(nameof(FindCommand), typeof(Window));
        public static RoutedCommand ReplaceCommand { get; }
            = new RoutedCommand(nameof(ReplaceCommand), typeof(Window));
        public static RoutedCommand UndoCommand { get; }
            = new RoutedCommand(nameof(UndoCommand), typeof(Window));
        public static RoutedCommand RedoCommand { get; }
            = new RoutedCommand(nameof(RedoCommand), typeof(Window));
        public static RoutedCommand CutCommand { get; }
            = new RoutedCommand(nameof(CutCommand), typeof(Window));
        public static RoutedCommand CopyCommand { get; }
            = new RoutedCommand(nameof(CopyCommand), typeof(Window));
        public static RoutedCommand PasteCommand { get; }
            = new RoutedCommand(nameof(PasteCommand), typeof(Window));
        public static RoutedCommand DeleteCommand { get; }
            = new RoutedCommand(nameof(DeleteCommand), typeof(Window));
        public static RoutedCommand SelectAllCommand { get; }
            = new RoutedCommand(nameof(SelectAllCommand), typeof(Window));
        public static RoutedCommand SelectContainingLineCommand { get; }
            = new RoutedCommand(nameof(SelectContainingLineCommand), typeof(Window));
        public static RoutedCommand SelectContainingBlockCommand { get; }
            = new RoutedCommand(nameof(SelectContainingBlockCommand), typeof(Window));
        public static RoutedCommand SelectContainingMemberCommand { get; }
            = new RoutedCommand(nameof(SelectContainingMemberCommand), typeof(Window));

        public static RoutedCommand FormatDocumentCommand { get; }
            = new RoutedCommand(nameof(FormatDocumentCommand), typeof(Window));
        public static RoutedCommand FormatSelectionCommand { get; }
            = new RoutedCommand(nameof(FormatSelectionCommand), typeof(Window));
        public static RoutedCommand IncreaseIndentationCommand { get; }
            = new RoutedCommand(nameof(IncreaseIndentationCommand), typeof(Window));
        public static RoutedCommand DecreaseIndentationCommand { get; }
            = new RoutedCommand(nameof(DecreaseIndentationCommand), typeof(Window));
        public static RoutedCommand MakeUppercaseCommand { get; }
            = new RoutedCommand(nameof(MakeUppercaseCommand), typeof(Window));
        public static RoutedCommand MakeLowercaseCommand { get; }
            = new RoutedCommand(nameof(MakeLowercaseCommand), typeof(Window));
        public static RoutedCommand DeleteHorizontalWhitespaceCommand { get; }
            = new RoutedCommand(nameof(DeleteHorizontalWhitespaceCommand), typeof(Window));
        public static RoutedCommand DeleteVerticalWhitespaceCommand { get; }
            = new RoutedCommand(nameof(DeleteVerticalWhitespaceCommand), typeof(Window));
        public static RoutedCommand MoveSelectedLinesUpCommand { get; }
            = new RoutedCommand(nameof(MoveSelectedLinesUpCommand), typeof(Window));
        public static RoutedCommand MoveSelectedLinesDownCommand { get; }
            = new RoutedCommand(nameof(MoveSelectedLinesDownCommand), typeof(Window));
        public static RoutedCommand MoveContainingMemberUpCommand { get; }
            = new RoutedCommand(nameof(MoveContainingMemberUpCommand), typeof(Window));
        public static RoutedCommand MoveContainingMemberDownCommand { get; }
            = new RoutedCommand(nameof(MoveContainingMemberDownCommand), typeof(Window));
        public static RoutedCommand CommentSelectedLinesCommand { get; }
            = new RoutedCommand(nameof(CommentSelectedLinesCommand), typeof(Window));
        public static RoutedCommand UncommentSelectedLinesCommand { get; }
            = new RoutedCommand(nameof(UncommentSelectedLinesCommand), typeof(Window));

        public static RoutedCommand ExpandAllFoldingsCommand { get; }
            = new RoutedCommand(nameof(ExpandAllFoldingsCommand), typeof(Window));
        public static RoutedCommand CollapseAllFoldingsCommand { get; }
            = new RoutedCommand(nameof(CollapseAllFoldingsCommand), typeof(Window));

        public static RoutedCommand ListMembersCommand { get; }
            = new RoutedCommand(nameof(ListMembersCommand), typeof(Window));
        public static RoutedCommand SignatureHelpCommand { get; }
            = new RoutedCommand(nameof(SignatureHelpCommand), typeof(Window));
        public static RoutedCommand QuickInfoCommand { get; }
            = new RoutedCommand(nameof(QuickInfoCommand), typeof(Window));
        public static RoutedCommand CompleteWordCommand { get; }
            = new RoutedCommand(nameof(CompleteWordCommand), typeof(Window));
        public static RoutedCommand SurroundWithCommand { get; }
            = new RoutedCommand(nameof(SurroundWithCommand), typeof(Window));
        public static RoutedCommand InsertSnippetCommand { get; }
            = new RoutedCommand(nameof(InsertSnippetCommand), typeof(Window));

        public static RoutedCommand RefactorRenameCommand { get; }
            = new RoutedCommand(nameof(RefactorRenameCommand), typeof(Window));
        public static RoutedCommand ExtractParameterCommand { get; }
            = new RoutedCommand(nameof(ExtractParameterCommand), typeof(Window));
        public static RoutedCommand ExtractMethodCommand { get; }
            = new RoutedCommand(nameof(ExtractMethodCommand), typeof(Window));
        public static RoutedCommand ExtractClassCommand { get; }
            = new RoutedCommand(nameof(ExtractClassCommand), typeof(Window));
        public static RoutedCommand ExtractInterfaceCommand { get; }
            = new RoutedCommand(nameof(ExtractInterfaceCommand), typeof(Window));
        public static RoutedCommand MoveDeclarationNearUsageCommand { get; }
            = new RoutedCommand(nameof(MoveDeclarationNearUsageCommand), typeof(Window));
        public static RoutedCommand ChangeSignatureCommand { get; }
            = new RoutedCommand(nameof(ChangeSignatureCommand), typeof(Window));
    }
}
