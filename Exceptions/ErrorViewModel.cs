namespace WebApplication1.Exceptions
{

	public class ErrorViewModel
	{
		public string? RequestId { get; set; }
		public string Message { get; set; } = "Произошла ошибка";
		public string? Details { get; set; }
		public Dictionary<string, string[]>? ValidationErrors { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

		public static string GetUserFriendlyMessage(Exception ex)
		{
			return ex switch
			{
				NotFoundException => ex.Message,
				ValidationException => "Ошибка валидации данных",
				_ => "Произошла ошибка. Пожалуйста, попробуйте позже."
			};
		}
	}
}
