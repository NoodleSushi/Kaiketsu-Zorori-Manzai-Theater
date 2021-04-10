using Godot;
using System;

namespace Rhythmer
{
	public class ASPConductor : AudioStreamPlayer
	{
		[Export] public float BPM = 120;
		[Export] public int StepsPerBeat = 4;

		private double nakedBeats = 0;
		private double beats = 0;
		private double steps = 0;

		public double MaxTime { get => (BPM * StepsPerBeat > 60) ? 60 * (double.MaxValue / (BPM * StepsPerBeat)) : double.MaxValue; }
		public double Steps { get => steps; }
		public double Beats { get => beats; }

		public override void _EnterTree()
		{
			ProcessPriority = int.MinValue;
		}

		public override void _Process(float delta)
		{
			if (!Playing) return;
			nakedBeats = Math.Max(nakedBeats, RS.GetASPTimeSec(this) * BPM);
			
			if (nakedBeats < 0) return;
			
			beats = nakedBeats / 60;
			
			double newSteps = nakedBeats * StepsPerBeat / 60;

			steps = newSteps;
		}

		public new void Play(float fromPosition = 0)
		{
			base.Play(fromPosition);
			nakedBeats = RS.GetASPTimeSec(this) * BPM;
		}

		public new void Seek(float toPosition = 0)
		{
			base.Seek(toPosition);
			nakedBeats = RS.GetASPTimeSec(this) * BPM;
		}
	}
}
