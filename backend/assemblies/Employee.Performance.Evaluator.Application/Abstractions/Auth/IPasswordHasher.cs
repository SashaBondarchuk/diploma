﻿namespace Employee.Performance.Evaluator.Application.Abstractions.Auth;

public interface IPasswordHasher
{
    string HashPassword(string password);

    bool VerifyPassword(string hashedPassword, string providedPassword);
}
