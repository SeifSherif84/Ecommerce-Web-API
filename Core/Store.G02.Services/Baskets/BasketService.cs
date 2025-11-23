using AutoMapper;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities.Baskets;
using Store.G02.Domain.Exceptions.BadRequest;
using Store.G02.Domain.Exceptions.NotFound;
using Store.G02.Services.Abstractions.Baskets;
using Store.G02.Shared.Dtos.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Baskets
{
    public class BasketService(IBasketRepository _basketRepository, IMapper _mapper) : IBasketService
    {
        public async Task<BasketDto?> GetBasketAsync(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            if (basket is null) throw new BasketNotFoundException(id);
            var basketDto = _mapper.Map<BasketDto>(basket);
            return basketDto;
        }

        public async Task<BasketDto?> CreateBasketAsync(BasketDto basketdto, TimeSpan duration)
        {
            var basket = _mapper.Map<CustomerBasket>(basketdto);
            var Result = await _basketRepository.CreateBasketAsync(basket, duration);
            if (Result is null) throw new CreateOrUpdateBasketBadRequestException();
            var basketDto = _mapper.Map<BasketDto>(Result);
            return basketDto;
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            var flag = await _basketRepository.DeleteBasketAsync(id);
            if (!flag) throw new DeleteBasketBadRequestException();
            return flag;
        }


    }
}
