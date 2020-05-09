namespace HarelTech.WMS.Common.Models
{
    public class RequestResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string Error { get; set; } = "";
    }
}
