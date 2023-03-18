using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CountingPrimes
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

            var a = new List<int>();
            var b = new List<int>();
            var c = new List<int>();


            void Process(int a)
            {
            }


            // Delightfully parallel

            foreach (var item in collection)
            {
                Process(item);
            }


            for (int i = 0; i < a.Count(); i++)
            {
                c[i] = a[i] + b[i];
            }




            // Parallel for


            // Sequential version
            foreach (var item in collection)
            {
                Process(item);
            }

            // Parallel equivalent
            Parallel.ForEach(collection, item =>
            {
                Process(item);
            });


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



            // PLINQ

            // Sequential version
            foreach (var item in collection)
            {
                Process(item);
            }

            // LINQ
            collection.ForEach(item => Process(item));

            // P LINQ
            collection.AsParallel()
                .ForAll(item => Process(item));



            // Map
            collection.AsParallel()
                .Select(x => x + 1);

            // Filter
            collection.AsParallel()
                .Where(x => x > 10);

            // Reduce
            collection.AsParallel()
                .Aggregate((x, y) => x + y);




            Thread t = new Thread(() =>
            {
                // Code to run in parallel
            });

            t.Start();

            t.Join();




            {
                List<Thread> threads = new List<Thread>();
                int workPerProcess = collection.Count() / Environment.ProcessorCount;

                for (int process = 0; process < Environment.ProcessorCount; process++)
                {
                    // Calculate start and end indexes
                    int start = process * workPerProcess;
                    int end = (process == Environment.ProcessorCount - 1) ? collection.Count() : start + workPerProcess;

                    // Assign work to the thread
                    threads.Add(new Thread(() =>
                    {
                        for (int i = start; i < end; i++)
                        {
                            Process(collection[i]);
                        }
                    }));
                }

                foreach (var thread in threads) thread.Start();
                foreach (var thread in threads) thread.Join();
            }




            {

                int workPerProcess = collection.Count() / Environment.ProcessorCount;
                // Keep track of the number of threads remaining to complete
                int remaining = Environment.ProcessorCount;

                using (ManualResetEvent mre = new ManualResetEvent(false))
                {
                    for (int process = 0; process < Environment.ProcessorCount; process++)
                    {
                        // Calculate start and end indexes
                        int start = process * workPerProcess;
                        int end = (process == Environment.ProcessorCount - 1) ? collection.Count() : start + workPerProcess;

                        // Assign work to the pool
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            for (int i = start; i < end; i++)
                            {
                                Process(collection[i]);
                            }

                            if (Interlocked.Decrement(ref remaining) == 0) mre.Set();
                        });
                    }
                    // Wait for all threads to complete
                    mre.WaitOne();
                }

            }


            {

                int workPerProcess = collection.Count() / Environment.ProcessorCount;
                // Keep track of the number of threads still waiting to complete
                int remaining = Environment.ProcessorCount;
                int nextWorkIndex = 0;

                using (ManualResetEvent mre = new ManualResetEvent(false))
                {
                    // Create each of the work items.
                    for (int process = 0; process < Environment.ProcessorCount; process++)
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            int i;
                            // Take the next piece of available work
                            while ((i = Interlocked.Increment(ref nextWorkIndex) - 1) < collection.Count())
                            {
                                Process(collection[i]);
                            }

                            if (Interlocked.Decrement(ref remaining) == 0)
                                mre.Set();
                        });
                    }

                    // Wait for all threads to complete.
                    mre.WaitOne();
                }


            }



        }


    }
}