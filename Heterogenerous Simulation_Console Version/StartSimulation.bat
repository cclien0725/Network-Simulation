@echo off

set files=n4kd4_1.brite;n4kd4_2.brite;n4kd4_3.brite;n4kd4_4.brite;n4kd4_5.brite;n4kd4_6.brite;n4kd4_7.brite;n4kd4_8.brite;n4kd4_9.brite;n4kd4_10.brite;n4kd4_11.brite;n4kd4_12.brite;n4kd4_13.brite;n4kd4_14.brite;n4kd4_15.brite;n5kd4_1.brite;n5kd4_2.brite;n5kd4_3.brite;n5kd4_4.brite;n5kd4_5.brite;n5kd4_6.brite;n5kd4_7.brite;n5kd4_8.brite;n5kd4_9.brite;n5kd4_10.brite;n5kd4_11.brite;n5kd4_12.brite;n5kd4_13.brite;n5kd4_14.brite;n5kd4_15.brite
set methods=RandomDeployment;KCutDeployment;KCutDeployment2

copy "Heterogenerous Simulation_Console Version.exe" "S.exe"
FOR %%F IN (%files%) DO FOR %%M IN (%methods%) DO START /WAIT /min S.exe %%F 80 0 20 10 1 100 30 1 10 0.5 0.5 0 0 false false 50 %%M