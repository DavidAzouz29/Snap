# Snap
Blockly for Unity Editor.
3rd Year Student Major Assessment. [LINK](https://github.com/DavidAzouz29/Snap)

![Alt tag](/meta/SnapperEditor.png?raw=true "Snapper Editor")

## Getting Started:
1) Navigate to "Window/Snapper/Snapper Quick Tools".
2) Navigate to "Window/Snapper/Snapper Editor".
3) Start dragging and dropping blocks. Tip: Click the "Live Updates" toggle for generated code on the fly.
4) Click "Live Updates" toggle OR "To JavaScript" when ready.
5) Click "Copy To Clipboard".
6) Click the "C#" button in "Snapper QT" OR navigate to "Window/Snapper/Save/Save as C# Script". Describe your script in the filename. Tip: use Pascal Case e.g. PlayerMovement.
7) Create a primitive by selecting your desired primitive from the dropdown. Then click on the "GameObject" button to add the latest Snapper script created to a primitive. Alternatively, right click in the hierarchy/ 3D Object/*choose your preference* e.g. Cube. Click "Add Component" and add your new script.

## 3rd Party:
- Blockly: https://github.com/google/blockly 
- ClipboardJs: https://clipboardjs.com/
- PolyGlot: https://github.com/agens-no/PolyglotUnity 
- Node Editor: https://github.com/Seneral/Node_Editor_Framework

## Notes:
- Blockly: Using Block Based Coding in your App https://youtu.be/wDJua9hgyZM
- https://docs.unrealengine.com/latest/INT/GettingStarted/FromUnity/
- C++ https://github.com/dineshLL/lisa
- https://github.com/cityindex/blockly-generator-csharp

## TODO:
- For _componentTypes, if the first letter of _componentNames[i] == prev, 
continue checking the next letter until they don't match anymore - set the folder name to that string.
- If the next _componentTypes contains folder name - add to that folder e.g. NavMeshAgent, OffMeshLink
- Click on the "Generate Categories" Button under Snap -> Generate Categories.