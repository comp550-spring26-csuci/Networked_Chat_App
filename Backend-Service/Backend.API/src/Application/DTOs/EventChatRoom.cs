// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 5 2026
//  Description: This DTO is used to represent a chat room in the context of an event. It contains only the
//  information about the chat room that is relevant to the event, such as its ID and name. This allows us to
//  include chat room information in events without having to include the entire ChatRoom entity, which may
//  contain additional data that is not necessary for the event.

namespace Backend.API.src.Application.DTOs
{
    public class EventChatRoom
    {
        private Guid _chatRoomId;
        private string? _chatRoomName;

        public Guid ChatRoomId
        {
            get => _chatRoomId;
            set => _chatRoomId = value;
        }

        public string? ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }
    }
}
