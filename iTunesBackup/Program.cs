//
//   iTunes library playlist file export script.
//
//   https://github.com/Apophenic
//   
//   Copyright (c) 2015 Justin Dayer (jdayer9@gmail.com)
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.

using PlistCS;
using System;
using System.Collections.Generic;
using System.IO;

namespace iTunesBackup
{
    /// <summary>
    /// Copys all files (w/ option to overwrite) from selected iTunes
    /// playlist and saves to selected directory.
    /// 
    /// iTunes stores every track entry with an integer key.
    /// Playlists are a list of these keys.
    /// So, we read the file, read the keys in the playlist,
    /// get the file path for every key in the playlist,
    /// and save to the new directory.
    /// 
    /// Warning: the casts are REAL.
    /// </summary>
    class Program
    {
        private static Dictionary<string, Object> itunesDict;
        private static string musicFolder;

        public static void Main(string[] args)
        {
            Console.WriteLine("iTunes Xml path:");
            string libPath = Console.ReadLine();
            Console.WriteLine("\nPlaylist name to copy files from:");
            string playlistName = Console.ReadLine();
            Console.WriteLine("\nDestination path:");
            string dest = Console.ReadLine() + "\\";
            Console.WriteLine("\nOverwrite duplicate files?(y/n):");
            bool overwrite = false;
            if (Console.ReadLine() == "y")
            {
                overwrite = true;
            }

            Console.WriteLine("Reading library file...");
            if(!File.Exists(libPath))
            {
                Console.WriteLine("Library file doesn't exist, press any key to exit");
                Console.ReadKey();
                return;
            }

            itunesDict = (Dictionary<string, Object>) Plist.readPlist(libPath);   //iTunes xml structure is preserved, but types are lost; hence casting
            musicFolder = convertUriToWindowsPath((string) itunesDict["Music Folder"] + "Music/");

            List<int> trackIDs = getTrackIdsFromPlaylist(playlistName);
            if (trackIDs.Count == 0)
            {
                Console.WriteLine("Playlist Name doesn't exist / is empty; press any key to exit");
                Console.ReadKey();
                return;
            }

            List<string> paths = getTrackPathsFromIds(trackIDs);
            copyFilesToDest(dest, paths, overwrite);
            Console.ReadKey();
        }

        /// <summary>
        /// Get a list of the track ID keys from the supplied playlist.
        /// </summary>
        /// <param name="playlistName">Name of the playlist.</param>
        /// <returns></returns>
        private static List<int> getTrackIdsFromPlaylist(string playlistName)
        {
            Console.WriteLine("Getting Track IDs from playlist");

            var trackIDs = new List<int>();
            var playlists = (List<Object>) itunesDict["Playlists"];
            foreach (Object playlist in playlists)
            {
                var field = (Dictionary<string, Object>) playlist;
                if ((string) field["Name"] == playlistName)
                {
                    var playlistItems = (List<Object>) field["Playlist Items"];
                    foreach (Object playlistItem in playlistItems)
                    {
                        var trackid = (Dictionary<string, Object>) playlistItem;
                        trackIDs.Add((int) trackid["Track ID"]);
                    }
                    break;
                }
            }
            return trackIDs;
        }

        /// <summary>
        /// Reads the file path for the supplied list of
        /// track ID keys.
        /// </summary>
        /// <param name="trackIds">The track ids.</param>
        /// <returns></returns>
        private static List<string> getTrackPathsFromIds(List<int> trackIds)
        {
            Console.WriteLine("Fetching Track paths from library");

            var paths = new List<string>();
            var tracksDict = (Dictionary<string, Object>) itunesDict["Tracks"];
            foreach(KeyValuePair<string, Object> entry in tracksDict)
            {
                int key = Int32.Parse(entry.Key);
                if(trackIds.Contains(key))
                {
                    var trackEntry = (Dictionary<string, Object>) entry.Value;
                    string location = convertUriToWindowsPath((string) trackEntry["Location"]);
                    paths.Add(location);
                    trackIds.Remove(key);
                }
            }
            return paths;
        }

        /// <summary>
        /// Copies the files to dest.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="isOverwrite">if set to <c>true</c> [is overwrite].</param>
        private static void copyFilesToDest(string dest, List<string> paths, bool isOverwrite)
        {
            Console.WriteLine("Copying...");

            int counter = 0;
            foreach(string path in paths)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("FAILED: " + path);
                    continue;
                }

                string destPath = dest + path.Replace(musicFolder, "");
                Directory.CreateDirectory(Directory.GetParent(destPath).ToString());
                File.Copy(path, destPath, isOverwrite);
                counter++;
            }
            Console.WriteLine(counter + " of " + paths.Count + " files copied successfully \nPress any key to exit.");
        }

        /// <summary>
        /// Converts the URI to acceptable windows file path.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        private static string convertUriToWindowsPath(string uri)
        {
            uri = uri.Replace("file://localhost/", "");
            uri = uri.Replace("&#38;", "&");
            return Uri.UnescapeDataString(uri);
        }
    }
}