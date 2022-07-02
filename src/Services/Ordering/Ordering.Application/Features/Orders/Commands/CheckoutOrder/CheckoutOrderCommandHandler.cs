using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(IOrderRepository repo, IMapper mapper, IEmailService emailService, ILogger<CheckoutOrderCommandHandler> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);

            var newOrder = await _repo.AddAsync(orderEntity);

            _logger.LogInformation($"Order {newOrder.Id} is successfully created.");

            await SendEmail(newOrder);
            return newOrder.Id;
        }

        private async Task SendEmail(Order newOrder)
        {
            var email = new Email { To = "abc@gmail.com", Body = "Order was created.", Subject = $"Order: {newOrder.Id}" };
            try
            {
                await _emailService.SendEmail(email);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Order {newOrder.Id} failed due to an error with the email service: {ex.Message}");
            }
        }
    }
}
