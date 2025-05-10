namespace Task_Manager.DTOs
{
    public class TaskTagDTO
    {
        public int TagID { get; set; }        // Unique identifier for the tag
        public int TaskID { get; set; }       // Associated TaskID
        public int UserID { get; set; }       // User who created the tag
        public string UserName { get; set; }  // User's name who created the tag
        public string TagName { get; set; }   // The tag name
        public DateTime CreatedDate { get; set; }  // When the tag was created
    }
}
