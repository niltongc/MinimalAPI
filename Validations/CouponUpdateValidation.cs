﻿using FluentValidation;
using MagicalVilla_CouponAPI.Models.DTO;

namespace MagicalVilla_CouponAPI.Validations
{
    public class CouponUpdateValidation : AbstractValidator<CouponUpdateDTO>
    {
        public CouponUpdateValidation() 
        {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(0, 100);
        }
    }
}
