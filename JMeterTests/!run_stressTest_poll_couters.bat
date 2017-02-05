
SET PRJ_PATH=%CD%

"c:\Program Files (x86)\JMeter\bin\jmeter" -e -n -t %PRJ_PATH%\StressTest_PollCounters_TestService.jmx -l %PRJ_PATH%\StressTest_PollCounters_TestService_res.jtl

rem  -l %PRJ_PATH%\test.jtl

pause