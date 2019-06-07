using System;
using System.IO;
using System.Threading.Tasks;

namespace DrawNew
{
    //interface pro PhotoLibrary
    public interface IPhotoLibrary
    {
        Task<Stream> PickPhotoAsync();

        Task<bool> SavePhotoAsync(byte[] data, string folder, string filename);
    }
}
