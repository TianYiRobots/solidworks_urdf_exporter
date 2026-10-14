# SolidWorks to URDF Exporter

Authored and maintained by [Stephen Brawner](brawner@gmail.com). Past supporters include [PickNik Consulting](https://picknik.ai), Verb Surgical, Open Robotics, and Willow Garage. 

## Modifications from upstream (1.6.1)

This fork fixes issues that made the exported URDF inconsistent with SolidWorks mass properties:

1. **Inertia tensor matches SolidWorks mass properties** — The original `ComputeInertialProperties` aggregated bodies via `AddBodies`, which gave wrong link inertia for assemblies. It now reads each leaf part's mass properties directly from its document and composes link inertia using the parallel-axis theorem, so the exported `inertial` block matches what SolidWorks shows in `Mass Properties`.
2. **Inertia product sign** — `Inertia.SetMomentMatrix` had a double negation on the off-diagonal terms (Ixy/Ixz/Iyz). The extra minus sign is removed.
3. **Joint axis direction** — `GetRefAxis` and `LocalizeJoint` now normalize the axis so the first non-zero component is positive. Previously an axis could come out as `[-1,0,0]`, `[0,-1,0]` or `[0,0,-1]`, which flipped the joint's positive direction and broke joint limits.

## Latest Release
https://github.com/ros/solidworks_urdf_exporter/releases/tag/1.6.0

## SolidWorks Version Requirements

1. The minimum required version of SolidWorks for use with this add-in is 2018 Service Pack 5. SolidWorks 2017 or earlier may work. See [this issue](https://github.com/ros/solidworks_urdf_exporter/issues/73).

## Usage

See the [ROS Wiki](http://wiki.ros.org/sw_urdf_exporter) and associated [tutorials](http://wiki.ros.org/sw_urdf_exporter/Tutorials).

## Development

1. Install Visual Studio 2017
1. Install .NET desktop development
    1. From Visual Studio: `Tools > Get Tools and Features...`
    1. Check `.NET desktop development` package
    1. Select `Modify`
1. Launch Visual Studio with admin privileges. Right click and select `Run as Administrator`
1. Open `sw2urdf/SW2URDF.sln`  
1. Enable Debugging
    1. Right click `SW2URDF` in the Solution Explorer
    1. Click the `Debug` Tab
    1. Ensure `Configuration:` is set to `Debug`
    1. Ensure `Start external program:` is pointing to the SolidWorks executable. For example `C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe`

### Trouble Shooting

1. `AxImp.exe` error - Check the installation of the .Net Tools. If there is no error, install the Windows 10 SDK.
1. `Resourse.resx` error - Check if `sw2urdf/SW2URDF/Resources.resx` exists and is empty. If empty, delete this file then right click the `SW2URDF` in the Solution Explorer and select `Properties`. Navigate to the Resources tab and click the button to create a new file.
