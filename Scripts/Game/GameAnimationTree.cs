using Godot;
using System;
using Rhythmer;

namespace Game
{
    public class GameAnimationTree : AnimationTree
    {
        private ASPTimedCallback @ASPTimedCallback;
        public AudioPlayer.Bgm BgmPlayer;

        public override void _Ready()
        {
            @ASPTimedCallback = GetNode<ASPTimedCallback>("ASPTimedCallback");
        }

        public void SetBgmPlayer(AudioPlayer.Bgm NewBgmPlayer)
        {
            BgmPlayer = NewBgmPlayer;
            @ASPTimedCallback.MusicPlayer = NewBgmPlayer;
        }

        public void QueueOneShot(string AnimationName, float time)
        {
            if (BgmPlayer is null)
                return;
            float calculatedTime = Mathf.Max(0, (time - (float) RS.GetASPTimeSec(BgmPlayer)) / BgmPlayer.PitchScale);
            ASPTimedCallback.AddCallback(this, calculatedTime, "set", $"parameters/{AnimationName}OneShot/active", true);
        }

        public void PlayOneShot(string AnimationName) => Set($"parameters/{AnimationName}OneShot/active", true);

        public void SetTimeScale(float TimeScale) => Set("parameters/TimeScale/scale", TimeScale);

        public void ClearAll() => @ASPTimedCallback.ClearCallbackQueue();

        public void GenerateBeginningOneShots(float Bpm, bool isBeginning)
        {
            for (int i = 0; i < 8; i++)
            {
                float time = (float)(i * 60 / Bpm);
                QueueOneShot("audience_pulse", time);
                QueueOneShot("mini_zorori_pulse", time);
                QueueOneShot("bgcontrol_pulse", time);
                if (isBeginning)
                {
                    QueueOneShot("zorori_pulse", time);
                    QueueOneShot("inoshishi_pulse", time);
                }
            }
        }
    }
}
