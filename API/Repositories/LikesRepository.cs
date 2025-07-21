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

public class LikesRepository : ILikesRepository
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;

    public LikesRepository(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }
    public void AddLike(UserLike userLike)
    {
        dataContext.Likes.Add(userLike);
    }

    public void DeleteLike(UserLike userLike)
    {
        dataContext.Likes.Remove(userLike);
    }
    //Get Ids of users liked by the current user
    public async Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int currentUserId)
    {
        return await dataContext.Likes
            .Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
    }
    //Get info of a user who like another user and info of the user being liked (by using two users ids)
    public async Task<UserLike?> GetUserLikeAsync(int sourceUserId, int targetUserId)
    {
        return await dataContext.Likes.FindAsync(sourceUserId, targetUserId);
    }
    //Get all users who liked the user or get all users liked by the user
    public async Task<PagedList<AppUserDTO>> GetUserLikesAsync(LikesParams likesParams)
    {
        var likes = dataContext.Likes.AsQueryable();
        IQueryable<AppUserDTO> query;
        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(x => x.SourceUserId == likesParams.UserId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<AppUserDTO>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<AppUserDTO>(mapper.ConfigurationProvider);
                break;

            default:
                var likeIds = await GetCurrentUserLikeIdsAsync(likesParams.UserId);
                query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<AppUserDTO>(mapper.ConfigurationProvider);
                break;
        }
        return await PagedList<AppUserDTO>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }
}
