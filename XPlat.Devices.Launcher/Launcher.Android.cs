﻿#if __ANDROID__
namespace XPlat.Device
{
    using System;
    using System.Threading.Tasks;
    using Android.Content;
    using XPlat.Storage;

    /// <summary>Starts the default app associated with the specified file or URI.</summary>
    public class Launcher : ILauncher
    {
        public Launcher()
        {
        }

        public Launcher(Context context)
        {
            this.Context = context;
        }

        /// <summary>Gets or sets the Android context to be used for launching an Activity with.</summary>
        public Context Context { get; set; }

        /// <summary>Launches a file explorer and displays the contents of the specified folder.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="folder">The folder to display in a file explorer.</param>
        public Task<bool> LaunchFolderAsync(IStorageFolder folder)
        {
            return Task.Run(
                () =>
                {
                    bool result = false;

                    try
                    {
                        if (folder != null && folder.Exists)
                        {
                            Intent intent = new Intent(Intent.ActionView);
                            intent.SetDataAndType(Android.Net.Uri.Parse(folder.Path), "*/*");
                            intent.SetFlags(ActivityFlags.ClearTop);
                            this.Context.StartActivity(intent);
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    return result;
                });
        }

        public Task<bool> LaunchUriAsync(Uri uri)
        {
            return Task.Run(
                () =>
                {
                    bool result = false;

                    try
                    {
                        Intent intent = new Intent(Intent.ActionView);
                        intent.SetData(Android.Net.Uri.Parse(uri.ToString()));
                        this.Context.StartActivity(intent);
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    return result;
                });
        }

        public Task<LaunchQuerySupportStatus> QueryUriSupportAsync(Uri uri)
        {
            return Task.Run(
                () =>
                {
                    LaunchQuerySupportStatus result = LaunchQuerySupportStatus.Unknown;

                    try
                    {
                        Intent intent = new Intent(Intent.ActionRun);
                        intent.SetData(Android.Net.Uri.Parse(uri.ToString()));
                        result = intent.ResolveActivity(this.Context.PackageManager) != null
                            ? LaunchQuerySupportStatus.Available
                            : LaunchQuerySupportStatus.AppNotInstalled;
                    }
                    catch (ActivityNotFoundException anfe)
                    {
                        result = LaunchQuerySupportStatus.NotSupported;
                        System.Diagnostics.Debug.WriteLine(anfe.ToString());
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    return result;
                });
        }

        /// <summary>Starts the default app associated with the specified file.</summary>
        /// <returns>The launch operation.</returns>
        /// <param name="file">The file.</param>
        public Task<bool> LaunchFileAsync(IStorageFile file)
        {
            return Task.Run(
                () =>
                {
                    bool result = false;

                    try
                    {
                        if (file != null && file.Exists)
                        {
                            string fileContentType = string.IsNullOrWhiteSpace(file.ContentType)
                                ? "*/*"
                                : file.ContentType;

                            Intent intent = new Intent(Intent.ActionView);
                            intent.SetDataAndType(Android.Net.Uri.Parse(file.Path), fileContentType);
                            intent.SetFlags(ActivityFlags.ClearTop);
                            this.Context.StartActivity(intent);
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    return result;
                });
        }
    }
}
#endif