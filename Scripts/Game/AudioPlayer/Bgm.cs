using Godot;
using System.Collections.Generic;

namespace Game.AudioPlayer
{
    public class Bgm : Rhythmer.ASPConductor
    {
        private static readonly List<AudioStreamOGGVorbis> BGM_SAMPLE_LIST = new List<AudioStreamOGGVorbis> { 
            GD.Load<AudioStreamOGGVorbis>("res://Audio/BGM0.ogg"), 
            GD.Load<AudioStreamOGGVorbis>("res://Audio/BGM1.ogg"), 
            GD.Load<AudioStreamOGGVorbis>("res://Audio/BGM2.ogg"),
            GD.Load<AudioStreamOGGVorbis>("res://Audio/BGM3.ogg"),
            GD.Load<AudioStreamOGGVorbis>("res://Audio/BGM4.ogg"),
            GD.Load<AudioStreamOGGVorbis>("res://Audio/BGM5.ogg"),
            GD.Load<AudioStreamOGGVorbis>("res://Audio/BGM6.ogg")
        };

        public void PlayBGMStage(int stageIndex = 0)
        {
            Stream = BGM_SAMPLE_LIST[stageIndex];
            VolumeDb = 0f;
            Play();
        }
    }
}