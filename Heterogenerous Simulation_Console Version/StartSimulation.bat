@echo off

set files=n100d3_01.brite;n100d3_02.brite
set methods=RandomDeployment;KCutDeployment;KCutDeployment2

copy "Heterogenerous Simulation_Console Version.exe" "S.exe"
FOR %%F IN (%files%) DO FOR %%M IN (%methods%) DO START /WAIT /min S.exe %%F 80 0 20 10 1 100 30 1 10 0.5 0.5 0 0 false false 50 %%M