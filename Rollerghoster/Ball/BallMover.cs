using Rollerghoster.GlobalEvents;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;
using Stride.Physics;

namespace Rollerghoster.Windows {
    public class BallMover : SyncScript {
        public Vector3 ballSpeed = new Vector3(6, 0, 10);
        private RigidbodyComponent rb;
        private Entity fpPivot;
        private CorpseTracker corpseTracker;
        private GhostTracker ghostTracker;
        private bool active = false;

        private EventReceiver finishedListener = new EventReceiver(GameGlobals.FinishedEventKey);
        private EventReceiver pauseListener = new EventReceiver(GameGlobals.PauseEventKey);
        private EventReceiver unpauseListener = new EventReceiver(GameGlobals.UnpauseEventKey);

        public override void Start() {

        }

        public void Activate() {
            rb = Entity.Get<RigidbodyComponent>();
            ResetVelocity();
            rb.OverrideGravity = false;
            rb.Gravity = new Vector3(0, -16, 0);
            fpPivot = Entity.Get<BallCamera>().fpPivot;
            corpseTracker = Entity.Get<CorpseTracker>();
            ghostTracker = Entity.Get<GhostTracker>();
            active = true;
            RepositionAtStart();
        }

        public override void Update() {
            if (finishedListener.TryReceive() || pauseListener.TryReceive()) {
                active = false;
            }

            if (unpauseListener.TryReceive()) {
                active = true;
            }

            if (active) {
                var dir = new Vector3(0);

                // Forward/Backward
                if (Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.Up)) {
                    dir.Z += 1;
                }
                if (Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.Down)) {
                    dir.Z -= 1;
                }

                // Left/Right
                if (Input.IsKeyDown(Keys.A) || Input.IsKeyDown(Keys.Left)) {
                    dir.X += 1;
                }
                if (Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Right)) {
                    dir.X -= 1;
                }

                dir *= ballSpeed;
                dir = Vector3.Transform(dir, fpPivot.Transform.Rotation);

                rb.ApplyForce(dir);

                if (Input.IsKeyPressed(Keys.R)) {
                    ResetVelocity();

                    corpseTracker.CreateCorpse();
                    ghostTracker.StoreBallLifePositions();

                    RepositionAtStart();
                }

                //sneaky
                if (Input.IsKeyPressed(Keys.T)) {
                    ResetVelocity();
                }
            }
        }

        private void RepositionAtStart() {
            Entity.Transform.Position = new Vector3(0, 1, 0);
            Entity.Transform.UpdateWorldMatrix();
            rb.UpdatePhysicsTransformation();
            GameGlobals.RepositionedBallAtStartEventKey.Broadcast();
        }

        public void ResetVelocity() {
            rb.AngularVelocity = new Vector3(0);
            rb.LinearVelocity = new Vector3(0);
        }
    }
}
