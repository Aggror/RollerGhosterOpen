using Rollerghoster.GlobalEvents;
using Rollerghoster.UI;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Physics;

namespace Rollerghoster.Track {
    public class Finish : SyncScript {
        public FinishUI finishUI;
        public Entity marble;
        public float distanceToFinish = 1.4f;
        public float suction = 10.0f;
        public bool finished = false;

        private RigidbodyComponent rb;
        private Vector3 finishForcePoint;

        private EventReceiver repositionedBallAtStartListener = new EventReceiver(GameGlobals.RepositionedBallAtStartEventKey);

        public override void Start() {

        }

        public override void Update() {
            if (repositionedBallAtStartListener.TryReceive()) {
                finished = false;
            }

            var distance = Vector3.Distance(marble.Transform.WorldMatrix.TranslationVector, Entity.Transform.WorldMatrix.TranslationVector);

            if (!finished && distance < distanceToFinish) {
                finished = true;
                rb = marble.Get<RigidbodyComponent>();
                rb.OverrideGravity = true;
                rb.Gravity = new Vector3(0, 0, 0);
                finishForcePoint = Entity.Transform.WorldMatrix.TranslationVector + new Vector3(0, 2, 0);

                GameGlobals.FinishedEventKey.Broadcast();
                finishUI.Activate();
            }

            if (finished) {
                var distVector = finishForcePoint - marble.Transform.WorldMatrix.TranslationVector;
                distVector.Normalize();
                var appliedSuction = suction;
                if (distance < distanceToFinish) {
                    rb.LinearVelocity = rb.LinearVelocity * 0.9f;
                    appliedSuction = (distance / distanceToFinish) * suction;
                }

                rb.ApplyForce(distVector * appliedSuction);
            }
        }
    }
}
