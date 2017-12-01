namespace Microsoft.Xbox.XTF
{
    using System.Runtime.InteropServices;

    //
    // TODO TFS 769989
    //
    // Make Microsoft.Xbox.XTF.dll public and remove redundant hresult.cs and extension.cs from other assemblies
    //

    internal static partial class Extensions
    {
        public static void ReleaseComObject<T>(ref T obj) where T : class
        {
            if(null != obj)
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
        }
    }
}
