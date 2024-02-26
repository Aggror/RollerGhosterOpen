using Rollerghoster.GlobalEvents;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;
using Stride.UI;
using Stride.UI.Controls;

namespace Rollerghoster.UI
{

    public class IngameUI : SyncScript
    {
        public Entity marble;

        private float currentLevelTimer;
        private UIPage activePage;
        private TextBlock corpsesHiddenText;
        private TextBlock ghostsHiddenText;
        private TextBlock onlineGhostsHiddenText;

        private TextBlock goldGhostsHiddenText { get; set; }
        private TextBlock silverGhostsHiddenText { get; set; }
        private TextBlock bronzeGhostsHiddenText { get; set; }

        private TextBlock retryText;
        private TextBlock timerText;
        private bool active = false;

        private EventReceiver finishedListener = new EventReceiver(GameGlobals.FinishedEventKey);
        private EventReceiver restartedLevelListener = new EventReceiver(GameGlobals.RepositionedBallAtStartEventKey);
        private EventReceiver pauseListener = new EventReceiver(GameGlobals.PauseEventKey);
        private EventReceiver unpauseListener = new EventReceiver(GameGlobals.UnpauseEventKey);

        public void Activate()
        {
            Game.Window.IsMouseVisible = false;
            currentLevelTimer = 0;

            Entity.Enable<UIComponent>(enabled: true);
            activePage = Entity.Get<UIComponent>().Page;

            corpsesHiddenText = activePage.RootElement.FindVisualChildOfType<TextBlock>("corpsesHiddenText");
            ghostsHiddenText = activePage.RootElement.FindVisualChildOfType<TextBlock>("ghostsHiddenText");
            onlineGhostsHiddenText = activePage.RootElement.FindVisualChildOfType<TextBlock>("onlineGhostsHiddenText");

            goldGhostsHiddenText = activePage.RootElement.FindVisualChildOfType<TextBlock>("ToggleGoldMedalGhost");
            silverGhostsHiddenText = activePage.RootElement.FindVisualChildOfType<TextBlock>("ToggleSilverMedalGhost");
            bronzeGhostsHiddenText = activePage.RootElement.FindVisualChildOfType<TextBlock>("ToggleBronzeMedalGhost");

            retryText = activePage.RootElement.FindVisualChildOfType<TextBlock>("Retries");
            timerText = activePage.RootElement.FindVisualChildOfType<TextBlock>("LevelTimer");

            active = true;
        }

        public void Deactivate()
        {
            active = false;
            Entity.Enable<UIComponent>(enabled: false);
        }

        public override void Update()
        {
            if (pauseListener.TryReceive())
            {
                active = false;
            }

            if (unpauseListener.TryReceive())
            {
                active = true;
            }

            if (finishedListener.TryReceive())
            {
                Deactivate();
            }

            if (restartedLevelListener.TryReceive())
            {
                Activate();
            }


            if (active)
            {
                if (Input.IsKeyPressed(Keys.R))
                {
                    currentLevelTimer = 0;
                }

                currentLevelTimer += (float)Game.UpdateTime.Elapsed.TotalSeconds;
                timerText.Text = currentLevelTimer.ToString("00.000");
            }
        }

        public float GetCurrentLevelTime()
        {
            return currentLevelTimer;
        }


        internal void SetGoldGhostsVisibilityText(bool goldGhostsHidden)
        {
            if (goldGhostsHiddenText != null)
                goldGhostsHiddenText.Text = "1 - Gold ghost " + (goldGhostsHidden ? "visible" : "hidden");
        }

        internal void SetSilverGhostsVisibilityText(bool silverGhostsHidden)
        {
            if (silverGhostsHiddenText != null)
                silverGhostsHiddenText.Text = "2 - Silver ghost " + (silverGhostsHidden ? "visible" : "hidden");
        }

        internal void SetBronzeGhostsVisibilityText(bool bronzeGhostsHidden)
        {
            if (bronzeGhostsHiddenText != null)
                bronzeGhostsHiddenText.Text = "3 - Bronze ghost " + (bronzeGhostsHidden ? "visible" : "hidden");
        }

        internal void SetOnlineGhostsVisibilityText(bool onlineGhostsHidden)
        {
            if (onlineGhostsHiddenText != null)
                onlineGhostsHiddenText.Text = "4 - Online ghosts " + (onlineGhostsHidden ? "visible" : "hidden");
        }

        internal void SetGhostsVisibilityText(bool ghostsHidden)
        {
            if (ghostsHiddenText != null)
                ghostsHiddenText.Text = "5 - Local ghosts " + (ghostsHidden ? "visible" : "hidden");
        }

        internal void SetCorpseVisibilityText(bool corpsesHidden)
        {
            if (corpsesHiddenText != null)
                corpsesHiddenText.Text = "7 - Corpses " + (corpsesHidden ? "visible" : "hidden");
        }

        public void SetRetries(int corpseCount)
        {
            if (retryText != null)
                retryText.Text = corpseCount + " retries";
        }
    }
}
