using Godot;
using System;

namespace Game
{
    public static class ConfigHandler
    {
        const string SAVE_DIRECTORY = "user://save.cfg";
        const string SECTION_MAIN = "main";
        const string KEY_SCORE = "score";
        private static int BestScore = 0;

        public static void SaveScore(int Score)
        {
            ConfigFile Config = new ConfigFile();
            Config.Load(SAVE_DIRECTORY);
            BestScore = Math.Max(Score, BestScore);
            Config.SetValue(SECTION_MAIN, KEY_SCORE, BestScore);
            Config.Save(SAVE_DIRECTORY);
        }

        public static int GetScore()
        {
            ConfigFile Config = new ConfigFile();
            Error Err = Config.Load(SAVE_DIRECTORY);

            if (Err != Error.Ok)
            {
                GD.Print(Err);
                return BestScore;
            }

            BestScore = Math.Max((int)Config.GetValue(SECTION_MAIN, KEY_SCORE, 0), BestScore);
            return BestScore;
        }
    }
}