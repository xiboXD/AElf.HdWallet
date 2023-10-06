#!/bin/bash

dotnet tool restore

dotnet jb inspectcode AElf.HdWallet.sln -f="Text" --no-build --include="**.cs" -o=".lint/CodeWarningResults.txt"

totalLines=$(file .lint/CodeWarningResults.txt | nl | wc -l)

if [[ "$totalLines" -gt 1 ]]; then
    echo "There are few linter warnings - please fix them before running the pipeline"
    cat .lint/CodeWarningResults.txt
    exit 1
fi