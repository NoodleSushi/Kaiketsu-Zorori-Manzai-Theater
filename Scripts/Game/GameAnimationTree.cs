using Godot;
using System;
using Rhythmer;

namespace Game
{
	public class GameAnimationTree : AnimationTree
	{
		public enum AnimKey
		{
			init,
			bgcontrol_pulse,
			next_stage,
			inoshishi_panic,
			inoshishi_pulse,
			ishishi_hai,
			noshishi_hai,
			zorori_pulse,
			zorori_gag,
			zorori_fail,
			zorori_unfrozen,
			mini_zorori_pulse,
			audience_pulse,
			audience_laugh,
			audience_laugh_fail,
		}

		public enum AnimState
		{
			Start,
			Intro,
			MainBlendTree,
			GameOver,
			GameOverState,
		}

		[Signal] public delegate void AnimStateChanged(AnimState oldAnimState, AnimState newAnimState);

		[Export] public NodePath ASPTimedCallbackNodePath;
		private ASPTimedCallback @ASPTimedCallback;
		private AnimationNodeStateMachinePlayback StateMachine;
		public AnimState CurrentAnimState = AnimState.Intro;
		private string CurrentStrState = "START";
		public AudioPlayer.Bgm BgmPlayer;

		public override void _Ready()
		{
			@ASPTimedCallback = GetNode<ASPTimedCallback>(ASPTimedCallbackNodePath);
			StateMachine = (AnimationNodeStateMachinePlayback) Get("parameters/playback");
		}

		public override void _Process(float delta)
		{
			string NewStrState = StateMachine.GetCurrentNode();
			if (CurrentStrState != NewStrState)
			{
				AnimState NewAnimState = AnimState.Start;
				switch (NewStrState)
				{
					case "intro":
						NewAnimState = AnimState.Intro;
						break;
					case "MainBlendTree":
						NewAnimState = AnimState.MainBlendTree;
						break;
					case "game_over":
						NewAnimState = AnimState.GameOver;
						break;
					case "game_over_state":
						NewAnimState = AnimState.GameOverState;
						break;
				}
				EmitSignal(nameof(AnimStateChanged), CurrentAnimState, NewAnimState);
				CurrentAnimState = NewAnimState;
				CurrentStrState = NewStrState;
			}
		}

		public void PlayIntro() => StateMachine.Start("intro");

		public void Reset()
		{
			Set("parameters/MainBlendTree/BaseInitBlend2/blend_amount", 0);
			PlayOneShot(AnimKey.init);
		}

		public void ClearAll() => @ASPTimedCallback.ClearCallbackQueue();

		public void SetBgmPlayer(AudioPlayer.Bgm NewBgmPlayer)
		{
			BgmPlayer = NewBgmPlayer;
			@ASPTimedCallback.MusicPlayer = NewBgmPlayer;
		}

		public void SetTimeScale(float TimeScale) => Set("parameters/MainBlendTree/TimeScale/scale", TimeScale);

		public void QueueOneShot(AnimKey AnimationName, float time)
		{
			if (BgmPlayer is null)
				return;
			float calculatedTime = Mathf.Max(0, (time - (float) RS.GetASPTimeSec(BgmPlayer)) / BgmPlayer.PitchScale);
			ASPTimedCallback.AddCallback(this, calculatedTime, "set", $"parameters/MainBlendTree/{Enum.GetName(typeof(AnimKey), AnimationName)}OneShot/active", true);
		}

		public void PlayOneShot(AnimKey AnimationName) => Set($"parameters/MainBlendTree/{Enum.GetName(typeof(AnimKey), AnimationName)}OneShot/active", true);

		public void PlayGameOver()
		{
			// PlayOneShot(AnimKey.game_over);
			// Set("parameters/MainBlendTree/BaseInitBlend2/blend_amount", 1);
			StateMachine.Start("game_over");
		}

		public void PlayFail()
		{
			PlayOneShot(AnimKey.inoshishi_panic);
			PlayOneShot(AnimKey.zorori_unfrozen);
			PlayOneShot(AnimKey.audience_laugh_fail);
		}

		public void PlaySafe(bool isNoshishi)
		{
			PlayOneShot(isNoshishi ? AnimKey.noshishi_hai : AnimKey.ishishi_hai);
			PlayOneShot(AnimKey.zorori_pulse);
			PlayOneShot(AnimKey.audience_laugh);
		}



		public void GenerateBeginningOneShots(float Bpm, bool isBeginning)
		{
			for (int i = 0; i < 8; i++)
			{
				float time = (float)(i * 60 / Bpm);
				QueueOneShot(AnimKey.audience_pulse, time);
				QueueOneShot(AnimKey.mini_zorori_pulse, time);
				QueueOneShot(AnimKey.bgcontrol_pulse, time);
				if (isBeginning)
				{
					QueueOneShot(AnimKey.zorori_pulse, time);
					QueueOneShot(AnimKey.inoshishi_pulse, time);
				}
			}
		}
	}
}
