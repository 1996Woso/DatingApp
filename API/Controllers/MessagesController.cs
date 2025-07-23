using API.Extensions;
using API.Interfaces;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUsersRepository usersRepository;
        private readonly IMapper mapper;

        public MessagesController(IMessageRepository messageRepository
        , IUsersRepository usersRepository
        , IMapper mapper)
        {
            this.messageRepository = messageRepository;
            this.usersRepository = usersRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var username = User.GetUsername();
            if (username == createMessageDTO.RecipientUsername.ToLower())
                return BadRequest("You cannot message yourself");
            var sender = await usersRepository.GetUserByUsernameAsync(username);
            var recipient = await usersRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null || sender == null)
                return BadRequest("Cannot send message at this time");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = username,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };
            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
                return Ok(mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to save message");

        }

        [HttpGet]
        public async Task<ActionResult> GetMessagesForUser(
            [FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await messageRepository.GetMessagesForUserAsync(messageParams);
            Response.AddPaginationHeader(messages);
            return Ok(messages);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await messageRepository.GetMessageThreadAsync(currentUsername, username));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await messageRepository.GetMessageAsync(id);

            if (message == null) return BadRequest("You cannot delete this message");

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Forbid();

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message is { SenderDeleted: true, RecipientDeleted: true })
            {
                messageRepository.DeleteMessage(message);
            }

            if (await messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("There was a problem while deleting message");

        }

    }
}
