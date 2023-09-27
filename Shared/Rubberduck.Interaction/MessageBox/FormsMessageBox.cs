using static System.Net.Mime.MediaTypeNames;
using Forms = System.Windows.Forms;

namespace Rubberduck.Interaction.MessageBox
{
    public class MessageBoxViewModel
    {
        public string Message { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;

        public Forms.MessageBoxButtons Buttons { get; set; } = Forms.MessageBoxButtons.OK;
        public Forms.MessageBoxIcon Icon { get; set; } = Forms.MessageBoxIcon.Information;
     
        public ConfirmationOutcome? Result { get; set; }
    }

    public class FormsMessageBox : IMessageBox
    {
        public void Show(MessageBoxViewModel viewModel)
        {
            Forms.MessageBox.Show(viewModel.Message, viewModel.Caption, viewModel.Buttons, viewModel.Icon);
        }

        public ConfirmationOutcome Confirm(MessageBoxViewModel viewModel)
        {
            var result = Forms.MessageBox.Show(viewModel.Message, viewModel.Caption, viewModel.Buttons, viewModel.Icon);
            switch (result)
            {
                case Forms.DialogResult.Cancel:
                    return ConfirmationOutcome.Cancel;
                case Forms.DialogResult.Yes:
                case Forms.DialogResult.OK:
                    return ConfirmationOutcome.Yes;
                case Forms.DialogResult.No:
                    return ConfirmationOutcome.No;
                default:
                    return ConfirmationOutcome.Cancel;
            }
        }

        public void Message(string text)
        {
            Forms.MessageBox.Show(text);
        }

        public void NotifyWarn(string text, string caption)
        {
            Forms.MessageBox.Show(text, caption, Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Exclamation);
        }

        public bool Question(string text, string caption)
        {
            return Forms.MessageBox.Show(text, caption, Forms.MessageBoxButtons.YesNo, Forms.MessageBoxIcon.Question) == Forms.DialogResult.Yes;
        }

        public bool ConfirmYesNo(string text, string caption)
        {
            return Forms.MessageBox.Show(text, caption, Forms.MessageBoxButtons.YesNo, Forms.MessageBoxIcon.Exclamation) == Forms.DialogResult.Yes;
        }

        public bool ConfirmYesNo(string text, string caption, bool suggestion)
        {
            return Forms.MessageBox.Show(text, caption, Forms.MessageBoxButtons.YesNo, Forms.MessageBoxIcon.Exclamation, suggestion ? Forms.MessageBoxDefaultButton.Button1 : Forms.MessageBoxDefaultButton.Button2) == Forms.DialogResult.Yes;
        }

        public ConfirmationOutcome Confirm(string text, string caption, ConfirmationOutcome suggestion = ConfirmationOutcome.Yes)
        {
            Forms.MessageBoxDefaultButton suggestionButton;
            switch (suggestion)
            {
                // default required to shut the compiler up about "unassigned variable"
                default:
                case ConfirmationOutcome.Yes:
                    suggestionButton = Forms.MessageBoxDefaultButton.Button1;
                    break;
                case ConfirmationOutcome.No:
                    suggestionButton = Forms.MessageBoxDefaultButton.Button2;
                    break;
                case ConfirmationOutcome.Cancel:
                    suggestionButton = Forms.MessageBoxDefaultButton.Button3;
                    break;
            }
            var result = Forms.MessageBox.Show(text, caption, Forms.MessageBoxButtons.YesNoCancel, Forms.MessageBoxIcon.Exclamation, suggestionButton);

            switch (result)
            {
                case Forms.DialogResult.Cancel:
                    return ConfirmationOutcome.Cancel;
                case Forms.DialogResult.Yes:
                    return ConfirmationOutcome.Yes;
                case Forms.DialogResult.No:
                    return ConfirmationOutcome.No;
                default:
                    return suggestion;
            }
        }

        public void NotifyError(string text, string caption, string details)
        {
            Forms.MessageBox.Show($"{text}\n\n{details}", caption, Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Error);
        }
    }
}
