using Godot;
using System.Collections.Generic;

namespace Rhythmer
{
    public class AnimTimed : Node
    {
        protected readonly Queue<PlaybackMarker> PlaybackQueue = new Queue<PlaybackMarker>();
        public AudioStreamPlayer MusicPlayer;

        public override void _Process(float delta)
        {
            if (MusicPlayer is null || !MusicPlayer.Playing) return;

            double ASPTime = RS.GetASPTimeSec(MusicPlayer);
            while (PlaybackQueue.Count > 0)
            {
                PlaybackMarker mission = PlaybackQueue.Peek();
                double playbackOffset = RS.GetOffsetTimeSec(ASPTime, mission.Time);

                if (playbackOffset < 0) return;

                /*CurrentAudioPlayer.Stream = mission.Stream;
                CurrentAudioPlayer.Play((float)playbackOffset);
                SwitchAudioPlayer();*/
                PlaybackQueue.Dequeue();
            }
        }

        public void AddPlaybackMarker(PlaybackMarker mission)
        {
            PlaybackQueue.Enqueue(mission);
        }

        public void ClearPlaybackMarkerQueue()
        {
            PlaybackQueue.Clear();
        }
    }
}
