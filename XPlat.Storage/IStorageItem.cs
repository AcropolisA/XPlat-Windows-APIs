﻿namespace XPlat.Storage
{
    using System;
    using System.Threading.Tasks;

    using XPlat.Storage.FileProperties;

    /// <summary>
    /// Manipulates storage items (files and folders) and their contents, and provides information about them.
    /// </summary>
    public interface IStorageItem
    {
        /// <summary>
        /// Renames the current item.
        /// </summary>
        /// <param name="desiredName">
        /// The desired, new name of the item.
        /// </param>
        /// <returns>
        /// An object that is used to manage the asynchronous operation.
        /// </returns>
        Task RenameAsync(string desiredName);

        /// <summary>
        /// Renames the current item. This method also specifies what to do if an existing item in the current item's location has the same name.
        /// </summary>
        /// <param name="desiredName">
        /// The desired, new name of the current item. If there is an existing item in the current item's location that already has the specified desiredName, the specified NameCollisionOption determines how Windows responds to the conflict.
        /// </param>
        /// <param name="option">
        /// The enum value that determines how Windows responds if the desiredName is the same as the name of an existing item in the current item's location.
        /// </param>
        /// <returns>
        /// An object that is used to manage the asynchronous operation.
        /// </returns>
        Task RenameAsync(string desiredName, NameCollisionOption option);

        /// <summary>
        /// Deletes the current item.
        /// </summary>
        /// <returns>
        /// An object that is used to manage the asynchronous operation.
        /// </returns>
        Task DeleteAsync();

        /// <summary>
        /// Gets the basic properties of the current folder or file group.
        /// </summary>
        /// <returns>
        /// When this method completes successfully, it returns the basic properties of the current folder or file group as an IBasicProperties object.
        /// </returns>
        Task<IBasicProperties> GetBasicPropertiesAsync();

        /// <summary>
        /// Determines whether the current IStorageItem matches the specified StorageItemTypes value.
        /// </summary>
        /// <param name="type">
        /// The value to match against.
        /// </param>
        /// <returns>
        /// True if the IStorageItem matches the specified value; otherwise false.
        /// </returns>
        bool IsOfType(StorageItemTypes type);
        
        /// <summary>
        /// Gets the attributes of the current storage item.
        /// </summary>
        FileAttributes Attributes { get; }

        /// <summary>
        /// Gets the date the item was created.
        /// </summary>
        DateTime DateCreated { get; }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full path to the item.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets a value indicating whether the item exists.
        /// </summary>
        bool Exists { get; }
    }
}