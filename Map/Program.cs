using System.Diagnostics;
//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Collections.Generic;

namespace ParallelMap
{
    class Map
    {
        public static T[] MapSequential<T>(T[] input, Func<T, T> function)
        {
            T[] output = new T[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = function(input[i]);
            }

            return output;
        }

        public static T[] MapParallelFor<T>(T[] input, Func<T, T> function)
        {
            T[] output = new T[input.Length];

            Parallel.For(0, input.Length, (i) =>
            {
                output[i] = function(input[i]);
            });

            return output;
        }

        public static T[] MapThreads<T>(T[] input, Func<T, T> function)
        {
            T[] output = new T[input.Length];
            List<Thread> threads = new List<Thread>();

            for (int processor = 0; processor < Environment.ProcessorCount; processor++)
            {

            }

            for (int i = 0; i < input.Length; i += Environment.ProcessorCount)
            {

            }

            return output;
        }

        public static T[] MapPLINQ<T>(T[] input, Func<T, T> function)
        {
            T[] output = new T[input.Length];

            input.AsParallel()
                .AsOrdered()
                .Select((value, index) => (value, index))
                .ForAll(x => { output[x.index] = function(x.value); });

            return output;
        }

        static void Main(string[] args)
        {
            int[] input = new int[500];

            Random r = new Random(0);

            // Construct data
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = r.Next(5000, 10000);
            }

            Console.WriteLine($"Running using {Environment.ProcessorCount} logical processors");
            Console.WriteLine($"Running on {Environment.OSVersion} using C# runtime {Environment.Version}\n");


            Stopwatch stopwatch = Stopwatch.StartNew();



        }
    }
}