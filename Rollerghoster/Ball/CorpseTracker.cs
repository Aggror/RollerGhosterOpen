using Rollerghoster.GlobalEvents;
using Rollerghoster.UI;
using System.Collections.Generic;
using System.Linq;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;

namespace Rollerghoster.Windows {
    public class CorpseTracker : SyncScript {

        public Prefab corpse;
        public IngameUI ui;

        private List<Entity> corpses;
        private bool corpsesHidden = true;
        private bool active = false;

        private EventReceiver pauseListener = new EventReceiver(GameGlobals.PauseEventKey);
        private EventReceiver unpauseListener = new EventReceiver(GameGlobals.UnpauseEventKey);

        public override void Start() {
            corpses = new List<Entity>();
        }

        public void Activate() {
            ClearCorpses();
            ui.SetRetries(corpses.Count);
            ui.SetCorpseVisibilityText(corpsesHidden);
            active = true;
        }

        public int GetCorpseCount() {
            return corpses.Count;
        }

        public override void Update() {
            if (active) {
                if (Input.IsKeyPressed(Keys.D7)) {
                    corpsesHidden = !corpsesHidden;
                    foreach (var corpse in corpses) {
                        corpse.Enable<ModelComponent>(enabled: corpsesHidden);
                    }
                    ui.SetCorpseVisibilityText(corpsesHidden);
                }

                if (Input.IsKeyPressed(Keys.D8)) {
                    ClearCorpses();
                }
            }
        }

        private void ClearCorpses() {
            foreach (var corpse in corpses) {
                SceneSystem.SceneInstance.Remove(corpse);
            }
            corpses = new List<Entity>();
        }

        public void CreateCorpse() {
            var newCorpse = corpse.Instantiate().First();
            newCorpse.Transform.Position = Entity.Transform.WorldMatrix.TranslationVector;
            newCorpse.Transform.UpdateWorldMatrix();
            corpses.Add(newCorpse);
            SceneSystem.SceneInstance.RootScene.Entities.Add(newCorpse);
            ui.SetRetries(corpses.Count);
        }
    }
}
