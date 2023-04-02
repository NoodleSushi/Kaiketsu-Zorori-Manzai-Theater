using Godot;
using System;
using System.Collections.Generic;

namespace Game
{
    public class PoolTimingAnalyzerClass
    {
        public const float DIFFERENCE_RANGE_MIN = -0.125f;
        public const float DIFFERENCE_RANGE_MAX = 0.1875f;

        private readonly List<float> InputBeatList = new List<float>();
        private List<float> MissionBeatList = new List<float>();
        private List<bool> IsPointGivenList = new List<bool>();

        public bool AppendInputTime(float inputTime)
        {
            InputBeatList.Add(inputTime);
            return IsPointGivenList[Math.Min(InputBeatList.Count - 1, IsPointGivenList.Count - 1)];
        }

        public void Reset((List<float> missionList, List<bool> isPointList) MissionIsPointList)
        {
            MissionBeatList = MissionIsPointList.missionList;
            IsPointGivenList = MissionIsPointList.isPointList;
            InputBeatList.Clear();
        }

        public void Reset()
        {
            MissionBeatList.Clear();
            InputBeatList.Clear();
            IsPointGivenList.Clear();
        }

        private bool IsOopsieFound(int index, float currentBeat)
        {
            bool isInputAvailable = InputBeatList.Count - 1 >= index;
            float inputTime = isInputAvailable ? InputBeatList[index] : currentBeat;
            float missionTime = MissionBeatList[index];
            float difference = inputTime - missionTime;
            bool isInputOutOfRange = !(DIFFERENCE_RANGE_MIN <= difference && difference <= DIFFERENCE_RANGE_MAX);
            bool isOopsie = (isInputOutOfRange && ((!isInputAvailable && inputTime > missionTime) || isInputAvailable));
            #if DEBUG
                if (isOopsie)
                {
                    GD.Print("isInputOutOfRange: ", isInputOutOfRange);   
                    GD.Print("difference: ", difference);
                }
            #endif
            return isOopsie;
        }

        public bool IsAcceptable(float currentBeat)
        {
            if (InputBeatList.Count > MissionBeatList.Count)
                return false;

            for (int i = 0; i < MissionBeatList.Count; i++)
            {
                if (IsOopsieFound(i, currentBeat))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", MissionBeatList)}]\n[{string.Join(",", IsPointGivenList)}]\n[{string.Join(",", InputBeatList)}]";
        }
    }
}