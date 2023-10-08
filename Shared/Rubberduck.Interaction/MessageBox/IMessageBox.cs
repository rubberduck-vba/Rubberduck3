namespace Rubberduck.Interaction.MessageBox
{
    public interface IMessageBox
    {
        void Show(MessageBoxViewModel viewModel);
        ConfirmationOutcome Confirm(MessageBoxViewModel viewModel);
        /// <summary>
        /// Show a message to the user. Will only return after the user has acknowledged the message.
        /// </summary>
        /// <param name="text">The message to show to the user</param>
        void Message(string text);
        /// <summary>
        /// Notify the user of a warning. Will only return after the user has acknowledged the message.
        /// </summary>
        /// <param name="text">The Warning text to show the user</param>
        /// <param name="caption">The caption of the dialog window</param>
        void NotifyWarn(string text, string caption);
        /// <summary>
        /// Notify the user of an error. Will only return after the user has acknowledged the message.
        /// </summary>
        /// <param name="text">The Error text to show the user</param>
        /// <param name="caption">The caption of the dialog window</param>
        /// <param name="details">Additional information about the error</param>
        void NotifyError(string text, string caption, string details);
        /// <summary>
        /// Ask the user a question. Neither user selection must have any non-reversible consequences.
        /// Will only return on user-input.
        /// </summary>
        /// <param name="text">The Question to ask the user</param>
        /// <param name="caption">The caption of the dialog window</param>
        /// <returns>true, if the user selects "Yes", false if the user selects "No"</returns>
        bool Question(string text, string caption);
        /// <summary>
        /// Ask the user for a simple confirmation. If the user selects an option, non-reversible consequences are acceptable.
        /// Will only return on user-input.
        /// </summary>
        /// <param name="text">The question to ask the user</param>
        /// <param name="caption">The caption of the dialog window</param>
        /// <param name="suggestion">The pre-selected result for the user, defaults to <b>Yes</b></param>
        /// <returns>true, if the user selects "Yes", false if the user selects "No"</returns>
        bool ConfirmYesNo(string text, string caption, bool suggestion = true);
        /// <summary>
        /// Ask the user for a confirmation. If the user selects an option that is not "Cancel", 
        /// non-reversible consequences are acceptable.
        /// Will only return on user-input.
        /// </summary>
        /// <param name="text">The question to ask the user</param>
        /// <param name="caption">The caption of the dialog window</param>
        /// <param name="suggestion">The pre-selected result for the user, defaults to <b>Cancel</b></param>
        /// <returns>Yes, No or Cancel respectively, according to the user's input</returns>
        ConfirmationOutcome Confirm(string text, string caption, ConfirmationOutcome suggestion = ConfirmationOutcome.Cancel);
    }
}
