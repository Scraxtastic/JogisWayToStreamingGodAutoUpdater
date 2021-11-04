# JogisWayToStreamingGodAutoUpdater
This Program is designed to automatically download and replace the newest Release for the 
[Jogi Game](https://github.com/Scraxtastic/JogisWayToStreamingGod).


## Properties
Added Property in the .csproj file. [Forum Post](https://docs.microsoft.com/en-us/answers/questions/296816/self-contained-single-file-does-not-produce-a-sing.html). The property is needed, to bundle the native libraries into the one .exe file, so that the user just has to download 1 file instead of n>1.
```XML
<IncludeAllContentForSelfExtract>true</IncludeAllContentForSelfExtract>
```

Furthermore you have to call following command in the console in order to make a publishable file.
```
dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained true
```

The User will only need the .exe file.