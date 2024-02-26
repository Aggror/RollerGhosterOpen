using Rollerghoster.Api;
using Rollerghoster.GlobalEvents;
using Rollerghoster.Track;
using Rollerghoster.Util;
using Rollerghoster.Windows;
using System;
using System.Linq;
using System.Runtime.Serialization;
using Stride.Engine;
using Stride.Games;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;
using Stride.UI.Panels;

namespace Rollerghoster.UI
{
    public class MainMenuUI : StartupScript
    {
        public TrackGenerator trackGenerator;
        public Entity marble;

        private IngameUI ingameUI;
        private SettingsUI settingsUI;
        private PauseUI pauseUI;
        private UIPage activePage;

        public Panel Top10SeedsPanel;
        public Panel Top10SeedsList;
        public Button Top10SeedsButton;

        public Panel Top10LatestPanel;
        public Panel Top10LatestList;
        public Button Top10LatestButton;

        public Panel RecordsPanel;
        public Panel RecordsList;
        public TextBlock RecordName;
        public TextBlock RecordTime;

        private Button RandomSeedBtn;
        private EditText SeedEditText;
        private EditText UserNameEditText;
        private Button SeedStartBtn;
        private Button SettingsBtn;
        private Button ExitBtn;

        [IgnoreDataMember]
        public string Seed = Settings.DefaultSeed;
        public string UserName = "";

        public override void Start()
        {
            Settings.Init();
            Game.Window.AllowUserResizing = true;

            var uiEntities = Entity.GetParent().GetChildren();
            settingsUI = uiEntities.Single(u => u.Name == "SettingsUI").Get<SettingsUI>();
            ingameUI = uiEntities.Single(u => u.Name == "IngameUI").Get<IngameUI>();
            pauseUI = uiEntities.Single(u => u.Name == "PauseUI").Get<PauseUI>();

            activePage = Entity.Get<UIComponent>().Page;

            UserNameEditText = activePage.RootElement.FindVisualChildOfType<EditText>("userNameInputTxt");
            RandomSeedBtn = activePage.RootElement.FindVisualChildOfType<Button>("RandomSeedBtn");
            SeedEditText = activePage.RootElement.FindVisualChildOfType<EditText>("seedInputTxt");
            SeedStartBtn = activePage.RootElement.FindVisualChildOfType<Button>("SeedStartBtn");
            SettingsBtn = activePage.RootElement.FindVisualChildOfType<Button>("SettingsBtn");
            ExitBtn = activePage.RootElement.FindVisualChildOfType<Button>("ExitBtn");

            Top10SeedsPanel = activePage.RootElement.FindVisualChildOfType<Panel>("Top10SeedsPanel");
            Top10SeedsList = activePage.RootElement.FindVisualChildOfType<Panel>("Top10SeedsList");
            Top10SeedsButton = activePage.RootElement.FindVisualChildOfType<Button>("Top10SeedBtn");

            Top10LatestPanel = activePage.RootElement.FindVisualChildOfType<Panel>("Top10LatestPanel");
            Top10LatestList = activePage.RootElement.FindVisualChildOfType<Panel>("Top10LatestList");
            Top10LatestButton = activePage.RootElement.FindVisualChildOfType<Button>("Top10LatestBtn");

            RecordsPanel = activePage.RootElement.FindVisualChildOfType<Panel>("TopRecordsPanel");
            RecordsList = activePage.RootElement.FindVisualChildOfType<Panel>("TopRecordsList");
            RecordName = activePage.RootElement.FindVisualChildOfType<TextBlock>("TopRecordName");
            RecordTime = activePage.RootElement.FindVisualChildOfType<TextBlock>("TopRecordTime");

            UserNameEditText.Text = Settings.UserName;

            RandomSeedBtn.Click += GenerateRandomSeed;
            SeedStartBtn.Click += StartCustomGame;
            UserNameEditText.TextChanged += UserNameInputChanged;
            SeedEditText.TextChanged += CustomSeedInputChanged;
            SettingsBtn.Click += ToggleOptions;
            ExitBtn.Click += ExitGame;

            SeedStartBtn.Visibility = Visibility.Hidden;
           
            Activate();
        }

        public void Activate()
        {
            Game.Window.IsMouseVisible = true;
            Entity.Enable<UIComponent>(enabled: true);

            Top10SeedsPanel.Visibility = Visibility.Hidden;
            Top10LatestPanel.Visibility = Visibility.Hidden;

            Entity.RemoveAll<GetTop10Seeds>();
            var getTop10Seeds = new GetTop10Seeds();
            Entity.Add(getTop10Seeds);

            if (string.IsNullOrWhiteSpace(Seed))
            {
                SeedEditText.Text = Settings.DefaultSeed;
            }
            else
            {
                UpdateRecordsForSeed();
            }
        }

        private void UserNameInputChanged(object sender, RoutedEventArgs e)
        {
            UserName = ("" + UserNameEditText.Text);
            if (string.IsNullOrWhiteSpace(UserName))
            {
                SeedStartBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                SeedStartBtn.Visibility = Visibility.Visible;
                Settings.UserName = UserNameEditText.Text;
                Settings.Save();
            }
        }

        public void SeedSelectedBtn(object sender, EventArgs args, string seed)
        {
            SeedEditText.Text = seed;
        }

        private void GenerateRandomSeed(object sender, EventArgs args)
        {
            SeedEditText.Text = new Random().Next(0, int.MaxValue).ToString();
        }

        private void CustomSeedInputChanged(object sender, EventArgs args)
        {
            Seed = ("" + SeedEditText.Text);
            trackGenerator.Generate(Seed);

            GameGlobals.SeedChangedEventKey.Broadcast();
            UpdateRecordsForSeed();
        }

        private void UpdateRecordsForSeed()
        {
            Entity.RemoveAll<GetRecordsForSeed>();
            var updateOnlineGhostsForSeed = new GetRecordsForSeed();
            Entity.Add(updateOnlineGhostsForSeed);
        }

        private void ToggleOptions(object sender, EventArgs args)
        {
            Entity.Enable<UIComponent>(enabled: false);
            settingsUI.Activate();
        }

        private void StartGame()
        {
            UserName = UserNameEditText.Text;
            UserNameEditText.IsSelectionActive = false;
            SeedEditText.IsSelectionActive = false;
            ingameUI.Entity.Enable<UIComponent>(enabled: true);
            ingameUI.Activate();
            pauseUI.Activate();
            marble.Get<BallMover>().Activate();
            marble.Get<BallCamera>().Activate();
            marble.Get<CorpseTracker>().Activate();
            marble.Get<GhostTracker>().Activate();
            Entity.Enable<UIComponent>(enabled: false);
        }

        private void StartCustomGame(object sender, EventArgs args)
        {
            StartGame();
        }

        private void ExitGame(object sender, RoutedEventArgs e)
        {
            ((GameBase)Game).Exit();
        }
    }
}
