using Godot;
using Utils;
using Rhythmer;

namespace Game.AudioPlayer
{
    public class Gag : ASPTimedSingle
    {
        private static readonly PackedScene ZORORI_SAMPLE_RESOURCES_SCENE = GD.Load<PackedScene>("res://Audio/ZororiSamples.tscn");
        private static readonly AudioStreamSample ZORORI_SAMPLE_FAIL = GD.Load<AudioStreamSample>("res://Audio/Zorori/ZororiFail2.wav");
        private static readonly ResourcePreloader ZororiSampleLibrary = (ResourcePreloader)ZORORI_SAMPLE_RESOURCES_SCENE.Instance();
        private static readonly RandomUniqueClass RandomUnique = new RandomUniqueClass(5, ZororiSampleLibrary.GetResourceList().Length);

        public void AddPlaybackMarker(double time = 0, bool isFail = false)
        {
            int castedIdx = RandomUnique.Cast();
            AddPlaybackMarker(new PlaybackMarker(
                time,
                //isFail ? ZORORI_SAMPLE_FAIL : (AudioStreamSample)ZororiSampleLibrary.GetResource(ZororiSampleLibrary.GetResourceList()[castedIdx]),
                isFail ? ZORORI_SAMPLE_FAIL : (AudioStreamSample)ZororiSampleLibrary.GetResource($"Zorori{castedIdx+1}"),
                isFail ? "fail" : "gag",
                castedIdx
                )
            );
        }
    }
}