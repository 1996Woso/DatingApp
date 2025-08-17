using System;
using API.Interfaces;

namespace API.Data;

public class UnitOfWork(DataContext dataContext, IUsersRepository usersRepository
, IMessageRepository messageRepository, ILikesRepository likesRepository) : IUnitOfWork
{
    public IUsersRepository UsersRepository => usersRepository;

    public IMessageRepository MessageRepository => messageRepository;

    public ILikesRepository LikesRepository => likesRepository;

    public async Task<bool> CompleteAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return dataContext.ChangeTracker.HasChanges();
    }
}
