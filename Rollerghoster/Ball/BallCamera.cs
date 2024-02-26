using Rollerghoster.GlobalEvents;
using Rollerghoster.Util;
using System.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Games;
using Stride.Input;

namespace Rollerghoster.Windows {
    public class BallCamera : SyncScript {
        public Entity cameraPivot;
        public Entity fpPivot;
        public Entity tpPivot;
        public Vector2 mouseSpeedMultiplier = new Vector2(15, 40);
        public bool smoothMouse = true;
        public float smoothMouseMultiplier = 0.2f;
        public Vector2 camAngles = new Vector2(20, 70);

        private Vector3 camOffset;
        private Vector2 mouseDif;
        private Vector3 camRotation;
        private bool active = false;
        private bool mouseUpdateNeeded = false;

        private EventReceiver finishedListener = new EventReceiver(GameGlobals.FinishedEventKey);
        private EventReceiver pauseListener = new EventReceiver(GameGlobals.PauseEventKey);
        private EventReceiver unpauseListener = new EventReceiver(GameGlobals.UnpauseEventKey);
        private EventReceiver restartedLevelListener = new EventReceiver(GameGlobals.RepositionedBallAtStartEventKey);

        public override void Start() {
            camOffset = cameraPivot.Transform.Position;
        }

        public void Activate() {
            Input.MousePosition = new Vector2(0.5f, 0.5f);
            camRotation = new Vector3(0.5f, 0, 0);
            active = true;
            mouseUpdateNeeded = true;
        }

        public override void Update() {
            if (finishedListener.TryReceive() || pauseListener.TryReceive()) {
                active = false;
            }

            if (unpauseListener.TryReceive()) {
                active = true;
            }

            if (restartedLevelListener.TryReceive()) {
                Activate();
            }

            if (!mouseUpdateNeeded && active) {
                var mouseMovement = new Vector2(0, 0);
                var deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;
                var mousePos = Input.MousePosition;
                mouseDif = new Vector2(0.5f - mousePos.X, 0.5f - mousePos.Y);

                //Adjust and set the camera rotation
                mouseMovement.X += mouseDif.X * mouseSpeedMultiplier.X * Settings.CONTROLS.CameraSensitivity * deltaTime;
                mouseMovement.Y += mouseDif.Y * mouseSpeedMultiplier.Y * Settings.CONTROLS.CameraSensitivity * deltaTime;

                //camRotation.X -= MathUtil.Clamp(mouseMovement.Y, camAngles.X, camAngles.Y);
                camRotation.X += Settings.CONTROLS.InvertMouseY ? mouseMovement.Y: -mouseMovement.Y;
                camRotation.Y += mouseMovement.X;

                fpPivot.Transform.Position = Entity.Transform.Position + new Vector3(0, 1.5f, 0);
                fpPivot.Transform.RotationEulerXYZ = camRotation;
                //Debug.WriteLine(cameraPivot.Transform.RotationEulerXYZ);
                //tpPivot.Transform.Position = Entity.Transform.Position + new Vector3(0, 1.5f, 0);
                //tpPivot.Transform.RotationEulerXYZ = camRotation;
                //tpPivot.Transform.Position += Vector3.Transform(camOffset, fpPivot.Transform.Rotation);

                cameraPivot.Transform.Position = Entity.Transform.Position;
                cameraPivot.Transform.RotationEulerXYZ = camRotation;
                cameraPivot.Transform.Position += Vector3.Transform(camOffset, fpPivot.Transform.Rotation);

                Input.MousePosition = new Vector2(0.5f);

                if (Input.IsKeyDown(Keys.R)) {
                    camRotation = new Vector3(0.5f, 0, 0);
                }
            }

            if (mouseUpdateNeeded) {
                Input.MousePosition = new Vector2(0.5f, 0.5f);
                mouseUpdateNeeded = false;
            }
        }
    }
}
