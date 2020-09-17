# Sample evaluator using PowerShell

- [Docker image](evaluator-container) performing the evaluation
- [Source code of the evaluator](evaluator-container/src) based on a PowerShell script
- [Evaluation configuration](evaluation-config) controlling the `Ahk CLI`
- [Sample student submissions](../sample-input)

Execute: `dotnet AHK.dll -k "./evaluation-config" -m "./../sample-input" -e "./evaluation-output-{date}"`
