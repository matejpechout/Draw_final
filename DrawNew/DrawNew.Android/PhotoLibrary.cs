/*using System.Threading.Tasks;
using System.IO;
using System;
using Android.Content;
using Android.Media;
using Android.OS;
using Java.IO;

using Xamarin.Forms;

using DrawNew.Droid;
using DrawNew;

[assembly: Dependency(typeof(PhotoLibrary))]

namespace DrawNew.Droid
{
    //Třída z projektu rozhleden, nedaří se zprovoznit
    public class PhotoLibrary : IPhotoLibrary
    {
        public Task<System.IO.Stream> PickPhotoAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Instance.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                MainActivity.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            MainActivity.Instance.PickImageTaskCompletionSource = new TaskCompletionSource<System.IO.Stream>();

            // Return Task object
            return MainActivity.Instance.PickImageTaskCompletionSource.Task;
        }

        public Task<bool> SavePhotoAsync(byte[] data, string folder, string filename)
        {
            throw new NotImplementedException();
        }


        
        public async Task<bool> SavePhotoAsync(byte[] data, string folder, string filename)
        {
            try
            {
                Java.IO.File picturesDirectory = System.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
                Java.IO.File folderDirectory = picturesDirectory;

                if (!string.IsNullOrEmpty(folder))
                {
                    folderDirectory = new Java.IO.File(picturesDirectory, folder);
                    folderDirectory.Mkdirs();
                }

                using (Java.IO.File bitmapFile = new Java.IO.File(folderDirectory, filename))
                {
                    bitmapFile.CreateNewFile();

                    using (FileOutputStream outputStream = new FileOutputStream(bitmapFile))
                    {
                        await outputStream.WriteAsync(data);
                    }

                    // Make sure it shows up in the Photos gallery promptly.
                    MediaScannerConnection.ScanFile(MainActivity.Instance,
                                                    new string[] { bitmapFile.Path },
                                                    new string[] { "image/png", "image/jpeg" }, null);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}*/