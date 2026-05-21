using BowlingGame.Models;
using BowlingGame.Services;

namespace BowlingGame.Tests;

public class BowlingValidationServiceTests
{
    private readonly BowlingValidationService _service = new();

    [Theory]
    [InlineData("")]
    [InlineData("X")]
    [InlineData("/")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("9")]
    public void IsValidSymbol_ValidValues_ReturnsTrue(string value)
    {
        Assert.True(_service.IsValidSymbol(value));
    }

    [Theory]
    [InlineData("A")]
    [InlineData("#")]
    [InlineData("%")]
    [InlineData("10")]
    [InlineData("-1")]
    public void IsValidSymbol_InvalidValues_ReturnsFalse(string value)
    {
        Assert.False(_service.IsValidSymbol(value));
    }

    [Fact]
    public void IsValidBowlingEntry_SpareCannotBeFirstRoll_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[0].Roll1 = "/";

        bool result = _service.IsValidBowlingEntry(game, 0, 1);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBowlingEntry_StrikeCannotBeSecondRollInStandardFrame_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[0].Roll1 = "4";
        game.Frames[0].Roll2 = "X";

        bool result = _service.IsValidBowlingEntry(game, 0, 2);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBowlingEntry_StandardFramePinTotalAboveNine_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[0].Roll1 = "8";
        game.Frames[0].Roll2 = "5";

        bool result = _service.IsValidBowlingEntry(game, 0, 2);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBowlingEntry_StandardFrameSpare_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[0].Roll1 = "8";
        game.Frames[0].Roll2 = "/";

        bool result = _service.IsValidBowlingEntry(game, 0, 2);

        Assert.True(result);
    }

    [Fact]
    public void IsSecondRollDisabled_StandardFrameStrike_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[0].Roll1 = "X";

        bool result = _service.IsSecondRollDisabled(game, 0);

        Assert.True(result);
    }

    [Fact]
    public void IsSecondRollDisabled_TenthFrameStrike_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";

        bool result = _service.IsSecondRollDisabled(game, 9);

        Assert.False(result);
    }

    [Fact]
    public void IsThirdRollDisabled_TenthFrameOpen_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "7";
        game.Frames[9].Roll2 = "2";

        bool result = _service.IsThirdRollDisabled(game);

        Assert.True(result);
    }

    [Fact]
    public void IsThirdRollDisabled_TenthFrameSpare_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "7";
        game.Frames[9].Roll2 = "/";

        bool result = _service.IsThirdRollDisabled(game);

        Assert.False(result);
    }

    [Fact]
    public void IsThirdRollDisabled_TenthFrameStrikeWithoutSecondRoll_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";

        bool result = _service.IsThirdRollDisabled(game);

        Assert.True(result);
    }

    [Fact]
    public void IsThirdRollDisabled_TenthFrameStrikeWithSecondRoll_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "8";

        bool result = _service.IsThirdRollDisabled(game);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameStrikeEightSpare_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "8";
        game.Frames[9].Roll3 = "/";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.True(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameStrikeEightFive_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "8";
        game.Frames[9].Roll3 = "5";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameSpareWithNumberBonus_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "7";
        game.Frames[9].Roll2 = "/";
        game.Frames[9].Roll3 = "5";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.True(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameSpareWithSpareBonus_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "7";
        game.Frames[9].Roll2 = "/";
        game.Frames[9].Roll3 = "/";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.False(result);
    }

    [Fact]
    public void GetNextRequiredInput_ReturnsFirstEmptyRequiredRoll()
    {
        var game = CreateGame();
        game.Frames[0].Roll1 = "8";

        var result = _service.GetNextRequiredInput(game);

        Assert.NotNull(result);
        Assert.Equal(0, result.Value.FrameIndex);
        Assert.Equal(2, result.Value.RollNumber);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameDoubleStrikeWithSpare_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "X";
        game.Frames[9].Roll3 = "/";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.False(result);
    }

    [Fact]
    public void GetNextRequiredInput_SkipsSecondRollAfterStandardFrameStrike()
    {
        var game = CreateGame();
        game.Frames[0].Roll1 = "X";

        var result = _service.GetNextRequiredInput(game);

        Assert.NotNull(result);
        Assert.Equal(1, result.Value.FrameIndex);
        Assert.Equal(1, result.Value.RollNumber);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameStrikeFiveStrike_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "5";
        game.Frames[9].Roll3 = "X";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameStrikeFiveSpare_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "5";
        game.Frames[9].Roll3 = "/";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.True(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameSecondRollSpareAfterStrike_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "/";

        bool result = _service.IsValidBowlingEntry(game, 9, 2);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameSecondRollStrikeAfterDigit_ReturnsFalse()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "5";
        game.Frames[9].Roll2 = "X";

        bool result = _service.IsValidBowlingEntry(game, 9, 2);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameSecondRollStrikeAfterStrike_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "X";

        bool result = _service.IsValidBowlingEntry(game, 9, 2);

        Assert.True(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameSecondRollSpareAfterDigit_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "5";
        game.Frames[9].Roll2 = "/";

        bool result = _service.IsValidBowlingEntry(game, 9, 2);

        Assert.True(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameSecondRollDigitValidSum_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "5";
        game.Frames[9].Roll2 = "3";

        bool result = _service.IsValidBowlingEntry(game, 9, 2);

        Assert.True(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameSpareWithStrikeBonus_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "7";
        game.Frames[9].Roll2 = "/";
        game.Frames[9].Roll3 = "X";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.True(result);
    }

    [Fact]
    public void IsValidBowlingEntry_TenthFrameStrikeFiveThree_ReturnsTrue()
    {
        var game = CreateGame();
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "5";
        game.Frames[9].Roll3 = "3";

        bool result = _service.IsValidBowlingEntry(game, 9, 3);

        Assert.True(result);
    }

    [Fact]
    public void GetNextRequiredInput_AllFramesComplete_ReturnsNull()
    {
        var game = CreateGame();
        for (int i = 0; i < 9; i++)
        {
            game.Frames[i].Roll1 = "1";
            game.Frames[i].Roll2 = "1";
        }
        game.Frames[9].Roll1 = "7";
        game.Frames[9].Roll2 = "2";

        var result = _service.GetNextRequiredInput(game);

        Assert.Null(result);
    }

    [Fact]
    public void GetNextRequiredInput_TenthFrameNeedsSecondRoll_ReturnsTenthFrameRoll2()
    {
        var game = CreateGame();
        for (int i = 0; i < 9; i++)
        {
            game.Frames[i].Roll1 = "1";
            game.Frames[i].Roll2 = "1";
        }
        game.Frames[9].Roll1 = "X";

        var result = _service.GetNextRequiredInput(game);

        Assert.NotNull(result);
        Assert.Equal(9, result.Value.FrameIndex);
        Assert.Equal(2, result.Value.RollNumber);
    }

    [Fact]
    public void GetNextRequiredInput_TenthFrameNeedsThirdRoll_ReturnsTenthFrameRoll3()
    {
        var game = CreateGame();
        for (int i = 0; i < 9; i++)
        {
            game.Frames[i].Roll1 = "1";
            game.Frames[i].Roll2 = "1";
        }
        game.Frames[9].Roll1 = "X";
        game.Frames[9].Roll2 = "5";

        var result = _service.GetNextRequiredInput(game);

        Assert.NotNull(result);
        Assert.Equal(9, result.Value.FrameIndex);
        Assert.Equal(3, result.Value.RollNumber);
    }

    [Fact]
    public void ClearRollsAfter_ClearsFutureRollsAfterEarlierEdit()
    {
        var game = CreateGame();
        var errors = Enumerable.Repeat<string?>(null, 10).ToList();

        game.Frames[0].Roll1 = "5";
        game.Frames[0].Roll2 = "3";
        game.Frames[1].Roll1 = "4";
        game.Frames[1].Roll2 = "2";
        errors[1] = "Some error";

        _service.ClearRollsAfter(game, errors, 0, 1);

        Assert.Equal("5", game.Frames[0].Roll1);
        Assert.True(string.IsNullOrEmpty(game.Frames[0].Roll2));
        Assert.True(string.IsNullOrEmpty(game.Frames[1].Roll1));
        Assert.True(string.IsNullOrEmpty(game.Frames[1].Roll2));
        Assert.Null(errors[1]);
    }

    private static BowlingGameModel CreateGame()
    {
        return new BowlingGameModel();
    }
}