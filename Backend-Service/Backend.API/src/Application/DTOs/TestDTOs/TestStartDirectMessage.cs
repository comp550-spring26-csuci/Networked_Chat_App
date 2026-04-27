namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestStartDirectMessage
    {
        private StartDirectMessage _startDirectMessage = null!;
        private string? _targetUsername = null!;
        
        public StartDirectMessage StartDirectMessage
        {
            get => _startDirectMessage;
            set => _startDirectMessage = value;
        }

        public string? TargetUsername
        {
            get => _targetUsername;
            set => _targetUsername = value;
        }

    }
}
