using System.Net;
using System.Threading.Tasks;
using System.Web;
using RestSharp;
using Rollerghoster.UI;
using Rollerghoster.Util;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;

namespace Rollerghoster.Api
{
    public class GetTop10Seeds : AsyncScript
    {

        private MainMenuUI mainMenuUI;

        public override async Task Execute()
        {

            mainMenuUI = Entity.Get<MainMenuUI>();

            var url = $"{Settings.ApiUrl}/seed= {mainMenuUI.Seed}";
            var client = new RestClient(url);

            var request = new RestRequest();
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteGetAsync<RgResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data != null)
                {
                    var placeHolderButton = mainMenuUI.Top10SeedsButton as Button;
                    var placeHolderTextBlock = placeHolderButton.VisualChildren[0] as TextBlock;
                    mainMenuUI.Top10SeedsList.Children.Clear();
                    mainMenuUI.Top10LatestList.Children.Clear();

                    foreach (var seed in response.Data.Top10UsedSeeds)
                    {
                        var button = DuplicateButtonWithText(placeHolderButton, placeHolderTextBlock, seed);
                        button.Click += (sender, e) => mainMenuUI.SeedSelectedBtn(sender, e, HttpUtility.UrlDecode(seed));

                        mainMenuUI.Top10SeedsList.Children.Add(button);
                    }

                    foreach (var seed in response.Data.Top10LatestSeeds)
                    {
                        var button = DuplicateButtonWithText(placeHolderButton, placeHolderTextBlock, seed);
                        button.Click += (sender, e) => mainMenuUI.SeedSelectedBtn(sender, e, HttpUtility.UrlDecode(seed));

                        mainMenuUI.Top10LatestList.Children.Add(button);
                    }

                    mainMenuUI.Top10SeedsPanel.Visibility = Visibility.Visible;
                    mainMenuUI.Top10LatestPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private static Button DuplicateButtonWithText(Button placeHolderButton, TextBlock placeHolderTextBlock, string seed)
        {
            return new Button
            {
                Content = new TextBlock
                {
                    Font = placeHolderTextBlock.Font,
                    TextSize = placeHolderTextBlock.TextSize,
                    Height = placeHolderButton.Content.Height,
                    Text = HttpUtility.UrlDecode(seed),
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Height = placeHolderButton.Height,
                NotPressedImage = placeHolderButton.NotPressedImage,
                PressedImage = placeHolderButton.PressedImage,
                MouseOverImage = placeHolderButton.MouseOverImage,
            };
        }
    }
}