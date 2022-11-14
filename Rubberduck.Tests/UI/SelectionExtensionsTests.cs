using ICSharpCode.AvalonEdit.Document;
using Rubberduck.UI;
using Rubberduck.VBEditor;


namespace Rubberduck.Tests.UI
{
    [TestClass]
    public class SelectionExtensionsTests
    {
        private TextDocument GetTestDocument()
        {
            var code = @"'@Folder ""Rubberduck3.Tests""
Option Explicit

Public Sub DoSomething()
    MsgBox ""Hello, world!""
End Sub
";

            return new TextDocument(code);
        }


        [TestMethod]
        public void ToTextEditorSelection_Home()
        {
            var selection = Selection.Home;
            var document = GetTestDocument();

            var (start, length) = selection.ToTextEditorSelection(document);

            Assert.AreEqual(1, start);
            Assert.AreEqual(1, length);
        }

        [TestMethod]
        public void ToTextEditorSelection_Multiline()
        {
            var startColumn = 10;
            var endColumn = 6;
            var selection = new Selection(1, startColumn, 2, endColumn);
            var document = GetTestDocument();

            var (start, length) = selection.ToTextEditorSelection(document);
            var line1 = document.GetLineByNumber(1);
            var line2 = document.GetLineByNumber(2);

            Assert.AreEqual(startColumn, start);
            Assert.AreEqual((line2.Offset + endColumn - 1) - (line1.Offset + startColumn - 1), length);
        }
    }
}