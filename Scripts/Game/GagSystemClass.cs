using Godot;
using System.Collections.Generic;

namespace Game
{
    public class GagSystemClass
    {
        private static readonly List<ushort> BOOK_DEFAULT = new List<ushort> { 0x8, 0xC8, 0x8C, 0x80, 0xC8, 0x82, 0xF9C8 };
        private List<ushort> Book = BOOK_DEFAULT;
        private sbyte Stage = 0;
        public sbyte SystemPointer = -1;
        private sbyte SoundPointer
        {
            get => (sbyte)(SystemPointer + 1);
        }
        public sbyte StageBGMIndex
        {
            // get => (sbyte)(Stage == 0 ? 0 : Stage == 6 ? 2 : 1);
            get => Stage;
        }
        public sbyte StageBGMBars
        {
            get => (sbyte)(Stage == 0 ? 4 : Stage == 6 ? 16 : 8);
        }

        public int Pointer2Out(bool IsSoundPointer = false)
        {
            return (Book[Stage] >> Mathf.Max(SystemPointer + (IsSoundPointer ? 1 : 0), 0)) & 0x1;
        }

        public bool IsPointer2Out(bool IsSoundPointer = false)
        {
            return Pointer2Out(IsSoundPointer) == 1;
        }

        public bool IsBeat2Out(float beat)
        {
            return ((Book[Stage] >> (int) Mathf.Max((beat-8f) / 4f, 0f)) & 0x1) == 1;
        }

        private void OverwritePage()
        {
            if (Stage != 0)
            {
                Book[Stage] = (ushort)(GD.Randi() % (Stage == 6 ? 0x8001 : 0x801));
            }
        }

        public void GotoNextStage()
        {
            OverwritePage();
            Stage = (sbyte)((Stage + 1) % 7);
            SystemPointer = -1;
        }

        public void Reset()
        {
            Book = BOOK_DEFAULT;
            Stage = 0;
            SystemPointer = -1;
        }

        public bool UpdatePointer(float Beats)
        {
            sbyte _NewPointer = (sbyte)Mathf.FloorToInt(Beats / 4.0f);
            if (_NewPointer != SystemPointer)
            {
                SystemPointer = _NewPointer;
                return true;
            }
            return false;
        }

        public List<float> GenerateMissionList()
        {
            List<float> missionList = new List<float>();
            for (sbyte bar = 0; bar < StageBGMBars; bar++)
            {
                bool isOut = ((Book[Stage] >> bar) & 0x1) == 0x1;
                missionList.Add((bar + 2) * 4f + 2.5f);
                if (!isOut)
                    missionList.Add((bar + 2) * 4f + 3f);
            }
            return missionList;
        }

        public List<bool> GenerateIsPointList()
        {
            List<bool> isPointList = new List<bool>();
            for (sbyte bar = 0; bar < StageBGMBars; bar++)
            {
                bool isOut = ((Book[Stage] >> bar) & 0x1) == 0x1;
                if (isOut) isPointList.Add(true);
                else
                {
                    isPointList.Add(false);
                    isPointList.Add(true);
                }
            }
            return isPointList;
        }
    }
}