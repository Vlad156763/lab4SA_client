#!/bin/bash
dotnet clean  
rm -rf bin obj
dotnet build  
echo "SUCSESS REASEMBLY"
dotnet run    
