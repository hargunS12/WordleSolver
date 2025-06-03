
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WordleSolver.Models;


namespace WordleSolver.Services;

/// <summary>
/// Central game engine that selects an answer, validates guesses, and produces feedback.
/// </summary>
public sealed class WordleService
{
    /// <summary>Absolute or relative path of the word-list file.</summary>
    private static readonly string WordListPath = Path.Combine("data", "wordle.txt");

    /// <summary>In-memory dictionary of valid five-letter words.</summary>
    private static readonly List<string> WordList = LoadWordList();

    /// <summary>
    /// One shared <see cref="Random"/> instance avoids repeat answers caused by identical seeds.
    /// </summary>
    private static readonly Random Rng = new();

    private string _answer = string.Empty;
    private int _guessesRemaining = MaxGuesses;
    private int _currentGuessNumber;
    private readonly List<GuessResult> _guessHistory = new();
    private GuessResult _lastResult = GuessResult.Default;

    /// <summary>
    /// Loads the dictionary from disk, filtering to distinct five-letter lowercase words.
    /// </summary>
    private static List<string> LoadWordList()
    {
        if (!File.Exists(WordListPath))
            throw new FileNotFoundException($"Word list not found at path: {WordListPath}");

        return File.ReadAllLines(WordListPath)
                   .Select(w => w.Trim().ToLowerInvariant())
                   .Where(w => w.Length == 5)
                   .Distinct()
                   .ToList();
    }

    /// <summary>
    /// Begins a new game by choosing a fresh answer and resetting counters.
    /// </summary>
    public void Play()
    {
        _answer = WordList[Rng.Next(WordList.Count)];
        _guessesRemaining = MaxGuesses;
        _currentGuessNumber = 0;
        _guessHistory.Clear();
        _lastResult = GuessResult.Default;
    }

    /// <summary>
    /// Evaluates a player's guess and returns detailed feedback.
    /// </summary>
    /// <param name="guess">The five-letter word the player is guessing.</param>
    /// <returns><see cref="GuessResult"/> containing feedback and updated game state.</returns>
    public GuessResult Guess(string guess)
    {
        guess = guess.ToLowerInvariant();

        // No guesses left – always return the last result.
        if (_guessesRemaining == 0)
            return _lastResult;

        // Build a provisional result.  History will be attached at the end.
        var result = new GuessResult
        {
            GuessNumber = _currentGuessNumber, 
            GuessesRemaining = _guessesRemaining,
            LetterStatuses = Enumerable.Repeat(LetterStatus.Unused, 5).ToArray(),
            IsValid = WordList.Contains(guess)
        };

        // Reject invalid words without consuming a turn.
        if (!result.IsValid)
        {
            result.Guesses = [.._guessHistory];
            return result;
        }

        // Valid guess – consume a turn.
        _currentGuessNumber++;
        _guessesRemaining--;

        result.Word = guess;
        result.GuessNumber = _currentGuessNumber;
        result.GuessesRemaining = _guessesRemaining;
        result.IsCorrect = guess == _answer;

        // Build per-letter feedback
        var answerCharCounts = _answer
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());

        // Pass 1 – correct letters
        for (int i = 0; i < 5; i++)
        {
            if (guess[i] == _answer[i])
            {
                result.LetterStatuses[i] = LetterStatus.Correct;
                answerCharCounts[guess[i]]--;
            }
        }

        // Pass 2 – misplaced / unused
        for (int i = 0; i < 5; i++)
        {
            if (result.LetterStatuses[i] == LetterStatus.Correct)
                continue;

            if (answerCharCounts.TryGetValue(guess[i], out int remaining) && remaining > 0)
            {
                result.LetterStatuses[i] = LetterStatus.Misplaced;
                answerCharCounts[guess[i]]--;
            }
            else
            {
                result.LetterStatuses[i] = LetterStatus.Unused;
            }
        }

        // Save history
        _guessHistory.Add(result);
        result.Guesses = [.._guessHistory]; 
        _lastResult = result;

        return result;
    }

    /// <summary>
    /// Maximum number of valid guesses allowed per game.
    /// </summary>
    public const int MaxGuesses = 2000;
}