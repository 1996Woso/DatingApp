using System;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLikeAsync(int sourceUserId, int targetUserId);
    Task<PagedList<AppUserDTO>> GetUserLikesAsync(LikesParams likesParams);
    Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int currentUserId);
    void DeleteLike(UserLike userLike);
    void AddLike(UserLike userLike);
}
