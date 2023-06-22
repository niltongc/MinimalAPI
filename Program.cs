using AutoMapper;
using FluentValidation;
using MagicalVilla_CouponAPI;
using MagicalVilla_CouponAPI.Data;
using MagicalVilla_CouponAPI.Models;
using MagicalVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Mapping));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/coupon", (ILogger<Program> _logger) => {

    APIResponse response = new();

    _logger.Log(LogLevel.Information, "Getting all Coupons");

    response.Result = CouponStore.couponList;
    response.IsSucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
    //return Results.Ok(CouponStore.couponList);
}).WithName("GetCoupons").Produces<APIResponse>(200);
//}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);

app.MapGet("/api/coupon/{id:int}", (ILogger <Program> _logger, int id) =>
{
    APIResponse response = new();
    response.Result = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
    response.IsSucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCoupon").Produces<APIResponse>(200);

app.MapPost("/api/coupon", async (IMapper _mapper,
    IValidator<CouponCreateDTO> _validation, [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    APIResponse response = new() { IsSucess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };


    var validationResult = await _validation.ValidateAsync(coupon_C_DTO);

    if( !validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    if(CouponStore.couponList.FirstOrDefault(u => u.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null )
    {
        response.ErrorMessages.Add("Coupon Name already Exists");
        return Results.BadRequest(response);
        //return Results.BadRequest("Coupon Name already Exists");
    }

    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

    
    coupon.Id = CouponStore.couponList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
    CouponStore.couponList.Add(coupon);

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    response.Result = couponDTO;
    response.IsSucess = true;
    response.StatusCode = HttpStatusCode.Created;
    return Results.Ok(response);

    //return Results.CreatedAtRoute("GetCoupon", new {id = coupon.Id }, couponDTO);
    //return Results.Created($"/api/coupon/{coupon.Id}",coupon);

}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

app.MapPut("/api/coupon", async (IMapper _mapper,
    IValidator<CouponUpdateDTO> _validation, [FromBody] CouponUpdateDTO coupon_C_DTO) =>
{
    APIResponse response = new() { IsSucess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };


    var validationResult = await _validation.ValidateAsync(coupon_C_DTO);

    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == coupon_C_DTO.Id);

    
    couponFromStore.IsActive = coupon_C_DTO.IsActive;
    couponFromStore.Name = coupon_C_DTO.Name;
    couponFromStore.Percent = coupon_C_DTO.Percent;
    couponFromStore.LastUpdated = DateTime.Now;



    response.Result = _mapper.Map<CouponDTO>(couponFromStore);
    response.IsSucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new() { IsSucess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };

   
    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
    if(couponFromStore != null)
    {
        CouponStore.couponList.Remove(couponFromStore);
        response.IsSucess = true;
        response.StatusCode = HttpStatusCode.NoContent;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessages.Add("Invalid Id");
        return Results.BadRequest(response);
    }


});

app.UseHttpsRedirection();

app.Run();
