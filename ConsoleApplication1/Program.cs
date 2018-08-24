using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static bool Contains(int[] arr, int element)
        {
            foreach (int i in arr)
                if (i == element)
                    return true;
            return false;
        }

        static void Main(string[] args)
        {
            int[] arr = { 1, 2, 2, 3, 3, 3, 4 };
            int[] result = new int[arr.Length];
            result[0] = arr[0];
            int index = 1;
            for (int i = 1; i < arr.Length; i++)
            {
                if (!Contains(result, arr[i]))
                {
                    result[index] = arr[i];
                    index++;
                }
            }

            for (int i = 0; i < index; i++)
                Console.WriteLine(result[i]);
            Console.ReadKey();
        }
    }
}