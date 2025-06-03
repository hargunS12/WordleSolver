
namespace WordleSolver.Models;


/// <summary>
/// Indicates how a single letter of a guess relates to the secret answer.
/// </summary>
public enum LetterStatus
{
    /// <summary>The letter is in the correct position.</summary>
    Correct,

    /// <summary>The letter exists in the answer but is in the wrong position.</summary>
    Misplaced,

    /// <summary>The letter does not appear in the answer at this place.</summary>
    Unused
}