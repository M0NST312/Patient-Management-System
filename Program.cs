using BCrypt.Net;

var password = "Admin@123";
var hash = BCrypt.HashPassword(password);
Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hash: {hash}");
Console.WriteLine($"Verify result: {BCrypt.Verify(password, hash)}");
Console.WriteLine($"Wrong password: {BCrypt.Verify("WrongPassword", hash)}");
