using Rollerghoster.Api;
using Rollerghoster.Windows;
using System;
using System.Diagnostics;
using System.Linq;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;
using Stride.UI.Panels;

namespace Rollerghoster.UI
{

    public class FinishUI : SyncScript
    {
        public Entity marble;
        public StackPanel OnlineRecordsPanel { get; private set; }
        public int CorpseCount { get; private set; }
        public float FinishTime { get; private set; }


        private IngameUI ingameUI;
        private PauseUI pauseUI;
        private MainMenuUI mainMenuUI;

        private UIPage activePage;
        private TextBlock LevelTimerTxt;
        private TextBlock RetriesTxt;

        private Button RestartLevelBtn;
        private Button ReplayBtn;
        private Button BackToMainMenuBtn;
        private UpdateOnlineHighScores updateOnlineHighScoresComponent;

        public override void Start()
        {
            var uiEntities = Entity.GetParent().GetChildren();
            ingameUI = uiEntities.Single(u => u.Name == "IngameUI").Get<IngameUI>();
            pauseUI = uiEntities.Single(u => u.Name == "PauseUI").Get<PauseUI>();
            mainMenuUI = uiEntities.Single(u => u.Name == "MainMenuUI").Get<MainMenuUI>();

            activePage = Entity.Get<UIComponent>().Page;

            RestartLevelBtn = activePage.RootElement.FindVisualChildOfType<Button>("RestartLevelBtn");
            ReplayBtn = activePage.RootElement.FindVisualChildOfType<Button>("ReplayBtn");
            BackToMainMenuBtn = activePage.RootElement.FindVisualChildOfType<Button>("BackToMainMenuBtn");

            OnlineRecordsPanel = activePage.RootElement.FindVisualChildOfType<StackPanel>("OnlineRecordsPanel");

            RestartLevelBtn.Click += RestartLevel;
            ReplayBtn.Click += ViewReplay;
            BackToMainMenuBtn.Click += BackToMainMenu;

        }

        public void Activate()
        {
            Game.Window.IsMouseVisible = true;

            Entity.Enable<UIComponent>(enabled: true);

            LevelTimerTxt = activePage.RootElement.FindVisualChildOfType<TextBlock>("LevelTimerTxt");
            RetriesTxt = activePage.RootElement.FindVisualChildOfType<TextBlock>("RetriesTxt");

            CorpseCount = marble.Get<CorpseTracker>().GetCorpseCount();
            FinishTime = ingameUI.GetCurrentLevelTime();

            SetRetries(CorpseCount);
            SetFinishTime(FinishTime);

            Entity.RemoveAll<UpdateOnlineHighScores>();
            updateOnlineHighScoresComponent = new UpdateOnlineHighScores();
            Entity.Add(updateOnlineHighScoresComponent);
        }


        private void RestartLevel(object sender, RoutedEventArgs e)
        {
            ingameUI.Activate();
            pauseUI.Activate();
            marble.Get<BallMover>().Activate();
            marble.Get<BallCamera>().Activate();
            marble.Get<CorpseTracker>().Activate();
            marble.Get<GhostTracker>().Activate();

            Entity.Enable<UIComponent>(enabled: false);
        }

        private void ViewReplay(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackToMainMenu(object sender, RoutedEventArgs e)
        {
            ingameUI.Deactivate();
            pauseUI.Deactivate();
            Entity.Enable<UIComponent>(enabled: false);
            mainMenuUI.Activate();
        }

        public override void Update()
        {
        }

        public void SetRetries(int corpseCount)
        {
            if (RetriesTxt != null)
                RetriesTxt.Text = corpseCount + (corpseCount == 1 ? " retry" : " retries");
        }

        public void SetFinishTime(float levelTime)
        {
            if (LevelTimerTxt != null)
                LevelTimerTxt.Text = GetFinishTime(levelTime);
        }

        private string GetFinishTime(float levelTime)
        {
            return levelTime.ToString("00.000");
        }
    }
}
