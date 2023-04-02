using Godot;
using System.Collections.Generic;

namespace Game
{
    // Book[PageIndex] = Page
    // ((Page >> GagIndex) & 0b1) == 0b1 = IsFailureGag
    public class GagSystemClass
    {
        private static readonly List<ushort> BOOK_DEFAULT = new List<ushort> {
            0b1000,
            0b1100_1000,
            0b1000_1100,
            0b1000_0000,
            0b1100_1000,
            0b1000_0010,
            0b1111_1001_1100_1000
        };
        private List<ushort> Book = BOOK_DEFAULT;
        private sbyte _pageIndex = 0;
        public sbyte PageIndex
        {
            get => _pageIndex;
            private set
            {
                _pageIndex = value;
            }
        }

        public sbyte StageBGMBarCount
        {
            get => (sbyte)(PageIndex == 0 ? 4 : PageIndex == 6 ? 16 : 8);
        }


        public bool GagIndex2IsFailureGag(sbyte GagIndex)
        {
            return ((Book[PageIndex] >> GagIndex) & 0b1) == 0b1;
        }

        public bool Beat2IsFailureGag(float beat)
        {
            return ((Book[PageIndex] >> (int) Mathf.Max((beat-8f) / 4f, 0f)) & 0x1) == 0b1;
        }

        private void OverwritePage()
        {
            if (PageIndex != 0)
            {
                Book[PageIndex] = (ushort)(GD.Randi() % (PageIndex == 6 ? 0x8001 : 0x801));
            }
        }

        public void TurnToNextPage()
        {
            OverwritePage();
            PageIndex = (sbyte)((PageIndex + 1) % 7);
        }

        public void Reset()
        {
            Book = BOOK_DEFAULT;
            PageIndex = 0;
        }

        public (List<float> missionList, List<bool> isPointList) GenerateMissionIsPointList()
        {
            List<float> missionList = new List<float>();
            List<bool> isPointList = new List<bool>();
            for (sbyte bar = 0; bar < StageBGMBarCount; bar++)
            {
                bool isOut = ((Book[PageIndex] >> bar) & 0b1) == 0b1;
                missionList.Add((bar + 2) * 4f + 2.5f);
                if (isOut)
                {
                    isPointList.Add(true);
                }
                else
                {
                    missionList.Add((bar + 2) * 4f + 3f);
                    isPointList.Add(false);
                    isPointList.Add(true);
                }
            }
            return (missionList, isPointList);
        }
    }
}
