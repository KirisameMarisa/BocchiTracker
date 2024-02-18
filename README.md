# BocchiTracker : Game BugReport Tool
[![Build and unit tests](https://github.com/KirisameMarisa/BocchiTracker/actions/workflows/main.yml/badge.svg)](https://github.com/KirisameMarisa/BocchiTracker/actions/workflows/main.yml)
[![CodeFactor](https://www.codefactor.io/repository/github/kirisamemarisa/bocchitracker/badge)](https://www.codefactor.io/repository/github/kirisamemarisa/bocchitracker)
[![codecov](https://codecov.io/gh/KirisameMarisa/BocchiTracker/graph/badge.svg?token=KOW5YDQVN0)](https://codecov.io/gh/KirisameMarisa/BocchiTracker)
[![MIT License](http://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://github.com/KirisameMarisa/BocchiTracker/blob/master/LICENCE.txt)

# What's this BocchiTracker?

BocchiTracker is a bug reporting tool that allows you to report issues to various services. 

<img src="./Documents/Resources/BocchiTracker_System.png" width="85%">

<br>
Currently supported capture features include:

||Screenshot|Log(Application hooks)|Log(file directly copy)|Movie (Use WebRTC)|Movie (Use OBSStudio)|
|:--:|:--:|:--:|:--:|:--:|:--:|
|Unity|◯|◯|◯|◯||
|UnrealEngine|◯|◯|◯|||
|Godot|◯|☓|◯|☓||

<br>


<br>
BocchiTracker View!

<img src="./Documents/Resources/BocchiTracker.png" width="70%">

<br>

Coordinates and other cumbersome inputs are automatically entered.
<br>
Screenshots can also be taken with the push of a button!
<img src="./Documents/Resources/Redmine_Sample.png" width="70%">

<br>
Currently supported report services include:

|Redmine|Github|Slack|Discord|Gitlab|JIRA|
|:--:|:--:|:--:|:--:|:--:|:--:|
|◯|◯|◯|||||

<br>
Currently supported upload service include:

|Dropbox|Explorer|
|:--:|:--:|
|||

<br>

## Features

- It can be gathered application-specific information if you Integrate plugin with game/application.
- Custom fields for reporting services can be supported.
- Supports attaching screenshots and core dumps to the bug reports.
- Offers directory monitoring to ensure thorough ticket reporting.

## Feature Plans

- Add to feature file upload functionality.
  - Dropbox
  - Explorer (Your file servers)
- Add to feature game play capture functionality.

## Getting Started

1. [Download BocchiTracker](https://github.com/KirisameMarisa/BocchiTracker/releases)
2. Setup Config 
   - ProjectConfig - [JP](/Documents/JP/ProjectConfigurationGuide.md)/[EN](/Documents/EN/ProjectConfigurationGuide.md)
   - UserConfig - [JP](/Documents/JP/UserConfigurationGuide.md)/[EN](/Documents/EN/UserConfigurationGuide.md)

3. Integrat your project - [JP](/Documents/JP/IntegrateYourAppGuide.md)/[EN](/Documents/EN/IntegrateYourAppGuide.md)
4. Read UserGuide - [JP](/Documents/JP/UserGuide.md)/[EN](/Documents/EN/UserGuide.md)

## How to build?

#### Require

* cmake (for flatbuffer)
* dotnet
* Set the PATH for msbuild (Windows only)
* Python
  * Install SCons using pip (for Godot)
* Unity (to export as .unitypackage)
  
#### BocchiTracker(WPF) Build

1. Run `dotnet build Application/BocchiTracker.WPF.sln`

#### flatbuffers Build

1. Run `python ExternalTools/BuildScripts/build_flatbuffers.py`

#### UE Build

※ Required flatbuffers build

1. Copy the `Plugins\UnrealEngine\BocchiTracker` directory to `[Project Root]/Plugins/BocchiTracker`.

2. Also, copy the `Plugins\UnrealEngine\ThirdParty\flatbuffers` directory to `[Project Root]/Source/ThirdParty/flatbuffers`.

#### Unity Build

※ Required flatbuffers build

1. Open `Plugins/Unity/project` on Unity

2. Export: [ToolBar] -> [BochiTracker] -> [Export Package]

#### Godot Build

※ Required flatbuffers build and installed sons

1. Run `python ExternalTools\BuildScripts\build_godot.py`

## Problems? 

If you encounter any issues, have feature requests, or wish to provide feedback, please feel free to create issues
