This script will copy all files contained in the chosen iTunes playlist into the chosen directory.

Input required:
-iTunes Xml path -> The file path to your library xml file. Should be named either iTunes Library.xml or iTunes Music Library.xml
-Playlist name to copy files from -> The literal playlist name to copy files from. Note this input is case sensitive.
-Destination path -> The directory to copy files to.
-Overwrite duplicate fles?(y/n) -> Will overwrite duplicate files in the destination path. Default behavior is not to overwrite.

NOTE: Folder structure will be preserved within the destination path, following the Music folder. For example, if you're copying a track in a playlist with a file path of "C:\Users\YourName\Music\Artist\Album\Song.mp3" to a destination path of "E:\NewDir", the resulting file path will be "E:\NewDir\Artist\Album\Song.mp3"