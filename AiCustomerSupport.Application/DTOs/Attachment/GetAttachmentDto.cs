namespace AiCustomerSupport.Application.DTOs.Attachment
{
    public class GetAttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
