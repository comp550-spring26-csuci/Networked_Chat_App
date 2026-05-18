// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the IChatClient interface, which represents the client-side contract for receiving real-time
//  updates and events related to chat messages, chat rooms, user status changes, and friendship updates. The interface includes
//  methods for handling incoming messages, message previews, chat events, errors, read receipts, and various user and group-related
//  events. Implementing this interface allows clients to react to changes in the chat system and update their UI accordingly.
// --------------------------------------------

using Backend.API.src.Application.DTOs;
using Backend.API.src.Application.DTOs.TestDTOs;

namespace Backend.API.src.Core.Interface
{
    public interface IChatClient
    {
        Task ReceiveMessage(TestMessage testMessage);
        Task ReceiveMessagePreview(TestMessagePreview testMessagePreview);
        Task ReceiveEvent(TestChatEvent testChatEvent);
        Task ReceiveMarkedAsRead(TestChatRoomRead testChatRoomRead);
        Task ReceiveError(string message);
        Task MyUserStatusChanged(ChatEventDto chatEvent);
        Task FriendUserStatusChanged(ChatEventDto chatEvent);
        Task ChatGroupMembershipAdded(ChatEventDto chatEvent);
        Task ChatGroupMembershipDeleted(ChatEventDto chatEvent);
        Task ChatGroupDeleted(ChatEventDto chatEvent);
        Task FriendshipAdded(ChatEventDto chatEvent);
        Task FriendshipDeleted(ChatEventDto chatEvent);
    }
}
