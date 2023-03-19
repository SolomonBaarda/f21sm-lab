﻿using System.Diagnostics;

namespace CountingPrimes
{
    class CountPrimesMain
    {

        static int numberOfProcessors => Environment.ProcessorCount;

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

        static void Main(string[] args)
        {
            int[] input = new int[150];

            Random r = new Random(0);

            // Construct data
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = 100 * (i + 1);
            }

            // Array will contain:
            // [100, 200, 300, ... ]

            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Restart();

            int result = CountAndSumPrimes(input);

            stopwatch.Stop();
            Console.WriteLine($"Completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");
        }


        // Sequential
#if true

        public static int CountAndSumPrimes(int[] input)
        {
            int output = 0;

            for (int i = 0; i < input.Length; i++)
            {
                output += CountPrimes(input[i]);
            }

            return output;
        }

#endif


        // Parallel for with parallel summing
#if false

        public static int CountAndSumPrimes(int[] input)
        {
            int sum = 0;

            // Parallel computation and summing
            Parallel.For(0, input.Length, new ParallelOptions { MaxDegreeOfParallelism = numberOfProcessors }, (index) =>
            {
                int result = CountPrimes(input[index]);
                Interlocked.Add(ref sum, result);
            });

            return sum;
        }

#endif


        // PLINQ
#if false

        public static int CountAndSumPrimes(int[] input)
        {
            return input.AsParallel()
                .WithDegreeOfParallelism(numberOfProcessors)
                .Select(x => CountPrimes(x))
                .Aggregate((x, y) => x + y);
        }

#endif


        // Chunked threads
#if false

        public static int CountAndSumPrimes(int[] input)
        {
            int sum = 0;
            List<Thread> threads = new List<Thread>();
            int workPerProcess = input.Length / numberOfProcessors;

            for (int process = 0; process < numberOfProcessors; process++)
            {
                // Calculate start and end indexes
                int start = process * workPerProcess;
                int end = (process == numberOfProcessors - 1) ? input.Length : start + workPerProcess;

                // Assign work to the thread
                threads.Add(new Thread(() =>
                {
                    int localSum = 0;

                    for (int index = start; index < end; index++)
                    {
                        int result = CountPrimes(input[index]);
                        localSum += result;
                    }

                    Interlocked.Add(ref sum, localSum);
                }));
            }

            foreach (var thread in threads) thread.Start();
            foreach (var thread in threads) thread.Join();

            return sum;
        }

#endif


        // Chunked thread pool
#if false

        public static int CountAndSumPrimes(int[] input)
        {
            int workPerProcess = input.Length / numberOfProcessors;
            // Keep track of the number of threads remaining to complete
            int remaining = numberOfProcessors;
            int sum = 0;

            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                for (int process = 0; process < numberOfProcessors; process++)
                {
                    // Calculate start and end indexes
                    int start = process * workPerProcess;
                    int end = (process == numberOfProcessors - 1) ? input.Length : start + workPerProcess;

                    // Assign work to the pool
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        int localSum = 0;

                        for (int index = start; index < end; index++)
                        {
                            int result = CountPrimes(input[index]);
                            localSum += result;
                        }

                        Interlocked.Add(ref sum, localSum);

                        if (Interlocked.Decrement(ref remaining) == 0) mre.Set();
                    });
                }

                // Wait for all threads to complete
                mre.WaitOne();
            }

            return sum;
        }

#endif



        // Chunked thread pool with dynamic partitioning 
#if false

        public static int CountAndSumPrimes(int[] input)
        {
            int workPerProcess = input.Length / numberOfProcessors;
            // Keep track of the number of threads remaining to complete
            int remaining = numberOfProcessors;
            int nextIteration = 0;
            int sum = 0;

            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                // Create each of the work items.
                for (int process = 0; process < numberOfProcessors; process++)
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        int index;
                        int localSum = 0;
                        while ((index = Interlocked.Increment(ref nextIteration) - 1) < input.Length)
                        {
                            int result = CountPrimes(input[index]);
                            localSum += result;
                        }

                        Interlocked.Add(ref sum, localSum);

                        if (Interlocked.Decrement(ref remaining) == 0)
                            mre.Set();
                    });
                }

                // Wait for all threads to complete.
                mre.WaitOne();
            }

            return sum;
        }

#endif

    }
}