using Godot;

namespace Game
{
    public class BestScoreLabelClass : Label
    {
        public int BestScore
        {
            set
            {
                Text = $"自己ベスト   {(value > 99 ? "" : (value > 9 ? " " : "  "))}{value}";
            }
        }
    }
}