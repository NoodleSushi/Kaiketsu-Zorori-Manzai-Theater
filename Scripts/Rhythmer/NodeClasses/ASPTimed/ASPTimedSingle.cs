using Godot;

namespace Rhythmer
{
    public class ASPTimedSingle : ASPTimedBase
    {
        [Signal]
        public delegate void NotifiedPlayback(bool isPlaying, string IdType, int Id);

        private readonly AudioStreamPlayer AudioPlayer = new AudioStreamPlayer();

        private string LatestIdType = "";
        private int LatestId = 0;

        public override AudioStreamPlayer CurrentAudioPlayer => AudioPlayer;

        public ASPTimedSingle(double allocatedTime = RECOMMENDED_ALLOCATED_TIME) : base(allocatedTime)
        {
        }

        protected override void AddAudioPlayerChildren()
        {
            AddChild(AudioPlayer);
        }

        protected override void SwitchAudioPlayer()
        {
            EmitSignal(nameof(NotifiedPlayback), false, LatestIdType, LatestId);
            LatestIdType = PlaybackQueue.Peek().IdType;
            LatestId = PlaybackQueue.Peek().Id;
            EmitSignal(nameof(NotifiedPlayback), true, LatestIdType, LatestId);
        }
    }
}
