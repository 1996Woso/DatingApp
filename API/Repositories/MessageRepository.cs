using System;
using API.Data;
using API.Interfaces;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class MessageRepository(IMapper mapper, DataContext dataContext) : IMessageRepository
{
    public void AddGroup(Group group)
    {
        dataContext.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        dataContext.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        dataContext.Messages.Remove(message);
    }

    public async Task<Connection?> GetConnectionAsync(string connectionId)
    {
        return await dataContext.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await dataContext.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessageAsync(int id)
    {
        return await dataContext.Messages.FindAsync(id);
    }

    public async Task<Group?> GetMessageGroupAsync(string groupName)
    {
        return await dataContext.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<PagedList<MessageDTO>> GetMessagesForUserAsync(MessageParams messageParams)
    {
        var query = dataContext.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username && x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username && x.SenderDeleted == false),
            _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null && x.RecipientDeleted == false)
        };

        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>
            .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
    {
        var query = dataContext.Messages
            .Where(x =>
                x.RecipientUsername == currentUsername && x.SenderUsername == recipientUsername && x.RecipientDeleted == false ||
                x.SenderUsername == currentUsername && x.RecipientUsername == recipientUsername && x.SenderDeleted == false
            )
            .OrderBy(x => x.MessageSent)
            .AsQueryable();
        var unreadMessages = query.Where(x => x.DateRead == null &&
            x.RecipientUsername == currentUsername).ToList();

        if (unreadMessages.Count > 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
        }

        return await query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider).ToListAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        dataContext.Connections.Remove(connection);
    }

}
