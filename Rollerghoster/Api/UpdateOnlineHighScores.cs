using Newtonsoft.Json;
using RestSharp;
using Rollerghoster.UI;
using Rollerghoster.Util;
using Rollerghoster.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace Rollerghoster.Api
{
    public class UpdateOnlineHighScores : AsyncScript
    {
        private FinishUI finishUi;
        private MainMenuUI mainMenuUI;

        public bool OnlinePostingDone { get; set; }

        public override async Task Execute()
        {
            var uiEntities = Entity.GetParent().GetChildren();
            finishUi = Entity.Get<FinishUI>();
            mainMenuUI = uiEntities.Single(u => u.Name == "MainMenuUI").Get<MainMenuUI>();
            await PostOnlineHighScore(finishUi, mainMenuUI.Seed, mainMenuUI.UserName);
        }

        private async Task PostOnlineHighScore(FinishUI finishUI, string seed, string userName)
        {
            var client = new RestClient(Settings.ApiUrl);

            var highscore = new RGHighScoreModel()
            {
                Seed = HttpUtility.UrlEncode(seed),
                UserName = HttpUtility.UrlEncode(userName),
                Time = finishUI.FinishTime,
                Retries = finishUI.CorpseCount,
                Ghost = GetGhostOfMarble()
            };
            
            var request = new RestRequest();
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(highscore);

            var response = await client.ExecutePostAsync<RgResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                await UpdateHighScoreUI(response.Data);
            }

            OnlinePostingDone = true;
        }

        private string GetGhostOfMarble()
        {
            var ghostTracker = finishUi.marble.Get<GhostTracker>();
            var finalGhost = ghostTracker.GetFinalGhost();
            
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(finalGhost);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private async Task UpdateHighScoreUI(RgResponse response)
        {
            var _scoreRowsPanel =
                finishUi.OnlineRecordsPanel.Children.Single(a => a.Name == "ScoreRowsPanel") as StackPanel;
            var _scoreRowTemplate = _scoreRowsPanel.Children[0] as StackPanel;

            while (_scoreRowsPanel.Children.Count > 1)
            {
                _scoreRowsPanel.Children.RemoveAt(_scoreRowsPanel.Children.Count - 1);
            }

            if (response.RgHighScores.Count > 0)
            {
                var nameTextTemplate = _scoreRowTemplate.Children[0] as TextBlock;
                var timeTextTemplate = _scoreRowTemplate.Children[1] as TextBlock;
                var retryTextTemplate = _scoreRowTemplate.Children[2] as TextBlock;

                foreach (var highScore in response.RgHighScores)
                {
                    var nameText =
                        CreateTextBlockFromTemplate(nameTextTemplate, HttpUtility.UrlDecode(highScore.UserName));
                    var timeText = CreateTextBlockFromTemplate(timeTextTemplate, GetFinishTime(highScore.Time));
                    var retryText = CreateTextBlockFromTemplate(retryTextTemplate, highScore.Retries.ToString());

                    _scoreRowsPanel.Children.Add(nameText);
                    _scoreRowsPanel.Children.Add(timeText);
                    _scoreRowsPanel.Children.Add(retryText);
                }
            }
        }

        private static TextBlock CreateTextBlockFromTemplate(TextBlock template, string text)
        {
            return new TextBlock()
            {
                Text = text,
                Font = template.Font,
                TextSize = template.TextSize,
                Height = template.Height,
                Width = template.Width,
                Margin = template.Margin,
                LocalMatrix = template.LocalMatrix,
                Size = template.Size,
                TextColor = template.TextColor,
                HorizontalAlignment = template.HorizontalAlignment,
                VerticalAlignment = template.VerticalAlignment
            };
        }

        private string GetFinishTime(float levelTime)
        {
            return levelTime.ToString("00.000");
        }
    }
}