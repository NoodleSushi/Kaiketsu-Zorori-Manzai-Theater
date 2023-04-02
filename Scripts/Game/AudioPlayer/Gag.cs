using Godot;
using Utils;
using Rhythmer;

namespace Game.AudioPlayer
{
    public class Gag : ASPTimedSingle
    {
        [Export] private readonly PackedScene ZORORI_SAMPLE_RESOURCES_SCENE;
        [Export] private readonly AudioStreamSample ZORORI_SAMPLE_FAIL;
        private ResourcePreloader ZororiSampleLibrary;
        private RandomUniqueClass RandomUnique;
        
        public override void _Ready()
        {
            ZororiSampleLibrary = (ResourcePreloader)ZORORI_SAMPLE_RESOURCES_SCENE.Instance();
            RandomUnique = new RandomUniqueClass(5, ZororiSampleLibrary.GetResourceList().Length);
            CurrentAudioPlayer.VolumeDb = -10f;
        }

        public void AddPlaybackMarker(double time = 0, bool isFail = false)
        {
            int castedIdx = RandomUnique.Cast();
            AddPlaybackMarker(new PlaybackMarker(
                time,
                isFail ? ZORORI_SAMPLE_FAIL : (AudioStreamSample)ZororiSampleLibrary.GetResource($"Zorori{castedIdx+1}"),
                isFail ? "fail" : "gag",
                castedIdx
                )
            );
        }
        
        public void Stop() => CurrentAudioPlayer.Stop();
    }
}
