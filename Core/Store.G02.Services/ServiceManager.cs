using AutoMapper;
using Store.G02.Domain.Contracts;
using Store.G02.Services.Abstractions;
using Store.G02.Services.Abstractions.Baskets;
using Store.G02.Services.Abstractions.Cashe;
using Store.G02.Services.Abstractions.Products;
using Store.G02.Services.Baskets;
using Store.G02.Services.Cache;
using Store.G02.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services
{
    public class ServiceManager
        (IUnitOfWork _unitOfWork,
        IMapper _mapper, 
        IBasketRepository _basketRepository,
        ICacheRepository _cacheRepository) 
        : IServiceManager
    {
        public IProductService productService { get; } = new ProductService(_unitOfWork, _mapper);
        public IBasketService basketService { get; } = new BasketService(_basketRepository, _mapper);
        public ICacheService cacheService { get; } = new CacheService(_cacheRepository);
    }
}
