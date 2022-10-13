using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace Streaming
{
    public class StreamHub : Hub
    {

        private readonly string _id;

        public StreamHub()
        {
            _id = Guid.NewGuid().ToString();
        }

        public string Call() => _id;

        public async IAsyncEnumerable<string> Download(
            Data data,
            [EnumeratorCancellation] CancellationToken cancellationToken
        )
        {
            for (var i = 0; i < data.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return $"{i}_{data.Message}_{_id}";

                await Task.Delay(5000, cancellationToken);
            }
        }

        public async Task Upload(IAsyncEnumerable<Data> dataStream)
        {
            await foreach (var data in dataStream)
            {
                Console.WriteLine("Received Data: {0},{1},{2}", data.Count, data.Message, _id);
            }
        }
    }

    public class Data
    {
        public int Count { get; set; }
        public string Message { get; set; }
    }
}
