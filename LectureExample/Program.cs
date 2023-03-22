﻿using System.Diagnostics;

namespace LectureExampleProgram
{
    class LectureExampleProgramMain
    {
        static int numberOfProcessors => Environment.ProcessorCount;

        private static int CountPrimes(int n)
        {
            if (n <= 2)
            {
                return 0;
            }

            int numberOfPrimes = 1;

            for (int i = 3; i < n; i++)
            {
                for (int j = 2; j < i; j++)
                {
                    if (i % j == 0) break;

                    if (j == i - 1 && i % j != 0)
                    {
                        numberOfPrimes++;
                    }
                }
            }

            return numberOfPrimes;
        }

        private static int Fibonacci(int n)
        {
            if (n == 0)
            {
                return 0;
            }
            else if (n == 1)
            {
                return 1;
            }

            int fibNMinusTwo = 0;
            int fibNMinusOne = 1;
            int fibN = 0;

            for (int i = 2; i <= n; i++)
            {
                fibN = fibNMinusOne + fibNMinusTwo;
                fibNMinusTwo = fibNMinusOne;
                fibNMinusOne = fibN;
            }

            return fibN;
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

            long result = ExampleProgram(input);

            stopwatch.Stop();
            Console.WriteLine($"Completed in {stopwatch.ElapsedMilliseconds} ms with a result of {result}");
        }


        // Sequential
#if true

        public static long ExampleProgram(int[] input)
        {
            // Calculate the sum of primes for the input
            int sumOfPrimes = 0;
            for (int index = 0; index < input.Length; index++)
            {
                sumOfPrimes += CountPrimes(input[index]);
            }

            // Calculate the sum of fibonacci numbers for the input
            int sumOfFibonacci = 0;
            for (int index = 0; index < input.Length; index++)
            {
                sumOfFibonacci += Fibonacci((int)Math.Sqrt(input[index]) * sumOfPrimes);
            }

            return sumOfFibonacci;
        }

#endif


        // Parallel for
#if false

        public static int ExampleProgram(int[] input)
        {
            // Calculate the sum of primes for the input
            int sumOfPrimes = 0;
            Parallel.For(0, input.Length, new ParallelOptions { MaxDegreeOfParallelism = numberOfProcessors }, (index) =>
            {
                int numberOfPrimes = CountPrimes(input[index]);

                Interlocked.Add(ref sumOfPrimes, numberOfPrimes);
            });

            // Calculate the sum of fibonacci numbers for the input
            int sumOfFibonacci = 0;
            Parallel.For(0, input.Length, new ParallelOptions { MaxDegreeOfParallelism = numberOfProcessors }, (index) =>
            {
                int fibonacci = Fibonacci((int)Math.Sqrt(input[index]) * sumOfPrimes);

                Interlocked.Add(ref sumOfFibonacci, fibonacci);
            });

            return sumOfFibonacci;
        }

#endif


        // PLINQ
#if false

        public static int ExampleProgram(int[] input)
        {
            int sumOfPrimes = input.AsParallel()
                .WithDegreeOfParallelism(numberOfProcessors)
                .Select(x => CountPrimes(x))
                .Aggregate((x, y) => x + y);

            return input.AsParallel()
                .WithDegreeOfParallelism(numberOfProcessors)
                .Select(x => Fibonacci((int)Math.Sqrt(x) * sumOfPrimes))
                .Aggregate((x, y) => x + y);
        }

#endif


        // Chunked threads
#if false

        public static int ExampleProgram(int[] input)
        {
            // Calculate the sum of primes for the input
            int sumOfPrimes = 0;
            List<Thread> threads = new List<Thread>();
            int numberOfIndexesPerProcess = input.Length / numberOfProcessors;

            for (int process = 0; process < numberOfProcessors; process++)
            {
                // Calculate start and end indexes
                int start = process * numberOfIndexesPerProcess;
                int end = (process == numberOfProcessors - 1) ? input.Length : start + numberOfIndexesPerProcess;

                // Assign work to the thread
                threads.Add(new Thread(() =>
                {
                    int localSum = 0;

                    for (int index = start; index < end; index++)
                    {
                        int result = CountPrimes(input[index]);
                        localSum += result;
                    }

                    Interlocked.Add(ref sumOfPrimes, localSum);
                }));
            }

            foreach (var thread in threads) thread.Start();
            foreach (var thread in threads) thread.Join();
            threads.Clear();

            // Calculate the sum of fibonacci numbers for the input
            int sumOfFibonacci = 0;

            for (int process = 0; process < numberOfProcessors; process++)
            {
                // Calculate start and end indexes
                int start = process * numberOfIndexesPerProcess;
                int end = (process == numberOfProcessors - 1) ? input.Length : start + numberOfIndexesPerProcess;

                // Assign work to the thread
                threads.Add(new Thread(() =>
                {
                    int localSum = 0;

                    for (int index = start; index < end; index++)
                    {
                        int fibonacci = Fibonacci((int)Math.Sqrt(input[index]) * sumOfPrimes);
                        localSum += fibonacci;
                    }

                    Interlocked.Add(ref sumOfFibonacci, localSum);
                }));
            }

            foreach (var thread in threads) thread.Start();
            foreach (var thread in threads) thread.Join();

            return sumOfFibonacci;
        }

#endif


        // Chunked thread pool
#if false

        public static int ExampleProgram(int[] input)
        {
            int numberOfIndexesPerProcess = input.Length / numberOfProcessors;
            // Keep track of the number of threads remaining to complete
            int remaining = numberOfProcessors;
            int sumOfPrimes = 0;
            int sumOfFibonacci = 0;

            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                for (int process = 0; process < numberOfProcessors; process++)
                {
                    // Calculate start and end indexes
                    int start = process * numberOfIndexesPerProcess;
                    int end = (process == numberOfProcessors - 1) ? input.Length : start + numberOfIndexesPerProcess;

                    // Assign work to the pool
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        int localSum = 0;

                        for (int index = start; index < end; index++)
                        {
                            int result = CountPrimes(input[index]);
                            localSum += result;
                        }

                        Interlocked.Add(ref sumOfPrimes, localSum);

                        if (Interlocked.Decrement(ref remaining) == 0) mre.Set();
                    });
                }

                // Wait for all threads to complete
                mre.WaitOne();

                // Calculate the sum of fibonacci numbers for the input
                mre.Reset();
                remaining = numberOfProcessors;

                for (int process = 0; process < numberOfProcessors; process++)
                {
                    // Calculate start and end indexes
                    int start = process * numberOfIndexesPerProcess;
                    int end = (process == numberOfProcessors - 1) ? input.Length : start + numberOfIndexesPerProcess;

                    // Assign work to the pool
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        int localSum = 0;

                        for (int index = start; index < end; index++)
                        {
                            int fibonacci = Fibonacci((int)Math.Sqrt(input[index]) * sumOfPrimes);
                            localSum += fibonacci;
                        }

                        Interlocked.Add(ref sumOfFibonacci, localSum);

                        if (Interlocked.Decrement(ref remaining) == 0) mre.Set();
                    });
                }

                // Wait for all threads to complete
                mre.WaitOne();
            }

            return sumOfFibonacci;
        }

#endif



        // Chunked thread pool with dynamic partitioning 
#if false

        public static int ExampleProgram(int[] input)
        {
            // Keep track of the number of threads remaining to complete
            int remaining = numberOfProcessors;
            int nextIteration = 0;
            int sumOfPrimes = 0;
            int sumOfFibonacci = 0;

            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                // Create each of the work items
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

                        Interlocked.Add(ref sumOfPrimes, localSum);

                        if (Interlocked.Decrement(ref remaining) == 0)
                            mre.Set();
                    });
                }

                // Wait for all threads to complete
                mre.WaitOne();

                mre.Reset();
                remaining = numberOfProcessors;
                nextIteration = 0;

                // Create each of the work items
                for (int process = 0; process < numberOfProcessors; process++)
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        int index;
                        int localSum = 0;
                        while ((index = Interlocked.Increment(ref nextIteration) - 1) < input.Length)
                        {
                            int fibonacci = Fibonacci((int)Math.Sqrt(input[index]) * sumOfPrimes);
                            localSum += fibonacci;
                        }

                        Interlocked.Add(ref sumOfFibonacci, localSum);

                        if (Interlocked.Decrement(ref remaining) == 0)
                            mre.Set();
                    });
                }

                // Wait for all threads to complete
                mre.WaitOne();
            }

            return sumOfFibonacci;
        }

#endif

    }
}