using Godot;
using System;

namespace Game
{
    public class ScoreLabelClass : Label
    {
        public override void _Ready()
        {
            Score = 0;
        }

        public int Score
        {
            set
            {
                Text = string.Format(Tr("LBL_SCORE"), value);
            }
        }
    }
}