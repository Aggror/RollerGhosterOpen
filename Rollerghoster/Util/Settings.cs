using System.Data.SQLite;

namespace Rollerghoster.Util
{
    public static class Settings
    {
        public static string DefaultSeed = "776025295";
        public static string UserName = "";
        public static Sound SOUND = null;
        public static Controls CONTROLS = null;
        public static General GENERAL = null;
        public static Visuals VISUALS = null;

        public static string ApiUrl {
             get { return "http://127.0.0.1:8080"; }
        }

        public static void Init()
        {
            GENERAL = new General();
            CONTROLS = new Controls();
            SOUND = new Sound();
            VISUALS = new Visuals();
            
            SOUND.MusicVolume = 100;
            SOUND.SoundEffectsVolume = 100;
            CONTROLS.CameraSensitivity = 50;
            CONTROLS.InvertMouseY = false;
            
            // if (SOUND == null || CONTROLS == null)
            // {
            //     var db = new SQLiteConnection("Data Source=settings.db;Version=3;");
            //     db.Open();
            //
            //     GENERAL = new General();
            //     CONTROLS = new Controls();
            //     SOUND = new Sound();
            //     VISUALS = new Visuals();
            //
            //     var sql = "select * from settings where name = 'Default'";
            //     var command = new SQLiteCommand(sql, db);
            //     var result = command.ExecuteReader();
            //     while (result.Read())
            //     {
            //         SOUND.MusicVolume = float.Parse(result["MusicVolume"].ToString());
            //         SOUND.SoundEffectsVolume = float.Parse(result["SoundEffectsVolume"].ToString());
            //         CONTROLS.CameraSensitivity = float.Parse(result["CameraSensitivity"].ToString());
            //         CONTROLS.InvertMouseY = result["InvertMouseY"].ToString() == "1";
            //         UserName = result["UserName"].ToString();
            //         break;
            //     }
            //
            //     db.Close();
            //     db.Dispose();
            // }
        }

        public static void Save()
        {
            // var db = new SQLiteConnection("Data Source=settings.db;Version=3;");
            // db.Close();
            // db.Open();
            //
            // var command = new SQLiteCommand(db);
            // var sql = @"
            //     UPDATE settings 
            //     SET MusicVolume = @MusicVolume,
            //     SoundEffectsVolume = @SoundEffectsVolume,
            //     InvertMouseY = @InvertMouseY,
            //     CameraSensitivity = @CameraSensitivity,
            //     UserName = @UserName
            //     WHERE Name = 'Default'";
            //
            // command.CommandText = sql;
            // command.Parameters.AddWithValue("@MusicVolume", SOUND.MusicVolume.ToString());
            // command.Parameters.AddWithValue("@SoundEffectsVolume", SOUND.SoundEffectsVolume.ToString());
            // command.Parameters.AddWithValue("@InvertMouseY", CONTROLS.InvertMouseY.ToString());
            // command.Parameters.AddWithValue("@CameraSensitivity", CONTROLS.CameraSensitivity.ToString());
            // command.Parameters.AddWithValue("@UserName", UserName.ToString());
            // command.ExecuteNonQuery();
            //
            // db.Close();   
            // db.Dispose();
        }

        public class Sound
        {
            public float MusicVolume;
            public float SoundEffectsVolume;
        }

        public class General
        {
            public bool ShowSubtitles;
        }

        public class Controls
        {
            public bool InvertMouseY;
            public float CameraSensitivity;
        }

        public class Visuals
        {
            public string Godrays;
            public string Bloom;
            public string AnisotopicFilter;
        }
    }
}
