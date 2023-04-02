using Godot;
using Utils;
using Rhythmer;

namespace Game.AudioPlayer
{
    public class Apl : ASPTimedSingle
    {
        [Export] private readonly AudioStream AUDIENCE_SAMPLE_1;
        [Export] private readonly AudioStream AUDIENCE_SAMPLE_2;

        public override void _Ready()
        {
            CurrentAudioPlayer.VolumeDb = -10f;
        }

        public void AddPlaybackMarker(double time = 0, bool isFail = false)
        {
            AddPlaybackMarker(new PlaybackMarker(
                time,
                isFail ? AUDIENCE_SAMPLE_2 : AUDIENCE_SAMPLE_1
                )
            );
        }
    }
}
