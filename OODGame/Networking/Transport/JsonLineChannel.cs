using OODGame.Networking.Protocol;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OODGame.Networking.Transport
{
    public sealed class JsonLineChannel : IAsyncDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);

        public JsonLineChannel(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            NetworkStream stream = tcpClient.GetStream();
            _reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            _writer = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true)
            {
                AutoFlush = true
            };
        }

        public async Task<MessageEnvelope?> ReceiveAsync(CancellationToken cancellationToken)
        {
            string? line = await _reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null)
                return null;

            return ProtocolJson.Deserialize(line);
        }

        public async Task SendAsync(MessageEnvelope envelope, CancellationToken cancellationToken)
        {
            string json = ProtocolJson.Serialize(envelope);
            await _writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _writer.WriteLineAsync(json).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            _writeLock.Dispose();
            _reader.Dispose();
            await _writer.DisposeAsync().ConfigureAwait(false);
            _tcpClient.Dispose();
        }
    }
}
