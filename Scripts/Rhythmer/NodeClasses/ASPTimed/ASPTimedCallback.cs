using Godot;
using System.Collections.Generic;

namespace Rhythmer
{
    public class ASPTimedCallback : Node
    {
        private struct CallbackStruct
        {
            public Object @Object;
            public float Duration;
            public string Callback;
            public object[] Params;

            public CallbackStruct(Object @object, float duration, string callback, params object[] @params)
            {
                @Object = @object;
                Duration = duration;
                Callback = callback;
                Params = @params;
            }
        }

        private readonly Queue<CallbackStruct> CallbackQueue = new Queue<CallbackStruct>();
        public AudioStreamPlayer MusicPlayer;

        public override void _PhysicsProcess(float delta)
        {
            if (MusicPlayer is null || !MusicPlayer.Playing) return;
            double ASPTime = RS.GetASPTimeSec(MusicPlayer) / MusicPlayer.PitchScale;
            // GD.Print(ASPTime);
            while (CallbackQueue.Count > 0)
            {
                CallbackStruct callback = CallbackQueue.Peek();
                double playbackOffset = RS.GetPlaybackOffset(ASPTime, callback.Duration, 1, 1, 0);
                //double playbackOffset = ASPTime - callback.Duration;
                if (playbackOffset < 0)
                    return;
                // callback.@Object.GetType().GetMethod(callback.Callback).Invoke(callback.@Object, callback.Params);
                callback.@Object.Call(callback.Callback, callback.Params);
                
                CallbackQueue.Dequeue();
            }
        }

        public void AddCallback(Object @object, float duration, string callback, params object[] @params)
        {
            CallbackQueue.Enqueue(new CallbackStruct(@object, duration, callback, @params));
        }

        public void ClearCallbackQueue()
        {
            CallbackQueue.Clear();
        }
    }
}
