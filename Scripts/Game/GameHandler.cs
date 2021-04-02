using Godot;
using Rhythmer;

namespace Game
{
    public class GameHandler : Node
    {
        const float MAIN_BPM = 98;

        [Export] public NodePath BgmPlayerPath;
        [Export] public NodePath GagPlayerPath;
        [Export] public NodePath PlrPlayerPath;
        [Export] public NodePath GameAnimationTreePath;
        [Export] public NodePath DebugLabelPath;
        [Export] public NodePath ScoreLabelPath;
        [Export] public NodePath BestScoreLabelPath;
        [Export] public NodePath SubtitleLabelPath;
        [Export] public NodePath BPMLabelPath;
        [Export] public bool isActive = true;
        private AudioPlayer.Bgm BgmPlayer;
        private AudioPlayer.Gag GagPlayer;
        private AudioPlayer.Plr PlrPlayer;
        private GameAnimationTree gameAnimationTree;
        private Label DebugLabel;
        private ScoreLabelClass ScoreLabel;
        private BestScoreLabelClass BestScoreLabel;
        private SubtitleLabelClass SubtitleLabel;
        private Label BPMLabel;

        readonly private GagSystemClass GagSystem = new GagSystemClass();
        readonly private PoolTimingAnalyzerClass PoolTimingAnalyzer = new PoolTimingAnalyzerClass();

        private float CurrentBPM = MAIN_BPM;
        private int CurrentStage = 0;
        private int score = 0;
        private int bestScore = 0;

        private int Safes = 0;

        private int Score
        {
            set
            {
                score = value;
                if (score > BestScore)
                    BestScore = score;
                if (ScoreLabel != null)
                    ScoreLabel.Score = value;
            }
            get => score;
        }
        private int BestScore
        {
            set
            {
                bestScore = value;
                if (BestScoreLabel != null)
                    BestScoreLabel.BestScore = value;
            }
            get => bestScore;
        }

        public override void _Ready()
        {
            GetParent<Node>().Connect("ready", this, nameof(_on_Parent_Ready));
        }

        public void _on_Parent_Ready()
        {
            if (!isActive)
                return;
            BgmPlayer = GetNode<AudioPlayer.Bgm>(BgmPlayerPath);
            BgmPlayer.BPM = MAIN_BPM;
            BgmPlayer.Connect("finished", this, nameof(_on_BgmPlayer_finished));

            GagPlayer = GetNode<AudioPlayer.Gag>(GagPlayerPath);
            GagPlayer.MusicPlayer = BgmPlayer;
            GagPlayer.AllocatedTime = 0;
            GagPlayer.TakeOverWhilePlaying = true;
            GagPlayer.CurrentAudioPlayer.Bus = "Zorori";

            PlrPlayer = GetNode<AudioPlayer.Plr>(PlrPlayerPath);

            gameAnimationTree = GetNode<GameAnimationTree>(GameAnimationTreePath);
            gameAnimationTree.SetBgmPlayer(BgmPlayer);


            DebugLabel = GetNode<Label>(DebugLabelPath);
            ScoreLabel = GetNode<ScoreLabelClass>(ScoreLabelPath);
            
            BestScoreLabel = GetNode<BestScoreLabelClass>(BestScoreLabelPath);
            BestScore = ConfigHandler.GetScore();

            SubtitleLabel = GetNode<SubtitleLabelClass>(SubtitleLabelPath);
            BPMLabel = GetNode<Label>(BPMLabelPath);
            SubtitleLabel.ConnectToASPTimedSingle(GagPlayer);

            isActive = false;
            //GenerateNPlayBGMStage();
        }

        public override void _Process(float delta)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                isActive = true;
                Reset();
                GenerateNPlayBGMStage();
            }

            if (!isActive)
                return;

            float inputBeat = (float)RS.GetASPBeatScaled(MAIN_BPM, BgmPlayer);
            bool isButtonPressed = Input.IsActionJustPressed("ui_accept") && inputBeat > 8;
            bool isFail = GagSystem.IsBeat2Out(inputBeat);
            bool isSafe;
            bool isPoint = false;

            if (isButtonPressed)
                isPoint = PoolTimingAnalyzer.AppendInput(inputBeat);

            isSafe = PoolTimingAnalyzer.IsAcceptable(inputBeat);

            if (isButtonPressed && isSafe)
            {
                if (isPoint)
                    Score += 1;

                PlrPlayer.Play(isFail);

                if (isFail)
                {
                    gameAnimationTree.PlayOneShot("inoshishi_panic");
                    gameAnimationTree.PlayOneShot("zorori_unfrozen");
                    gameAnimationTree.PlayOneShot("audience_laugh_fail");
                }
                else
                {
                    gameAnimationTree.PlayOneShot(Safes % 2 == 0 ? "noshishi_hai" : "ishishi_hai");
                    gameAnimationTree.PlayOneShot("zorori_pulse");
                    gameAnimationTree.PlayOneShot("audience_laugh");
                    Safes++;
                }
            }

            DebugLabel.Text = $"{PoolTimingAnalyzer}\n{isSafe}";

            if (!isSafe)
            {
                isActive = false;
                Stop();
            }
        }

        // Update BPM
        private void SetCurrentBPM(float newBPM)
        {
            CurrentBPM = newBPM;
            float newPitchScale = newBPM / MAIN_BPM;
            BgmPlayer.PitchScale = newPitchScale;
            GagPlayer.CurrentAudioPlayer.PitchScale = newPitchScale;
            PlrPlayer.PitchScale = newPitchScale;
            gameAnimationTree.SetTimeScale(newBPM / 60);
        }

        private static float Beat2Time(sbyte bar, double beat)
        {
            return (float) ((beat + 4 * bar) * 60 / MAIN_BPM);
        }

        private void Stop()
        {
            GagPlayer.ClearPlaybackMarkerQueue();
            PlrPlayer.Stop();
            BgmPlayer.Stop();
            gameAnimationTree.ClearAll();
            SubtitleLabel.Clear();
            ConfigHandler.SaveScore(BestScore);
        }

        private void Reset()
        {
            Score = 0;
            CurrentStage = 0;
            GagSystem.Reset();
            PoolTimingAnalyzer.Reset();
            SetCurrentBPM(MAIN_BPM);
        }

        private void GenerateNPlayBGMStage()
        {
            Safes = 0;
            bool isFirstStage = CurrentStage == 0;
            if (!isFirstStage)
                GagSystem.GotoNextStage();

            SetCurrentBPM(MAIN_BPM + CurrentStage * 3);
            BPMLabel.Text = $"{CurrentBPM} BPM";
            BgmPlayer.PlayBGMStage(GagSystem.StageBGMIndex);
            GagPlayer.ClearPlaybackMarkerQueue();
            gameAnimationTree.GenerateBeginningOneShots(MAIN_BPM, isFirstStage);
            sbyte stageBars = GagSystem.StageBGMBars;
            for (sbyte bar = 0; bar < stageBars; bar++)
            {
                GagSystem.SystemPointer = bar;
                GagPlayer.AddPlaybackMarker(Beat2Time(bar, 7));
                if (GagSystem.IsPointer2Out())
                {
                    GagPlayer.AddPlaybackMarker(Beat2Time(bar, 9 + 5.0 / 24.0) - 0.05, true);
                    gameAnimationTree.QueueOneShot("zorori_fail", Beat2Time(bar, 8));
                }
                else
                {
                    gameAnimationTree.QueueOneShot("zorori_gag", Beat2Time(bar, 8));
                }
            }

            if (!isFirstStage)
                gameAnimationTree.PlayOneShot("next_stage");

            // gameAnimationTree.PlayOneShots();
    
            PoolTimingAnalyzer.Reset(GagSystem.GenerateMissionList(), GagSystem.GenerateIsPointList());

            CurrentStage++;
        }

        public void _on_BgmPlayer_finished()
        {
            if (!isActive)
                return;
            SubtitleLabel.Clear();
            GenerateNPlayBGMStage();
        }
    }
}