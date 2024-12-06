using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static readonly int MAX_SEATS = 3;
    static SemaphoreSlim semaphore = new SemaphoreSlim(MAX_SEATS, MAX_SEATS);

    static async Task EnterRoomAsync(int threadId)
    {
        Console.WriteLine($"Thread {threadId} is attempting to enter the room...");
        await semaphore.WaitAsync();

        Console.WriteLine($"Thread {threadId} has entered the room.");

        Random rand = new Random();
        await Task.Delay(rand.Next(1000, 3000));  

        Console.WriteLine($"Thread {threadId} is leaving the room.");
        semaphore.Release();
    }

    public class Printer
    {
        private readonly SemaphoreSlim _semaphore;

        public Printer()
        {
            _semaphore = new SemaphoreSlim(2, 2);  
        }

        public async Task PrintDocumentAsync(string documentName)
        {
            await _semaphore.WaitAsync();

            try
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} started printing {documentName}");
                await Task.Delay(2000);  
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished printing {documentName}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    static async Task Main()
    {
        List<Task> tasks = new List<Task>();

        for (int i = 0; i < 10; i++)
        {
            int threadId = i; 
            tasks.Add(EnterRoomAsync(threadId));
        }

        await Task.WhenAll(tasks);

        Printer printer = new Printer();

        Task[] printTasks = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            printTasks[i] = printer.PrintDocumentAsync($"Document {i + 1}");
        }
        await Task.WhenAll(printTasks);

        Console.WriteLine("All documents printed.");
        Console.ReadKey();
    }
}
