@echo off

rd /s /q ..\networking\networking\communication\generated
mkdir ..\networking\networking\communication\generated

for /r %%i in (messages/*.proto) do (
	echo Generating code for: messages\%%~nxi
	protogen -i:messages/%%~nxi -o:..\networking\networking\communication\generated\%%~nxi.cs
)

echo done