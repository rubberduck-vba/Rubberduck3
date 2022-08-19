using System;
using System.Windows.Forms;

namespace Rubberduck.Interaction
{
    public interface IDialogView : IDisposable
    {
        DialogResult ShowDialog();
    }
}
