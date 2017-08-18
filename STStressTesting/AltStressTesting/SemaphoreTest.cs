using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

namespace STStressTesting
{
    public class SemaphoreTest
    {
        static SemaphoreSlim _semaphore = new SemaphoreSlim(Environment.ProcessorCount);

        public void Run()
        {
            var threadList = new List<Thread>();

            for (int i = 1; i <= 20; i++)
            {
                Thread newThread = new Thread(new ParameterizedThreadStart(ThreadCall));
                threadList.Add(newThread);
                newThread.Start(null);
            }
            foreach (Thread thread in threadList)
            {
                thread.Join();
            }

            Console.WriteLine($"Final sema: {_semaphore.CurrentCount}");
        }
        private void Output(string text)
        {
            var sdate = DateTime.Now.ToString("mm:ss.fffffff");
            Console.WriteLine($"{sdate}, {text} (id: {Thread.CurrentThread.ManagedThreadId})");
        }

        private void ThreadCall(object othreadSettings)
        {
            Console.WriteLine($"sema: {_semaphore.CurrentCount}");

            Stopwatch sw = new Stopwatch();
            Output("Request made: " + sw.ElapsedMilliseconds);
            if (_semaphore.Wait(5000))
            {
                Console.WriteLine("Value: Executing");

                try
                {
                    Thread.Sleep(6000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                Output("Busy: Time out " + sw.ElapsedMilliseconds);
            }
            Output("Elapsed " + sw.ElapsedMilliseconds);
        }
    }
}
