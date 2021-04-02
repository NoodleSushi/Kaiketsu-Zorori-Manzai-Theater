using Godot;
using System.Collections.Generic;

namespace Rhythmer
{
    public class ASPTimedGroup : ASPTimedBase
    {
        private readonly List<AudioStreamPlayer> AudioPlayerList = new List<AudioStreamPlayer>();
        private readonly int AudioPlayerCount;
        
        private int AudioPlayerListIndex = 0;

        public override AudioStreamPlayer CurrentAudioPlayer => AudioPlayerList[AudioPlayerListIndex];

        public ASPTimedGroup(double allocatedTime = RECOMMENDED_ALLOCATED_TIME, int audioPlayerCount = 1) : base(allocatedTime)
        {
            AudioPlayerCount = audioPlayerCount;
        }

        protected override void AddAudioPlayerChildren()
        {
            for (int i = 0; i < AudioPlayerCount; i++)
            {
                AudioStreamPlayer newAudioPlayer = new AudioStreamPlayer();
                AudioPlayerList.Add(newAudioPlayer);
                AddChild(newAudioPlayer);
            }
        }

        protected override void SwitchAudioPlayer()
        {
            AudioPlayerListIndex = (AudioPlayerListIndex + 1) % AudioPlayerCount;
        }
    }
}
