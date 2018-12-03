cd c:/code;
C:/bin/nuget.exe restore;
msbuild matrix-io-malos.sln /t:build /p:Configuration=Release /p:Platform="any cpu" /m;
