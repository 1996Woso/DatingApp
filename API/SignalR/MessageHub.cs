using System;
using API.Extensions;
using API.Interfaces;
using API.Models;
using API.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];

        if (Context.User == null || string.IsNullOrEmpty(otherUser))
            throw new HubException("Cannot join group");
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser!);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        var messages = await unitOfWork.MessageRepository.GetMessageThreadAsync(Context.User.GetUsername(), otherUser!);
        if (unitOfWork.HasChanges()) await unitOfWork.CompleteAsync();
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
        var username = Context.User?.GetUsername() ?? throw new HubException("Cannot get the user");
        if (username.Equals(createMessageDTO.RecipientUsername, StringComparison.CurrentCultureIgnoreCase))
            throw new Exception("You cannot message yourself");
        var sender = await unitOfWork.UsersRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UsersRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if (recipient == null || sender == null || recipient.UserName == null)
            throw new HubException("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = username,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        var groupName = GetGroupName(sender.UserName!, recipient.UserName);
        var group = await unitOfWork.MessageRepository.GetMessageGroupAsync(groupName);
        if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else//Notify a user got a new message
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null && connections?.Count != null)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new { username = sender.UserName, knownAs = sender.KnownAs });
            }
        }

        unitOfWork.MessageRepository.AddMessage(message);

        if (await unitOfWork.CompleteAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
        }
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
        var group = await unitOfWork.MessageRepository.GetMessageGroupAsync(groupName);
        var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };

        if (group == null)
        {
            group = new Group { Name = groupName };
            unitOfWork.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);
        if (await unitOfWork.CompleteAsync()) return group;
        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (connection != null && group != null)
        {
            unitOfWork.MessageRepository.RemoveConnection(connection);
            if (await unitOfWork.CompleteAsync()) return group;
        }

        throw new Exception("Failed to remove from group");
    }
}
