#!/bin/bash

rm -rf *.sln
dotnet new sln
dotnet sln add **/**/*.fsproj