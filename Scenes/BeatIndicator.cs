using Godot;
using System;

namespace Game {
    [Tool]
    public class BeatIndicator : Control
    {
        const string SECTION = "settings";
        const string CFG_PATH = "user://settings.cfg";
        const string KEY = "indicator";

        private Rect2 _indicatorRect;
        private float _beatCircleRadius;
        private Vector2 _indicatorSize = Vector2.Zero;
        [Export]
        public Vector2 IndicatorSize {
            set {
                _indicatorSize = value;
                _indicatorRect = new Rect2(-IndicatorSize / 2.0f, IndicatorSize);
                _beatCircleRadius = IndicatorSize.y / 2.0f;
                PointerPos.y = _beatCircleRadius;
            }
            get => _indicatorSize;
        }
        [Export]
        public Texture PointerTex;
        [Export]
        public Color BGColor = Colors.Black;
        [Export]
        public Color BeatColor = Colors.White;
        [Export]
        public Color IndicatorColor = Colors.White;
        private Vector2 PointerPos = Vector2.Zero;
        public PoolTimingAnalyzerClass PoolTimingRef;
        public float CurrentBeats = 0.0f;

        public override void _Ready()
        {
            ConfigFile cfg = new ConfigFile();
            cfg.Load(CFG_PATH);
            Visible = (bool) cfg.GetValue(SECTION, KEY, false);
        }

        public override void _Process(float delta)
        {
            if (Visible)
                Update();
        }

        private float Beat2XPosMod(float beat) => beat % 4.0f * _indicatorSize.x / 4.0f;
        private float Beat2XPos(float beat) => beat * _indicatorSize.x / 4.0f;

        public override void _Draw()
        {
            DrawRect(_indicatorRect, BGColor);
            for (float i = 0; i <= 4; i += 0.5f)
            {
                DrawLine(
                    _indicatorRect.Position + new Vector2(Beat2XPos(i), (i % 1 == 0) ? 0 : (_indicatorRect.Size.y / 2)), 
                    _indicatorRect.Position + new Vector2(Beat2XPos(i), _indicatorRect.Size.y), 
                    BeatColor, 1.0f
                );
            }
            if (PoolTimingRef != null)
            {
                int minBeat = (int)(CurrentBeats / 4.0f) * 4;
                int maxBeat = minBeat + 4;
                int idx = 0;
                for (; idx < PoolTimingRef.MissionBeatList.Count && PoolTimingRef.MissionBeatList[idx] < minBeat; idx++) { }
                int startIdx = idx;
                for (idx = startIdx; idx < PoolTimingRef.MissionBeatList.Count && PoolTimingRef.MissionBeatList[idx] < maxBeat; idx++) 
                {
                    DrawLine(
                        _indicatorRect.Position + new Vector2(Beat2XPosMod(PoolTimingRef.MissionBeatList[idx]), 0), 
                        _indicatorRect.Position + new Vector2(Beat2XPosMod(PoolTimingRef.MissionBeatList[idx]), _indicatorRect.Size.y),
                        Colors.Red, 2.0f
                    );
                    // DrawCircle(
                    //     new Vector2(_indicatorRect.Position.x + Beat2XPosMod(PoolTimingRef.MissionBeatList[idx]), 0), 
                    //     _beatCircleRadius, Colors.Red
                    // );
                }
                for (idx = startIdx; idx < PoolTimingRef.InputBeatList.Count && PoolTimingRef.InputBeatList[idx] < maxBeat; idx++) 
                {
                    DrawLine(
                        _indicatorRect.Position + new Vector2(Beat2XPosMod(PoolTimingRef.InputBeatList[idx]), 0), 
                        _indicatorRect.Position + new Vector2(Beat2XPosMod(PoolTimingRef.InputBeatList[idx]), _indicatorRect.Size.y),
                        Colors.Blue, 2.0f
                    );
                    // DrawCircle(
                    //     new Vector2(_indicatorRect.Position.x + Beat2XPosMod(PoolTimingRef.InputBeatList[idx]), 0), 
                    //     _beatCircleRadius, Colors.Blue
                    // );
                }
            }
            float xpos = Beat2XPosMod(CurrentBeats);
            PointerPos.x = _indicatorRect.Position.x + xpos - PointerTex.GetSize().x / 2.0f;
            DrawTexture(PointerTex, PointerPos);

            DrawLine(
                _indicatorRect.Position + new Vector2(xpos, 0), 
                _indicatorRect.Position + new Vector2(xpos, _indicatorRect.Size.y), 
                IndicatorColor, 2.0f
            );
        }
    }

}