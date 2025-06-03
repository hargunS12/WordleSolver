using WordleSolver.Models;

namespace WordleSolver.Strategies;

/// <summary>
/// Contract that all student solver implementations must satisfy.
/// </summary>
public interface IWordleSolverStrategy
{
    /// <summary>
    /// Reset the current solver progress to begin a new game - this is useful if you are keeping
    /// track of remaining possible words
    /// </summary>
	void Reset();

    /// <summary>
    /// Determines the next word to guess given feedback from the previous guess.
    /// </summary>
    /// <param name="previousResult">
    /// The <see cref="GuessResult"/> returned by the game engine for the last guess
    /// (or <see cref="GuessResult.Default"/> if this is the first turn).
    /// </param>
    /// <returns>A five-letter lowercase word.</returns>
    string PickNextGuess(GuessResult previousResult);
}