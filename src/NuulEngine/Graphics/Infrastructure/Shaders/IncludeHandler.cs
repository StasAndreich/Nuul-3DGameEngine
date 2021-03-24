using System;
using SharpDX.D3DCompiler;
using System.IO;
using System.Windows.Forms;

namespace Para_1
{
    internal sealed class IncludeHandler : Include
    {
        private Stream _stream;

        public IDisposable Shadow { get; set; }

        public void Close(Stream stream)
        {
            _stream.Dispose();
            _stream = null;
        }

        public Stream Open(IncludeType type, string fileName, Stream parentStream)
        {
            string path = Application.StartupPath;
            FileInfo[] files = new DirectoryInfo(path)
                .GetFiles(fileName, SearchOption.AllDirectories);
            var fileStream = new FileStream(files[0].FullName, FileMode.Open);
            return fileStream;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
