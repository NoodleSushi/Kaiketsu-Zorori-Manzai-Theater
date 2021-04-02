using Godot;

namespace Rhythmer
{
    /// <summary>
    /// Rhythm Solver: 
    /// Contains all methods for solving basic rhythm-related calculations.
    /// </summary>
    public static class RS
    {
        public static double GetASPTimeSec(AudioStreamPlayer ASP)
        {
            return ASP.GetPlaybackPosition() + GetASPLatency();
        }

        public static double GetScaledASPTimeSec(AudioStreamPlayer ASP)
        {
            return GetASPTimeSec(ASP) / ASP.PitchScale;
        }

        public static double Beat2Sec(double bpm, double beat)
        {
            return beat * 60 / bpm;
        }

        public static double Beat2Sec(double bpm, double beat, float timeScale)
        {
            return beat * 60 / bpm * timeScale;
        }

        public static double Sec2Beat(double bpm, double sec)
        {
            return sec * bpm / 60;
        }

        public static double Sec2Beat(double bpm, double sec, float timeScale)
        {
            return sec * bpm / 60 * timeScale;
        }

        public static double GetASPBeat(double bpm, AudioStreamPlayer ASP)
        {
            return GetASPTimeSec(ASP) * bpm / 60;
        }

        public static double GetASPBeatScaled(double bpm, AudioStreamPlayer ASP)
        {
            return GetASPTimeSec(ASP) * bpm / 60;
            //return (ASP.GetPlaybackPosition() + GetASPLatency() * ASP.PitchScale) * bpm / 60;
        }

        public static double GetASPLatency()
        {
            return AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency();
        }

        public static double GetPlayLatency()
        {
            return AudioServer.GetTimeToNextMix();
        }

        public static double GetPlaybackOffset(double playbackTimeSec, double missionTimeSec, 
                                    float musicPitchScale, float slavePitchScale, double allocatedTimeSec)
        {
            return allocatedTimeSec - GetOffsetTimeSec(playbackTimeSec, missionTimeSec, musicPitchScale, slavePitchScale);
        }

        public static double GetPlaybackOffset(double playbackTimeSec, double missionTimeSec,
                                    double allocatedTimeSec)
        {
            return allocatedTimeSec - GetOffsetTimeSec(playbackTimeSec, missionTimeSec);
        }

        public static double GetOffsetTimeSec(double playbackTimeSec, double missionTimeSec)
        {
            return missionTimeSec - playbackTimeSec + GetASPLatency();
        }

        public static double GetOffsetTimeSec(double playbackTimeSec, double missionTimeSec,
                                       float musicPitchScale, float slavePitchScale)
        {
            return ((missionTimeSec - playbackTimeSec - AudioServer.GetOutputLatency())
                    * slavePitchScale
                    + AudioServer.GetTimeSinceLastMix()) / musicPitchScale;
        }

        public static double GetOffsetTimeSec(double playbackTimeSec, double missionTimeSec,
                                       AudioStreamPlayer ASPMusic, AudioStreamPlayer ASPSlave)
        {
            return GetOffsetTimeSec(playbackTimeSec, missionTimeSec, ASPMusic.PitchScale, ASPSlave.PitchScale);
        }
    }
}
