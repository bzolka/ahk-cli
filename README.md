# Ahk CLI

[![Build](https://github.com/akosdudas/ahk-cli/actions/workflows/build.yml/badge.svg)](https://github.com/akosdudas/ahk-cli/actions/workflows/build.yml)

Ahk CLI is a .NET Core console application that executes evaluation on student homework assignments by running a containerized evaluation logic.

> Please refer to <https://akosdudas.github.io/automated-homework-evaluation/> for the "big picture" of automation of homework evaluation.

## Example

Suppose you have homework submissions in a folder, one subfolder or zip file for each student. And you have a Docker container to evaluate a homework. The you can use this tool as follows:

![Example evaluation process](docs/images/cli-exec-example.gif)

The process will give you an Excel file with the results:

![Example output](docs/images/output-excel.png)

## Usage

The application uses **.NET Core 3.1**. There are no binaries distributed; you need to build the application from source:

```
dotnet build -c Release
dotnet publish -c Release -o ./bin --no-build
```

To check the usage of the application run `dotnet ahk.dll --help`.

To run the evaluation execute `dotnet ahk.dll eval docker trx|consolemessage -s <folder>`.

### Arguments of `eval docker`

There are two modes of grading a submission: `TRX` and `ConsoleMessages`. The mode determines how the evaluation application communicates the results. You can refer to the two samples in folder [sample-evaluator](sample-evaluator).

The arguments are as follows.

#### `-s|--submissions`

A directory containing the submissions to evaluate. The current directory if not specified.

#### `-o|--output`

A directory to use as output; the results of the evaluation are placed here. A new directory in the current directory if not specified.

#### `-d|--studentid`

A file name expected in the root of every submission folder/zip containing the identifier of the student. If value is empty string uses the name of the folder/file as student identifier. Default value is: `neptun.txt`.

#### `--image`

The name of the Docker image to execute. A new container is spawned for each submission.

#### `--mount-path`

The path within the container to mount the submission folder to. The container should process the submission from this folder. Default value: `/submission`.

#### `--artifact-path`

The path within the container to fetch artifacts from (e.g., output files, generated content, etc.); defaults to none (nothing is fetched).

#### `--timeout`

Maximum time frame for a container to finish evaluation; the container is terminated if exceeded. Default: `00:03:00`.

#### `--container-arg`

Optional parameters for container creation, e.g., memory amount. Each item is a key-value pair in the form of `key=value`. The key is a parameter of [Docker Engine API CreateContainer](https://docs.docker.com/engine/api/v1.25/#operation/ContainerCreate) operations. (Supports only string and number values.) E.g.: `--container-arg memory=268435456 --container-arg hostname=app`

#### `[eval docker trx] --trxfile`

Enables the `TRX` grading mechanism and specifies the name of the `TRX` file to parse for results. Expected in the output folder of the container (`--artifact-path`).

#### `[eval docker consolemessage] --validationcode`

Enables the `ConsoleMessage` grading mechanism and specifies the _validation code_ expected in every message printed to console by the evaluation application.
