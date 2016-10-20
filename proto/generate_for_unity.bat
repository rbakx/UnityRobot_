@echo off

rd /s /q ..\Unity\RobotMaster\Assets\scripts\generated
mkdir ..\Unity\RobotMaster\Assets\scripts\generated

for /r %%i in (messages/*.proto) do (
	echo Generating code for: messages\%%~nxi
	protogen -i:messages/%%~nxi -o:..\Unity\RobotMaster\Assets\scripts\generated\%%~nxi.cs
)

echo done