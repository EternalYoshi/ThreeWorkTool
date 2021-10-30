# ThreeWorkTool

ThreeWorkTool is a GUI Tool I made to make it easier and less time consuming to extract, rename, and insert files into Ultimate Marvel vs Capcom 3's Archive files.

It also supports texture previews and DDS texture imports now.

Expect weirdness and glitches. They will be fixed over time.

This is built targeting .NET Framework 4.7 if that concerns anyone.

Thanks to TGE and smb123w64gb for assistance and giving me the resources that massively help made this a thing.

By Eternal Yoshi

Current Changelog:
V0.30

- Reorganized and Rewrote file reading and file writing coding of all types of files to be more stable, optimal, and quicker.
- Improved error checking:
- Now detects invalid arc files that are not in the inverse endian.
- Fixed crashing when trying to open an invalid or unsupported Arc.
- Fixed crash if Close is selected without opening a file.
- Fixed an issue causing replaced DDS files to use the newly replaced filename rather than the original.
- Used the Actual complete Hash List and fixed typos with .ean and .xsew files.
- Export All Option added(Folders only)
- Partial MSD Support(Only English Alphabet characters and some accents supported); has a new form that must be loaded to view or edit.
- The FileAmount parameter on Arc Files actually shows the amount of files inside the .arc file.
- Fixed an issue with saving arc files with over 300 files that would cause data to overwrite and corrupt the arc.
- Added the selected Item's Context Menu Options to the Edit button dynamically; anything you have to right click to do can be done in the Edit Menu.
- LMT support that still needs more testing, right now supports replacing and exporting individual entries in the .LMT files.

V2.1
- Fixed an issue that prevented Arc files with more than 255 files from being read and saved correctly.

V2.0
- Added a near complete resource hashes in the cfg thanks to TGE.
- Added texture preview support via Pfim
- Added support for importing DDS files as textures, along with setting the texture type to the more well known and used formats. (Cube Maps not supported)
- Added a dialog for replacing and importing textures via DDS files.
- Added support for .lpr files, aka ResourcePathlists
- Close command closes the currently open files and nulls any variables from that previously open file
- Small improvements to the log that's logged as the program runs
- Adjusted the way the PropertyGrid is updated, allowing it to be updated on Up and Down arrow presses as well as clicking nodes

--------------------------|Notes and Advice|--------------------------

- LRP files are formatted the way they are for a reason. If you wish to add a file, follow the syntax and do not make typos in the extension or you will have problems. It's always path then filename then extension.

- Transparency is expressed as a very obvious Magenta background. This can make some semi-transparent textures hard to see in the preview.

DXT1 and DXT5 have differing compression formulas meaning you can't interchange them or you will get corrupted pixels in game or just straight up crashing.
- DXT1 files can only be imported as regular textures without transparency and specular textures. If you want to save it as other formats, you have to add an alpha channel in your image editing software(if it doesn't exist already) and save it as a DXT5.
- DXT5 can't be saved as normal textures without transparency and specular textures. Just save it as a DXT1 in your image editing software.
- Normal Maps in Marvel 3 have their Red and Alpha channels swapped. If you make or edit these normal maps, keep this in mind.
- Portraits have to be saved DXT5 in your image editing software to work, no need to save it as a DXT1 and then hex edit it to DXT5. In the TexConverter Dialog, select the option that says Problematic Portrait Texture or whatever when importing.
- This program was designed around DDS files generated from the newer NVIDA Photoshop DDS Plugin found here: https://developer.nvidia.com/2020.1.1/nvidia_texture_tools_exporter_photoshop_plugin
- MSD files must be viewed and edited by Right Clicking/Selecting Edit and selecting Preview/Edit. The encoding is unknown(it is NOT ASCII)
and only English Alphabet characters and accents used in French and Spanish are supported. The Line break denotations are there
for a reason; don't remove them unless you know what you are doing. Edit at your own risk!

- LMT Entry names start from 0, not 1.

- To get this to work I had to redefine the .m3a format. Any .m3a files made from other methods will NOT be compatible with this program.
