using Godot;
using System.Collections.Generic;

namespace Game.AudioPlayer
{
    public class Bgm : Rhythmer.ASPConductor
    {
        [Export] private readonly List<AudioStream> BGM_SAMPLE_LIST;

        public void PlayBGMStage(int stageIndex = 0)
        {
            Stream = BGM_SAMPLE_LIST[stageIndex];
            VolumeDb = -10f;
            Play();
        }
    }
}
