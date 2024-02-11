using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Rubberduck.Parsing.Model.Symbols;

/// <summary>
/// Represents a comment.
/// </summary>
public class CommentNode
{
    /// <summary>
    /// Creates a new comment node.
    /// </summary>
    /// <param name="comment">The comment line text, without the comment marker.</param>
    /// <param name="location">The information required to locate and select this node in the editor.</param>
    public CommentNode(string comment, string marker, Location location)
    {
        CommentText = comment;
        Marker = marker;
        Location = location;
    }

    /// <summary>
    /// Gets the comment text, without the comment marker.
    /// </summary>
    public string CommentText { get; }

    /// <summary>
    /// The token used to indicate a comment.
    /// </summary>
    public string Marker { get; }

    /// <summary>
    /// Gets the information required to locate and select this node in the editor.
    /// </summary>
    public Location Location { get; }

    /// <summary>
    /// Returns the comment text.
    /// </summary>
    public override string ToString() => CommentText;
}
