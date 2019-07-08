@echo off
@echo Deleting all BIN, OBJ folders...
for /d /r . %%d in (bin,obj)  do @if exist "%%d" rd /s/q "%%d"
del /a /f /s /q d:\svn\CHNMed\CHNMed.Web\Areas\Plat\Views\*.generated.cs
del /a /f /s /q d:\svn\CHNMed\CHNMed.Web\Areas\Admin\Views\*.generated.cs
del /a /f /s /q d:\svn\CHNMed\CHNMed.Web\Views\*.generated.cs
@echo.
@echo BIN and OBJ folders successfully deleted :) Close the window.
@echo.
@echo.
pause > nul