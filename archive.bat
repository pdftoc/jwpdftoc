@echo off

set dest=jwpdctoc

if exist %dest% goto build
mkdir %dest%



:build
devenv /rebuild release src\jwpubtoc.sln



:pack

copy /Y /V files\LICENSE.txt       %dest%
copy /Y /V files\License.Ionic.txt %dest%
copy /Y /V files\License.zlib.txt  %dest%
copy /Y /V files\README.txt        %dest%
copy /Y /V files\README_J.txt      %dest%
copy /Y /V files\publication.xml   %dest%

xcopy /Y /V /D /I files\toc %dest%\toc

copy /Y /V src\bin\Release\jwpubtoc.exe  %dest%
copy /Y /V src\bin\Release\Ionic.Zip.dll %dest%
