using WordleSolver.Models;

using System;
using WordleSolver.Strategies;

namespace WordleSolver.Services;

/// <summary>
/// Executes multiple automated games with a supplied solver strategy and reports statistics.
/// </summary>
public sealed class StudentGuesserService
{
    private readonly WordleService _service;
    private readonly IWordleSolverStrategy _strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentGuesserService"/> class.
    /// </summary>
    /// <param name="service">The game engine used to play Wordle.</param>
    /// <param name="strategy">The student's guess-selection algorithm.</param>
    public StudentGuesserService(WordleService service, IWordleSolverStrategy strategy)
    {
        _service = service;
        _strategy = strategy;
    }

    /// <summary>
    /// Runs <paramref name="numberOfGames"/> games and prints win-rate and average guesses.
    /// </summary>
    /// <param name="numberOfGames">The number of independent games to simulate.</param>
    public void Run(int numberOfGames)
    {
        int totalGuessesAcrossWins = 0;
        int wins = 0;

        for (int i = 0; i < numberOfGames; i++)
        {
            _service.Play();   // Start a new Wordle game
            _strategy.Reset(); // Reset the strategy state for solving Wordle game
            var result = GuessResult.Default;

            for (int attempt = 0; attempt < WordleService.MaxGuesses; attempt++)
            {
                string guess = _strategy.PickNextGuess(result);
                result = _service.Guess(guess);

                // Skip invalid guesses without consuming attempts.
                if (!result.IsValid)
                {
                    Console.WriteLine($"Invalid guess rejected: '{guess}'");
                    continue;
                }

                if (result.IsCorrect)
                {
                    totalGuessesAcrossWins += result.GuessNumber;
                    wins++;
                    break;
                }

                // Stop early if no guesses remain.
                if (result.GuessesRemaining == 0)
                    break;
            }
        }

        Console.WriteLine($"Completed {numberOfGames} game(s).");
        Console.WriteLine($"Wins: {wins}/{numberOfGames}");
        Console.WriteLine($"Average guesses per win: {(wins > 0 ? ((double)totalGuessesAcrossWins / wins).ToString("0.00") : "N/A")}");
    }
}