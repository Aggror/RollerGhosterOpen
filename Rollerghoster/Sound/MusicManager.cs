using Rollerghoster.GlobalEvents;
using Rollerghoster.Util;
using Stride.Audio;
using Stride.Engine;
using Stride.Engine.Events;

namespace Rollerghoster.UI {
    public class MusicManager : SyncScript {
        public Sound SoundMusic;
        private SoundInstance music;

        private Sound finishSound;
        private SoundInstance finishSoundInstance;

        private EventReceiver finishListener = new EventReceiver(GameGlobals.FinishedEventKey);
        private EventReceiver musicVolumeChangedListener = new EventReceiver(GameGlobals.MusicVolumeChangedEventKey);


        public override void Start() {
            finishSound = Content.Load<Sound>("Sounds/finish");
            finishSoundInstance = finishSound.CreateInstance();
            finishSoundInstance.IsLooping = false;

            music = SoundMusic.CreateInstance();
            music.IsLooping = true;
            music.Volume = Settings.SOUND.MusicVolume / 100;
            music.Play();
        }

        public override void Update() {
            if (musicVolumeChangedListener.TryReceive()) {
                music.Volume = Settings.SOUND.MusicVolume / 100;
            }

            if (finishListener.TryReceive()) {
                finishSoundInstance.Play();
                finishSoundInstance.Volume = Settings.SOUND.SoundEffectsVolume / 100;
            }
        }
    }
}
