using System;
using System.Collections.Generic;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.Physics;

namespace Rollerghoster.Track {
    public class TrackGenerator : SyncScript {
        public int trackAmount = 20;
        public Entity start;
        public Entity finish;
        public List<Prefab> trackPrefabs = new List<Prefab>();

        private List<Entity> _newTracks;
        private bool debugPhysics = false;

        public override void Start() {
        }

        public void Generate(string seed) {
            Clean();

            _newTracks.Add(start);

            var randSeed = seed.ToCharArray().Sum(x => x) % 100;
            var rand = new Random(randSeed);

            for (int i = 0; i < trackAmount; i++) {
                var randomTrackNumber = rand.Next(0, trackPrefabs.Count);
                var newTrackBase = trackPrefabs[randomTrackNumber].Instantiate().First();

                newTrackBase.Transform.Position = new Vector3(i * -5, 0, 0);
                newTrackBase.Transform.UpdateWorldMatrix();
                SceneSystem.SceneInstance.RootScene.Entities.Add(newTrackBase);

                var previousTrack = _newTracks[i];
                var end = previousTrack.GetChildren().Single(child => child.Name == "End");
                end.Transform.GetWorldTransformation(out Vector3 post, out Quaternion rot, out Vector3 scale);
                newTrackBase.Transform.Rotation = rot;
                var pos = previousTrack.Transform.Position + Vector3.Transform(end.Transform.Position, previousTrack.Transform.Rotation);
                newTrackBase.Transform.Position = pos;

                newTrackBase.Transform.UpdateWorldMatrix();

                foreach (var child in newTrackBase.GetChildren()) {
                    child.Transform.UpdateWorldMatrix();
                    var collider = child.Get<StaticColliderComponent>();
                    if (collider != null) {
                        collider.UpdatePhysicsTransformation();
                    }
                }
                _newTracks.Add(newTrackBase);
            }

            //Create the finish line
            var lastTrack = _newTracks[^1];
            var lastTrackEnd = _newTracks[^1].GetChildren().Single(child => child.Name == "End");
            lastTrackEnd.Transform.GetWorldTransformation(out Vector3 posta, out Quaternion rota, out Vector3 scalea);
            finish.Transform.Rotation = rota;
            var posb = lastTrack.Transform.Position + Vector3.Transform(lastTrackEnd.Transform.Position, lastTrack.Transform.Rotation);
            finish.Transform.Position = posb;
            finish.Transform.UpdateWorldMatrix();

            foreach (var child in finish.GetChildren()) {
                child.Transform.UpdateWorldMatrix();
                var collider = child.Get<StaticColliderComponent>();
                if (collider != null) {
                    collider.UpdatePhysicsTransformation();
                }
            }
            _newTracks.Add(finish);
        }

        private void Clean() {
            if (_newTracks != null) {
                foreach (var track in _newTracks) {
                    if (track.Name != "StartBase" && track.Name != "FinishBase") {
                        SceneSystem.SceneInstance.Remove(track);
                    }
                }
            }
            _newTracks = new List<Entity>();
        }

        public override void Update() {
            if (Input.IsKeyReleased(Keys.P)) {
                this.GetSimulation().ColliderShapesRendering = debugPhysics = !debugPhysics;
            }
        }
    }
}