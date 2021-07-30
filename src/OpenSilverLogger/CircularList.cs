using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSilverLogger
{
    public class CircularList
    {
        private string[] list;
        private int index = 0;

        public CircularList(int size)
        {
            list = new string[size];
        }

        private void IncreaseIndex()
        {
            index++;
            index = index % list.Length;
        }

        public void Add(string val)
        {
            list[index] = val;
            IncreaseIndex();
        }

        public void Print()
        {
            for (var i = 0; i < list.Length; i++)
            {
                Console.WriteLine(list[index]);
                IncreaseIndex();
            }
        }
    }
}
