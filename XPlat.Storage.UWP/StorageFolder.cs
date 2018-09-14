﻿namespace XPlat.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using XPlat.Storage.FileProperties;

    using BasicProperties = Windows.Storage.FileProperties.BasicProperties;

    /// <summary>
    /// Defines an application folder.
    /// </summary>
    public sealed class StorageFolder : IStorageFolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageFolder"/> class.
        /// </summary>
        /// <param name="folder">
        /// The associated <see cref="StorageFolder"/>
        /// </param>
        internal StorageFolder(Windows.Storage.StorageFolder folder)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }

            this.Originator = folder;
        }

        public static implicit operator StorageFolder(Windows.Storage.StorageFolder folder)
        {
            return new StorageFolder(folder);
        }

        /// <summary>
        /// Gets the originating Windows storage folder.
        /// </summary>
        public Windows.Storage.StorageFolder Originator { get; }

        /// <inheritdoc />
        public DateTime DateCreated => this.Originator.DateCreated.DateTime;

        /// <inheritdoc />
        public string Name => this.Originator.Name;

        /// <inheritdoc />
        public string DisplayName => this.Originator.DisplayName;

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <remarks>
        /// This is currently not implemented. You can access the Originator.Properties value to get the native StorageFile properties.
        /// </remarks>
        public IStorageItemContentProperties Properties { get; }

        /// <inheritdoc />
        public string Path => this.Originator.Path;

        /// <inheritdoc />
        public bool Exists => this.Originator != null;


        /// <inheritdoc />
        public FileAttributes Attributes => (FileAttributes)(int)this.Originator.Attributes;

        /// <inheritdoc />
        public Task RenameAsync(string desiredName)
        {
            return this.RenameAsync(desiredName, NameCollisionOption.FailIfExists);
        }

        /// <inheritdoc />
        public async Task RenameAsync(string desiredName, NameCollisionOption option)
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(this.Name, "Cannot rename a folder that does not exist.");
            }

            if (string.IsNullOrWhiteSpace(desiredName))
            {
                throw new ArgumentNullException(nameof(desiredName));
            }

            await this.Originator.RenameAsync(desiredName, option.ToNameCollisionOption());
        }

        /// <inheritdoc />
        public async Task DeleteAsync()
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(this.Name, "Cannot delete a folder that does not exist.");
            }

            await this.Originator.DeleteAsync();
        }

        /// <inheritdoc />
        public bool IsOfType(StorageItemTypes type)
        {
            return type == StorageItemTypes.Folder;
        }

        /// <inheritdoc />
        public async Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get properties for a folder that does not exist.");
            }

            Windows.Storage.StorageFolder storageFolder = this.Originator;
            if (storageFolder == null) return null;

            BasicProperties basicProperties = await storageFolder.GetBasicPropertiesAsync();
            return basicProperties.ToBasicProperties();
        }

        /// <inheritdoc />
        public async Task<IStorageFolder> GetParentAsync()
        {
            Windows.Storage.StorageFolder parent = await this.Originator.GetParentAsync();
            return parent == null ? null : new StorageFolder(parent);
        }

        /// <inheritdoc />
        public bool IsEqual(IStorageItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item.Path.Equals(this.Path, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <inheritdoc />
        public Task<IStorageFile> CreateFileAsync(string desiredName)
        {
            return this.CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);
        }

        /// <inheritdoc />
        public async Task<IStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot create a file in a folder that does not exist.");
            }

            if (string.IsNullOrWhiteSpace(desiredName))
            {
                throw new ArgumentNullException(nameof(desiredName));
            }

            Windows.Storage.StorageFile storageFile = await this.Originator.CreateFileAsync(desiredName, options.ToCreationCollisionOption());
            return new StorageFile(storageFile);
        }

        /// <inheritdoc />
        public Task<IStorageFolder> CreateFolderAsync(string desiredName)
        {
            return this.CreateFolderAsync(desiredName, CreationCollisionOption.FailIfExists);
        }

        /// <inheritdoc />
        public async Task<IStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot create a folder in a folder that does not exist.");
            }

            if (string.IsNullOrWhiteSpace(desiredName))
            {
                throw new ArgumentNullException(nameof(desiredName));
            }

            Windows.Storage.StorageFolder storageFolder =
                await this.Originator.CreateFolderAsync(desiredName, options.ToCreationCollisionOption());
            return new StorageFolder(storageFolder);
        }

        /// <inheritdoc />
        public Task<IStorageFile> GetFileAsync(string name)
        {
            return this.GetFileAsync(name, false);
        }

        /// <inheritdoc />
        public async Task<IStorageFile> GetFileAsync(string name, bool createIfNotExists)
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get a file from a folder that does not exist.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Windows.Storage.StorageFile storageFile = null;

            try
            {
                storageFile = await this.Originator.GetFileAsync(name);
            }
            catch (Exception)
            {
                if (!createIfNotExists)
                {
                    throw;
                }
            }

            if (createIfNotExists && storageFile == null)
            {
                return await this.CreateFileAsync(name, CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
            }
            else
            {
                return new StorageFile(storageFile);
            }
        }

        /// <inheritdoc />
        public Task<IStorageFolder> GetFolderAsync(string name)
        {
            return this.GetFolderAsync(name, false);
        }

        /// <inheritdoc />
        public async Task<IStorageFolder> GetFolderAsync(string name, bool createIfNotExists)
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get a folder from a folder that does not exist.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Windows.Storage.StorageFolder storageFolder = null;

            try
            {
                storageFolder = await this.Originator.GetFolderAsync(name);
            }
            catch (Exception)
            {
                if (!createIfNotExists)
                {
                    throw;
                }
            }

            if (createIfNotExists && storageFolder == null)
            {
                return await this.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
            }

            return new StorageFolder(storageFolder);
        }

        /// <inheritdoc />
        public async Task<IStorageItem> GetItemAsync(string name)
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get an item from a folder that does not exist.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Windows.Storage.IStorageItem storageItem = await this.Originator.GetItemAsync(name);

            if (storageItem == null)
            {
                throw new StorageItemNotFoundException(name, "The item could not be found in the folder.");
            }

            if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.None))
            {
                throw new InvalidOperationException("The item is not a valid storage type.");
            }

            if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.File))
            {
                Windows.Storage.StorageFile storageFile = storageItem as Windows.Storage.StorageFile;
                return new StorageFile(storageFile);
            }

            if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.Folder))
            {
                Windows.Storage.StorageFolder storageFolder = storageItem as Windows.Storage.StorageFolder;
                return new StorageFolder(storageFolder);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<IStorageFile>> GetFilesAsync()
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get files from a folder that does not exist.");
            }

            IReadOnlyList<Windows.Storage.StorageFile> storageFiles = await this.Originator.GetFilesAsync();
            return storageFiles.Select(storageFile => new StorageFile(storageFile)).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<IStorageFolder>> GetFoldersAsync()
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get folders from a folder that does not exist.");
            }

            IReadOnlyList<Windows.Storage.StorageFolder> storageFolders = await this.Originator.GetFoldersAsync();
            return storageFolders.Select(storageFolder => new StorageFolder(storageFolder)).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get items from a folder that does not exist.");
            }

            IReadOnlyList<Windows.Storage.IStorageItem> storageItems = await this.Originator.GetItemsAsync();

            List<IStorageItem> result = new List<IStorageItem>();

            foreach (Windows.Storage.IStorageItem storageItem in storageItems)
            {
                if (storageItem == null || storageItem.IsOfType(Windows.Storage.StorageItemTypes.None))
                {
                    continue;
                }

                if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.File))
                {
                    Windows.Storage.StorageFile storageFile = storageItem as Windows.Storage.StorageFile;
                    result.Add(new StorageFile(storageFile));
                }
                else if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.Folder))
                {
                    Windows.Storage.StorageFolder storageFolder = storageItem as Windows.Storage.StorageFolder;
                    result.Add(new StorageFolder(storageFolder));
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<IStorageItem>> GetItemsAsync(int startIndex, int maxItemsToRetrieve)
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get items from a folder that does not exist.");
            }

            IReadOnlyList<Windows.Storage.IStorageItem> storageItems = await this.Originator.GetItemsAsync((uint)startIndex, (uint)maxItemsToRetrieve);

            List<IStorageItem> result = new List<IStorageItem>();

            foreach (Windows.Storage.IStorageItem storageItem in storageItems)
            {
                if (storageItem == null || storageItem.IsOfType(Windows.Storage.StorageItemTypes.None))
                {
                    continue;
                }

                if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.File))
                {
                    Windows.Storage.StorageFile storageFile = storageItem as Windows.Storage.StorageFile;
                    result.Add(new StorageFile(storageFile));
                }
                else if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.Folder))
                {
                    Windows.Storage.StorageFolder storageFolder = storageItem as Windows.Storage.StorageFolder;
                    result.Add(new StorageFolder(storageFolder));
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IStorageItem> TryGetItemAsync(string name)
        {
            if (!this.Exists)
            {
                throw new StorageItemNotFoundException(
                    this.Name,
                    "Cannot get an item from a folder that does not exist.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Windows.Storage.IStorageItem storageItem = await this.Originator.TryGetItemAsync(name);
            if (storageItem == null)
            {
                return null;
            }

            if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.File))
            {
                Windows.Storage.StorageFile storageFile = storageItem as Windows.Storage.StorageFile;
                return new StorageFile(storageFile);
            }

            if (storageItem.IsOfType(Windows.Storage.StorageItemTypes.Folder))
            {
                Windows.Storage.StorageFolder storageFolder = storageItem as Windows.Storage.StorageFolder;
                return new StorageFolder(storageFolder);
            }

            return null;
        }

        /// <summary>
        /// Gets the folder that has the specified absolute path in the file system.
        /// </summary>
        /// <param name="path">
        /// The absolute path in the file system (not the Uri) of the folder to get.
        /// </param>
        /// <returns>
        /// When this method completes successfully, it returns an IAppFolder that represents the specified folder.
        /// </returns>
        public static async Task<IStorageFolder> GetFolderFromPathAsync(string path)
        {
            Windows.Storage.StorageFolder pathFolder;

            try
            {
                pathFolder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(path);
            }
            catch (Exception)
            {
                pathFolder = null;
            }

            if (pathFolder == null)
            {
                return null;
            }

            StorageFolder resultFolder = new StorageFolder(pathFolder);

            return resultFolder;
        }
    }
}
