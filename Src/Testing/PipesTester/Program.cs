using System.IO.Pipelines;

namespace PipesTester;

class Program
{
    static private int MessageCount;
    
    static void Main(string[] args) {

        Pipe CommPipe = new Pipe();

        Task Writer = WriteToPipe(); 
        Task Reader = ReadFromPipe();

        Task.WhenAll(Writer, Reader);
    }

    static async Task WriteToPipe() {
        

        Interlocked.Increment(ref MessageCount);
    }

    static async Task ReadFromPipe() {

        while (true)
        {
            if (MessageCount > 0)
            {


                Interlocked.Decrement(ref MessageCount);
            }            
        }
        
    }
}
