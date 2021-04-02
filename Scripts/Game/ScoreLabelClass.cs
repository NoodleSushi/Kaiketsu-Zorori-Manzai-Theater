using Godot;
using System;

namespace Game
{
    public class ScoreLabelClass : Label
    {
        public int Score
        {
            set
            {
                Text = $"{value} 笑";
            }
        }
    }
}