#!/bin/bash
# Test if the BCrypt module works correctly
dotnet new console -n AuthTest --force -q
cd AuthTest
cat > Program.cs << 'CSHARP'
using BCrypt.Net;

var password = "Admin@123";
var hash = BCrypt.HashPassword(password);
Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hash: {hash}");
Console.WriteLine($"Verify result: {BCrypt.Verify(password, hash)}");
Console.WriteLine($"Wrong password: {BCrypt.Verify("WrongPassword", hash)}");
CSHARP

dotnet add package BCrypt.Net-Next
dotnet run
