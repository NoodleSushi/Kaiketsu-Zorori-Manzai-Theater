using Godot;
using System;

namespace Game
{
    public static class ConfigHandler
    {
        const string SaveDirectory = "user://save.cfg";
        const string SectionMain = "main";
        const string KeyScore = "score";
        private static int BestScore = 0;

        public static void SaveScore(int Score)
        {
            ConfigFile Config = new ConfigFile();
            Error Err = Config.Load(SaveDirectory);
            BestScore = Math.Max(Score, BestScore);
            Config.SetValue(SectionMain, KeyScore, BestScore);
            Config.Save(SaveDirectory);
        }

        public static int GetScore()
        {
            ConfigFile Config = new ConfigFile();
            Error Err = Config.Load(SaveDirectory);

            if (Err != Error.Ok)
            {
                GD.Print(Err);
                return BestScore;
            }

            BestScore = Math.Max((int)Config.GetValue(SectionMain, KeyScore, 0), BestScore);
            return BestScore;
        }
    }
}