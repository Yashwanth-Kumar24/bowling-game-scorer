using BowlingGame.Models;

namespace BowlingGame.Services;

public class BowlingValidationService : IBowlingValidationService
{
    public bool IsValidSymbol(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        value = value.Trim().ToUpper();

        return value == "X" ||
               value == "/" ||
               int.TryParse(value, out int pins) && pins >= 0 && pins <= 9;
    }

    public bool IsValidBowlingEntry(BowlingGameModel game, int frameIndex, int rollNumber)
    {
        var frame = game.Frames[frameIndex];

        if (frameIndex < 9)
        {
            return IsValidStandardFrameEntry(frame, rollNumber);
        }

        return IsValidTenthFrameEntry(frame, rollNumber);
    }

    public bool IsSecondRollDisabled(BowlingGameModel game, int frameIndex)
    {
        if (frameIndex == 9)
        {
            return false;
        }

        return game.Frames[frameIndex].Roll1 == "X";
    }

    public bool IsThirdRollDisabled(BowlingGameModel game)
    {
        var frame = game.Frames[9];

        if (string.IsNullOrWhiteSpace(frame.Roll1))
        {
            return true;
        }

        if (frame.Roll1 == "X")
        {
            return string.IsNullOrWhiteSpace(frame.Roll2);
        }

        if (frame.Roll2 == "/")
        {
            return false;
        }

        return true;
    }

    public InputPosition? GetNextRequiredInput(BowlingGameModel game)
    {
        for (int i = 0; i < game.Frames.Count; i++)
        {
            var frame = game.Frames[i];

            if (string.IsNullOrWhiteSpace(frame.Roll1))
            {
                return new InputPosition(i, 1);
            }

            if (i < 9)
            {
                if (frame.Roll1 == "X")
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(frame.Roll2))
                {
                    return new InputPosition(i, 2);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(frame.Roll2))
                {
                    return new InputPosition(i, 2);
                }

                bool needsThirdRoll = frame.Roll1 == "X" || frame.Roll2 == "/";

                if (needsThirdRoll && string.IsNullOrWhiteSpace(frame.Roll3))
                {
                    return new InputPosition(i, 3);
                }
            }
        }

        return null;
    }

    public int GetInputOrder(int frameIndex, int rollNumber)
    {
        return frameIndex * 3 + rollNumber;
    }

    public void ClearRollsAfter(
        BowlingGameModel game,
        List<string?> errorMessages,
        int frameIndex,
        int rollNumber)
    {
        for (int i = frameIndex; i < game.Frames.Count; i++)
        {
            var frame = game.Frames[i];

            if (i == frameIndex)
            {
                if (rollNumber == 1)
                {
                    frame.Roll2 = "";
                    frame.Roll3 = "";
                }
                else if (rollNumber == 2)
                {
                    frame.Roll3 = "";
                }
            }
            else
            {
                frame.Roll1 = "";
                frame.Roll2 = "";
                frame.Roll3 = "";
            }

            errorMessages[i] = null;
        }
    }

    private bool IsValidStandardFrameEntry(FrameModel frame, int rollNumber)
    {
        if (rollNumber == 1)
        {
            return frame.Roll1 != "/";
        }

        if (rollNumber == 2)
        {
            if (frame.Roll2 == "X")
            {
                return false;
            }

            if (frame.Roll2 == "/")
            {
                return !string.IsNullOrWhiteSpace(frame.Roll1) && frame.Roll1 != "X";
            }

            int roll1 = ParseNumber(frame.Roll1);
            int roll2 = ParseNumber(frame.Roll2);

            if (roll1 >= 0 && roll2 >= 0)
            {
                return roll1 + roll2 <= 9;
            }
        }

        return true;
    }

    private bool IsValidTenthFrameEntry(FrameModel frame, int rollNumber)
    {
        if (rollNumber == 1)
        {
            return frame.Roll1 != "/";
        }

        if (rollNumber == 2)
        {
            if (frame.Roll2 == "/")
            {
                return frame.Roll1 != "X" && !string.IsNullOrWhiteSpace(frame.Roll1);
            }

            if (frame.Roll1 != "X" && frame.Roll2 == "X")
            {
                return false;
            }

            int roll1 = ParseNumber(frame.Roll1);
            int roll2 = ParseNumber(frame.Roll2);

            if (roll1 >= 0 && roll2 >= 0)
            {
                return roll1 + roll2 <= 9;
            }

            return true;
        }

        if (rollNumber == 3)
        {
            bool eligibleForThirdRoll = frame.Roll1 == "X" || frame.Roll2 == "/";

            if (!eligibleForThirdRoll)
            {
                return false;
            }

            if (frame.Roll1 == "X" && frame.Roll2 == "X" && frame.Roll3 == "/")
            {
                return false;
            }

            if (frame.Roll3 == "/")
            {
                return frame.Roll2 != "/" && !string.IsNullOrWhiteSpace(frame.Roll2);
            }

            if (frame.Roll1 == "X" && frame.Roll2 != "X")
            {
                if (frame.Roll3 == "X")
                {
                    return false;
                }

                int roll2 = ParseNumber(frame.Roll2);
                int roll3 = ParseNumber(frame.Roll3);

                if (roll2 >= 0 && roll3 >= 0)
                {
                    return roll2 + roll3 <= 9;
                }
            }
        }

        return true;
    }

    private int ParseNumber(string? value)
    {
        return int.TryParse(value, out int pins) ? pins : -1;
    }
}