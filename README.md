# ReachyTeleoperationXR

Unity-based application that allows to control a [Reachy robot](https://www.pollen-robotics.com/reachy/) (version 2021+), or a [virtual one](https://github.com/pollen-robotics/reachy2021-unity-package), with a VR headset.

## Requirements

The app should run with any VR headset compatible with Unity. It has been tested with the Oculus Quest 2, and the Valve Index. The Oculus Quest 2 can be used in standalone mode or with the Oculus link.

For any custom development we recommend to use Unity LTS 2020.3. Versions above 2020.3 will not work due to compatibility issues with the gRPC library.

## Installation

### Using a release build [recommended]

For the Oculus Quest 2, you may ask to join the list of beta users to install the app directly from the app store. Please contact us on our [discord channel](https://discord.com/channels/519098054377340948/991321051835404409)!

For Windows and Android platforms, the simplest way to use the application is to download a [release here](https://github.com/pollen-robotics/ReachyTeleoperation/releases) (*Assets* section). The Windows package is a zip file that contains the .exe to run. Your VR headset should be plugged in and ready to be used. The Android package is an *.apk that should be installed on your device.


### From source

Clone the **main** branch of the repo. Make sure that git lfs is enabled.
```
git clone -b main https://github.com/pollen-robotics/ReachyTeleoperation.git
```

Download the [grpc_unity_package](https://packages.grpc.io/archive/2022/04/67538122780f8a081c774b66884289335c290cbe-f15a2c1c-582b-4c51-acf2-ab6d711d2c59/csharp/grpc_unity_package.2.47.0-dev202204190851.zip) from the [gRPC daily builds](https://packages.grpc.io/archive/2022/04/67538122780f8a081c774b66884289335c290cbe-f15a2c1c-582b-4c51-acf2-ab6d711d2c59/index.xml). Unzip it in the **Assets/Plugins** folder. You can now run the app from Unity or build an executable for your platform.

## Usage

Teleoperating a robot takes place in three basic steps:

### 1. Connect to a robot

The first step is to select the robot you want to control. For that you'll need the ip address of the robot (please refer to the [robot documentation](https://docs.pollen-robotics.com/dashboard/introduction/first-connection/) for the first connection), or the ip address of the computer running the Unity simulator. Press *new robot button* and add your robot.

![alt text](Docs/img/connection.jpg)

Note that there is a built-in virtual robot for local testing of the application.

### 2. Get ready for the teleoperation

This step checks that the connection to the (virtual) robot is fine, and allows to set various parameters. Side menus (status, help, settings) can be opened by clicking on the related icons.

Get familiar with the controls of the robot. Press X to play with the emotions or change the gripper grasping mode. Once you are ready, press the *Ready* button and button A to take control of the robot.

![alt text](Docs/img/mirror.jpg)

### 3. Take control!

You are now controlling Reachy! Press and hold A to return to the previous step. 

![alt text](Docs/img/teleop.jpg)

## Issues / Contributions

If you have any problem, you can create an issue or chat with us on our [discord server](https://discord.com/channels/519098054377340948/991321051835404409). 
Finally, anyone is welcome to contribute to this project. Feel free to checkout the **develop** branch and to pull request any modifications.