using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.IO
{
    public sealed class FileInfo
    {
        public string FileName;
        public FileAttributes FileAttributes;
        public ulong CreationTime;
        public ulong LastAccessTime;
        public ulong LastWriteTime;
        public ulong FileSize;

        public FileInfo()
        {
        }

        internal FileInfo(XTFFILEINFO src)
        {
            FileName = src.pszFileName;
            FileAttributes = (FileAttributes)src.dwFileAttributes;
            CreationTime = src.ullCreationTime;
            LastAccessTime = src.ullLastAccessTime;
            LastWriteTime = src.ullLastWriteTime;
            FileSize = src.ullFileSize;
        }
    }

    [Flags]
    public enum FindFileFlags : uint
    {
        None = 0x00000000
    }

    [Flags]
    public enum CopyFileFlags : uint
    {
        None = 0x00000000,
        Defer = 0x80000000,
    }

    [Flags]
    public enum MoveFileFlags : uint
    {
        None = 0x00000000,
        Rename = 0x00000001,
        Defer = 0x80000000,
    }

    [Flags]
    public enum DeleteFileFlags : uint
    {
        None = 0x00000000,
        Defer = 0x80000000,
    }

    [Flags]
    public enum RemoveDirectoryFlags : uint
    {
        None = 0x00000000,
        Force = 0x00000001,
        Defer = 0x80000000,
    }

    public class FindFileEventArgs : EventArgs
    {
        public string RootDirectory;
        public string SearchPattern;
        public FileInfo FileInfo;

        internal FindFileEventArgs(string pszRootDirectory, string pszSearchPattern, XTFFILEINFO pFileInfo)
        {
            RootDirectory = pszRootDirectory;
            SearchPattern = pszSearchPattern;
            FileInfo = new FileInfo(pFileInfo);
        }
    }

    public delegate void FindFileEventHandler(object sender, FindFileEventArgs e);

    internal class FindFileCallback : IXtfFindFileCallback
    {
        FileIOClient _Client;
        FindFileEventHandler _EventHandler;

        public static FindFileCallback Create(FileIOClient client, FindFileEventHandler eventHandler)
        {
            if(null == eventHandler)
            {
                return null;
            }

            return new FindFileCallback(client, eventHandler);
        }

        FindFileCallback(FileIOClient client, FindFileEventHandler eventHandler)
        {
            _Client = client;
            _EventHandler = eventHandler;
        }

        void IXtfFindFileCallback.OnFoundFile(string rootDirectory, string searchPattern, ref XTFFILEINFO fileInfo)
        {
            _EventHandler(_Client, new FindFileEventArgs(rootDirectory, searchPattern, fileInfo));
        }
    }

    public class CopyFileStartEventArgs : FindFileEventArgs
    {
        public string TargetFileName;

        public CopyFileStartEventArgs(string rootDirectory, string searchPattern, XTFFILEINFO fileInfo, string dstFileName)
            : base(rootDirectory, searchPattern, fileInfo)
        {
            TargetFileName = dstFileName;
        }
    }

    public delegate void CopyFileStartEventHandler(object sender, CopyFileStartEventArgs e);

    public class CopyFileProgressEventArgs
    {
        public string SourceFileName;
        public string TargetFileName;
        public ulong FileSize;
        public ulong BytesCopied;

        public CopyFileProgressEventArgs(string srcFileName, string dstFileName, ulong fileSize, ulong bytesCopied)
        {
            SourceFileName = srcFileName;
            TargetFileName = dstFileName;
            FileSize = fileSize;
            BytesCopied = bytesCopied;
        }
    }

    public delegate void CopyFileProgressEventHandler(object sender, CopyFileProgressEventArgs e);

    internal class CopyFileCallback : IXtfCopyFileCallback
    {
        FileIOClient _Client;
        CopyFileStartEventHandler _StartEventHandler;
        CopyFileProgressEventHandler _ProgressEventHandler;

        public static CopyFileCallback Create(FileIOClient client, CopyFileStartEventHandler startEventHandler, CopyFileProgressEventHandler progressEventHandler)
        {
            if((null == startEventHandler) && (null == progressEventHandler))
            {
                return null;
            }

            return new CopyFileCallback(client, startEventHandler, progressEventHandler);
        }

        CopyFileCallback(FileIOClient client, CopyFileStartEventHandler startEventHandler, CopyFileProgressEventHandler progressEventHandler)
        {
            _Client = client;
            _StartEventHandler = startEventHandler;
            _ProgressEventHandler = progressEventHandler;
        }

        void IXtfCopyFileCallback.OnStartFileCopy(string pszRootDirectory, string pszSearchPattern, ref XTFFILEINFO pSrcFileInfo, string pszDstFileName)
        {
            if(null != _StartEventHandler)
            {
                _StartEventHandler(_Client, new CopyFileStartEventArgs(pszRootDirectory, pszSearchPattern, pSrcFileInfo, pszDstFileName));
            }
        }

        void IXtfCopyFileCallback.OnFileCopyProgress(string pszSrcFileName, string pszDstFileName, ulong ullFileSize, ulong ullBytesCopied)
        {
            if(null != _ProgressEventHandler)
            {
                _ProgressEventHandler(_Client, new CopyFileProgressEventArgs(pszSrcFileName, pszDstFileName, ullFileSize, ullBytesCopied));
            }
        }

        void IXtfCopyFileCallback.OnEndFileCopy(string pszRootDirectory, string pszSearchPattern, ref XTFFILEINFO pSrcFileInfo, string pszDstFileName, int errorCode)
        {
        }
    }

    public sealed class FileIOClient : IDisposable
    {
        internal IXtfFileIOClient BaseObject;
        private bool disposed = false;

        public FileIOClient(string address)
        {
            System.Guid riid = typeof(IXtfFileIOClient).GUID;
            IntPtr ppvObj;

            HRESULT.CHK(NativeMethods.XtfCreateFileIOClient(address, ref riid, out ppvObj));

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfFileIOClient;
            Marshal.Release(ppvObj);
        }

        public FileIOClient()
        {
            System.Guid riid = typeof(IXtfFileIOClient).GUID;
            IntPtr ppvObj;

            HRESULT.CHK(NativeMethods.XtfCreateFileIOClient(null, ref riid, out ppvObj));

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfFileIOClient;
            Marshal.Release(ppvObj);
        }

        ~FileIOClient()
        {
            this.Dispose(false);
        }

        public void CopyFiles(string sourceFileName, FileAttributes includeAttributes, FileAttributes excludeAttributes, uint recursionLevels, string targetFileName, CopyFileFlags flags, CopyFileStartEventHandler startEventHandler, CopyFileProgressEventHandler progressEventHandler)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            BaseObject.CopyFiles(sourceFileName, (uint)includeAttributes, (uint)excludeAttributes, recursionLevels, targetFileName, (uint)flags, CopyFileCallback.Create(this, startEventHandler, progressEventHandler));
        }

        public void CreateDirectory(string directory)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            BaseObject.CreateDirectory(directory);
        }

        public void DeleteFiles(string fileName, FileAttributes includeAttributes, FileAttributes excludeAttributes, uint recursionLevels, DeleteFileFlags flags, FindFileEventHandler eventHandler)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            BaseObject.DeleteFiles(fileName, (uint)includeAttributes, (uint)excludeAttributes, recursionLevels, (uint)flags, FindFileCallback.Create(this, eventHandler));
        }

        public void FindFiles(string searchPattern, FileAttributes includeAttributes, FileAttributes excludeAttributes, uint recursionLevels, FindFileFlags flags, FindFileEventHandler eventHandler)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            BaseObject.FindFiles(searchPattern, (uint)includeAttributes, (uint)excludeAttributes, recursionLevels, (uint)flags, FindFileCallback.Create(this, eventHandler));
        }

        public FileAttributes GetFileAttributes(string fileName)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            return (FileAttributes)BaseObject.GetFileAttributes(fileName);
        }

        public ulong GetFileSize(string fileName)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            return BaseObject.GetFileSize(fileName);
        }

        public void RemoveDirectory(string directory, RemoveDirectoryFlags flags, FindFileEventHandler eventHandler)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            try
            {
                BaseObject.RemoveDirectory(directory, (uint)flags, FindFileCallback.Create(this, eventHandler));
            }
            catch(DirectoryNotFoundException)
            {
            }
        }

        public void SetFileAttributes(string fileName, FileAttributes includeAttributes, FileAttributes excludeAttributes, uint recursionLevels, FileAttributes addAttributes, FileAttributes removeAttributes, FindFileEventHandler eventHandler)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            BaseObject.SetFileAttributes(fileName, (uint)includeAttributes, (uint)excludeAttributes, recursionLevels, (uint)addAttributes, (uint)removeAttributes, FindFileCallback.Create(this, eventHandler));
        }

        public FileInfo GetFileInfo(string fileName)
        {
            XTFFILEINFO fileInfo;
            BaseObject.GetFileInfo(fileName, out fileInfo);
            return new FileInfo(fileInfo);
        }

        public void Dispose()
        {
            this.Dispose(true);

            //
            // Avoid cleaning up twice.  Calling Dispose(true)
            // implies that managed objects will be cleaned up as well
            // as unmanaged objects so the garbage collector will
            // have no work to do when finalizing this object.
            //
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                Extensions.ReleaseComObject(ref BaseObject);

                //
                // Since the garbage collector's Finalize() runs on
                // a background thread, managed objects are not safe
                // to reference.  Only clean up managed objects if this
                // is being explicitly disposed.
                //
                // if (disposing)
                // {
                //    // ... Clean up managed objects
                // }
                //

                this.disposed = true;
            }
        }
    }
}
