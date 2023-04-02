using Godot;
using Godot.Collections;

namespace Utils
{
    public class BtnComboDetector : Reference
    {
        public struct ComboStatus {
            public bool IsDetected;
            public bool IsSuccessful;

            public ComboStatus(bool isDetected, bool isSuccessful)
            {
                IsDetected = isDetected;
                IsSuccessful = isSuccessful;
            }
        }

        private string[] actionList;
        private int actionFlags = 0b0;
        private int actionFlagsIdeal = 0b0;
        private bool isDetecting = false;
        private ulong startingMilli = 0;
        private ulong maxMilli = 100;

        public BtnComboDetector()
        {
            actionList = new string[0] { };
        }

        public BtnComboDetector(params string[] actions)
        {
            actionList = actions;
            actionFlagsIdeal = (0b1 << (actionList.Length)) - 1;
        }

        public ComboStatus GetComboStatus()
        {
            ComboStatus comboStatus = new ComboStatus(false, false);
            ulong currentMilli = Time.GetTicksMsec();
            for (int i = 0; i < actionList.Length; i++)
            {
                if (((actionFlags >> i) & 0b1) == 0 && Input.IsActionJustPressed(actionList[i]))
                    actionFlags |= 0b1 << i;
            }
            if (actionFlags > 0 && !isDetecting)
            {
                startingMilli = currentMilli;
                isDetecting = true;
            }
            if (isDetecting)
            {
                bool isComboComplete = actionFlags == actionFlagsIdeal;
                bool isComboTimely = (currentMilli - startingMilli) <= maxMilli;
                if (isComboComplete || !isComboTimely)
                {
                    isDetecting = false;
                    actionFlags = 0b0;
                    comboStatus = new ComboStatus(true, isComboComplete && isComboTimely);
                }
            }
            return comboStatus;
        }
    }
}