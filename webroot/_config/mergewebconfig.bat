@echo off

echo.
echo Usage "mergewebconfig.bat <environment> <projectroot>" where environment is dev, staging, live or local and projectroot is the base path
echo.
echo Generating web.config file with environment token
echo.

SET environment=%1
SET projectroot=%2
SET webconfig=%projectroot%Web.config
SET webconfigmaster=%projectroot%Web-master.config
SET token=!environment!
SET tokenreplace=%environment%
SET xchange=%projectroot%_config\Xchang32.exe

SET dns=!dns!
SET dnsreplace=localhost

IF %environment% == dev (
	SET dnsreplace=ws.dev.hansoninc.com
)

IF %environment% == staging (
	SET dnsreplace=ws.staging.hansoninc.com
)

IF %environment% == live (
	SET dnsreplace=ws.owenscorning.com
)

REM map token replace to appropriate
IF NOT %environment% == dev (
	IF NOT %environment% == staging (
		IF NOT %environment% == live (
			IF NOT %environment% == local (
				goto NoMatchingEnvironment
			)
		)
	)
)

echo environment:     %environment%
echo projectroot:     %projectroot%
echo webconfig:       %webconfig%
echo webconfigmaster: %webconfigmaster%
echo token:           %token%
echo tokenreplace:    %tokenreplace%
echo xchange:         %xchange%
echo.

echo Checking for %webconfigmaster%
if not exist %webconfigmaster% goto FileNotFoundWebConfigFinal
echo.

echo Checking for "%xchange%"
if not exist %xchange% goto FileNotFoundXchang32
echo.

echo Deleting %webconfig% if it exists
if exist %webconfig% del %webconfig%
echo.

echo Creating new %webconfig% from %webconfigmaster%
copy %webconfigmaster% %webconfig%
echo.

echo Replacing "%token%" with "%tokenreplace%" in %webconfig%
call %xchange% /i /s "%webconfig%" "%token%" "%tokenreplace%"
echo.

echo Replacing "%dns%" with "%dnsreplace%" in %webconfig%
call %xchange% /i /s "%webconfig%" "%dns%" "%dnsreplace%"
echo.

goto END

:FileNotFoundWebConfigFinal
echo Unable to locate %webconfigfinal%
goto END

:FileNotFoundXchang32
echo Xchang32.exe not found, download Clay's utilities.
goto END

:NoMatchingEnvironment
echo No environment found that matches %environment%, choices are dev, staging, live or local
goto END

:END
echo Done