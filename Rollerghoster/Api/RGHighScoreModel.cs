using System;

namespace Rollerghoster.Api
{
    public class RGHighScoreModel {

        public string UserName { get; set; }

        public DateTime ScoreDate { get; set; }

        public string Seed { get; set; }

        public int Retries { get; set; }
        public float Time { get; set; }
        public string Ghost { get; set; }
    }
}
