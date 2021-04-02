using Godot;
using System.Collections.Generic;

namespace Game.AudioPlayer
{
    public class Plr : AudioStreamPlayer
    {
        private static readonly AudioStreamSample PLR_HAI = GD.Load<AudioStreamSample>("res://Audio/Hai! voice.wav");
        private static readonly AudioStreamSample PLR_GETOVERIT = GD.Load<AudioStreamSample>("res://Audio/What's wrong with you!.wav");

        public void Play(bool isFail = false)
        {
            Stream = isFail ? PLR_GETOVERIT : PLR_HAI;
            base.Play();
        }
    }
}