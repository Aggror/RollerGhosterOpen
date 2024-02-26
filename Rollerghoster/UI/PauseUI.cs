using System;
using System.Linq;
using Rollerghoster.GlobalEvents;
using Rollerghoster.Windows;
using Stride.Audio;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Games;
using Stride.Input;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;

namespace Rollerghoster.UI {

    public class PauseUI : SyncScript {
        public Entity marble;
        private IngameUI ingameUI;
        private MainMenuUI mainMenuUI;

        private bool Paused = false;
        private UIPage activePage;
        private Button RestartLevelBtn;
        private Button BackToMainMenuBtn;
        private Button ExitBtn;

        private bool active = false;

        private EventReceiver finishedListener = new EventReceiver(GameGlobals.FinishedEventKey);
        //private EventReceiver restartedLevelListener = new EventReceiver(GameGlobals.RestartedLevelEventKey);

        public override void Start() {
            var uiEntities = Entity.GetParent().GetChildren();
            ingameUI = uiEntities.Single(u => u.Name == "IngameUI").Get<IngameUI>();
            mainMenuUI = uiEntities.Single(u => u.Name == "MainMenuUI").Get<MainMenuUI>();

            activePage = Entity.Get<UIComponent>().Page;

            RestartLevelBtn = activePage.RootElement.FindVisualChildOfType<Button>("RestartLevelBtn");
            BackToMainMenuBtn = activePage.RootElement.FindVisualChildOfType<Button>("BackToMainMenuBtn");
            ExitBtn = activePage.RootElement.FindVisualChildOfType<Button>("ExitGameBtn");

            RestartLevelBtn.Click += RestartLevel;
            BackToMainMenuBtn.Click += BackToMainMenu;
            ExitBtn.Click += ExitGame;
        }

        public void Activate() {
            active = true;
        }

        public void Deactivate() {
            active = false;
            Paused = false;
        }


        private void RestartLevel(object sender, RoutedEventArgs e) {
            active = true;
            Paused = false;
            ingameUI.Activate();
            marble.Get<BallMover>().Activate();
            marble.Get<BallCamera>().Activate();
            marble.Get<CorpseTracker>().Activate();
            marble.Get<GhostTracker>().Activate();

            Entity.Enable<UIComponent>(enabled: false);
        }

        private void ExitGame(object sender, RoutedEventArgs e) {
            ((GameBase)Game).Exit();
        }

        private void BackToMainMenu(object sender, RoutedEventArgs e) {
            active = false;
            Entity.Enable<UIComponent>(enabled: false);
            ingameUI.Deactivate();
            mainMenuUI.Activate();
        }

        public override void Update() {
            if (finishedListener.TryReceive()) {
                active = false;
                Entity.Enable<UIComponent>(enabled: false);
            }

            if (active && Input.IsKeyPressed(Keys.Escape)) {
                Paused = !Paused;
                if (Paused) {
                    Pause();
                }
                else {
                    Resume();
                }
            }
        }

        private void Pause() {
            Game.Window.IsMouseVisible = true;
            Entity.Enable<UIComponent>(enabled: true);
            GameGlobals.PauseEventKey.Broadcast();
        }

        private void Resume() {
            Game.Window.IsMouseVisible = false;
            Entity.Enable<UIComponent>(enabled: false);
            GameGlobals.UnpauseEventKey.Broadcast();
        }
    }
}
