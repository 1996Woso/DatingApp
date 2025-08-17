using System;

namespace API.Interfaces;

public interface IUnitOfWork
{
    IUsersRepository UsersRepository { get; }
    IMessageRepository MessageRepository { get; }
    ILikesRepository LikesRepository { get; }
    Task<bool> CompleteAsync();
    bool HasChanges();
}
