
using System.Collections.Generic;
using System.Linq;
using WordleSolver.Services;

namespace WordleSolver.Models;




/// <summary>
/// Contains complete feedback for a single guess and running game state.
/// </summary>
public sealed class GuessResult
{
	/// <summary>
	/// Gets or sets the value of the word that was guessed
	/// </summary>
	public string Word { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the guess exactly matches the answer.
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the guessed word exists in the dictionary.
    /// Invalid guesses do not decrement turns or advance the guess number.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the ordinal number of this guess (first valid guess = 1).
    /// A value of 0 represents "no guesses yet".
    /// </summary>
    public int GuessNumber { get; set; } = 0;

    /// <summary>
    /// Gets or sets the remaining number of guesses the player may make in this game.
    /// </summary>
    public int GuessesRemaining { get; set; }

    /// <summary>
    /// Gets or sets the per-letter status feedback for this guess.
    /// Always contains exactly five elements.
    /// </summary>
    public LetterStatus[] LetterStatuses { get; set; } = new LetterStatus[5];

    /// <summary>
    /// Gets or sets the list of all valid <see cref="GuessResult"/> instances made
    /// so far in the current game (including this result once returned from <c>Guess</c>).
    /// </summary>
    public List<GuessResult> Guesses { get; set; } = new();

    /// <summary>
    /// Provides a default, empty result representing the state before any guesses have occurred.
    /// </summary>
    public static GuessResult Default => new()
    {
        IsCorrect = false,
        IsValid = true,
        GuessNumber = 0,
        GuessesRemaining = WordleService.MaxGuesses,
        LetterStatuses = Enumerable.Repeat(LetterStatus.Unused, 5).ToArray(),
        Guesses = new List<GuessResult>()
    };
}