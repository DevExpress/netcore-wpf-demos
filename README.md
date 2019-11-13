# .Net Core 3 WPF Demos

> ⚠ **Important Note**
> 
> This repository contains a pre-release version of DevExpress demo applications for the .NET Core 3 platform.
> 
> Please visit [https://www.devexpress.com/dotnet-core-3/](https://www.devexpress.com/dotnet-core-3/) to download the most recent version of the DevExpress .NET Core 3 installer (contains shipping versions of all DevExpress .NET Core 3 demos).

This repository contains the DevExpress demo applications that target .Net Core 3:
- Outlook-Inspired App
- Hybrid App
- Mail Client
- Realtor World

## Requirements

Install the following software to build and run the demo applications:

- **Visual Studio 2019 v16.3** (or later) with the **.NET desktop development** workload installed

- [.NET core 3](https://dotnet.microsoft.com/download/dotnet-core/3.0)

## Getting started

Clone the repository to a working folder, navigate to './src'.

Open a solution in Visual Studio. 

## Integrate DevExpress WPF Controls into a .NET Core 3 application

You need the DevExpress NuGet packages to build and run these demos. Follow the steps below to add the packages to a solution:

1. [Obtain your NuGet feed URL](https://docs.devexpress.com/GeneralInformation/116042/installation/install-devexpress-controls-using-nuget-packages/obtain-your-nuget-feed-url).
2. In Visual Studio, go to **Tools | NuGet Package Manager | Manage NuGet Packages for Solution**
3. Open "Settings"...

    ... and add a new NuGet feed with the following credentials:

    **Name:** _DevExpress_  
    **Source:** `https://nuget.devexpress.com/{your feed authorization key}/api`

4. Select the **DevExpress** package source.

5. In the "Browse" tab, search for the _'WindowsDesktop.Wpf'_ keyword and install the **DevExpress.WindowsDesktop.Wpf** and **DevExpress.WindowsDesktop.Wpf.Themes.All** packages for the current project. Read and accept the license agreement. 

## Documentation

[.NET Core 3 Support](https://docs.devexpress.com/WPF/401165/dotnet-core-support)

## Feedback

We'd like to hear from you: wpfteam@devexpress.com

## Copyright

Developer Express Inc. All rights reserved.
