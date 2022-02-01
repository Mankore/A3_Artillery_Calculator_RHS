# WPF Artillery calculator for Arma 3 RHS Artillery
## About

This app allows you to calculate solutions for RHS artillery in Arma 3 by using 4-digit coordinates and altitude values. 
Was mainly developed for **2S1 (Direct Fire)** Artillery, since it has no build-in Artillery Computer and its shells have airFriction values, unlike the classic artillery in Arma 3 (which doesn't have airFriction).

Could be used for any artillery type, since the algorithm works for any airFriction values. 

## UI

![UI Preview Image](preview.png?raw=true)
![Map Window Preview Image](preview_map.png?raw=true)

## How to use

1. Select Artillery Type, Charge and Shell
2. Input Battery/Target coordinates and altitude
3. Press Compute Button

### Additional Features:

- High Arc Solutions
- Load last used coordinates from "Coordinates Log" area on the right
- Compute Range Table
- Use map to quickly calculate solutions

### Map Window Controls

**Scroll Wheel**: Zoom\
**Hold LMB**: Pan\
**RMB Click**: Reset Pan and Zoom\

**Shift + LMB Click**: Place Battery Coordinates\
**Ctrl + LMB Click**: Place Target Coordinates\
**Alt + LMB Click**: Place 500m Radius Trigger\
**Delete + Mouse Hover** over Battery/Target Coordinates: Remove Marker\

## About Maps

You can add your own maps, you need to do the following:

- Export .txt file with map coordinates/height values, with coordinates step = 10 meters
- Place it in ./coordinates folder with preferable map name
- Place mapName_opt.png in ./img folder

To export ArmA 3 Coordinates and Altitude: [Beowulf.ArmaTerrainExport](https://github.com/Keithenneu/Beowulf.ArmaTerrainExport)
To export ArmA 3 Map SVG: [diag_exportTerrainSVG](https://community.bistudio.com/wiki/diag_exportTerrainSVG)

