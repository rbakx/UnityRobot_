@echo off

del ..\Unity\RobotMaster\Assets\scripts\generated\*.proto.cs

for /r %%i in (messages/*.proto) do (
	echo Generating code for: messages\%%~nxi
	protogen -i:messages/%%~nxi -o:..\Unity\RobotMaster\Assets\scripts\generated\%%~nxi.cs
)

echo done