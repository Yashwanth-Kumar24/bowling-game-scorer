using BowlingGame.Models;

namespace BowlingGame.Services;

public interface IBowlingValidationService
{
    bool IsValidSymbol(string value);

    bool IsValidBowlingEntry(BowlingGameModel game, int frameIndex, int rollNumber);

    bool IsSecondRollDisabled(BowlingGameModel game, int frameIndex);

    bool IsThirdRollDisabled(BowlingGameModel game);

    InputPosition? GetNextRequiredInput(BowlingGameModel game);

    int GetInputOrder(int frameIndex, int rollNumber);

    void ClearRollsAfter(BowlingGameModel game, List<string?> errorMessages, int frameIndex, int rollNumber);
}