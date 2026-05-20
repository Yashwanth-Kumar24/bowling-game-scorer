using BowlingGame.Models;
using BowlingGame.Services;

namespace BowlingGame.Tests;

public class BowlingScoreServiceTests
{
    private readonly BowlingScoreService _service = new();

    [Fact]
    public void CalculateScores_ExampleGame_ReturnsExpectedScores()
    {
        var game = CreateGame(
            ("8", "/", null),
            ("5", "4", null),
            ("9", "0", null),
            ("X", null, null),
            ("X", null, null),
            ("5", "/", null),
            ("5", "3", null),
            ("6", "3", null),
            ("9", "/", null),
            ("9", "/", "X")
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(new int?[] { 15, 24, 33, 58, 78, 93, 101, 110, 129, 149 }, scores);
    }

    [Fact]
    public void CalculateScores_PerfectGame_Returns300()
    {
        var game = CreateGame(
            ("X", null, null),
            ("X", null, null),
            ("X", null, null),
            ("X", null, null),
            ("X", null, null),
            ("X", null, null),
            ("X", null, null),
            ("X", null, null),
            ("X", null, null),
            ("X", "X", "X")
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(300, scores[9]);
    }

    [Fact]
    public void CalculateScores_AllSpares_Returns150()
    {
        var game = CreateGame(
            ("5", "/", null),
            ("5", "/", null),
            ("5", "/", null),
            ("5", "/", null),
            ("5", "/", null),
            ("5", "/", null),
            ("5", "/", null),
            ("5", "/", null),
            ("5", "/", null),
            ("5", "/", "5")
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(150, scores[9]);
    }

    [Fact]
    public void CalculateScores_AllOpenFrames_Returns20()
    {
        var game = CreateGame(
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(20, scores[9]);
    }

    [Fact]
    public void CalculateScores_GutterGame_ReturnsZero()
    {
        var game = CreateGame(
            ("0", "0", null),
            ("0", "0", null),
            ("0", "0", null),
            ("0", "0", null),
            ("0", "0", null),
            ("0", "0", null),
            ("0", "0", null),
            ("0", "0", null),
            ("0", "0", null),
            ("0", "0", null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(0, scores[9]);
    }

    [Fact]
    public void CalculateScores_IncompleteOpenFrame_ShowsRunningTotal()
    {
        var game = CreateGame(
            ("8", "0", null),
            ("4", null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(8, scores[0]);
        Assert.Equal(12, scores[1]);
        Assert.Null(scores[2]);
    }

    [Fact]
    public void CalculateScores_PartialSpareBeforeNextFrame_ShowsTen()
    {
        var game = CreateGame(
            ("8", "/", null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(10, scores[0]);
        Assert.Null(scores[1]);
    }

    [Fact]
    public void CalculateScores_SpareUpdatesAfterNextRoll()
    {
        var game = CreateGame(
            ("8", "/", null),
            ("5", null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(15, scores[0]);
        Assert.Equal(20, scores[1]);
        Assert.Null(scores[2]);
    }

    [Fact]
    public void CalculateScores_StrikeOnly_ShowsTen()
    {
        var game = CreateGame(
            ("X", null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(10, scores[0]);
        Assert.Null(scores[1]);
    }

    [Fact]
    public void CalculateScores_StrikeUpdatesWithOneFutureRoll()
    {
        var game = CreateGame(
            ("X", null, null),
            ("4", null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(14, scores[0]);
        Assert.Equal(18, scores[1]);
        Assert.Null(scores[2]);
    }

    [Fact]
    public void CalculateScores_StrikeFollowedByCompletedOpenFrame_RecalculatesCorrectly()
    {
        var game = CreateGame(
            ("X", null, null),
            ("4", "3", null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(17, scores[0]);
        Assert.Equal(24, scores[1]);
        Assert.Null(scores[2]);
    }

    [Fact]
    public void CalculateScores_DoubleStrikePartialFrame_RecalculatesCorrectly()
    {
        var game = CreateGame(
            ("X", null, null),
            ("X", null, null),
            ("6", null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(26, scores[0]);
        Assert.Equal(42, scores[1]);
        Assert.Equal(48, scores[2]);
        Assert.Null(scores[3]);
    }

    [Fact]
    public void CalculateScores_DoubleStrikeCompletedOpenFrame_RecalculatesCorrectly()
    {
        var game = CreateGame(
            ("X", null, null),
            ("X", null, null),
            ("6", "2", null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(26, scores[0]);
        Assert.Equal(44, scores[1]);
        Assert.Equal(52, scores[2]);
        Assert.Null(scores[3]);
    }

    [Fact]
    public void CalculateScores_TenthFrameOpen_ReturnsTotal()
    {
        var game = CreateGame(
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("7", "2", null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(27, scores[9]);
    }

    [Fact]
    public void CalculateScores_TenthFrameSpare_ReturnsTotalWithBonusRoll()
    {
        var game = CreateGame(
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("7", "/", "5")
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(33, scores[9]);
    }

    [Fact]
    public void CalculateScores_TenthFrameStrike_ReturnsTotalWithTwoBonusRolls()
    {
        var game = CreateGame(
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("X", "X", "X")
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(48, scores[9]);
    }

    [Fact]
    public void CalculateScores_DoubleStrikeCompletedFrameThenNextRoll_DoesNotChangeCompletedBonuses()
    {
        var game = CreateGame(
            ("X", null, null),
            ("X", null, null),
            ("6", "2", null),
            ("1", null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null),
            (null, null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(26, scores[0]);
        Assert.Equal(44, scores[1]);
        Assert.Equal(52, scores[2]);
        Assert.Equal(53, scores[3]);
        Assert.Null(scores[4]);
    }

    [Fact]
    public void CalculateScores_TenthFramePartialInput_ShowsRunningTotal()
    {
        var game = CreateGame(
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("1", "1", null),
            ("X", null, null)
        );

        var scores = _service.CalculateScores(game);

        Assert.Equal(28, scores[9]);
    }

    private static BowlingGameModel CreateGame(params (string? Roll1, string? Roll2, string? Roll3)[] frames)
    {
        var game = new BowlingGameModel();

        for (int i = 0; i < frames.Length; i++)
        {
            game.Frames[i].Roll1 = frames[i].Roll1;
            game.Frames[i].Roll2 = frames[i].Roll2;
            game.Frames[i].Roll3 = frames[i].Roll3;
        }

        return game;
    }
}