using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace TestPaint
{
    public static class StorageManager
    {
        public static async void SaveCanvas(InkCanvas inkCanvas, CanvasDataObject canvasDataObject)
        {
            StorageFolder storageFolder = await GetOrCreateWorkDirectory();
            StorageFile file = await storageFolder.GetFileAsync(canvasDataObject.FullName);
            
            WriteCanvaseInFile(inkCanvas, file);
        }

        public static async Task<CanvasDataObject> CreateCanvas()
        {
            StorageFolder storageFolder = await GetOrCreateWorkDirectory();
            StorageFile file = await storageFolder.CreateFileAsync("Untitled" + CanvasDataObject.FileType, CreationCollisionOption.GenerateUniqueName);

            InkCanvas inkCanvas = new InkCanvas();
            CanvasDataObject canvasDataObject = GetCanvasDataObject(file);

            WriteCanvaseInFile(inkCanvas, file);

            return canvasDataObject;

        }

        public static async void LoadCanvas(InkCanvas inkCanvas, CanvasDataObject canvasDataObject)
        {
            StorageFolder storageFolder = await GetOrCreateWorkDirectory();
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
        }

        public static async Task<IEnumerable<CanvasDataObject>> GetAllCanvasFromWorkDirectory()
        {
            StorageFolder workDirectory = await GetOrCreateWorkDirectory();

            var queryOptions = new QueryOptions(CommonFolderQuery.DefaultQuery);
            var query = workDirectory.CreateFileQueryWithOptions(queryOptions);
            IEnumerable<StorageFile> fileList = await query.GetFilesAsync();

            List<CanvasDataObject> canvasDataObjects = new List<CanvasDataObject>();
            foreach (StorageFile file in fileList)
            {
                if (file.FileType == CanvasDataObject.FileType)
                    canvasDataObjects.Add(GetCanvasDataObject(file));
            }
            return canvasDataObjects;
        }

        public static async void DeleteCanvas(CanvasDataObject canvasDataObject)
        {
            StorageFolder storageFolder = await GetOrCreateWorkDirectory();
            StorageFile file = await storageFolder.GetFileAsync(canvasDataObject.FullName);

            await file.DeleteAsync();
        }

        public static async Task<CanvasDataObject> RenameCanvas(CanvasDataObject canvasDataObject, string newName)
        {
            StorageFolder storageFolder = await GetOrCreateWorkDirectory();
            StorageFile file = await storageFolder.GetFileAsync(canvasDataObject.FullName);

            await file.RenameAsync(newName + CanvasDataObject.FileType, NameCollisionOption.ReplaceExisting);
            return GetCanvasDataObject(file);
        }

        private static CanvasDataObject GetCanvasDataObject(StorageFile file)
        {
            return new CanvasDataObject(file.DisplayName, file.DateCreated.DateTime);
        }

        private static async void WriteCanvaseInFile(InkCanvas inkCanvas, StorageFile file)
        {
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
                {
                    await inkCanvas.InkPresenter.StrokeContainer.SaveAsync(outputStream, Windows.UI.Input.Inking.InkPersistenceFormat.GifWithEmbeddedIsf);
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
        }

        private static async Task<StorageFolder> GetOrCreateWorkDirectory()
        {
            StorageFolder picturesLibrary = await KnownFolders.GetFolderAsync(KnownFolderId.PicturesLibrary);
            StorageFolder workDirectory = await picturesLibrary.CreateFolderAsync("TestPaint", CreationCollisionOption.OpenIfExists);
            return workDirectory;
        }
    }
}
