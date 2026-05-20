using BowlingGame.Models;

namespace BowlingGame.Services;

public class BowlingScoreService : IBowlingScoreService
{
    public List<int?> CalculateScores(BowlingGameModel game)
    {
        var scores = Enumerable.Repeat<int?>(null, 10).ToList();

        var rolls = GetRollValues(game);

        int runningTotal = 0;
        int rollIndex = 0;

        for (int frameIndex = 0; frameIndex < 10; frameIndex++)
        {
            if (rollIndex >= rolls.Count)
            {
                break;
            }

            if (frameIndex == 9)
            {
                int? tenthFrameScore = GetPartialTenthFrameScore(game.Frames[9]);

                if (tenthFrameScore == null)
                {
                    break;
                }

                runningTotal += tenthFrameScore.Value;
                scores[frameIndex] = runningTotal;
                break;
            }

            if (IsStrike(rolls, rollIndex))
            {
                int availableBonus = 0;

                if (rollIndex + 1 < rolls.Count)
                {
                    availableBonus += rolls[rollIndex + 1];
                }

                if (rollIndex + 2 < rolls.Count)
                {
                    availableBonus += rolls[rollIndex + 2];
                }

                runningTotal += 10 + availableBonus;
                scores[frameIndex] = runningTotal;

                rollIndex += 1;
            }
            else
            {
                int roll1 = rolls[rollIndex];

                if (rollIndex + 1 >= rolls.Count)
                {
                    runningTotal += roll1;
                    scores[frameIndex] = runningTotal;
                    break;
                }

                int roll2 = rolls[rollIndex + 1];

                if (roll1 + roll2 == 10)
                {
                    int spareBonus = 0;

                    if (rollIndex + 2 < rolls.Count)
                    {
                        spareBonus = rolls[rollIndex + 2];
                    }

                    runningTotal += 10 + spareBonus;
                    scores[frameIndex] = runningTotal;
                    rollIndex += 2;
                }
                else
                {
                    runningTotal += roll1 + roll2;
                    scores[frameIndex] = runningTotal;
                    rollIndex += 2;
                }
            }
        }

        return scores;
    }

    private List<int> GetRollValues(BowlingGameModel game)
    {
        var rolls = new List<int>();

        for (int frameIndex = 0; frameIndex < game.Frames.Count; frameIndex++)
        {
            var frame = game.Frames[frameIndex];

            AddRoll(rolls, frame.Roll1, 0);

            int previousRoll = rolls.Count > 0 ? rolls.Last() : 0;
            AddRoll(rolls, frame.Roll2, previousRoll);

            if (frameIndex == 9)
            {
                previousRoll = rolls.Count > 0 ? rolls.Last() : 0;
                AddRoll(rolls, frame.Roll3, previousRoll);
            }
        }

        return rolls;
    }

    private void AddRoll(List<int> rolls, string? value, int previousRoll)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        value = value.Trim().ToUpper();

        if (value == "X")
        {
            rolls.Add(10);
        }
        else if (value == "/")
        {
            rolls.Add(10 - previousRoll);
        }
        else if (int.TryParse(value, out int pins))
        {
            rolls.Add(pins);
        }
    }

    private bool IsStrike(List<int> rolls, int rollIndex)
    {
        return rolls[rollIndex] == 10;
    }

    private int? GetPartialTenthFrameScore(FrameModel frame)
    {
        int? roll1 = ParseRoll(frame.Roll1, 0);

        if (roll1 == null)
        {
            return null;
        }

        int total = roll1.Value;

        int? roll2 = ParseRoll(frame.Roll2, roll1.Value);

        if (roll2 != null)
        {
            total += roll2.Value;
        }

        int? roll3 = ParseRoll(frame.Roll3, roll2 ?? 0);

        if (roll3 != null)
        {
            total += roll3.Value;
        }

        return total;
    }

    private int? ParseRoll(string? value, int previousRoll)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim().ToUpper();

        if (value == "X")
        {
            return 10;
        }

        if (value == "/")
        {
            return 10 - previousRoll;
        }

        if (int.TryParse(value, out int pins))
        {
            return pins;
        }

        return null;
    }
}