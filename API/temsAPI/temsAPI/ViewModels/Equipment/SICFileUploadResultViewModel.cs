using temsAPI.System_Files.Exceptions;

namespace temsAPI.ViewModels.Equipment
{
    public class SICFileUploadResultViewModel
    {
        public string FileName { get; set; }
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public int EllapsedMiliseconds { get; set; }
    }
}
