﻿namespace WebApplication1.Exceptions
{
	public class ValidationException : Exception
	{
		public Dictionary<string, string[]> Errors { get; }
		public ValidationException(Dictionary<string, string[]> errors) : base("Validation failed")
			=> Errors = errors;
	}
}
