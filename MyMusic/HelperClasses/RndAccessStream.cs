using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace MyMusic.HelperClasses
{
    public class StreamingRandomAccessStream : IRandomAccessStream
    {
        private static HttpClient _client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = false, Credentials = new NetworkCredential("user", "password", "DOMAIN") });
        private readonly ulong _size;
        private IInputStream _stream;
        private ulong _requestedPosition;
        private readonly Uri _requestedUri = null;

        public StreamingRandomAccessStream(Stream startStream, Uri uri, ulong size)
        {
            if (startStream != null)
                _stream = startStream.AsInputStream();
            _requestedUri = uri;
            _size = size;
        }

        public async static Task<HttpContentHeaders> GetHeaders(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _client.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);
            return response.Content.Headers;
        }

        private static async Task<Stream> GetStreamWithRange(Uri uri, ulong start, ulong? end)
        {
            var testHeader1 =
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.3; WOW64; Trident/7.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729; InfoPath.3)";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Range = new RangeHeaderValue((long?)start, (long?)end);
                request.Headers.UserAgent.ParseAdd(testHeader1);
                var response = await _client.SendAsync(request);
                return await response.Content.ReadAsStreamAsync();
            }
            catch (WebException ex)
            {
                var message = ex.Message;
                throw;
            }
            catch (InvalidOperationException ex)
            {
                var message = ex.Message;
                throw;
            }
            catch (ArgumentNullException ex)
            {
                var message = ex.Message;
                throw;
            }
        }

        public void Dispose()
        {
            if (_stream != null)
                _stream.Dispose();
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return AsyncInfo.Run<IBuffer, uint>(async (cancellationToken, progress) =>
            {
                progress.Report(0);
                if (_stream == null)
                {
                    var netStream = await GetStreamWithRange(_requestedUri, _requestedPosition, _requestedPosition + count);
                    _stream = netStream.AsInputStream();
                }

                return await _stream.ReadAsync(buffer, count, options).AsTask(cancellationToken, progress);
            });
        }

        public void Seek(ulong position)
        {
            if (_stream != null)
                _stream.Dispose();
            _requestedPosition = position;
            _stream = null;
        }

        public bool CanRead { get { return true; } }
        public bool CanWrite { get { return false; } }
        public ulong Position { get { return _requestedPosition; } }
        public ulong Size { get { return _size; } set { throw new NotSupportedException(); } }
        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer) { throw new NotSupportedException(); }
        public IAsyncOperation<bool> FlushAsync() { throw new NotSupportedException(); }
        public IInputStream GetInputStreamAt(ulong position) { throw new NotSupportedException(); }
        public IOutputStream GetOutputStreamAt(ulong position) { throw new NotSupportedException(); }
        public IRandomAccessStream CloneStream() { throw new NotSupportedException(); }
    }
}
