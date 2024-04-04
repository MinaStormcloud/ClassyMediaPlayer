using System;
using Android.Content;
using Android.OS;
using Android.Database;
using Android.Provider;
using Java.IO;

namespace ClassyMediaPlayer
{
    public class SelectedFilePath
    {
        public static string GetActualPathFromFile(Context context, Android.Net.Uri uri)
        {
            bool IsKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

            if (IsKitKat && DocumentsContract.IsDocumentUri(context.ApplicationContext, uri))
            {                
                if (IsExternalStorageDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }

                    else
                    {                        
                        string path = null;
                        File[] dirs = context.ApplicationContext.GetExternalFilesDirs(null);
                        if (dirs != null && Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState))
                        {
                            path = dirs[dirs.Length - 1].AbsolutePath;
                            path = path.Substring(0, path.IndexOf("Android"));
                        }
                        return path + "/" + split[1];
                    }
                }
                
                else if (IsDownloadsDocument(uri))
                {
                    string id = DocumentsContract.GetDocumentId(uri);

                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                                    Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                    return getDataColumn(context.ApplicationContext, contentUri, null, null);
                }
                
                else if (IsMediaDocument(uri))
                {
                    String docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    String[] split = docId.Split(chars);

                    String type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    String selection = "_id=?";
                    String[] selectionArgs = new String[] { split[1] };

                    return getDataColumn(context.ApplicationContext, contentUri, selection, selectionArgs);
                }
            }
            
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {                
                if (IsGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                return getDataColumn(context.ApplicationContext, uri, null, null);
            }
            
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        public static String getDataColumn(Context context, Android.Net.Uri uri, String selection, String[] selectionArgs)
        {
            ICursor cursor = null;
            String column = "_data";
            String[] projection = { column };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }
        
        public static bool IsExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }
        
        public static bool IsDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }
        
        public static bool IsMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }
        
        public static bool IsGooglePhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority);
        }
    }
}