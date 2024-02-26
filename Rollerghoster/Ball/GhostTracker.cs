using Rollerghoster.GlobalEvents;
using Rollerghoster.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;

namespace Rollerghoster.Windows
{
    public class GhostTracker : SyncScript
    {
        public float storePosTime = 0.5f;
        public Prefab localGhostPrefab;
        public Prefab onlineGhostPrefab;
        public Prefab onlineGoldGhostPrefab;
        public Prefab onlineSilverGhostPrefab;
        public Prefab onlineBronzeGhostPrefab;
        public IngameUI ui;

        private List<Ghost> localGhosts;
        private List<Ghost> onlineGhosts;
        private Ghost onlineGoldGhost;
        private Ghost onlineSilverGhost;
        private Ghost onlineBronzeGhost;

        private List<Vector3> currentBallLifePositions;
        private float timeStamp = 0;
        private float ballTrackTimer = 0;
        private bool goldGhostsHidden = true;
        private bool silverGhostsHidden = true;
        private bool bronzeGhostsHidden = true;
        private bool ghostsHidden = true;
        private bool onlineGhostsHidden = true;
        private bool active = false;

        private EventReceiver finishedListener = new EventReceiver(GameGlobals.FinishedEventKey);
        private EventReceiver pauseListener = new EventReceiver(GameGlobals.PauseEventKey);
        private EventReceiver unpauseListener = new EventReceiver(GameGlobals.UnpauseEventKey);
        private EventReceiver seedChangedListener = new EventReceiver(GameGlobals.SeedChangedEventKey);

        public override void Start()
        {
            localGhosts = new List<Ghost>();
            onlineGhosts = new List<Ghost>();
            CreateStartingPositionList();
        }

        public void Activate()
        {
            ballTrackTimer = 0;
            timeStamp = 0;

            ClearGhosts();
            CreateStartingPositionList();
            ui.SetGhostsVisibilityText(ghostsHidden);
            active = true;
        }

        private void ClearGhosts()
        {
            foreach (var ghost in localGhosts)
            {
                SceneSystem.SceneInstance.Remove(ghost.Entity);
            }
            localGhosts = new List<Ghost>();
        }

        public override void Update()
        {
            if (finishedListener.TryReceive() || pauseListener.TryReceive())
            {
                active = false;
            }

            if (unpauseListener.TryReceive())
            {
                active = true;
            }

            if (seedChangedListener.TryReceive())
            {
                ClearOnlineGhosts();

            }

            if (active)
            {
                ballTrackTimer += (float)Game.UpdateTime.Elapsed.TotalSeconds;
                if (ballTrackTimer - timeStamp > storePosTime)
                {
                    timeStamp = ballTrackTimer;

                    currentBallLifePositions.Add(Entity.Transform.WorldMatrix.TranslationVector + new Vector3(0, 0.0f, 0));
                }

                var remainder = ballTrackTimer % storePosTime;
                var lerpIndex = (int)(ballTrackTimer / storePosTime);
                var amount = remainder / storePosTime;

                if (onlineGoldGhost != null)
                {
                    UpdateGhost(lerpIndex, amount, onlineGoldGhost);
                }

                if (onlineSilverGhost != null)
                {
                    UpdateGhost(lerpIndex, amount, onlineSilverGhost);
                }

                if (onlineBronzeGhost != null)
                {
                    UpdateGhost(lerpIndex, amount, onlineBronzeGhost);
                }

                foreach (var onlineGhost in onlineGhosts)
                {
                    UpdateGhost(lerpIndex, amount, onlineGhost);
                }

                foreach (var ghost in localGhosts)
                {
                    UpdateGhost(lerpIndex, amount, ghost);
                }

                if (Input.IsKeyPressed(Keys.D1))
                {
                    goldGhostsHidden = !goldGhostsHidden;
                    ToggleMedalGhost(onlineGoldGhost, goldGhostsHidden, "Gold");
                }

                if (Input.IsKeyPressed(Keys.D2))
                {
                    silverGhostsHidden = !silverGhostsHidden;
                    ToggleMedalGhost(onlineSilverGhost, silverGhostsHidden, "Silver");
                }

                if (Input.IsKeyPressed(Keys.D3))
                {
                    bronzeGhostsHidden = !bronzeGhostsHidden;
                    ToggleMedalGhost(onlineBronzeGhost, bronzeGhostsHidden, "Bronze");
                }

                if (Input.IsKeyPressed(Keys.D4))
                {
                    onlineGhostsHidden = !onlineGhostsHidden;
                    foreach (var ghost in onlineGhosts)
                    {
                        ghost.Entity.Enable<ModelComponent>(enabled: onlineGhostsHidden);
                    }
                    ui.SetOnlineGhostsVisibilityText(ghostsHidden);
                }

                if (Input.IsKeyPressed(Keys.D5))
                {
                    ghostsHidden = !ghostsHidden;
                    foreach (var ghost in localGhosts)
                    {
                        ghost.Entity.Enable<ModelComponent>(enabled: ghostsHidden);
                    }
                    ui.SetGhostsVisibilityText(ghostsHidden);
                }

                if (Input.IsKeyPressed(Keys.D6))
                {
                    ClearGhosts();
                }
            }
        }

        private static void UpdateGhost(int lerpIndex, float amount, Ghost ghost)
        {
            if (ghost.positions.Count > 1 && lerpIndex < ghost.positions.Count - 1)
            {
                var pos = Vector3.Lerp(ghost.positions[lerpIndex], ghost.positions[lerpIndex + 1], amount);
                ghost.Entity.Transform.Position = pos;
            }
        }

        private void ToggleMedalGhost(Ghost medalGhost, bool hidden, string medalType)
        {
            if (medalGhost != null)
            {
                medalGhost.Entity.Enable<ModelComponent>(enabled: hidden);

                if (medalType == "Gold")
                {
                    ui.SetGoldGhostsVisibilityText(hidden);
                }
                else if (medalType == "Silver")
                {
                    ui.SetSilverGhostsVisibilityText(hidden);
                }
                else if (medalType == "Bronze")
                {
                    ui.SetBronzeGhostsVisibilityText(hidden);
                }
            }
        }

        public string GetFinalGhost()
        {
            string positionsString = "";
            foreach (var position in currentBallLifePositions)
            {
                // Separate the different values in each Vector3 in pointsPosition and Quaternion in pointsRotation
                string x = Math.Round(position.X, 2).ToString();
                string y = Math.Round(position.Y, 2).ToString();
                string z = Math.Round(position.Z, 2).ToString();

                // Create and format a string and add it to the points list.
                positionsString += $"{x},{y},{z}_";
            }

            return positionsString;
        }

        public void StoreBallLifePositions()
        {
            var ghostEntity = localGhostPrefab.Instantiate().First();
            var ghostComponent = ghostEntity.Get<Ghost>();

            ghostComponent.SetPositions(currentBallLifePositions);
            localGhosts.Add(ghostComponent);
            SceneSystem.SceneInstance.RootScene.Entities.Add(ghostEntity);
            CreateStartingPositionList();
            ballTrackTimer = 0;
            timeStamp = 0;
        }

        public void StoreOnlineGhosts(List<Ghost> ghosts)
        {
            int ghostCount = 1;
            foreach (var onlineGhost in ghosts)
            {
                Entity ghostEntity = InstantiateGhostFromOnlineGhostCount(ghostCount);
                var ghostComponent = ghostEntity.Get<Ghost>();
                ghostComponent.SetPositions(onlineGhost.positions);

                if (ghostCount == 1)
                {
                    onlineGoldGhost = ghostComponent;
                }
                else if (ghostCount == 2)
                {
                    onlineSilverGhost = ghostComponent;
                }
                else if (ghostCount == 3)
                {
                    onlineBronzeGhost = ghostComponent;
                }
                else
                {
                    onlineGhosts.Add(ghostComponent);
                }

                ghostCount++;
                SceneSystem.SceneInstance.RootScene.Entities.Add(ghostEntity);
            }
        }

        private Entity InstantiateGhostFromOnlineGhostCount(int ghostCount)
        {
            if (ghostCount == 1)
            {
                return onlineGoldGhostPrefab.Instantiate().First();
            }
            else if (ghostCount == 2)
            {
                return onlineSilverGhostPrefab.Instantiate().First();
            }
            else if (ghostCount == 3)
            {
                return onlineBronzeGhostPrefab.Instantiate().First();
            }
            else
            {
                return onlineGhostPrefab.Instantiate().First();
            }
        }

        private void ClearOnlineGhosts()
        {
            //Clear medal ghosts
            if (onlineGoldGhost != null)
            {
                SceneSystem.SceneInstance.Remove(onlineGoldGhost.Entity);
                onlineGoldGhost = null;

            }

            if (onlineSilverGhost != null)
            {
                SceneSystem.SceneInstance.Remove(onlineSilverGhost.Entity);
                onlineSilverGhost = null;

            }

            if (onlineBronzeGhost != null)
            {
                SceneSystem.SceneInstance.Remove(onlineBronzeGhost.Entity);
                onlineBronzeGhost = null;
            }

            //Clear remainder
            foreach (var ghost in onlineGhosts)
            {
                SceneSystem.SceneInstance.Remove(ghost.Entity);
            }
            onlineGhosts = new List<Ghost>();

        }

        private void CreateStartingPositionList()
        {
            currentBallLifePositions = new List<Vector3>();
            currentBallLifePositions.Add(Entity.Transform.WorldMatrix.TranslationVector);
        }
    }
}
