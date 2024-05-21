# ThreeWorkTool

ThreeWorkTool is a GUI Tool I made to make it easier and less time consuming to extract, replace, and overall manage files inside Ultimate Marvel vs Capcom 3's Archive files.

It supports DDS texture imports, character animation keyframe import/exports, and more.

This is built targeting .NET Framework 4.7 if that concerns anyone.

Thanks to TGE, Gneiss, and smb123w64gb for assistance and giving me the resources that massively help made this a thing.

By Eternal Yoshi

Current Changelog:

V0.76

- Made adjustment to saving arc files to avoid having redundant data when the new file is smaller.
- Fixed an issue regarding scale upon re-reimported animations.
- Added some basic data on Track type 224; not complete yet.
Blender Plugin updated to V0.8.
- Rewrote Export error handler for abnormal fcurve data so it works properly, ignores the unusable data, and continues as normal.


V0.75
- Added forgotten export code for BaseAct files.
- Made sure that certain subforms focus back on main ThreeWorkTool form when closed.
- Adjusted Track Editor Labels to be more accurate; WXYZ is now displayed as XYZW.
- Now has "Extract All Animations" option to extract all animations from a lmt into individual .yml files.
- Blender Plugin updated to V0.7. Can now:
-Can now has the option to import multiple animations into individual actions.
- Now has the option to save each imported animation as a fake user.

v0.72
-Fixed Missing Absolute Transformation coding in Blender Script. That's it.

V0.71

- Fixed an issue preventing Scale Keyframes from being exported.
- Fixed an issue resulting in the plugin's Export failing when the Blender file's scene name isn't "Scene".
- Suppressed Bogus Track size/pointer errors.
- Adjusted import filters so a yet to be implemented extension is removed.

V0.7
Animation Importing and Exporting are Online!
- Can import and export individual animations' Keyframes via cusotmized .yml fomrat.
- Added Notifications when Animation Track's buffer size is invalid; you won't get keyframe data from these.
- Included Blender Python plugin (3.0 and up) for importing animation ymls generated from ThreeWorkTool.
- EndFramesAdditiveScenePosition(Tentative name) can now be edited. 
- Fixed issue with undocumented texture types not exporting. They'll only export as raw .tex files.
- Fixed issue that causes mass importing moveset files to have improper hiearchy.
- Re-labelled ReferenceData floats in the Track Editor.
- Adjusted error checking in Track Editor so "Bogus Value" error doesn't repeat when trying to close the form.
- Enforced a minimum size for .gem files to prevent crashing with .gem files with only a small amount of data to them.
- Updated CFG with real extension names provided by Andoryuuta.
- Quality of Life Shortcut Changes.
- Added the ability to add entries to .slo files.
- Now Chceks the Recent Release Version to notify if user is out of date.

V0.63d
- Updated LMT and M3a definitions.
- New Track Editor which lets you edit the Reference Data, Track Type, and bone ID used.
- Can now rebuild LMT using the current .m3a files. Use this to apply certain changes like from the one below.
- Can now set the Reuse Animation Flag in the Property Grid. Is applied to the lmt when the Rebuild LMT command is used.
- Fixed an issue resulting in incorrect event buffer pointers in .m3a files.
- Small update to Model Entry definition.
- Fixed visual issue with replacing .m3a entries that resulted in the name being changed.
- Small Addition to Notes and Advice Form. Read it if you haven't.
NOTE: This above does mean that loose .m3a files exported/created from previous versions and methods are no longer valid without hex editing.

V0.63c
- Replace All feature implemented for files on Arc and Folder nodes.
- Limited Most Recently Used list to 12 items maximum to prevent performance hit when loading the program.
- Updated Notes and Advice.
- Fixed an issue with the Manifest Tool's Refresh File list button including nodes that were not meant to be included.
- Added basic support for various file formats that have matching names for the sake of disambiguation for the Replace All feature. They shall be expanded upon another time.

V0.63d
- Updated LMT and M3a definitions.
- New Track Editor which lets you edit the Reference Data, Track Type, BufferType, and bone ID used.
- Can now rebuild LMT using the current .m3a files. Use this to apply certain changes like from the one below.
- Can now set the Reuse Animation Flag in the Property Grid. Is applied to the lmt when the Rebuild LMT command is used.
- Fixed an issue resulting in incorrect event buffer pointers in .m3a files.
- Small update to Model Entry definition.
- Fixed visual issue with replacing .m3a entries that resulted in the name being changed.
- Small Addition to Notes and Advice Form. Read it if you haven't.
NOTE: This above does mean that loose .m3a files exported/created from previous versions and methods are no longer valid without hex editing.

V0.63b
- Fixed an issue where .cst files would not be built properly and cause abnormalities in game.
- ChainList format redefined, the text box now has numerical values for each Chain references.
- Fixed an issue where drag and drop opening a file would not be counted as a open file, which prevented saving.
- Replace Text now supports ChainList and Gem Entries.
- Fixed an issue where imported ShotLists would not be recognized as such and could not have been edited.

V0.63
- Fixed an issue where .msd files weren't given any extension when using Export All.
- STQR Support Implemented.
- Material names in Model Entries can be edited.
- Model and Material files are more defined overall.
- Added the ability to export .mrl files as .YML files. Will add more functionality soon.
- Adjusted archive_filetypes.cfg to replace double underscore with double colons to be more accurate to the game.
- Added .cfg with the game's known default Material names.

V6.1

- Fixed an issue that occurred when deleting folders in arc files above a certain file count.
- .fsm, .lmcm, and .fsd files are now given the correct extension.
- Made sure the log is not written in the same directory as the open .arc file when using ThreeWorkTool via double clicking an arc file.



V0.6

- StgObjLayout file support implemented.
- Fixed an issue with importing .riff files that resulted in incorrect pathing.
- Checks certain Files with Hash Extensions and saves it with the proper extension when saving, allowing support of importing and exporting certain files's Hashcodes as extensions.
- New Folder Sorting Algorithm; Organizes files in stages correctly now; Packed-in moveset files not yet tested
- Fixed issue resulting in Textbox not updating properly.
- Fixed an issue resulting in the Replace All command not being available after replacing a MaterialEntry.
- Implemented drag and drop functionality to open arc files, and ONLY arc files.
- Arcs now open when The tool is set to the default app.
- Adjusted coding to look for necessary .cfg files so it looks for them in the same directory as the exe itself, which should fix some issues with finding the things.
- All other changes from Bimonthly builds implemented.


V0.51

- Added Jump To Line Function to the MSD Viewer/Editor.
- Improved M3a Error Checking so the program reports the problematic entry and continues to read the rest of the arc and file.
- Fixed an issue with Manifest text separation.

V0.5

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

V0.4
- Fixed an issue resulting in Cube Maps crashing on Export/Export All. Can only export them as raw .tex files.
- Fixed an issue that happens when replacing Normal Maps.
- Added support for importing multiple raw .tex files simultaneously.
- No longer will stop you from having multiple Sub-Materials with the same texture reference.
- Fixed the formula used for "Change Texture Reference Via rename" so it works properly now.

V0.35
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

V0.31
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

V0.21
- Fixed an issue that prevented Arc files with more than 255 files from being read and saved correctly.

V0.2
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
