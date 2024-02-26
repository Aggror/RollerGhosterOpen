using System.Collections.Generic;

namespace Rollerghoster.Api;

public class RgResponse
{
    public List<RGHighScoreModel> RgHighScores { get; set; }
    public List<string> Top10UsedSeeds { get; set; }
    public List<string> Top10LatestSeeds { get; set; }
}