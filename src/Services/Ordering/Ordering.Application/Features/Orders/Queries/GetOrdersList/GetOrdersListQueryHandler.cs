using System;
using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrderVm>>
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _mapper;

        public GetOrdersListQueryHandler(IOrderRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<List<OrderVm>> Handle(GetOrdersListQuery request, 
                                        CancellationToken cancellationToken)
        {
            var orders = await _repo.GetOrdersByUserName(request.UserName);
            return _mapper.Map<List<OrderVm>>(orders);
        }
    }
}
