using Stride.Engine.Events;

namespace Rollerghoster.GlobalEvents {
    static class GameGlobals {
        public static EventKey FinishedEventKey = new EventKey("Global", "Finished");
        public static EventKey PauseEventKey = new EventKey("Global", "Pause");
        public static EventKey UnpauseEventKey = new EventKey("Global", "Unpause");
        public static EventKey SeedChangedEventKey = new EventKey("Global", "SeedChanged");
        public static EventKey ActivateMainMenuUIEventKey = new EventKey("Global", "ActivateMainMenuUI");
        public static EventKey ActivateIngameUIEventKey = new EventKey("Global", "ActivateIngameUI");
        public static EventKey RepositionedBallAtStartEventKey = new EventKey("Global", "RestartedLevelEventKey");



        public static EventKey MusicVolumeChangedEventKey = new EventKey("Global", "MusicVolumeChangedEventKey");
    }
}
