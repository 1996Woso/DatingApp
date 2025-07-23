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

public class MessageRepository : IMessageRepository
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;

    public MessageRepository(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }
    public void AddMessage(Message message)
    {
        dataContext.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        dataContext.Messages.Remove(message);
    }

    public async Task<Message?> GetMessageAsync(int id)
    {
        return await dataContext.Messages.FindAsync(id);
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
        var messages = await dataContext.Messages
            .Include(x => x.Sender).ThenInclude(x => x.Photos)
            .Include(x => x.Recipient).ThenInclude(x => x.Photos)
            .Where(x =>
                x.RecipientUsername == currentUsername && x.SenderUsername == recipientUsername && x.RecipientDeleted == false ||
                x.SenderUsername == currentUsername && x.RecipientUsername == recipientUsername && x.SenderDeleted == false
            )
            .OrderBy(x => x.MessageSent)
            .ToListAsync();
        var unreadMessages = messages.Where(x => x.DateRead == null &&
            x.RecipientUsername == currentUsername).ToList();

        if (unreadMessages.Count > 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            await dataContext.SaveChangesAsync();
        }

        return mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }
}
