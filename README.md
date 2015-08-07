# iTunes Playlist Backup
------------------------
This script will copy all files contained in the chosen iTunes playlist into the chosen directory.

Input required:
* iTunes Xml path -> The file path to your library xml file. Should be named either _iTunes Library.xml_ or _iTunes
Music Library.xml_
* Playlist name to copy files from -> The literal playlist name to copy files from. This input is case sensitive.
* Destination path -> The directory to copy files to.
* Overwrite duplicate files?(y/n) -> Will overwrite duplicate files in the destination path. Default behavior is not to
 overwrite.

__Folder structure will be preserved within the destination path, following the Music folder.__
For example, if you're copying a track in a playlist with a file path of ``C:\Users\YourName\Music\Artist\Album\Song
.mp3`` to a destination path of ``E:\NewDir``, the resulting file path will be ``E:\NewDir\Artist\Album\Song.mp3``