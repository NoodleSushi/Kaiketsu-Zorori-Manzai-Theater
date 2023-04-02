using Godot;
using System.Collections.Generic;

namespace Game.AudioPlayer
{
    public class Plr : AudioStreamPlayer
    {
        [Export] private readonly AudioStream PLR_HAI;
        [Export] private readonly AudioStream PLR_GETOVERIT;

        public void Play(bool isFail = false)
        {
            Stream = isFail ? PLR_GETOVERIT : PLR_HAI;
            base.Play();
        }
    }
}
