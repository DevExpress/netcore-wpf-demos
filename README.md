# .Net Core 3 WPF Demos

This repository contains the DevExpress demo applications that target .Net Core 3:
- Outlook-Inspired App
- Hybrid App
- Mail Client
- Realtor World

## Requirements

Install the following software to build and run the demo application:

- **Visual Studio 2019** with the **.NET desktop development** workload installed

- [.NET core 3 preview 5 or more recent](https://dotnet.microsoft.com/download/dotnet-core/3.0)

## Getting started

Clone the repository to a working folder, navigate to './src'.

Open a solution in Visual Studio. 

Before you build the solution, ensure that the 'Use previews of the .NET Core SDK' option is enabled. 
You can find this setting from the Visual Studio main menu: 
- In Visual Studio 2019 version 16.1+: Tools -> Options -> Environment -> Preview Features
- In Visual Studio 2019 version 16.0: Tools -> Options -> Project and Solutions -> .NET Core

## Integrate DevExpress WPF Controls into a .NET Core 3 application

You need the DevExpress NuGet packages to create a .Net Core 3 project. Follow the steps below to add the packages to a solution:

1. [Register](https://docs.devexpress.com/GeneralInformation/116698/installation/install-devexpress-controls-using-nuget-packages/setup-visual-studio%27s-nuget-package-manager) the DevExpress Early Access feed in Visual Studio's NuGet Package Manager. Note that you should enable the **Include prerelease** option.

    `https://nuget.devexpress.com/early-access/api`

2. Install the DevExpress.WindowsDesktop.Wpf package for .Net Core 3. 

## Feedback

We'd like to hear from you: wpfteam@devexpress.com

## Copyright

Developer Express Inc. All rights reserved.
