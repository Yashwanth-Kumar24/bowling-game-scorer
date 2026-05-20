namespace BowlingGame.Models;

public class BowlingGameModel
{
    public List<FrameModel> Frames { get; set; } =
        Enumerable.Range(1, 10)
            .Select(_ => new FrameModel())
            .ToList();
}