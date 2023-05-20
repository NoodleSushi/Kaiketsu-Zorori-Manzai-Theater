using Godot;
using Rhythmer;
using AnimKey = Game.GameAnimationTree.AnimKey;
using AnimState = Game.GameAnimationTree.AnimState;
using BtnComboDetector = Utils.BtnComboDetector;

namespace Game
{
    public class GameHandler : Node
    {
        const float MAIN_BPM = 98;
        const string SECTION = "settings";
        const string CFG_PATH = "user://settings.cfg";
        const string KEY = "slap_mode";

        [Signal] public delegate void BGMStageGenerated();
        [Signal] public delegate void GameOvered();

        [Export] public NodePath BgmAudioPlayerPath; private AudioPlayer.Bgm BgmAudioPlayer;
        [Export] public NodePath GagAudioPlayerPath; private AudioPlayer.Gag GagAudioPlayer;
        [Export] public NodePath PlrAudioPlayerPath; private AudioPlayer.Plr PlrAudioPlayer;
        [Export] public NodePath AplAudioPlayerPath; private AudioPlayer.Apl AplAudioPlayer;
        [Export] public NodePath GameOverAudioPlayerPath; private AudioStreamPlayer GameOverAudioPlayer;
        [Export] public NodePath GameAnimationTreePath; private GameAnimationTree @GameAnimationTree;
        [Export] public NodePath DebugLabelPath; private Label DebugLabel;
        [Export] public NodePath BeatIndicatorPath; private BeatIndicator BeatIndicator;
        [Export] public NodePath ScoreLabelPath; private ScoreLabelClass ScoreLabel;
        [Export] public NodePath BestScoreLabelPath; private BestScoreLabelClass BestScoreLabel;
        [Export] public NodePath BestScoreOverLabelPath; private Label BestScoreOverLabel;
        [Export] public NodePath BPMLabelPath; private Label BPMLabel;
        [Export] public NodePath GameOverScreenPath; private Control GameOverScreen;
        [Export] public bool isActive = true;
        public int SlapMode = 0;

        readonly private GagSystemClass GagSystem = new GagSystemClass();
        readonly private PoolTimingAnalyzerClass PoolTimingAnalyzer = new PoolTimingAnalyzerClass();
        readonly private BtnComboDetector bcd = new BtnComboDetector("btn_primary", "btn_secondary");

        private bool IsGameOver = false;
        private float CurrentBPM = MAIN_BPM;
        private int CurrentStage = 0;
        private int SafeHaiAttempts = 0;
        private int currentScore = 0; private int CurrentScore
        {
            set
            {
                currentScore = value;
                if (currentScore > BestScore)
                    BestScore = currentScore;
                if (ScoreLabel != null)
                    ScoreLabel.Score = value;
            }
            get => currentScore;
        }
        private int bestScore = 0; private int BestScore
        {
            set
            {
                bestScore = value;
                if (BestScoreLabel != null)
                    BestScoreLabel.BestScore = value;
            }
            get => bestScore;
        }

        private void GetNodeReferences()
        {
            BgmAudioPlayer = GetNode<AudioPlayer.Bgm>(BgmAudioPlayerPath);
            GagAudioPlayer = GetNode<AudioPlayer.Gag>(GagAudioPlayerPath);
            AplAudioPlayer = GetNode<AudioPlayer.Apl>(AplAudioPlayerPath);
            PlrAudioPlayer = GetNode<AudioPlayer.Plr>(PlrAudioPlayerPath);
            GameOverAudioPlayer = GetNode<AudioStreamPlayer>(GameOverAudioPlayerPath);
            @GameAnimationTree = GetNode<GameAnimationTree>(GameAnimationTreePath);
            DebugLabel = GetNode<Label>(DebugLabelPath);
            BeatIndicator = GetNode<BeatIndicator>(BeatIndicatorPath);
            ScoreLabel = GetNode<ScoreLabelClass>(ScoreLabelPath);
            BestScoreLabel = GetNode<BestScoreLabelClass>(BestScoreLabelPath);
            BestScoreOverLabel = GetNode<Label>(BestScoreOverLabelPath);
            BPMLabel = GetNode<Label>(BPMLabelPath);
            GameOverScreen = GetNode<Control>(GameOverScreenPath);
        }

        public override void _Ready()
        {
            ConfigFile cfg = new ConfigFile();
            cfg.Load(CFG_PATH);
            SlapMode = (int) cfg.GetValue(SECTION, KEY, 0);
            if (SlapMode < 0 || SlapMode > 2)
                SlapMode = 0;

            GetNodeReferences();

            BgmAudioPlayer.BPM = MAIN_BPM;
            BgmAudioPlayer.Connect("finished", this, nameof(_on_BgmPlayer_finished));

            GagAudioPlayer.MusicPlayer = BgmAudioPlayer;
            GagAudioPlayer.AllocatedTime = 0;
            GagAudioPlayer.TakeOverWhilePlaying = true;

            AplAudioPlayer.MusicPlayer = BgmAudioPlayer;
            AplAudioPlayer.AllocatedTime = 0;
            AplAudioPlayer.TakeOverWhilePlaying = true;

            @GameAnimationTree.SetBgmPlayer(BgmAudioPlayer);
            @GameAnimationTree.Active = true;
            @GameAnimationTree.PlayIntro();
            @GameAnimationTree.Connect(nameof(@GameAnimationTree.AnimStateChanged), this, nameof(_on_GameAnimationTree_AnimStateChanged));

            BeatIndicator.PoolTimingRef = PoolTimingAnalyzer;

            BestScore = ConfigHandler.GetScore();

            GagSystem.Reset();
        }

        public override void _Process(float delta)
        {
//            if (Input.IsActionJustPressed("ui_cancel") && !isActive)
//            {
//                isActive = true;
//                Reset();
//                GenerateNPlayBGMStage();
//            }
            if (Input.IsActionJustPressed("restart") && IsGameOver)
                GetNode<CanvasLayer>("/root/SceneTransition").Call("reload_current_scene");
            if (Input.IsActionJustPressed("ui_cancel") && IsGameOver)
                GetNode<CanvasLayer>("/root/SceneTransition").Call("transition_to_scene", "res://Scenes/Menu.tscn");
            if (!isActive)
                return;

            float inputBeat = (float) RS.GetASPBeatScaled(MAIN_BPM, BgmAudioPlayer);
            BeatIndicator.CurrentBeats = inputBeat;
            bool isFail = GagSystem.Beat2IsFailureGag(inputBeat);
            bool shallDetectButton = inputBeat > 8;
            if (!shallDetectButton)
                return;
            bool isButtonPressed = false;
            bool isButtonValid = true;

            switch (SlapMode)
                {
                    case 0:
                        isButtonPressed = Input.IsActionJustPressed("btn_primary");
                        break;
                    case 1:
                        if (!isFail)
                        {
                            isButtonPressed = Input.IsActionJustPressed("btn_primary");
                            isButtonValid = !Input.IsActionJustPressed("btn_secondary");
                        }
                        else
                        {
                            isButtonPressed = Input.IsActionJustPressed("btn_secondary");
                            isButtonValid = !Input.IsActionJustPressed("btn_primary");
                        }
                        break;
                    case 2:
                        if (!isFail)
                        {
                            isButtonPressed = Input.IsActionJustPressed("btn_primary");
                            isButtonValid = !Input.IsActionJustPressed("btn_secondary");
                        }
                        else
                        {
                            var comboStatus = bcd.GetComboStatus();
                            isButtonPressed = comboStatus.IsDetected;
                            if (isButtonPressed)
                                isButtonValid = comboStatus.IsSuccessful;
                        }
                        break;
                }

            bool isSafe = true;
            bool isPoint = false;

            if (isButtonPressed && isButtonValid)
                isPoint = PoolTimingAnalyzer.AppendInputTime(inputBeat);
            
            isSafe = PoolTimingAnalyzer.IsAcceptable(inputBeat) && isButtonValid;

            if (isButtonPressed && isSafe)
            {
                if (isPoint)
                    CurrentScore += 1;

                PlrAudioPlayer.Play(isFail);

                if (isFail) 
                    @GameAnimationTree.PlayFail();
                else
                {
                    @GameAnimationTree.PlaySafe((SafeHaiAttempts & 0x1) == 0);
                    SafeHaiAttempts++;
                }
            }

            #if DEBUG
                DebugLabel.Text = $"{PoolTimingAnalyzer}\n{isSafe}";
            #endif

            if (!isSafe)
            {
                isActive = false;
                GameOver();
            }
        }

        // Update BPM
        private void SetCurrentBPM(float newBPM)
        {
            CurrentBPM = newBPM;
            float newPitchScale = newBPM / MAIN_BPM;
            BgmAudioPlayer.PitchScale = newPitchScale;
            GagAudioPlayer.CurrentAudioPlayer.PitchScale = newPitchScale;
            PlrAudioPlayer.PitchScale = newPitchScale;
            @GameAnimationTree.SetTimeScale(newBPM / 60);
        }

        private static float Beat2Time(sbyte bar, double beat)
        {
            return (float) ((beat + 4 * bar) * 60 / MAIN_BPM);
        }

        private void GameOver()
        {
            GagAudioPlayer.ClearPlaybackMarkerQueue();
            GagAudioPlayer.Stop();
            AplAudioPlayer.ClearPlaybackMarkerQueue();
            PlrAudioPlayer.Stop();
            BgmAudioPlayer.Stop();
            @GameAnimationTree.ClearAll();
            bool isNewBestScore = ConfigHandler.SaveScore(BestScore);
            BestScoreOverLabel.Visible = isNewBestScore;
            @GameAnimationTree.PlayGameOver();
            EmitSignal(nameof(GameOvered));
        }

        private void Reset()
        {
            CurrentScore = 0;
            CurrentStage = 0;
            GagSystem.Reset();
            PoolTimingAnalyzer.Reset();
            @GameAnimationTree.Reset();
            SetCurrentBPM(MAIN_BPM);
        }

        private void GenerateNPlayBGMStage()
        {
            SafeHaiAttempts = 0;
            bool isFirstStage = (CurrentStage == 0);
            if (!isFirstStage)
                GagSystem.TurnToNextPage();

            SetCurrentBPM(MAIN_BPM + CurrentStage * 3);
            BPMLabel.Text = $"{CurrentBPM} BPM";
            BgmAudioPlayer.PlayBGMStage(GagSystem.PageIndex);
            GagAudioPlayer.ClearPlaybackMarkerQueue();
            AplAudioPlayer.ClearPlaybackMarkerQueue();
            @GameAnimationTree.GenerateBeginningOneShots(MAIN_BPM, isFirstStage);
            sbyte stageBars = GagSystem.StageBGMBarCount;
            for (sbyte bar = 0; bar < stageBars; bar++)
            {
                GagAudioPlayer.AddPlaybackMarker(Beat2Time(bar, 7));
                if (GagSystem.GagIndex2IsFailureGag(bar))
                {
                    GagAudioPlayer.AddPlaybackMarker(Beat2Time(bar, 9 + 5.0 / 24.0) - 0.05, true);
                    AplAudioPlayer.AddPlaybackMarker(Beat2Time(bar, 11), true);
                    @GameAnimationTree.QueueOneShot(AnimKey.zorori_fail, Beat2Time(bar, 8));
                }
                else
                {
                    AplAudioPlayer.AddPlaybackMarker(Beat2Time(bar, 11), false);
                    @GameAnimationTree.QueueOneShot(AnimKey.zorori_gag, Beat2Time(bar, 8));
                }
                @GameAnimationTree.QueueOneShot(AnimKey.inoshishi_pulse, Beat2Time(bar, 8));
                @GameAnimationTree.QueueOneShot(AnimKey.inoshishi_pulse, Beat2Time(bar, 9));
                @GameAnimationTree.QueueOneShot(AnimKey.inoshishi_pulse, Beat2Time(bar, 10));
            }

            if (!isFirstStage)
                @GameAnimationTree.PlayOneShot(AnimKey.next_stage);
    
            PoolTimingAnalyzer.Reset(GagSystem.GenerateMissionIsPointList());

            CurrentStage++;
            EmitSignal(nameof(BGMStageGenerated));
        }

        public void _on_BgmPlayer_finished()
        {
            if (!isActive)
                return;
            GenerateNPlayBGMStage();
        }

        public void _on_GameAnimationTree_AnimStateChanged(AnimState oldAnimState, AnimState newAnimState)
        {
            if (oldAnimState == AnimState.Intro && newAnimState == AnimState.MainBlendTree)
            {
                isActive = true;
                GenerateNPlayBGMStage();
            }
            
            if (newAnimState == AnimState.GameOverState)
            {
                GD.Print("FUCK YEA BOI");
                GameOverAudioPlayer.Play();
                GameOverScreen.Visible = true;
                IsGameOver = true;
            }
        }
    }
}
