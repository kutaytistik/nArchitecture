using Application.Features.Brands.Dtos;
using Application.Features.Brands.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommand : IRequest<CreatedBrandDto>
    {
        public string Name { get; set; }

        public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, CreatedBrandDto>
        {

            private readonly IBrandRepository _brandRepository;
            private readonly IMapper _mapper;
            private readonly BrandBusinessRules _brandBusinessRules;

            public CreateBrandCommandHandler(IBrandRepository brandRepository, IMapper mapper,BrandBusinessRules brandBusinessRules)
            {
                _brandRepository = brandRepository;
                _mapper = mapper;
                _brandBusinessRules = brandBusinessRules;
            }

            public async Task<CreatedBrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
            {

                await _brandBusinessRules.BrandNameCanNotBeDuplicatedWhenInserted(request.Name);


                // gelen request'i brand'e çevir
                Brand mappedBrand = _mapper.Map<Brand>(request);
                // veritabanına gittim ekledim
                Brand createdBrand = await _brandRepository.AddAsync(mappedBrand);
                // ama son kullanıcıya aynı veriyi vermek istemiyorum onu encapsule ettim başka bir nesne olarak veriyorum
                // o nesneyi verirken mapleme yöntemi kullanarak işimi kolaylaştırıyorum
                CreatedBrandDto createdBrandDto = _mapper.Map<CreatedBrandDto>(createdBrand);


                return createdBrandDto;

            }
        }

    }

    // Dto   ---> id,name
    // Brand ---> id,name,x 
    // kullanıcıya yarın bir gün brand nesnesine eklenen bir şeyi göstermemek isteyebiliriz 
    // onun için dto oluşturup onu döndüürüyoruz son kullanıcıya defensive programming deniliyor buna
    // mapper ----> bu brandi sana zahmet dto ya çevir bunları mapler buda yep yeni bir dto verir bize 
}
