﻿namespace EcommerceApp.Application.DTO.Responses
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static Result<T> Success(T data, string message)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default
            };
        }
    }
}
