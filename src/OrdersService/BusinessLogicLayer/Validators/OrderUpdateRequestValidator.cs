﻿using OrdersService.BusinessLogicLayer.DTO;
using FluentValidation;

namespace OrdersService.BusinessLogicLayer.Validators;

public class OrderUpdateRequestValidator : AbstractValidator<OrderUpdateRequest>
{
  public OrderUpdateRequestValidator()
  {
    //OrderID
    RuleFor(temp => temp.OrderID)
      .NotEmpty().WithErrorCode("Order ID can't be blank");

    //UserID
    RuleFor(temp => temp.UserID)
      .NotEmpty().WithErrorCode("User ID can't be blank");

    //OrderDate
    RuleFor(temp => temp.OrderDate)
      .NotEmpty().WithErrorCode("Order Date can't be blank");

    //OrderItems
    RuleFor(temp => temp.OrderItems)
      .NotEmpty().WithErrorCode("Order Items can't be blank");
  }
}
