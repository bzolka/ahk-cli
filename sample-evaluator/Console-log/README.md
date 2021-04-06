# Sample evaluator using PowerShell

A sample evaluator using a PowerShell script. See the Dockerfile and the source code of the evaluator [here](evaluator-container).

Build the Docker image: `docker build -t sampleevaluator2 evaluator-container`

Execute the evaluation: `dotnet ahk.dll eval docker consolemessage --validationCode Valid55Code --image sampleevaluator2 --mount-path /submission --artifact-path /result -s ./../sample-input -o "./evaluation-output-{date}"`
