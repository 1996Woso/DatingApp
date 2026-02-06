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
    public class MessagesController(IMapper mapper, IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var username = User.GetUsername();
            if (username.Equals(createMessageDTO.RecipientUsername, StringComparison.CurrentCultureIgnoreCase))
                return BadRequest("You cannot message yourself");
            var sender = await unitOfWork.UsersRepository.GetUserByUsernameAsync(username);
            var recipient = await unitOfWork.UsersRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null || sender == null || recipient.UserName == null)
                return BadRequest("Cannot send message at this time");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = username,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };
            unitOfWork.MessageRepository.AddMessage(message);

            if (await unitOfWork.CompleteAsync())
                return Ok(mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to save message");

        }

        [HttpGet]
        public async Task<ActionResult> GetMessagesForUser(
            [FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await unitOfWork.MessageRepository.GetMessagesForUserAsync(messageParams);
            Response.AddPaginationHeader(messages);
            return Ok(messages);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await unitOfWork.MessageRepository.GetMessageThreadAsync(currentUsername, username));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await unitOfWork.MessageRepository.GetMessageAsync(id);

            if (message == null) return BadRequest("You cannot delete this message");

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Forbid();

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message is { SenderDeleted: true, RecipientDeleted: true })
            {
                unitOfWork.MessageRepository.DeleteMessage(message);
            }

            if (await unitOfWork.CompleteAsync()) return Ok();

            return BadRequest("There was a problem while deleting message");

        }

    }
}
