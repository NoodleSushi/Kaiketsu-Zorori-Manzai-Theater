using Godot;

namespace Game
{
    public class BestScoreLabelClass : Label
    {
        public int BestScore
        {
            set
            {
                Text = string.Format(Tr("LBL_RECORD"), value);
            }
        }
    }
}