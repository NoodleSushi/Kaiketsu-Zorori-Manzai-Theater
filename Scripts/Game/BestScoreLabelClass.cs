using Godot;
using System;

namespace Game
{
    public class BestScoreLabelClass : Label
    {
        public int BestScore
        {
            set
            {
                Text = $"自己ベスト    {(value > 9 ? " " : "")}{value}";
            }
        }
    }
}