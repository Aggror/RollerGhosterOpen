using Rollerghoster.GlobalEvents;
using Rollerghoster.Util;
using System;
using System.Diagnostics;
using System.Linq;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;
using static Rollerghoster.Util.Settings;

namespace Rollerghoster.UI
{
    public class SettingsUI : StartupScript {
        private MainMenuUI mainMenuUI;
        private UIPage activePage;

        Slider MusicVolumeSlider;
        Slider SoundEffectsVolumeSlider;
        Slider CameraSensitivitySlider;


        public Sound _changedSoundSettings = null;
        public Controls _changedControlsSettings = null;
        public General _changedGeneralSettings = null;
        public Visuals _changedVisualsSettings = null;

        ToggleButton InvertMouseYBtn;

        public override void Start() {
            var uiEntities = Entity.GetParent().GetChildren();
            mainMenuUI = uiEntities.Single(u => u.Name == "MainMenuUI").Get<MainMenuUI>();
        }

        public void Activate() {
            Game.Window.IsMouseVisible = true;
            Entity.Enable<UIComponent>(enabled: true);
            activePage = Entity.Get<UIComponent>().Page;

            _changedSoundSettings = Settings.SOUND;
            _changedControlsSettings = Settings.CONTROLS;
            _changedGeneralSettings = Settings.GENERAL;
            _changedVisualsSettings = Settings.VISUALS;

            var SaveBtn = activePage.RootElement.FindVisualChildOfType<Button>("SaveBtn");
            var BackBtn = activePage.RootElement.FindVisualChildOfType<Button>("BackBtn");

            SaveBtn.Click += SaveSettings;
            BackBtn.Click += BackToMainMenu;

            MusicVolumeSlider = activePage.RootElement.FindVisualChildOfType<Slider>("MusicVolumeSlider");
            SoundEffectsVolumeSlider = activePage.RootElement.FindVisualChildOfType<Slider>("SoundEffectsVolumeSlider");
            MusicVolumeSlider.ValueChanged += UpdateMusicVolumeSlider;
            SoundEffectsVolumeSlider.ValueChanged += UpdateSoundEffectsVolumeSlider;


            CameraSensitivitySlider = activePage.RootElement.FindVisualChildOfType<Slider>("CameraSensitivitySlider");
            InvertMouseYBtn = activePage.RootElement.FindVisualChildOfType<ToggleButton>("InvertMouseYBtn");
            CameraSensitivitySlider.ValueChanged += UpdateCameraSensitivity;
            InvertMouseYBtn.Checked += UpdateInvertMouseY;

            ApplyGameSettings();
        }



        private void UpdateSoundEffectsVolumeSlider(object sender, RoutedEventArgs e) {
            _changedSoundSettings.SoundEffectsVolume = SoundEffectsVolumeSlider.Value;
        }

        private void UpdateMusicVolumeSlider(object sender, RoutedEventArgs e) {
            _changedSoundSettings.MusicVolume = MusicVolumeSlider.Value;

            GameGlobals.MusicVolumeChangedEventKey.Broadcast();
        }

        private void UpdateCameraSensitivity(object sender, RoutedEventArgs e) {
            _changedControlsSettings.CameraSensitivity = CameraSensitivitySlider.Value;
        }

        private void UpdateInvertMouseY(object sender, RoutedEventArgs e) {
            _changedControlsSettings.InvertMouseY = InvertMouseYBtn.State == ToggleState.Checked;
        }

        private void ApplyGameSettings() {
            //Sound
            MusicVolumeSlider.Value = Settings.SOUND.MusicVolume;
            SoundEffectsVolumeSlider.Value = Settings.SOUND.SoundEffectsVolume;

            //Controls
            CameraSensitivitySlider.Value = Settings.CONTROLS.CameraSensitivity;
            InvertMouseYBtn.State = Settings.CONTROLS.InvertMouseY ? ToggleState.Checked : ToggleState.UnChecked;
            Debug.WriteLine(InvertMouseYBtn.State);
        }

        private void BackToMainMenu(object sender, EventArgs args) {
            Game.Window.IsMouseVisible = true;
            Entity.Enable<UIComponent>(enabled: false);
            mainMenuUI.Activate();
        }

        private void SaveSettings(object sender, EventArgs args) {
            //Sound
            SOUND.MusicVolume = _changedSoundSettings.MusicVolume;
            SOUND.SoundEffectsVolume = _changedSoundSettings.SoundEffectsVolume;

            //Controls
            CONTROLS.CameraSensitivity = _changedControlsSettings.CameraSensitivity;
            CONTROLS.InvertMouseY = _changedControlsSettings.InvertMouseY;

            Save();
        }
    }
}
