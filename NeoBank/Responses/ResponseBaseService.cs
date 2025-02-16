using System;
using System.Collections.Generic;

namespace NeoBank.Responses
{
    public class ResponseBaseService
    {
        private readonly Dictionary<string, string> _responseMessages = new()
        {
            { ResponseCodes.SUCCESS, "Operation completed successfully." },
            { ResponseCodes.FAIL, "The operation failed." },
            { ResponseCodes.BAD_REQUEST, "The request is invalid." },
            { ResponseCodes.ACCOUNT_NOT_FOUND, "The specified account was not found." },
            { ResponseCodes.INSUFFICIENT_BALANCE, "Insufficient account balance." },
            { ResponseCodes.EXCEPTION, "An unexpected error occurred." },
            {ResponseCodes.EMAIL_ALREADY_EXIST, "Email already exist." },
            {ResponseCodes.REQUEST_NOT_SUCCESSFUL, "Request not successful." },
            {ResponseCodes.WITHDRAWAL_AMOUNT_MUST_BE_GREATER_THAN_ZERO, "Withdrawal amount must be greater than zero" },
            {ResponseCodes.INSUFFICIENT_BALANCE_MINIMUM_BALANCE_OF_100_IS_REQUIRED, "Insufficient balance. Minimum balance of ₦100 is required." },
            {ResponseCodes.ACCOUNT_CREATED_SUCCESSFULLY, "Account created successfully." },
            {ResponseCodes.ACCOUNT_CREDITED_SUCCESSFULLY, "Account credited successfully." },
            {ResponseCodes.TRANSFER_SUCCESSFUL, "Transfer successful." },
            {ResponseCodes.REQUEST_SUCCESSFUL, "Request successful." }

        };

        public ServerResponse<T> SetErrorValidation<T>(ServerResponse<T> response, string responseCode, string message)
        {
            response.Error = new ErrorResponse
            {
                ResponseCode = responseCode,
                ResponseDescription = $"{message} {_responseMessages.GetValueOrDefault(responseCode, "Unknown error occurred.")}"
            };
            return response;
        }

        public ServerResponse<T> SetError<T>(ServerResponse<T> response, string responseCode)
        {
            response.Error = new ErrorResponse
            {
                ResponseCode = responseCode,
                ResponseDescription = _responseMessages.GetValueOrDefault(responseCode, "Unknown error occurred.")
            };
            return response;
        }

        public ServerResponse<T> SetErrorWithMessage<T>(ServerResponse<T> response, string responseCode, string message)
        {
            response.Error = new ErrorResponse
            {
                ResponseCode = responseCode,
                ResponseDescription = message
            };
            return response;
        }

        public ServerResponse<T> SetSuccess<T>(ServerResponse<T> response, T data, string responseCode)
        {
            response.SuccessMessage = _responseMessages.GetValueOrDefault(responseCode, "Operation completed successfully.");
            response.IsSuccessful = true;
            response.Data = data;
            return response;
        }

        public ServerResponse<T> SetErrorWithStatus<T>(ServerResponse<T> response, string responseCode, string status)
        {
            string baseMessage = _responseMessages.GetValueOrDefault(responseCode, "Unknown error occurred.");
            response.Error = new ErrorResponse
            {
                ResponseCode = responseCode,
                ResponseDescription = baseMessage.Replace("{status}", status)
            };
            return response;
        }
    }
}
