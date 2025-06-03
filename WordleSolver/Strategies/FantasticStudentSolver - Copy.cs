using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WordleSolver.Models;


namespace WordleSolver.Strategies;

/// <summary>
/// A much smarter Wordle solver strategy that:
/// - Starts with a strong word ("arose")
/// - Filters remaining words using feedback (Correct, Misplaced, Unused)
/// - Picks the next guess using letter-frequency heuristics
/// </summary>
public sealed class FantasticStudentSolver : IWordleSolverStrategy
{
    private static readonly string WordListPath = Path.Combine("data", "wordle.txt");
    private static readonly List<string> WordList = LoadWordList();
    private List<string> _remainingWords = new();

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

    public void Reset()
    {
        _remainingWords = [.. WordList];
    }

    public string PickNextGuess(GuessResult previousResult)
    {
        if (!previousResult.IsValid)
            throw new InvalidOperationException("PickNextGuess shouldn't be called if previous result isn't valid");

        // First guess: return a strong starting word
        if (previousResult.Guesses.Count == 0)
        {
            string firstWord = "arose";
            _remainingWords.Remove(firstWord);
            return firstWord;
        }

        // Otherwise, filter remaining words based on last guess
        string lastGuess = previousResult.Word;
        var statuses = previousResult.LetterStatuses;

        _remainingWords = _remainingWords.Where(candidate =>
        {
            for (int i = 0; i < 5; i++)
            {
                char guessedChar = lastGuess[i];
                char candidateChar = candidate[i];

                if (statuses[i] == LetterStatus.Correct)
                {
                    if (candidateChar != guessedChar)
                        return false;
                }
                else if (statuses[i] == LetterStatus.Misplaced)
                {
                    if (candidateChar == guessedChar || !candidate.Contains(guessedChar))
                        return false;
                }
                else if (statuses[i] == LetterStatus.Unused)
                {
                    // Don't eliminate the character if it was used somewhere else (Correct/Misplaced)
                    bool guessedElsewhere = lastGuess
                        .Select((c, idx) => new { c, idx })
                        .Any(g => g.c == guessedChar && statuses[g.idx] != LetterStatus.Unused);

                    if (!guessedElsewhere && candidate.Contains(guessedChar))
                        return false;
                }
            }

            return true;
        }).ToList();

        // Choose the best word based on letter frequency
        string choice = ChooseBestRemainingWord();
        _remainingWords.Remove(choice);
        return choice;
    }

    private string ChooseBestRemainingWord()
    {
        if (_remainingWords.Count == 0)
            throw new InvalidOperationException("No remaining words to choose from");

        // Count letter frequencies across all remaining words
        var frequency = new Dictionary<char, int>();
        foreach (var word in _remainingWords)
        {
            foreach (char c in word.Distinct())
            {
                if (!frequency.ContainsKey(c))
                    frequency[c] = 0;
                frequency[c]++;
            }
        }

        // Pick the word with the highest sum of unique letter frequencies
        string bestWord = _remainingWords
            .OrderByDescending(word => word.Distinct().Sum(c => frequency[c]))
            .First();

        return bestWord;
    }
}
