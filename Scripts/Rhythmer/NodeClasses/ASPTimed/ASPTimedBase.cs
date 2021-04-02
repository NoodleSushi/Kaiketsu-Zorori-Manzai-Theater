using Godot;
using System.Collections.Generic;

namespace Rhythmer
{
    public abstract class ASPTimedBase : Node
    {
        public const double RECOMMENDED_ALLOCATED_TIME = 0.1;

        public double AllocatedTime;
        protected readonly Queue<PlaybackMarker> PlaybackQueue = new Queue<PlaybackMarker>();
        public AudioStreamPlayer MusicPlayer;

        public bool TakeOverWhilePlaying = false;

        public virtual AudioStreamPlayer CurrentAudioPlayer { get; }

        public ASPTimedBase(double allocatedTime = RECOMMENDED_ALLOCATED_TIME)
        {
            AllocatedTime = allocatedTime;
        }

        public override void _EnterTree()
        {
            AddAudioPlayerChildren();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (MusicPlayer is null || !MusicPlayer.Playing) return;
            double ASPTime = RS.GetASPTimeSec(MusicPlayer);
            while (PlaybackQueue.Count > 0)
            {
                PlaybackMarker mission = PlaybackQueue.Peek();
                double playbackOffset = RS.GetPlaybackOffset(ASPTime, mission.Time, MusicPlayer.PitchScale, CurrentAudioPlayer.PitchScale, AllocatedTime);
                if (playbackOffset < 0 || (CurrentAudioPlayer.Playing && !TakeOverWhilePlaying))
                    return;

                if (playbackOffset < mission.Stream.GetLength())
                {
                    CurrentAudioPlayer.Stream = mission.Stream;
                    CurrentAudioPlayer.Play((float)playbackOffset);
                    
                    SwitchAudioPlayer();
                }
                PlaybackQueue.Dequeue();
            }
        }

        protected virtual void AddAudioPlayerChildren()
        {
        }

        protected virtual void SwitchAudioPlayer()
        {
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
