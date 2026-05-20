using BowlingGame.Models;

namespace BowlingGame.Services;

public interface IBowlingScoreService
{
    List<int?> CalculateScores(BowlingGameModel game);
}