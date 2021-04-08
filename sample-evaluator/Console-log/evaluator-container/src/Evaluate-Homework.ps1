$Exercise1File = "/submission/submission.txt"
$Exercise2File = "/submission/imsc.txt"


# Output any message to the console for diagnostics
Write-Output "Starting evaluation"


# Connecting to the service container
$WebServerDns = $Env:WEBSERVER
$ConnectionTestRetry = 10
$ConnectionTestOk = $false
Write-Host "Checking connection to $WebServerDns"
# The service container might need some time to start up, poll for status
while($ConnectionTestRetry -gt 0)
{
    $ConnectionTestRetry--
    Write-Host "Trying to connect..."
    try {
        Invoke-WebRequest -UseBasicParsing -Uri $WebServerDns
        $ConnectionTestOk = $true
        Write-Output "Web server operational"
        break;
    }
    catch
    {
        Start-Sleep -Seconds 3
    }
}
if(-not $ConnectionTestOk) {
    Write-Output "Cannot connect to web server"
    exit 0
}



# **** First group of exercises
# The group of exercises contain multiple exercises, the results aggregated into a scalar point value at the end.
# Any "passed" test is worth 1 point, while "failed" tests are 0 point. Arbitrary points can also be awarded.

# First test: checks if the file exists.
if (Test-Path $Exercise1File) {
    # Passing test: mandatory prefix # validation code # command: test result will follow # exercise name # success status "passed"
    Write-Output "###ahk#Valid55Code#testresult#Exercise 1#passed"
}
else {
    # Failid test: mandatory prefix # validation code # command: test result will follow # exercise name # status is "failed" # opcional problem description
    Write-Output "###ahk#Valid55Code#testresult#Exercise 1#failed#File does not exist"
}

# Second test: check file contents
$Exercise1Content = Get-Content $Exercise1File
if ($Exercise1Content -like "*42*") {
    # Correct content is worth 2 points
    Write-Output "###ahk#Valid55Code#testresult#Exercise 2#2#file content is ok"
}
elseif ($Exercise1Content -like "*84*") {
    # The solution cannot be evaluated yielding an "inconclusive" result
    Write-Output "###ahk#Valid55Code#testresult#Exercise 2#inconclusive#Exercise need manual checking"
}
else {
    # The error message can contain multiple lines; a newline starts with the \ character (`n is PowerShell's newline here)
    Write-Output "###ahk#Valid55Code#testresult#Exercise 2#0#Valus is not correct\`nMultiline message\`nprinted this way"
}




# **** Second group of exercises, e.g., optional exercises
# The name of the exercise before the @ character specifies the name of the group (imsc@Exercise 3)
# @ is optiopnal, if the exercise group has no name, ommit it

if (Test-Path $Exercise2File) {
    if ((Get-Content $Exercise2File) -like "*84*") {
        Write-Output "###ahk#Valid55Code#testresult#imsc@Exercise 3#passed#Optional exercise ok"
    }
    else {
        Write-Output "###ahk#Valid55Code#testresult#imsc@Exercise 3#failed#Wrong value"
    }
}