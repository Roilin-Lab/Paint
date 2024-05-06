using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace TestPaint
{
    public static class StorageManager
    {
        public static async void SaveCanvas(InkCanvas inkCanvas, CanvasDataObject canvasDataObject)
        {
            StorageFolder storageFolder = await KnownFolders.GetFolderAsync(KnownFolderId.PicturesLibrary);
            StorageFile file = await storageFolder.CreateFileAsync(canvasDataObject.FullName, CreationCollisionOption.ReplaceExisting);
            
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
                {
                    await inkCanvas.InkPresenter.StrokeContainer.SaveAsync(outputStream);
                    await outputStream.FlushAsync();
                }
                stream.Dispose();

                Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    // File saved.
                }
                else
                {
                    // File couldn't be saved.
                }
            }
            // User selects Cancel and picker returns null.
            else
            {
                // Operation cancelled.
            }
        }

        public static async void LoadCanvas(InkCanvas inkCanvas, CanvasDataObject canvasDataObject)
        {
            StorageFolder storageFolder = await KnownFolders.GetFolderAsync(KnownFolderId.PicturesLibrary);
            StorageFile file = await storageFolder.GetFileAsync(canvasDataObject.FullName);

            

            if (file != null)
            {
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(inputStream);
                }
                stream.Dispose();
            }
            // User selects Cancel and picker returns null.
            else
            {
                // Operation cancelled.
            }
        }
    }
}
