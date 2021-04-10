using Godot;
using System.Collections.Generic;

namespace Game
{
    public class PoolTimingAnalyzerClass
    {
        public const float DIFFERENCE_RANGE_MIN = -0.125f;
        public const float DIFFERENCE_RANGE_MAX = 0.1875f;

        private List<float> MissionList = new List<float>();
        private List<float> InputList = new List<float>();
        private List<bool> IsPointList = new List<bool>();

        public bool AppendInput(float inputTime)
        {
            InputList.Add(inputTime);
            return IsPointList[InputList.Count - 1];
        }

        public void Reset(List<float> newMission, List<bool> newIsPointList)
        {
            MissionList = newMission;
            IsPointList = newIsPointList;
            InputList.Clear();
        }

        public void Reset()
        {
            MissionList.Clear();
            InputList.Clear();
            IsPointList.Clear();
        }

        private bool isOopsieFound(int index, float inputPlaceholder)
        {
            bool isInputAvailable = InputList.Count - 1 >= index;
            float inputTime = isInputAvailable ? InputList[index] : inputPlaceholder;
            float missionTime = MissionList[index];
            float difference = inputTime - missionTime;
            bool isInputOutOfRange = !(DIFFERENCE_RANGE_MIN <= difference && difference <= DIFFERENCE_RANGE_MAX);
            bool isOopsie = (isInputOutOfRange && ((!isInputAvailable && inputTime > missionTime) || isInputAvailable));
            if (isOopsie)
            {
                GD.Print("isInputOutOfRange: ", isInputOutOfRange);   
                GD.Print("difference: ", difference);
            }
            return isOopsie;
        }

        public bool IsAcceptable(float inputPlaceholder)
        {
            if (InputList.Count > MissionList.Count)
                return false;

            for (int i = 0; i < MissionList.Count; i++)
            {
                if (isOopsieFound(i, inputPlaceholder))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", MissionList)}]\n[{string.Join(",", IsPointList)}]\n[{string.Join(",", InputList)}]";
        }
    }
}