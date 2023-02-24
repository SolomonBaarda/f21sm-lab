using System.Diagnostics;
//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Collections.Generic;

namespace ParallelMap
{
    class CountPrimesMain
    {
        public static int CountPrimes(int n)
        {
            if (n <= 2)
            {
                return 0;
            }

            int count = 1;

            for (int i = 3; i < n; i++)
            {
                for (int j = 2; j < i; j++)
                {
                    if (i % j == 0) break;

                    if (j == i - 1 && i % j != 0)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int Sequential(int[] input)
        {
            int output = 0;

            for (int i = 0; i < input.Length; i++)
            {
                output += CountPrimes(input[i]);
            }

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

            // Sequential
            stopwatch.Restart();
            var result = Sequential(input);
            stopwatch.Stop();
            Console.WriteLine($"Sequential completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");

            // Parallel for #1
            stopwatch.Restart();
            result = ParallelFor1(input);
            stopwatch.Stop();
            Console.WriteLine($"Parallel for #1 completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");

            // Parallel for #2
            stopwatch.Restart();
            result = ParallelFor2(input);
            stopwatch.Stop();
            Console.WriteLine($"Parallel for #2 completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");

            // Threads #1
            stopwatch.Restart();
            result = ParallelThreads1(input);
            stopwatch.Stop();
            Console.WriteLine($"Threads #1 completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");

            // Threads #2
            stopwatch.Restart();
            result = ParallelThreads2(input);
            stopwatch.Stop();
            Console.WriteLine($"Threads #2 completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");

            // Threads #3
            stopwatch.Restart();
            //result = ParallelThreads3(input);
            stopwatch.Stop();
            Console.WriteLine($"Threads #3 completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");



#if false
        // Tasks
        List<Task> tasks = new List<Task>();

        // Best example
        for (int i = 0; i < numberOfCores; i++) {
            int numElementsToProcess = (size / numberOfCores) + 1;

            Task t = Task.Factory.StartNew(() => {
                for (int j = numElementsToProcess * i; j < numElementsToProcess && j < input.Length; j++) {
                    output[j] = f(input[j]);
                }
            });

            tasks.Add(t);
        }


        foreach (Task t in tasks) {
            t.Wait();
        }
#endif



            //input.AsParallel().AsOrdered().ForAll(input, index => f(input));


        }

        public static int ParallelFor1(int[] input)
        {
            int sum = 0;
            int[] sums = new int[input.Length];

            Parallel.For(0, input.Length, (index) =>
            {
                int result = CountPrimes(input[index]);
                sums[index] = result;
            });

            // Sum the values
            foreach (int i in sums)
            {
                sum += i;
            }

            return sum;
        }

        public static int ParallelFor2(int[] input)
        {
            int sum = 0;

            Parallel.For(0, input.Length, (index) =>
            {
                int result = CountPrimes(input[index]);
                Interlocked.Add(ref sum, result);
            });

            return sum;
        }

        public static int ParallelThreads1(int[] input)
        {
            int sum = 0;
            List<Thread> threads = new List<Thread>();

            // Bad example
            for (int index = 0; index < input.Length; index++)
            {
                int i = index;

                Thread t = new Thread(new ThreadStart(() =>
                {
                    // This block is only executed when the thread starts
                    int result = CountPrimes(input[i]);
                    Interlocked.Add(ref sum, result);
                }));

                threads.Add(t);
                t.Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            return sum;
        }

        public const int NumberOfProcessors = 6;

        public static int ParallelThreads2(int[] input)
        {
            int sum = 0;
            List<Thread> threads = new List<Thread>();

            for (int processor = 0; processor < NumberOfProcessors; processor++)
            {
                int processorCopy = processor;

                Thread t = new Thread(new ThreadStart(() =>
                {
                    int localSum = 0;
                    for (int index = processorCopy; index < input.Length; index += NumberOfProcessors)
                    {
                        int result = CountPrimes(input[index]);
                        localSum += result;
                    }

                    Interlocked.Add(ref sum, localSum);
                }));

                threads.Add(t);
                t.Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            return sum;
        }

        public static int ParallelThreads3(int[] input)
        {
            int sum = 0;
            List<Thread> threads = new List<Thread>();

            for (int processor = 0; processor < NumberOfProcessors; processor++)
            {
                int numElementsToProcess = (input.Length / NumberOfProcessors) + 1;

                Thread t = new Thread(new ThreadStart(() =>
                {
                    for (int index = numElementsToProcess * processor; index < numElementsToProcess && index < input.Length; index++)
                    {
                        int result = CountPrimes(input[index]);
                        Interlocked.Add(ref sum, result);
                    }
                }));
                threads.Add(t);
                t.Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            return sum;
        }



    }
}