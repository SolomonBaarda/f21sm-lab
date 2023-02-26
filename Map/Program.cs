using System.Collections.ObjectModel;
using System.Diagnostics;

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
                int p = processor;
                Thread t = new Thread(new ThreadStart(() =>
                {
                    for (int i = p; i < input.Length; i += Environment.ProcessorCount)
                    {
                        output[i] = function(input[i]);
                    }
                }));

                threads.Add(t);
                t.Start();
            }

            // Wait for all threads to complete
            foreach (Thread t in threads)
            {
                t.Join();
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










        // LECTURE EXAMPLES


        void a()
        {

            var collection = new List<int>();

            void Process(int a)
            {

            }

            // Sequential version
            foreach (var item in collection)
            {
                Process(item);
            }

            // Parallel equivalent
            Parallel.ForEach(collection, item => Process(item));


            // Sequential version
            for (int i = 0; i < collection.Count; i++)
            {
                Process(collection[i]);
            }

            // Parallel equivalent
            Parallel.For(0, collection.Count, (i) =>
            {
                Process(collection[i]);
            });


        }


    }
}