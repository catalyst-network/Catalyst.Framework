using Catalyst.Common.FileSystem;
using Catalyst.Common.Rpc;

namespace Catalyst.Common.Interfaces.FileSystem
{
    public interface IFileTransfer
    {
        AddFileToDfsResponseCode InitializeTransfer(FileTransferInformation fileTransferInformation);
        AddFileToDfsResponseCode WriteChunk(string fileName, uint chunkId, byte[] fileChunk);
    }
}
