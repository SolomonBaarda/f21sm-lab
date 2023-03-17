using System.Diagnostics;

namespace CountingPrimes
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

        static void Main(string[] args)
        {
            int[] input = new int[100];

            Random r = new Random(0);

            // Construct data
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = 500 * (i + 1);
            }

            // Array will contain:
            // [500, 1000, 1500, ... ]

            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Restart();

            int result = CountAndSumPrimes(input);

            stopwatch.Stop();
            Console.WriteLine($"Sequential completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");
        }

        // public static int CountAndSumPrimes(int[] input)
        // {
        //     int output = 0;

        //     for (int i = 0; i < input.Length; i++)
        //     {
        //         output += CountPrimes(input[i]);
        //     }

        //     return output;
        // }

        // public static int CountAndSumPrimes(int[] input)
        // {
        //     int sum = 0;
        //     int[] sums = new int[input.Length];

        //     // Parallel computation
        //     Parallel.For(0, input.Length, (index) =>
        //     {
        //         int result = CountPrimes(input[index]);
        //         sums[index] = result;
        //     });

        //     // Sequential summing
        //     foreach (int i in sums)
        //     {
        //         sum += i;
        //     }

        //     return sum;
        // }

        // public static int CountAndSumPrimes(int[] input)
        // {
        //     int sum = 0;

        //     // Parallel computation and summing
        //     Parallel.For(0, input.Length, (index) =>
        //     {
        //         int result = CountPrimes(input[index]);
        //         Interlocked.Add(ref sum, result);
        //     });

        //     return sum;
        // }

        // public static int CountAndSumPrimes(int[] input)
        // {
        //     return input.AsParallel()
        //         .Select(x => CountPrimes(x))
        //         .Aggregate((x, y) => x + y);
        // }


        // Chunked threads
        // public static int CountAndSumPrimes(int[] input)
        // {
        //     int sum = 0;
        //     List<Thread> threads = new List<Thread>();
        //     int workPerProcess = input.Length / Environment.ProcessorCount;

        //     for (int process = 0; process < Environment.ProcessorCount; process++)
        //     {
        //         // Calculate start and end indexes
        //         int start = process * workPerProcess;
        //         int end = (process == Environment.ProcessorCount - 1) ? input.Length : start + workPerProcess;

        //         // Assign work to the thread
        //         threads.Add(new Thread(() =>
        //         {
        //             int localSum = 0;

        //             for (int index = start; index < end; index++)
        //             {
        //                 int result = CountPrimes(input[index]);
        //                 localSum += result;
        //             }

        //             Interlocked.Add(ref sum, localSum);
        //         }));
        //     }

        //     foreach (var thread in threads) thread.Start();
        //     foreach (var thread in threads) thread.Join();

        //     return sum;
        // }

        // Chunked thread pool
        // public static int CountAndSumPrimes(int[] input)
        // {
        //     int workPerProcess = input.Length / Environment.ProcessorCount;
        //     // Keep track of the number of threads remaining to complete
        //     int remaining = Environment.ProcessorCount;
        //     int sum = 0;

        //     using (ManualResetEvent mre = new ManualResetEvent(false))
        //     {
        //         for (int process = 0; process < Environment.ProcessorCount; process++)
        //         {
        //             // Calculate start and end indexes
        //             int start = process * workPerProcess;
        //             int end = (process == Environment.ProcessorCount - 1) ? input.Length : start + workPerProcess;

        //             // Assign work to the pool
        //             ThreadPool.QueueUserWorkItem(delegate
        //             {
        //                 int localSum = 0;

        //                 for (int index = start; index < end; index++)
        //                 {
        //                     int result = CountPrimes(input[index]);
        //                     localSum += result;
        //                 }

        //                 Interlocked.Add(ref sum, localSum);

        //                 if (Interlocked.Decrement(ref remaining) == 0) mre.Set();
        //             });
        //         }

        //         // Wait for all threads to complete
        //         mre.WaitOne();
        //     }

        //     return sum;
        // }



        // Chunked thread pool dynamic 
        public static int CountAndSumPrimes(int[] input)
        {
            int workPerProcess = input.Length / Environment.ProcessorCount;
            // Keep track of the number of threads remaining to complete
            int remaining = Environment.ProcessorCount;
            int nextIteration = 0;
            int sum = 0;

            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                // Create each of the work items.
                for (int process = 0; process < Environment.ProcessorCount; process++)
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










    }
}