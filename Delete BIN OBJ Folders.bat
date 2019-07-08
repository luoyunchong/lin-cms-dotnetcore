@echo off
@echo Deleting all BIN, OBJ folders...
for /d /r . %%d in (bin,obj)  do @if exist "%%d" rd /s/q "%%d"
@echo.
@echo BIN and OBJ folders successfully deleted :) Close the window.
@echo.
@echo.
pause > nul