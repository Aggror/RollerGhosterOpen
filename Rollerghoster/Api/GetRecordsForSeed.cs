using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using RestSharp;
using Rollerghoster.UI;
using Rollerghoster.Util;
using Rollerghoster.Windows;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI.Controls;

namespace Rollerghoster.Api
{
    public class GetRecordsForSeed : AsyncScript
    {
        private MainMenuUI mainMenuUI;

        public override async Task Execute()
        {
            mainMenuUI = Entity.Get<MainMenuUI>();
            var url = $"{Settings.ApiUrl}?seed={mainMenuUI.Seed}";
            
            Debug.WriteLine(url);
            var client = new RestClient(url);

            var request = new RestRequest();
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteGetAsync<RgResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var recordsList = mainMenuUI.RecordsList;
                recordsList.Children.Clear();

                if (response.Data.RgHighScores.Count > 0)
                {
                    var ghostTracker = mainMenuUI.marble.Get<GhostTracker>();
                    var onlineGhosts = new List<Ghost>();

                    foreach (var highScore in response.Data.RgHighScores)
                    {
                        try
                        {
                            ConvertGhostData(onlineGhosts, highScore);

                            var nameText = CreateTextBlockFromTemplate(mainMenuUI.RecordName, HttpUtility.UrlDecode(highScore.UserName));
                            var timeText = CreateTextBlockFromTemplate(mainMenuUI.RecordTime, GetFinishTime(highScore.Time));

                            recordsList.Children.Add(nameText);
                            recordsList.Children.Add(timeText);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    ghostTracker.StoreOnlineGhosts(onlineGhosts);
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
                VerticalAlignment = template.VerticalAlignment,
                TextAlignment = template.TextAlignment
            };
        }

        private string GetFinishTime(float levelTime)
        {
            return levelTime.ToString("00.000");
        }

        private static void ConvertGhostData(List<Ghost> ghosts, RGHighScoreModel highscore)
        {
            var positionLines = highscore.Ghost.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            var ghostPositions = new List<Vector3>();
            foreach (var line in positionLines)
            {
                var positions = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (positions.Length == 3)
                {
                    var vec3posititions = new Vector3(float.Parse(positions[0]), float.Parse(positions[1]), float.Parse(positions[2]));
                    ghostPositions.Add(vec3posititions);
                }
            }

            var ghost = new Ghost();
            ghost.SetPositions(ghostPositions);
            ghosts.Add(ghost);
        }
    }
}
