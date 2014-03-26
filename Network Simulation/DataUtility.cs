using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network_Simulation
{
    public class DataUtility
    {
        /// <summary>
        /// Generate the array of random number.
        /// </summary>
        /// <param name="count">The count of random number.</param>
        /// <returns></returns>
        public static int[] RandomArray(int count) 
        {
            Random rd = new Random();
            int[] randomArray = new int[count];

            for (int i = 0; i < randomArray.Length; i++)
                randomArray[i] = i;

            for (int i = 0; i < randomArray.Length; i++)
            {
                int j = rd.Next(randomArray.Length);
                int tmp = randomArray[i];
                randomArray[i] = randomArray[j];
                randomArray[j] = tmp;
            }

            return randomArray;
        }

        public static void Log(string msg, bool isShowDate = true)
        {
            if (isShowDate)
                Console.Write("[{0}]: {1}", DateTime.Now.ToLongTimeString(), msg);
            else
                Console.Write("{0}", msg);
        }
    }
}
