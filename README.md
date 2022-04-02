# ThreeWorkTool

ThreeWorkTool is a GUI Tool I made to make it easier and less time consuming to extract, rename, and insert files into Ultimate Marvel vs Capcom 3's Archive files.

It also supports texture previews and DDS texture imports now.

Expect weirdness and glitches. They will be fixed over time.

This is built targeting .NET Framework 4.7 if that concerns anyone.

Thanks to TGE and smb123w64gb for assistance and giving me the resources that massively help made this a thing.

By Eternal Yoshi

Current Changelog:

V5.0

ANIMATION IMPORTING IS NOT READY YET, BUT MANY THINGS HAVE BEEN FIXED/CHANGE ENOUGH TO WARRANT ONE MORE UPDATE.

- A few more M3a parameters are labeled and documented.
- Fixed an issue with replacing M3a files resulting in parameters not being properly updated.
- Transparent Texture Backgrounds are now Checkered Board a la Photoshop, no more Magneta.
- Has support for Manifests for file organization when saving files.
- Can now add folders to arc files and other folders.
- Added .mis Mission file support.
- Rewrote .MSD Reading and Writing and make proper .MSD files & small updates to definitions in MSDTable.cfg.
- Now has implemented Recently Used Files list.
- Can now make UMVC3 Compatible .arc files from scratch.
- Labelled LAB Color format textures.
- Upgraded Import Multiple Textures to Import Multiple Items Into Folder.
- Fixed an issue resulting in arc files with srqr files failing to save.
- Upgraded About Form to include direct link to project's Github to show users where to check for updates.
- Small improvements to error checking and logging. This also means:
- Fixed unhandled exception when replacing & importing textures with malinformed or unsupported .DDS file.
- Fixed unhandled exception when attempting to save over an arc file or export and replace a file in use by another process. 

V4.0
- Fixed an issue resulting in Cube Maps crashing on Export/Export All. Can only export them as raw .tex files.
- Fixed an issue that happens when replacing Normal Maps.
- Added support for importing multiple raw .tex files simultaneously.
- No longer will stop you from having multiple Sub-Materials with the same texture reference.
- Fixed the formula used for "Change Texture Reference Via rename" so it works properly now.

V3.5
- Wrote and Rewrote Partial Model and Material Support - Not all information is shown but some of it is in Read Only Form.
- Added a touch more information on LMT files.
- Made better attempt to log exceptions in log.txt.
- Rewrote and Refactored how texture previews and stored and made. This improves stability and fixes the random but rare crash 
when selecting a Texture node.
- Replace and Insert methods updated to use superior and more stable methods.
- Fixed an issue preventing .tex files from being replaced as raw .tex files.
- Updated notes and advice tab.
- Added Export All option for Arcs.
- Fixed an issue with Export All not exporting defined extensions correctly.
- Replacing of LMTs, Materials, and Models now update the child nodes to accurately reflect the new file.

V3.1
- Fixed an issue resulting in textures failling to load in game.
- Files/Nodes from Archives that are in the same directory are no longer ordered alphabetically.
- Added the option to reorder Files in the same directory manually.
- Keyboard Shortcuts! Actions that have shortcuts will have them displayed next to the command.
- Added support for .cst files, aka ChainList files.

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
- Normal Maps in Marvel 3 have their Red and Alpha channels swapped and the Green Channel inverted. If you make or edit these normal maps, keep this in mind.
- Portraits have to be saved DXT5 in your image editing software to work, no need to save it as a DXT1 and then hex edit it to DXT5. In the TexConverter Dialog, select the option that says ~~Problematic Portrait Texture or whatever~~ DXT5 when importing. Make sure it has an alpha channel in your image editing software before you save it.
This program was designed around DDS files generated from the newer NVIDA Photoshop DDS Plugin found here: https://developer.nvidia.com/2020.1.1/nvidia_texture_tools_exporter_photoshop_plugin

MSD files must be viewed and edited by Right Clicking/Selecting Edit and selecting Preview/Edit. The encoding is unknown(it is NOT ASCII)
and only English Alphabet characters and accents used in French and Spanish are supported. The Line break denotations are there
for a reason; don't remove them unless you know what you are doing. Edit at your own risk!

- LMT Entry names start from 0, not 1. Those using the AnmChr editor have to be aware that the IDs here are written in decimal and not in hex, often going up to 255.
- To get this to work I had to redefine the .m3a format. Any .m3a files made from other methods will NOT be compatible with this program.
- While you can't make new entries in an LMT(yet), you can replace a blank entry in the LMT file just like you would with a populated entry.
- Replacing/injecting m3a entries means the respective LMT file is going to be rebuilt so replacing will take an extra second or two.

Remember that you can export textures as .DDS files. You can select the extension between .tex(which is by default), .dds, and .png via the Export option's Dialog.

You can now reorder nodes in the same directory. You can do so by right clicking and selecting move up or move down, or just press CTRL + UP/CTRL + DOWN.

Chain List files control what physics files(.chn and .ccl)are used on the character. Removing an entry will disable that physics file from being used.
